using MedicalStock.Data;
using MedicalStock.Exceptions;
using MedicalStock.Models;
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
        public void GetBatchById_ExistingId_ReturnsBatch()
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
                context.SaveChanges();

                Assert.NotNull(batch);
                Assert.Equal(batch, batchService.GetBatchById(batch.Id));
            }
        }

        [Fact]
        public void GetBatchById_NonExistingId_ReturnsNull()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Null(batchService.GetBatchById(999));
            }
        }

        [Fact]
        public void CreateBatch_InvalidProductId_ThrowsProductNotFoundException()
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
        public void CreateBatch_InvalidQuantity_ThrowsInvalidQuantityException(int quantity)
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
        public void CreateBatch_InvalidExpirationDate_ThrowsInvalidExpirationDateException()
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
                batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(-5), DateTime.Today));
            }
        }

        [Fact]
        public void CreateBatch_InvalidReceivedDate_ThrowsInvalidReceivedDateException()
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
        public void GetBatches_WhenBatchesExist_ReturnsAllBatches()
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

                Assert.Equal(3, batchService.GetBatches().Count);
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

        [Fact]
        public void GetBatchesByProduct_WhenBatchesExist_ReturnsBatches()
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

                var batches = batchService.GetBatchesByProduct(product.Id);
                Assert.Equal(3, batches.Count);
            }
        }

        [Fact]
        public void GetBatchesByProduct_WhenNoBatchesExist_ReturnsEmptyCollection()
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

                var batches = batchService.GetBatchesByProduct(product.Id);
                Assert.NotNull(batches);
                Assert.Empty(batches);
            }
        }

        [Fact]
        public void GetBatchesByProduct_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                batchService.GetBatchesByProduct(99));
            }
        }

        [Fact]
        public void GetBatchesByFEFO_WhenBatchesExist_ReturnsBatches()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(12), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(5), DateTime.Today);
                var batch3 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                var batches = batchService.GetBatchesByFEFO(product.Id);
                Assert.NotNull(batches);
                Assert.NotNull(batch1);
                Assert.NotNull(batch2);
                Assert.NotNull(batch3);
                Assert.Equal(3, batches.Count);
                Assert.Equal(batch2.Id, batches[0].Id);
                Assert.Equal(batch3.Id, batches[1].Id);
                Assert.Equal(batch1.Id, batches[2].Id);
            }
        }

        [Fact]
        public void GetBatchesByFEFO_WhenNoBatchesExist_ReturnsEmptyList()
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
                var batches = batchService.GetBatchesByFEFO(product.Id);

                Assert.Empty(batches);
            }
        }

        [Fact]
        public void GetBatchesByFEFO_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                batchService.GetBatchesByFEFO(99));
            }
        }

        [Fact]
        public void UpdateBatch_ValidData_UpdateBatch()
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
                var oldBatch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today.AddDays(-5));
                batchService.UpdateBatch(oldBatch.Id, null, DateTime.Today.AddDays(20), DateTime.Today);
                var updatedBatch = batchService.GetBatchById(oldBatch.Id);

                Assert.NotNull(oldBatch);
                Assert.NotNull(updatedBatch);
                Assert.Equal(product.Id, updatedBatch.ProductId);
                Assert.Equal(100, updatedBatch.Quantity);
                Assert.Equal(DateTime.Today.AddDays(20), updatedBatch.ExpirationDate);
                Assert.Equal(DateTime.Today, updatedBatch.ReceivedAt);
            }
        }

        [Fact]
        public void UpdateBatch_AllOptionalParametersNull_KeepsProductUnchanged()
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
                var oldBatch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batchService.UpdateBatch(oldBatch.Id, null, null, null);
                var updatedBatch = batchService.GetBatchById(oldBatch.Id);

                Assert.NotNull(oldBatch);
                Assert.NotNull(updatedBatch);
                Assert.Equal(product.Id, updatedBatch.ProductId);
                Assert.Equal(100, updatedBatch.Quantity);
                Assert.Equal(DateTime.Today.AddDays(10), updatedBatch.ExpirationDate);
                Assert.Equal(DateTime.Today, updatedBatch.ReceivedAt);
            }
        }

        [Fact]
        public void UpdateBatch_InvalidBatchId_ThrowsBatchNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Throws<BatchNotFoundException>(() =>
                batchService.UpdateBatch(99, null, null, null));
            }
        }

        [Fact]
        public void UpdateBatch_InvalidProductId_ThrowsProductNotFoundException()
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
                var batch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.Throws<ProductNotFoundException>(() =>
                batchService.UpdateBatch(batch.Id, 99, null, null));
            }
        }

        [Fact]
        public void UpdateBatch_InvalidExpirationDate_ThrowsInvalidExpirationDateException()
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
                var batch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(20), DateTime.Today);

                Assert.Throws<InvalidExpirationDateException>(() =>
                batchService.UpdateBatch(batch.Id, null, DateTime.Today.AddDays(-100), null));
                Assert.Throws<InvalidExpirationDateException>(() =>
                batchService.UpdateBatch(batch.Id, null, DateTime.Today.AddDays(40), DateTime.Today.AddDays(50)));
            }
        }

        [Fact]
        public void UpdateBatch_InvalidReceivedDate_ThrowsInvalidReceivedDateException()
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
                var batch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(50), DateTime.Today);

                Assert.Throws<InvalidReceivedDateException>(() =>
                batchService.UpdateBatch(batch.Id, null, null, DateTime.Today.AddDays(10)));
            }
        }

        [Fact]
        public void DeactivateBatch_BatchWithZeroQuantity_DeactivatesBatch()
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
                Batch batch = batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(50), DateTime.Today);
                batch.Quantity = 0;
                context.SaveChanges();
                batchService.DeactivateBatch(batch.Id);

                Assert.NotNull(batchService.GetBatchById(batch.Id));
                Assert.False(batchService.GetBatchById(batch.Id)!.IsActive);
            }
        }

        [Fact]
        public void DeactivateBatch_BatchWithRemainingStock_ThrowsDeactivateBatchWithProductsException()
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
                Batch batch = batchService.CreateBatch(product.Id, 100, DateTime.Today.AddDays(50), DateTime.Today);
                context.SaveChanges();
                Assert.Throws<DeactivateBatchWithProductsException>(() =>
                batchService.DeactivateBatch(batch.Id));
            }
        }

        [Fact]
        public void DeactivateBatch_InvalidBatchId_ThrowsBatchNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var batchService = new BatchService(context);

                Assert.Throws<BatchNotFoundException>(() =>
                batchService.DeactivateBatch(99));
            }
        }
    }
}