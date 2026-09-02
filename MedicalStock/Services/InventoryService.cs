using MedicalStock.Data;
using MedicalStock.Models;
using MedicalStock.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;
        private readonly BatchService _batchService;
        private readonly ProductService _productService;

        public InventoryService(AppDbContext context)
        {
            _context = context;
            _batchService = new BatchService(_context);
            _productService = new ProductService(_context);
        }
                public List<StockMovement> GetMovements()
        {
            return _context.StockMovements
                .OrderBy(sm => sm.MovementDate)
                .ToList();
        }

        public List<StockMovement> GetMovementsByBatch(int batchId)
        {
            return _context.StockMovements
                .Where(sm => sm.BatchId == batchId)
                .OrderBy(sm => sm.MovementDate)
                .ToList();
        }

        public List<StockMovement> GetMovementsByProduct(int productId)
        {
            return _context.StockMovements
                .Include(sm => sm.Batch)
                .Where(sm => sm.Batch!.ProductId == productId)
                .OrderBy(sm => sm.MovementDate)
                .ToList();
        }

        public int GetNumberOfProducts(int productId)
        {
            return _batchService
                .GetBatchesByFEFO(productId)
                .Sum(b => b.Quantity);
        }

        public List<Batch> GetExpiredBatches()
        {
            return _batchService .GetBatches()
                .Where(b => (b.ExpirationDate - DateTime.Today).Days < 0)
                .Where(b => b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToList();
        }

        public List<Batch> GetBatchesNearExpiration(int days)
        {
            return _batchService.GetBatches()
                .Where(b => (b.ExpirationDate - DateTime.Today).Days <= days)
                .Where(b => (b.ExpirationDate - DateTime.Today).Days >= 0)
                .Where(b => b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToList();
        }

        public int GetDayUntilExpiration(int batchId)
        {
            Batch? batch = _batchService.GetBatchById(batchId);

            if (batch == null)
                return -1;

            return (batch.ExpirationDate - DateTime.Today).Days;
        }

        public bool HasAvaliableStock(int productId, int quantity)
        {
            if (quantity <= 0) return false;
            
            if (!_context.Products.Any(p => p.Id == productId)) return false;

            List<Batch> batches = _batchService.GetBatchesByFEFO(productId);

            int totalStock = GetNumberOfProducts(productId);

            if (totalStock < quantity)
                return false;

            return true;
        }

        public bool HasStock(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return false;

            return GetNumberOfProducts(productId) <= 0 ? false : true;
        }

        public bool IsLowStock(int productId)
        {
            var product = _productService.GetProductById(productId);

            if(product == null)
                return false;

            if (product.MinimumStock <= 0)
                return false;

            int actualQuantity = GetNumberOfProducts(productId);

            if(actualQuantity <= product.MinimumStock)
                return true;

            return false;
        }

        public List<Product> GetLowStockProducts()
        {
            List<Product> products = new List<Product>();

            foreach (var product in _productService.GetProducts())
            {
                if (IsLowStock(product.Id))
                    products.Add(product);
            }

            return products;
        }

        public void AddStock(int productId, int quantity, DateTime expirationDate, DateTime? receivedAt)
        {
            if (!receivedAt.HasValue)
                receivedAt = DateTime.Now;
                        
            Batch batch = _batchService.CreateBatch(productId,quantity,expirationDate,receivedAt.Value);

            var stockMovement = new StockMovement(batch,quantity,MovementType.Entry,receivedAt.Value);
            
            _context.StockMovements.Add(stockMovement);
            _context.SaveChanges();
        }

        public void OutflowStock(int productId, int quantity)
        {
            if (quantity <= 0) 
                throw new InvalidQuantityException();

            if (!_context.Products.Any(p => p.Id == productId))
                throw new ProductNotFoundException(productId);

            int totalStock = GetNumberOfProducts(productId);

            if (totalStock < quantity) 
                throw new InsufficientStockException(productId,quantity,totalStock);

            var batches = _batchService.GetBatchesByFEFO(productId);

            RemoveStock(batches, quantity, MovementType.Outflow);
        }

        public void DisposalStock(int productId)
        {
            if (!_context.Products.Any(p => p.Id == productId))
                throw new ProductNotFoundException(productId);

            var batches = _batchService.GetBatchesByProduct(productId)
                .Where(b => b.ExpirationDate.Date < DateTime.Today)
                .Where(b => b.Quantity > 0)
                .ToList();
            
            if(batches.Count == 0)
                throw new NoExpiredStockException();

            int totalStock = batches.Sum(b => b.Quantity);

            RemoveStock(batches, totalStock, MovementType.Disposal);
        }

        private void RemoveStock(List<Batch> batches, int quantity, MovementType type)
        {
            if (quantity <= 0)
                throw new InvalidQuantityException();

            for (int i = 0; i < batches.Count; i++)
            {
                if (quantity <= 0) break;

                var batch = batches[i];

                int removedQuantity = batch.Quantity;

                if (batch.Quantity <= quantity)
                {
                    batch.Quantity = 0;
                    quantity -= removedQuantity;
                }
                else
                {
                    batch.Quantity -= quantity;
                    removedQuantity = quantity;
                    quantity = 0;
                }

                var stockMovement = new StockMovement(
                    batch,
                    removedQuantity,
                    type,
                    DateTime.Now
                );

                _context.StockMovements.Add(stockMovement);
            }

            _context.SaveChanges();
        }

        public string GetProductStockInfo(int productId)
        {
            Product? product = _productService.GetProductById(productId);

            if (product == null) return "Product not found.";

            string text = $"{product}";


            text += $"\nBatches containing this product:";
            foreach (var batch in _batchService.GetBatchesByFEFO(productId))
            {
                text += $"\n{@" \- "}{batch}";
            }
            text += $"\n{@" \-- "}Current number of products: {GetNumberOfProducts(productId)}";

            return text;
        }

        public string GetBatchStockInfo(int batchId)
        {
            Batch? batch = _batchService.GetBatchById(batchId);

            if (batch == null) return "Batch not found.";

            string text = $"{batch}";
            int currentQuantity = 0;

            text += $"\nMovements in this batch:";
            foreach (var sm in GetMovementsByBatch(batchId))
            {
                text += $"\n{@" \- "}{sm}";
                if (sm.Type == MovementType.Outflow)
                    currentQuantity -= sm.Quantity;
                else if (sm.Type == MovementType.Entry)
                    currentQuantity += sm.Quantity;
            }
            text += $"\n{@" \-- "}Current number of products in this batch: {currentQuantity}";

            return text;
        }

        public string GetProductAllStockInfo(int productId)
        {
            Product? product = _productService.GetProductById(productId);

            if (product == null) return "Product not found.";

            string text = $"{product}";


            text += $"\nBatches containing this product:";
            foreach (var batch in _batchService.GetBatchesByFEFO(productId))
            {
                int currentQuantity = 0;
                text += $"\n{@" \- "}{batch}";
                text += $"\n{@"  \- "}Movements in this batch:";
                foreach (var sm in GetMovementsByBatch(batch.Id))
                {
                    text += $"\n{@"   \- "}{sm}";
                    if (sm.Type == MovementType.Outflow)
                        currentQuantity -= sm.Quantity;
                    else if (sm.Type == MovementType.Entry)
                        currentQuantity += sm.Quantity;
                }
                text += $"\n{@"   \-- "}Current number of products in this batch: {currentQuantity}";
            }
            text += $"\n{@" \-- "}Current number of products: {GetNumberOfProducts(productId)}";

            return text;
        }

        public string GetExpirationBatchesInfo()
        {
            var today = DateTime.Today;

            var listLastWeek = GetExpiredBatches()
                .Where(b => (b.ExpirationDate - today).Days > -8)
                .Where(b => (b.ExpirationDate - today).Days <= -1)
                .OrderBy(b => b.ExpirationDate)
                .ToList();

            var listWeek = GetBatchesNearExpiration(7)
                .Where(b => (b.ExpirationDate - today).Days > -1)
                .Where(b => (b.ExpirationDate - today).Days <= 7)
                .OrderBy(b => b.ExpirationDate)
                .ToList();

            var listMonth = GetBatchesNearExpiration(31)
                .Where(b => (b.ExpirationDate - today).Days > 7)
                .Where(b => (b.ExpirationDate - today).Days <= 31)
                .OrderBy(b => b.ExpirationDate)
                .ToList();

            string text = string.Empty ;

            if (listLastWeek.Count > 0)
            {

                text += $"Batches expired last week:";
                foreach (var b in listLastWeek)
                {
                    text += $"\n{@" \- "}{b}";
                }
            }
            else
                text += $"No batches expired last week.";

            if (listWeek.Count > 0)
            {
                text += $"\nBatches expiring this week:";
                foreach (var b in listWeek)
                {
                    text += $"\n{@" \- "}{b}";
                }
            }
            else
                text += $"\nNo batches expiring this week.";

            if (listMonth.Count > 0) {
            text += $"\nBatches expiring this month:";
                foreach (var b in listMonth)
                {
                    text += $"\n{@" \- "}{b}";
                }
            }
            else
                text += $"\nNo batches expiring this month.";

            return text;
        }

    }
}
