using MedicalStock.Data;
using MedicalStock.Models;

namespace MedicalStock.Services
{
    public class BatchService
    {
        private readonly AppDbContext _context;

        public BatchService(AppDbContext context)
        {
            _context = context;
        }

        public Batch? CreateBatch(int productId, int quantity, DateTime expirationDate, DateTime receivedAt)
        {
            if (quantity <= 0)
                return null;
            if (expirationDate <= receivedAt)
                return null;
            if (receivedAt.Date > DateTime.Today)
                return null;
            if (!_context.Products.Any(p => p.Id == productId))
                return null;

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
                .OrderBy(b => b.ExpirationDate)
                .ToList();
        }

        public Batch? GetBatchById(int id)
        {
            return _context.Batches.FirstOrDefault(b => b.Id == id);
        }

        public bool UpdateBatch(int id, int? productId, int? quantity, DateTime? expirationDate, DateTime? receivedAt)
        {
            var batch = GetBatchById(id);

            if (batch == null) return false;

            if (productId.HasValue)
            {
                if (!_context.Products.Any(p => p.Id == productId))
                    return false;

                batch.ProductId = productId.Value;
            }

            if (quantity.HasValue)
            {
                if (quantity.Value <= 0)
                    return false;

                batch.Quantity = quantity.Value;
            }

            if (expirationDate.HasValue)
                batch.ExpirationDate = expirationDate.Value;

            if (receivedAt.HasValue)
                batch.ReceivedAt = receivedAt.Value;


            if (expirationDate.HasValue || receivedAt.HasValue)
                if (batch.ExpirationDate <= batch.ReceivedAt)
                    return false;

            _context.SaveChanges();

            return true;
        }

        public bool DeleteBatch(int id)
        {
            var batch = _context.Batches.FirstOrDefault(b => b.Id == id);

            if (batch == null) return false;

            _context.Batches.Remove(batch);
            _context.SaveChanges();

            return true;
        }

    }
}
