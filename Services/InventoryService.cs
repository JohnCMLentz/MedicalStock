using MedicalStock.Data;
using MedicalStock.Models;
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

        public bool AddStock(int productId, int quantity, DateTime expirationDate, DateTime? receivedAt)
        {
            if (!receivedAt.HasValue)
                receivedAt = DateTime.Now;
                        
            Batch? batch = _batchService.CreateBatch(productId,quantity,expirationDate,receivedAt.Value);
            if (batch == null) return false;

            var stockMovement = new StockMovement(batch,quantity,MovementType.Entry,receivedAt.Value);
            
            _context.StockMovements.Add(stockMovement);
            _context.SaveChanges();
            return true;
        }

        public bool RemoveStock(int productId, int quantity)
        {
            if (quantity <= 0) return false;

            if (!_context.Products.Any(p => p.Id == productId)) return false;

            List<Batch> batches = _batchService.GetBatchesByFEFO(productId);

            int totalStock = GetNumberOfProducts(productId);
            
            if (totalStock < quantity) return false;

            for (int i = 0; i < batches.Count; i++)
            {
                if (quantity <= 0) break;

                var batch = batches[i];

                int removedQuantity = batch.Quantity;

                if (batch.Quantity <= quantity)
                {
                    batch.Quantity = 0;
                    quantity -= removedQuantity;
                    removedQuantity = 0;
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
                    MovementType.Outflow,
                    DateTime.Now
                    );

                _context.StockMovements.Add(stockMovement);
            }

            _context.SaveChanges();
            return true;
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

    }
}
