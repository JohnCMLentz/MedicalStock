using MedicalStock.Data;
using MedicalStock.Models;

namespace MedicalStock.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public bool RemoveStock(int productId, int quantity)
        {
            if (quantity <= 0) return false;

            if (!_context.Products.Any(p => p.Id == productId)) return false;

            List<Batch> batches = _context.Batches
                .Where(b => b.ProductId == productId && b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToList();

            int totalStock = batches.Sum(b => b.Quantity);
            
            if (totalStock < quantity) return false;

            for (int i = 0; i < batches.Count; i++)
            {
                if (quantity <= 0) break;

                var n = batches[i].Quantity;

                if (batches[i].Quantity <= quantity)
                {
                    batches[i].Quantity = 0;
                    quantity -= n;
                }
                else
                {
                    batches[i].Quantity -= quantity;
                    quantity = 0;
                }
            }

            _context.SaveChanges();
            return true;
        }
    }
}
