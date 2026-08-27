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

            productService.CreateProduct
                (
                "Amoxicillin 500mg",
                "7891000000035",
                "Eurofarma",
                22.50m,
                2
                );
            productService.CreateProduct
                (
                "Azithromycin 500mg",
                "7891000000042",
                "Medley",
                18.90m,
                2
                );

            productService.CreateProduct
                (
                "Loratadine 10mg",
                "7891000000059",
                "EMS",
                12.50m,
                3
                );
            productService.CreateProduct
                (
                "Cetirizine 10mg",
                "7891000000066",
                "Neo Química",
                14.90m,
                3
                );
            */
            if (productService.CreateProduct
                (
                "Cetirizine 10mg",
                "7891000000067",
                "Neo Química",
                14.90m,
                3
                ))
            {
                Console.WriteLine("Medicamento criado!");
            }
            else
                Console.WriteLine("Erro ao criar!");

            Console.WriteLine();
            foreach (var i in productService.GetProducts())
            {
                Console.WriteLine(i.ToString());
            }

            if (productService.DeleteProduct(7))
            {
                Console.WriteLine("Medicamento excluido!");
            }
            else
                Console.WriteLine("Erro ao excluir!");

            Console.WriteLine();
            foreach (var i in productService.GetProducts())
            {
                Console.WriteLine(i.ToString());
            }
        }
    }
}
