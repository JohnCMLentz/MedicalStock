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
            var invetoryService = new InventoryService(context);

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
            
            if (batchService.CreateBatch
                (
                1,
                100,
                DateTime.Parse("2027-12-10"),
                null
                ))
            {
                Console.WriteLine("Batch criado!");
            }
            else
                Console.WriteLine("Erro ao criar!");
            */








            /*
            Console.WriteLine();
            foreach (var i in productService.GetProducts())
            {
                Console.WriteLine(i.ToString());
            }
            
            Console.WriteLine();
            foreach (var i in batchService.GetBatches())
            {
                Console.WriteLine(i);
            }

            foreach (var i in batchService.GetBatchesByFEFO(1))
            {
                Console.WriteLine(i);
            }
            Console.WriteLine();
            */
        }
    }
}
