using MedicalStock.Data;
using MedicalStock.Exceptions;
using MedicalStock.Models;
using MedicalStock.Services;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            using AppDbContext context = new AppDbContext();
            var categoryService = new CategoryService(context);
            var productService = new ProductService(context);
            var batchService = new BatchService(context);
            var inventoryService = new InventoryService(context);

            Console.WriteLine("=== TESTE 2 - DESCARTE DE ESTOQUE VENCIDO ===");

            var category = categoryService.CreateCategory("Medicamentos");

            var product = productService.CreateProduct(
                "Paracetamol",
                "7899876543210",
                "EMS",
                9.90m,
                30,
                category.Id
            );

            var expiredBatch1 = inventoryService.AddStock(
                product.Id,
                100,
                DateTime.Today.AddDays(30),
                DateTime.Today
            );

            var expiredBatch2 = inventoryService.AddStock(
                product.Id,
                50,
                DateTime.Today.AddDays(30),
                DateTime.Today
            );

            var validBatch = inventoryService.AddStock(
                product.Id,
                200,
                DateTime.Today.AddDays(60),
                DateTime.Today
            );

            // Simula lotes que ficaram vencidos posteriormente
            expiredBatch1.ExpirationDate = DateTime.Today.AddDays(-10);
            expiredBatch2.ExpirationDate = DateTime.Today.AddDays(-5);

            context.SaveChanges();

            Console.WriteLine($"Estoque disponível antes do descarte: {inventoryService.GetNumberOfProducts(product.Id)}");

            var expiredBatches = inventoryService.GetExpiredBatches();

            Console.WriteLine($"Lotes vencidos encontrados: {expiredBatches.Count}");

            inventoryService.DisposalStock(product.Id);

            Console.WriteLine($"Estoque disponível após descarte: {inventoryService.GetNumberOfProducts(product.Id)}");

            var batches = batchService.GetBatchesByProduct(product.Id);

            foreach (var batch in batches)
            {
                Console.WriteLine(
                    $"Batch {batch.Id} | Quantity: {batch.Quantity} | Expiration: {batch.ExpirationDate:dd/MM/yyyy}"
                );
            }

            var disposalMovements = inventoryService
                .GetMovementsByProduct(product.Id)
                .Where(sm => sm.Type == MovementType.Disposal)
                .ToList();

            Console.WriteLine($"\nMovimentos de descarte: {disposalMovements.Count}");

            foreach (var movement in disposalMovements)
            {
                Console.WriteLine(
                    $"Batch: {movement.BatchId} | Quantity: {movement.Quantity}"
                );
            }

            Console.WriteLine("\nResultado esperado:");
            Console.WriteLine("Estoque disponível antes do descarte: 200");
            Console.WriteLine("Lotes vencidos encontrados: 2");
            Console.WriteLine("Batch vencido 1: 0");
            Console.WriteLine("Batch vencido 2: 0");
            Console.WriteLine("Batch válido: 200");
            Console.WriteLine("Estoque disponível após descarte: 200");
            Console.WriteLine("2 movimentos Disposal");
        }
    }
}
