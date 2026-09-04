using MedicalStock.Data;
using MedicalStock.Exceptions;
using MedicalStock.Models;
using MedicalStock.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MedicalStock.Tests
{
    public class InventoryServiceTests
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

        // GetMovements

        [Fact]
        public void GetMovements_WhenMovementsExist_ReturnsAllMovements()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                var movements = inventoryService.GetMovements();

                Assert.NotEmpty(movements);
                Assert.Equal(4, movements.Count);
            }
        }

        [Fact]
        public void GetMovements_WhenNoMovementsExist_ReturnsEmptyCollection()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                var movements = inventoryService.GetMovements();

                Assert.Empty(movements);
            }
        }

        // GetMovementsByBatch

        [Fact]
        public void GetMovementsByBatch_ExistingBatch_ReturnsBatchMovements()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                var movements = inventoryService.GetMovementsByBatch(batch.Id);

                Assert.NotEmpty(movements);
                Assert.Equal(3, movements.Count);
            }
        }

        [Fact]
        public void GetMovementsByBatch_InvalidBatchId_ReturnsEmpty()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Empty(inventoryService.GetMovementsByBatch(99));
            }
        }

        // GetMovementsByProduct

        [Fact]
        public void GetMovementsByProduct_ExistingProduct_ReturnsProductMovements()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                var movements = inventoryService.GetMovementsByProduct(product.Id);

                Assert.NotEmpty(movements);
                Assert.Equal(4, movements.Count);
            }
        }

        [Fact]
        public void GetMovementsByProduct_InvalidProductId_ReturnsEmpty()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Empty(inventoryService.GetMovementsByProduct(99));
            }
        }

        // GetNumberOfProducts

        [Fact]
        public void GetNumberOfProducts_ExistingStock_ReturnsAvailableQuantity()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                var quantity = inventoryService.GetNumberOfProducts(product.Id);

                Assert.Equal(60, quantity);
            }
        }

        [Fact]
        public void GetNumberOfProducts_NoStock_ReturnsZero()
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
                inventoryService.OutflowStock(product.Id, 100);

                var quantity = inventoryService.GetNumberOfProducts(product.Id);

                Assert.Equal(0, quantity);
            }
        }

        // HasAvaliableStock

        [Fact]
        public void HasAvaliableStock_EnoughStock_ReturnsTrue()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                Assert.True(inventoryService.HasAvaliableStock(product.Id, 60));
            }
        }

        [Fact]
        public void HasAvaliableStock_InsufficientStock_ReturnsFalse()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                Assert.False(inventoryService.HasAvaliableStock(product.Id, 100));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void HasAvaliableStock_InvalidQuantity_ThrowsInvalidQuanityException(int quantity)
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

                Assert.Throws<InvalidQuantityException>(() =>
                inventoryService.HasAvaliableStock(product.Id,quantity));
            }
        }

        [Fact]
        public void HasAvaliableStock_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);
                Assert.Throws<ProductNotFoundException>(() =>
                inventoryService.HasAvaliableStock(99, 60));
            }
        }

        // IsLowStock

        [Fact]
        public void IsLowStock_StockBelowMinimum_ReturnsTrue()
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
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 100, 1);

                inventoryService.AddStock(product.Id, 50, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.True(inventoryService.IsLowStock(product.Id));
            }
        }

        [Fact]
        public void IsLowStock_StockEqualToMinimum_ReturnsTrue()
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
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 100, 1);

                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.True(inventoryService.IsLowStock(product.Id));
            }
        }

        [Fact]
        public void IsLowStock_StockAboveMinimum_ReturnsFalse()
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
                var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 100, 1);

                inventoryService.AddStock(product.Id, 200, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.False(inventoryService.IsLowStock(product.Id));
            }
        }

        [Fact]
        public void IsLowStock_MinimumStockDisabled_ReturnsFalse()
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

                Assert.False(inventoryService.IsLowStock(product.Id));
            }
        }
        
        [Fact]
        public void IsLowStock_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                inventoryService.IsLowStock(99));
            }
        }

        // GetLowStockProducts

        [Fact]
        public void GetLowStockProducts_WhenLowStockProductsExist_ReturnsOnlyLowStockProducts()
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
                var product1 = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
                var product2 = productService.CreateProduct("Test Product", "12345678912", "Manufacturer", 10.0m, 100, 1);
                var product3 = productService.CreateProduct("Test Product", "12345678913", "Manufacturer", 10.0m, 200, 1);

                inventoryService.AddStock(product1.Id, 50, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product2.Id, 150, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product2.Id, 50, DateTime.Today.AddDays(10), DateTime.Today);

                var productsStock = inventoryService.GetLowStockProducts();

                Assert.NotEmpty(productsStock);
                Assert.Single(productsStock);
                Assert.Equal(product3.Id, productsStock[0].Id);
            }
        }

        [Fact]
        public void GetLowStockProducts_WhenNoLowStockProductsExist_ReturnsEmptyCollection()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Empty(inventoryService.GetLowStockProducts());
            }
        }

        // AddStock

        [Fact]
        public void AddStock_ValidData_CreatesBatch()
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

                Assert.NotNull(batch);
                Assert.Equal(product.Id, batch.ProductId);
                Assert.Equal(100, batch.Quantity);
                Assert.Equal(DateTime.Today.AddDays(10), batch.ExpirationDate);
                Assert.Equal(DateTime.Today, batch.ReceivedAt);
            }
        }

        [Fact]
        public void AddStock_ValidData_CreatesEntryMovement()
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

                var movements = inventoryService.GetMovementsByBatch(batch.Id);

                Assert.NotEmpty(movements);
                Assert.Equal(MovementType.Entry, movements[0].Type);
            }
        }

        [Fact]
        public void AddStock_NullReceivedAt_UsesCurrentDate()
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

                var batch = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), null);

                Assert.NotNull(batch);
                Assert.Equal(DateTime.Today, batch.ReceivedAt.Date);
            }
        }

        [Fact]
        public void AddStock_MultipleEntries_CreatesSeparateBatches()
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

                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), null);
                inventoryService.AddStock(product.Id, 150, DateTime.Today.AddDays(10), null);
                inventoryService.AddStock(product.Id, 300, DateTime.Today.AddDays(10), null);

                var batches = batchService.GetBatchesByProduct(product.Id);

                Assert.NotEmpty(batches);
                Assert.Equal(3, batches.Count);
            }
        }

        [Fact]
        public void AddStock_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                inventoryService.AddStock(99, 100, DateTime.Today.AddDays(10), DateTime.Today));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void AddStock_InvalidQuantity_ThrowsInvalidQuantityException( int quantity)
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

                Assert.Throws<InvalidQuantityException>(() =>
                inventoryService.AddStock(product.Id, quantity, DateTime.Today.AddDays(10), DateTime.Today));
            }
        }

        [Fact]
        public void AddStock_InvalidExpirationDate_ThrowsInvalidExpirationDateException()
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

                Assert.Throws<InvalidExpirationDateException>(() =>
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)));
                Assert.Throws<InvalidExpirationDateException>(() =>
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(-5), DateTime.Today));
            }
        }

        [Fact]
        public void AddStock_InvalidReceivedDate_ThrowsInvalidReceivedDateException()
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

                Assert.Throws<InvalidReceivedDateException>(() =>
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(20), DateTime.Today.AddDays(10)));
            }
        }

        // OuflowStock

        [Fact]
        public void OutflowStock_ValidQuantity_DecreasesAvailableStock()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                Assert.NotNull(batch);
                Assert.Equal(60, batch.Quantity);
            }
        }

        [Fact]
        public void OutflowStock_ExactQuantity_LeavesStockAtZero()
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
                inventoryService.OutflowStock(product.Id, 50);
                inventoryService.OutflowStock(product.Id, 50);

                Assert.NotNull(batch);
                Assert.Equal(0, batch.Quantity);
            }
        }

        [Fact]
        public void OutflowStock_QuantitySpansMultipleBatches_RemovesCorrectTotalQuantity()
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

                inventoryService.AddStock(product.Id, 75, DateTime.Today.AddDays(5), DateTime.Today);
                inventoryService.AddStock(product.Id, 25, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(20), DateTime.Today);
                inventoryService.OutflowStock(product.Id, 50);
                inventoryService.OutflowStock(product.Id, 50);

                var quantity = inventoryService.GetNumberOfProducts(product.Id);

                Assert.Equal(100, quantity);
            }
        }

        [Fact]
        public void OutflowStock_ValidQuantity_CreatesOutflowMovement()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                var movements = inventoryService.GetMovementsByProduct(product.Id)
                    .Where(sm => sm.Type == MovementType.Outflow);

                Assert.NotEmpty(movements);
                Assert.Equal(2, movements.Count());
            }
        }

        [Fact]
        public void OutflowStock_MultipleBatches_CreatesMovementForEachAffectedBatch()
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

                var batch1 = inventoryService.AddStock(product.Id, 25, DateTime.Today.AddDays(5), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 30, DateTime.Today.AddDays(10), DateTime.Today);
                var batch3 = inventoryService.AddStock(product.Id, 10, DateTime.Today.AddDays(20), DateTime.Today);
                inventoryService.OutflowStock(product.Id, 30);
                inventoryService.OutflowStock(product.Id, 30);

                var movements1 = inventoryService.GetMovementsByBatch(batch1.Id)
                    .Where(sm => sm.Type == MovementType.Outflow);
                var movements2 = inventoryService.GetMovementsByBatch(batch2.Id)
                    .Where(sm => sm.Type == MovementType.Outflow);
                var movements3 = inventoryService.GetMovementsByBatch(batch3.Id)
                    .Where(sm => sm.Type == MovementType.Outflow);

                Assert.NotEmpty(movements1);
                Assert.NotEmpty(movements2);
                Assert.NotEmpty(movements3);
                Assert.Single(movements1);
                Assert.Equal(2, movements2.Count());
                Assert.Single(movements3);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void OutflowStock_InvalidQuantity_ThrowsInvalidQuantityException(int quantity)
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

                Assert.Throws<InvalidQuantityException>(() =>
                inventoryService.OutflowStock(product.Id, quantity));
            }
        }

        [Fact]
        public void OutflowStock_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                inventoryService.OutflowStock(99,10));
            }
        }

        [Fact]
        public void OutflowStock_InsufficientStock_ThrowsInsufficientStockException()
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
                inventoryService.OutflowStock(product.Id, 20);
                inventoryService.OutflowStock(product.Id, 20);

                Assert.Throws<InsufficientStockException>(() =>
                inventoryService.OutflowStock(product.Id, 100));
            }
        }

        // DisposalStock

        [Fact]
        public void DisposalStock_ExpiredStock_DisposeExpiredQuantity()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch1.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                context.SaveChanges();

                inventoryService.DisposalStock(product.Id);
                var quantity = batchService.GetBatchesByProduct(product.Id)
                    .Sum(b => b.Quantity);

                Assert.Equal(0, 
                    batchService.GetBatchesByProduct(product.Id).Sum(b => b.Quantity));
            }
        }

        [Fact]
        public void DisposalStock_ExpiredStock_CreatesDisposalMovement()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch3 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch3.ExpirationDate = DateTime.Today.AddDays(-10);
                context.SaveChanges();

                inventoryService.DisposalStock(product.Id);

                var batches = batchService.GetBatches();
                var disposalBatches = inventoryService.GetMovements()
                    .Where(b => b.Type == MovementType.Disposal);

                Assert.NotEmpty(batches);
                Assert.NotEmpty(disposalBatches);
                Assert.Equal(2, disposalBatches.Count());
            }
        }

        [Fact]
        public void DisposalStock_MultipleExpiredBatches_RemovesAllExpiredStock()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch3 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch3.ExpirationDate = DateTime.Today.AddDays(-10);
                context.SaveChanges();

                inventoryService.DisposalStock(product.Id);

                var batches = batchService.GetBatches();
                var emptyBatches = batches.Where(b => b.Quantity == 0);

                Assert.NotEmpty(batches);
                Assert.NotEmpty(emptyBatches);
                Assert.Equal(2, emptyBatches.Count());
            }
        }

        [Fact]
        public void DisposalStock_MixedBatches_DisposesOnlyExpiredBatches()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch3 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch3.ExpirationDate = DateTime.Today.AddDays(-10);
                context.SaveChanges();

                inventoryService.DisposalStock(product.Id);
                var batches = batchService.GetBatches();
                var disposalBatches = inventoryService.GetMovements()
                    .Where(b => b.Type == MovementType.Disposal);

                Assert.NotEmpty(batches);
                Assert.NotEmpty(disposalBatches);
                Assert.Equal(2, disposalBatches.Count());
                Assert.Equal(100, batch1.Quantity);
                Assert.Equal(0, batch2.Quantity);
                Assert.Equal(0, batch3.Quantity);
            }
        }

        [Fact]
        public void DisposalStock_NoExpiredStock_ThrowsNoExpiredStockException()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                Assert.Throws<NoExpiredStockException>(() =>
                inventoryService.DisposalStock(product.Id));
            }
        }

        [Fact]
        public void DisposalStock_InvalidProductId_ThrowsProductNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Throws<ProductNotFoundException>(() =>
                inventoryService.DisposalStock(99));
            }
        }

        // GetExpiredBatches

        [Fact]
        public void GetExpiredBatches_WhenExpiredBatchesExist_ReturnsExpiredBatches()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch1.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                context.SaveChanges();

                var expiredBatches = inventoryService.GetExpiredBatches();

                Assert.NotEmpty(expiredBatches);
                Assert.Equal(2, expiredBatches.Count());
            }
        }

        [Fact]
        public void GetExpiredBatches_IgnoresZeroQuantityBatches()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch1.ExpirationDate = DateTime.Today.AddDays(-10);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                batch2.ExpirationDate = DateTime.Today.AddDays(-10);
                batch2.Quantity = 0;
                context.SaveChanges();

                var expiredBatches = inventoryService.GetExpiredBatches();
                var expiredQuantity = expiredBatches
                    .Where(b => b.Quantity == 0);

                Assert.NotEmpty(expiredBatches);
                Assert.Single(expiredBatches);
                Assert.Empty(expiredQuantity);
            }
        }

        [Fact]
        public void GetExpiredBatches_WhenNoExpiredBatchesExist_ReturnsEmptyCollection()
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

                var expiredBatches = inventoryService.GetExpiredBatches();

                Assert.Empty(inventoryService.GetExpiredBatches());
            }
        }

        // GetBatchesNearExpiration

        [Fact]
        public void GetBatchesNearExpiration_WithinRange_ReturnsBatches()
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

                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(5), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(25), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(50), DateTime.Today);

                var expireIn7Days = inventoryService.GetBatchesNearExpiration(7);
                var expireIn30Days = inventoryService.GetBatchesNearExpiration(30);

                Assert.NotEmpty(expireIn7Days);
                Assert.NotEmpty(expireIn30Days);
                Assert.Single(expireIn7Days);
                Assert.Equal(3, expireIn30Days.Count);
            }
        }

        [Fact]
        public void GetBatchesNearExpiration_OutsideRange_DoesNotReturnBatches()
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
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(25), DateTime.Today);
                inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(50), DateTime.Today);

                var expireIn7Days = inventoryService.GetBatchesNearExpiration(7);

                Assert.Empty(expireIn7Days);
            }
        }

        // GetDayUntilExpiration

        [Fact]
        public void GetDayUntilExpiration_ExistingBatch_ReturnsCorrectDays()
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

                var batch1 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(5), DateTime.Today);
                var batch2 = inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

                var expireIn7Days = inventoryService.GetBatchesNearExpiration(7);
                var expireIn30Days = inventoryService.GetBatchesNearExpiration(30);

                Assert.Equal(5, inventoryService.GetDayUntilExpiration(batch1.Id));
                Assert.Equal(10, inventoryService.GetDayUntilExpiration(batch2.Id));
            }
        }

        [Fact]
        public void GetDayUntilExpiration_InvalidBatchId_ThrowsBatchNotFoundException()
        {
            var (connection, context) = CreateTestContext();
            using (connection)
            using (context)
            {
                var inventoryService = new InventoryService(context);

                Assert.Throws<BatchNotFoundException>(() =>
                inventoryService.GetDayUntilExpiration(99));
            }
        }
    }
}
