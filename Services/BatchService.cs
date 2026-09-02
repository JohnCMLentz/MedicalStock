using MedicalStock.Data;
using MedicalStock.Models;
using MedicalStock.Exceptions;

namespace MedicalStock.Services
{
    public class BatchService
    {
        private readonly AppDbContext _context;

        public BatchService(AppDbContext context)
        {
            _context = context;
        }

        public List<Batch> GetBatches()
        {
            return _context.Batches.ToList();
        }

        public List<Batch> GetBatchesByProduct(int productId)
        {
            return _context.Batches.Where(b => b.ProductId == productId).ToList();
        }

        public List<Batch> GetBatchesByFEFO(int productId)
        {
            return _context.Batches
                .Where(b => b.ProductId == productId && b.Quantity > 0)
                .Where(b => b.ExpirationDate.Date >= DateTime.Today)
                .OrderBy(b => b.ExpirationDate)
                .ToList();
        }

        public Batch? GetBatchById(int id)
        {
            return _context.Batches.FirstOrDefault(b => b.Id == id);
        }

        public Batch CreateBatch(int productId, int quantity, DateTime expirationDate, DateTime receivedAt)
        {
            if (quantity <= 0)
                throw new InvalidQuantityException();
            if (expirationDate <= receivedAt ||
                expirationDate < DateTime.Today)
                throw new InvalidExpirationDateException(expirationDate);
            if (receivedAt.Date > DateTime.Today)
                throw new InvalidReceivedDateException();
            if (!_context.Products.Any(p => p.Id == productId))
                throw new ProductNotFoundException(productId);

            var batch = new Batch
                (
                productId,
                quantity,
                expirationDate,
                receivedAt
                );

            _context.Batches.Add(batch);

            return batch;
        }

        public void UpdateBatch(int id, int? productId, DateTime? expirationDate, DateTime? receivedAt)
        {
            var batch = GetBatchById(id);

            if (batch == null)
                throw new BatchNotFoundException(id);

            if (productId.HasValue)
            {
                if (!_context.Products.Any(p => p.Id == productId))
                    throw new ProductNotFoundException(productId.Value);

                batch.ProductId = productId.Value;
            }


            if (expirationDate.HasValue || receivedAt.HasValue)
            {
                var expiration = expirationDate.HasValue ? expirationDate.Value : batch.ExpirationDate;
                var receive = receivedAt.HasValue ? receivedAt.Value : batch.ReceivedAt;

                if (expirationDate.HasValue && expiration < DateTime.Today)
                    throw new InvalidExpirationDateException(expiration);

                if (expiration <= receive)
                    throw new InvalidExpirationDateException(expiration);

                if (expirationDate.HasValue)
                    batch.ExpirationDate = expiration;

                if (receivedAt.HasValue)
                {
                    if (receive > DateTime.Today)
                        throw new InvalidReceivedDateException();

                    batch.ReceivedAt = receive;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteBatch(int id)
        {
            var batch = _context.Batches.FirstOrDefault(b => b.Id == id);

            if (batch == null)
                throw new BatchNotFoundException(id);

            if (_context.StockMovements.Any(sm => sm.BatchId == id))
                throw new BatchHasMovementsException(id);

            _context.Batches.Remove(batch);
            _context.SaveChanges();
        }

    }
}
