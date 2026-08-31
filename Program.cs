using MedicalStock.Data;
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

            /*
            productService.CreateProduct
                (
                "Paracetamol 500mg",
                "7891000000011",
                "MedPharma",
                8.50m,
                1
                );
            productService.CreateProduct
                (
                "Dipyrone 500mg",
                "7891000000028",
                "FarmaBrasil",
                7.90m,
                1
                );
            
            if (inventoryService.AddStock(
                1,
                110,
                DateTime.Parse("2027-08-05"),
                DateTime.Parse("2026-08-30")))
                Console.WriteLine("Stock adicionado");
            else
                Console.WriteLine("Erro ao adicionar!");

            if (inventoryService.RemoveStock(
                1,
                120))
                Console.WriteLine("Stock removido");
            else
                Console.WriteLine("Erro ao remover!");
            */

            if (inventoryService.AddStock(
                1,
                110,
                DateTime.Parse("2026-09-25"),
                null))
                Console.WriteLine("Stock adicionado");
            else
                Console.WriteLine("Erro ao adicionar!");

            Console.WriteLine(inventoryService.GetExpiretionBatchesInfo());

            /*
            Console.WriteLine();
            foreach (var i in productService.GetProducts())
            {
                Console.WriteLine(i.ToString());
            }
            
            Console.WriteLine(inventoryService.GetProductAllStockInfo(1));
            Console.WriteLine();

            */
        }
    }
}
