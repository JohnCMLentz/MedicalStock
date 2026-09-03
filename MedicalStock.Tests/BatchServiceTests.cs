using MedicalStock.Data;
using MedicalStock.Exceptions;
using MedicalStock.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock.Tests
{
    public class BatchServiceTests
    {
        private (SqliteConnection connection, AppDbContext context) CreateTestContext()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);

            context.Database.EnsureCreated();

            return (connection, context);
        }

        [Fact]
        public void CreateBatch_ValidData_CreateBatch()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var categoryService = new CategoryService(context);
                var productService = new ProductService(context);
                var batchService = new BatchService(context);
                categoryService.CreateCategory("Test Category");
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

                var batch = batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.NotNull(batch);
                Assert.Equal(product.Id, batch.ProductId);
                Assert.Equal(100, batch.Quantity);
                Assert.Equal(DateTime.Today.AddDays(10), batch.ExpirationDate);
                Assert.Equal(DateTime.Today, batch.ReceivedAt);
            }
        }

        [Fact]
        public void CreateBatch_InvalidProductId_ThrwosProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                batchService.CreateBatch(99, 100, DateTime.Today.AddDays(10), DateTime.Today));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CreateBatch_InvalidQuantity_ThrwosInvalidQuantityException(int quantity)
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var categoryService = new CategoryService(context);
                var productService = new ProductService(context);
                var batchService = new BatchService(context);
                categoryService.CreateCategory("Test Category");
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

                Assert.Throws<InvalidQuantityException>(() =>
                batchService.CreateBatch(product.Id, quantity, DateTime.Today.AddDays(10), DateTime.Today));
            }
        }

        [Fact]
        public void CreateBatch_InvalidExpirationDate_ThrwosInvalidExpirationDateException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var categoryService = new CategoryService(context);
                var productService = new ProductService(context);
                var batchService = new BatchService(context);
                categoryService.CreateCategory("Test Category");
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

                Assert.Throws<InvalidExpirationDateException>(() =>
                batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)));
                Assert.Throws<InvalidExpirationDateException>(() =>
                batchService.CreateBatch(product.Id, 100, DateTime.Parse("2000-05-05"), DateTime.Today));
            }
        }

        [Fact]
        public void CreateBatch_InvalidReceivedDate_ThrwosInvalidReceivedDateException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var categoryService = new CategoryService(context);
                var productService = new ProductService(context);
                var batchService = new BatchService(context);
                categoryService.CreateCategory("Test Category");
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

                Assert.Throws<InvalidReceivedDateException>(() =>
                batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(20), DateTime.Today.AddDays(10)));
            }
        }

        [Fact]
        public void GetBatches_WhenBatchesExist_ReturnAllProducts()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var categoryService = new CategoryService(context);
                var productService = new ProductService(context);
                var batchService = new BatchService(context);
                var inventoryService = new InventoryService(context);
                categoryService.CreateCategory("Test Category");
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.Equal(3,batchService.GetBatches().Count);
            }
        }

        [Fact]
        public void GetBatches_WhenNoBatchesExist_ReturnsEmptyCollection()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Empty(batchService.GetBatches());
            }
        }
    }
}
