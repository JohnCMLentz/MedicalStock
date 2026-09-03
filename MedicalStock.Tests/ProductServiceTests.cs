using MedicalStock.Data;
using MedicalStock.Exceptions;
using MedicalStock.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock.Tests;

public class ProductServiceTests
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
    public void CreateProduct_ValidData_CreatesProduct()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Assert
            Assert.Equal("Test Product", product.Name);
            Assert.Equal("12345678911", product.Barcode);
            Assert.Equal("Manufacturer", product.Manufacturer);
            Assert.Equal(10.0m, product.Price);
            Assert.Equal(0, product.MinimumStock);
            Assert.Equal(1, product.CategoryId);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateProduct_InvalidName_ThrowsInvalidProductNameException(string name)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<InvalidProductNameException>(() =>
            productService.CreateProduct(name, "12345678911", "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateProduct_InvalidBarcode_ThrowsInvalidProductBarcodeException(string barcode)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<InvalidProductBarcodeException>(() =>
            productService.CreateProduct("Test Product", barcode, "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateProduct_InvalidManufacturer_ThrowsInvalidProductManufacturerException(string manufacturer)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<InvalidProductManufacturerException>(() =>
            productService.CreateProduct("Test Product", "12345678911", manufacturer, 10.0m, 0, 1));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CreateProduct_InvalidPrice_ThrowsInvalidProductPriceException(decimal price)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<InvalidProductPriceException>(() =>
            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", price, 0, 1));
        }
    }

    [Fact]
    public void CreateProduct_InvalidMinimumStock_ThrowsInvalidMinimumStockException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<InvalidProductMinimumStockException>(() =>
            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, -10, 1));
        }
    }

    [Fact]
    public void CreateProduct_NonExistingCategory_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);

            // Act + Assert
            Assert.Throws<CategoryNotFoundException>(() =>
            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Fact]
    public void CreateProduct_DuplicateBarcode_ThrowsDuplicateBarcodeException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act
            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Assert
            Assert.Throws<DuplicateBarcodeException>(() =>
            productService.CreateProduct("Test Product 2", "12345678911", "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Fact]
    public void GetProductById_ExistingId_ReturnsProduct()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            var foundProduct = productService.GetProductById(product.Id);

            // Act + Assert
            Assert.NotNull(foundProduct);
            Assert.Equal(product.Id, foundProduct.Id);
        }
    }

    [Fact]
    public void GetProductById_NonExistingId_ReturnsNull()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);


            // Act + Assert
            Assert.Null(productService.GetProductById(999));
        }
    }

    [Fact]
    public void GetProductByBarcode_ExistingBarcode_ReturnsProduct()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            var foundProduct = productService.GetProductByBarcode(product.Barcode);

            // Act + Assert
            Assert.NotNull(foundProduct);
            Assert.Equal(product.Barcode, foundProduct.Barcode);
        }
    }

    [Fact]
    public void GetProductByBarcode_NonExistingBarcode_ReturnsNull()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);

            // Act + Assert
            Assert.Null(productService.GetProductByBarcode("999999"));
        }
    }

    [Fact]
    public void GetProducts_WhenProductsExist_ReturnsAllProducts()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            productService.CreateProduct("Test Product", "12345678912", "Manufacturer", 10.0m, 0, 1);
            productService.CreateProduct("Test Product", "12345678913", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Equal(3, productService.GetProducts().Count);
        }
    }

    [Fact]
    public void GetProducts_WhenNoProductsExist_ReturnsEmptyCollection()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);

            // Act + Assert
            Assert.Empty(productService.GetProducts());
        }
    }
    
    // Update

    [Fact]
    public void UpdateProduct_ValidData_UpdatesProduct()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act
            productService.UpdateProduct(product.Id, "new name", "12345678911", null, null, null, null);
            var updateProduct = productService.GetProductById(product.Id);

            // Assert
            Assert.NotNull(updateProduct);
            Assert.True(updateProduct.Name == "new name");
        }
    }

    [Fact]
    public void UpdateProduct_AllOptionalParametersNull_KeepsProductUnchanged()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act
            productService.UpdateProduct(product.Id, null, null, null, null, null, null);
            var updateProduct = productService.GetProductById(product.Id);

            // Assert
            Assert.NotNull(updateProduct);
            Assert.True(product.Name == updateProduct.Name);
            Assert.True(product.Barcode == updateProduct.Barcode);
            Assert.True(product.Manufacturer == updateProduct.Manufacturer);
            Assert.True(product.Price == updateProduct.Price);
            Assert.True(product.MinimumStock == updateProduct.MinimumStock);
            Assert.True(product.CategoryId == updateProduct.CategoryId);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateProduct_InvalidName_ThrowsInvalidProductNameException(string name)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<InvalidProductNameException>(() =>
            productService.UpdateProduct(product.Id,name, "12345678911", "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateProduct_InvalidBarcode_ThrowsInvalidBarcodeException(string barcode)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<InvalidProductBarcodeException>(() =>
            productService.UpdateProduct(product.Id,"Test Product", barcode, "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Fact]
    public void UpdateProduct_SameBarcode_DoesNotThrowException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            productService.UpdateProduct(product.Id, null, "12345678911", null, null, null, null);

            // Act + Assert
            Assert.NotNull(product);
            Assert.Equal("12345678911", product.Barcode);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateProduct_InvalidManufacturer_ThrowsInvalidManufacturerException(string manufacturer)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<InvalidProductManufacturerException>(() =>
            productService.UpdateProduct(product.Id, "Test Product", "12345678911", manufacturer, 10.0m, 0, 1));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void UpdateProduct_InvalidPrice_ThrowsInvalidPriceException(decimal price)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<InvalidProductPriceException>(() =>
            productService.UpdateProduct(product.Id, "Test Product", "12345678911", "Manufacturer", price, 0, 1));
        }
    }

    [Fact]
    public void UpdateProduct_InvalidMinimumStock_ThrowsInvalidMinimumStockException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<InvalidProductMinimumStockException>(() =>
            productService.UpdateProduct(product.Id, "Test Product", "12345678911", "Manufacturer", 10.0m, -10, 1));
        }
    }

    [Fact]
    public void UpdateProduct_NonExistingCategory_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act + Assert
            Assert.Throws<CategoryNotFoundException>(() =>
            productService.UpdateProduct(product.Id, "Test Product", "12345678911", "Manufacturer", 10.0m, 0, 99));
        }
    }

    [Fact]
    public void UpdateProduct_DuplicateBarcode_ThrowsDuplicateBarcodeException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");

            // Act
            productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            var product = productService.CreateProduct("Test Product", "00000000000", "Manufacturer", 10.0m, 0, 1);

            // Assert
            Assert.Throws<DuplicateBarcodeException>(() =>
            productService.UpdateProduct(product.Id, "Test Product 2", "12345678911", "Manufacturer", 10.0m, 0, 1));
        }
    }

    [Fact]
    public void UpdateProduct_NonExistingId_ThrowsProductNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);

            // Act + Assert
            Assert.Throws<ProductNotFoundException>(() =>
            productService.UpdateProduct(999, "Test Product", "12345678911", "Manufacturer", 10.0m, 0, 99));
        }
    }

    // Delete

    [Fact]
    public void DeleteProduct_NonExistingId_ThrowsProductNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);

            // Act + Assert
            Assert.Throws<ProductNotFoundException>(() =>
            productService.DeleteProduct(999));
        }
    }

    [Fact]
    public void DeleteProduct_ExistingProductWithoutBatches_DeletesProduct()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);

            // Act
            productService.DeleteProduct(product.Id);

            // Assert
            Assert.Null(productService.GetProductById(product.Id));
        }
    }

    [Fact]
    public void DeleteProduct_ProductWithBatches_ThrowsProductHasBatchesException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            var inventoryService = new InventoryService(context);

            categoryService.CreateCategory("Test Category");
            var product = productService.CreateProduct("Test Product", "12345678911", "Manufacturer", 10.0m, 0, 1);
            inventoryService.AddStock(product.Id, 100, DateTime.Today.AddDays(10), DateTime.Today);

            // Act + Assert
            Assert.Throws<ProductHasBatchesException>(() =>
            productService.DeleteProduct(product.Id));
        }
    }

}

