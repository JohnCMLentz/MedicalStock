# Medical Stock Management System

A medical and pharmaceutical stock management system developed in C# with .NET, Entity Framework Core, SQLite, and xUnit.

This project was created as a learning and portfolio project, with a focus on object-oriented programming, relational database modeling, CRUD operations, Entity Framework Core, inventory management business rules, domain exceptions, and automated testing.

---

## Project Status

**Version 1.0.0 — Core project completed**

The first stable version of the project is complete.

The application includes the core database structure, entity relationships, CRUD services, stock movements, FEFO stock rotation, expiration rules, minimum-stock monitoring, disposal operations, domain-specific exception handling, Entity Framework Core migrations, and automated tests with xUnit.

The project is intentionally being closed at this stage as a console-based learning and portfolio project. Future development concepts, such as REST APIs, ASP.NET Core, SQL Server, and more advanced application architecture, will be explored in a separate project.

---

## Project Goals

The main goals of this project are:

- Practice C# and object-oriented programming
- Learn and apply Entity Framework Core
- Work with relational databases using SQLite
- Practice CRUD operations
- Understand database relationships and foreign keys
- Implement inventory management rules
- Implement FEFO (First Expired, First Out)
- Track stock movement history
- Apply domain-specific exception handling
- Practice migrations and database versioning
- Practice automated testing with xUnit
- Develop a structured and maintainable application
- Build a portfolio project following common software development practices

---

## Technologies

- C#
- .NET
- Entity Framework Core
- SQLite
- LINQ
- xUnit
- Microsoft.NET.Test.Sdk
- coverlet.collector
- Visual Studio
- Git / GitHub

---

## Project Structure

The project follows a simple separation of responsibilities:

```text
MedicalStock
|
|-- Data
|   `-- AppDbContext.cs
|
|-- Models
|   |-- Product.cs
|   |-- Batch.cs
|   |-- Category.cs
|   |-- StockMovement.cs
|   `-- Enums
|       `-- MovementType.cs
|
|-- Services
|   |-- ProductService.cs
|   |-- CategoryService.cs
|   |-- BatchService.cs
|   `-- InventoryService.cs
|
|-- Exceptions
|   `-- Domain exceptions
|
|-- Migrations
|
|-- MedicalStock.db
|
`-- Program.cs

MedicalStock.Tests
|
|-- CategoryServiceTests.cs
|-- ProductServiceTests.cs
|-- BatchServiceTests.cs
`-- InventoryServiceTests.cs
```

---

## Domain Model

The database model consists of four main entities.

### Category

Represents a category used to organize products.

```text
Category
|-- Id
`-- Name
```

### Product

Represents a product stored in the medical/pharmaceutical inventory.

```text
Product
|-- Id
|-- Name
|-- Barcode
|-- Manufacturer
|-- Price
|-- MinimumStock
`-- CategoryId
```

A product belongs to one category and can have multiple batches.

### Batch

Represents a specific batch of a product.

```text
Batch
|-- Id
|-- ProductId
|-- Quantity
|-- ExpirationDate
|-- ReceivedAt
`-- IsActive
```

A product can have multiple batches, allowing different quantities and expiration dates to be tracked independently.

`IsActive` represents the administrative state of the batch. Stock availability is still determined by quantity and expiration rules.

### StockMovement

Represents an inventory movement associated with a specific batch.

```text
StockMovement
|-- Id
|-- BatchId
|-- Quantity
|-- Type
`-- MovementDate
```

Stock movements preserve inventory history for entries, outflows, and disposals.

---

## Entity Relationships

The current relationships are:

```text
Category
   |
   | 1:N
   v
Product
   |
   | 1:N
   v
Batch
   |
   | 1:N
   v
StockMovement
```

### Category -> Product

One category can contain multiple products.

### Product -> Batch

One product can contain multiple batches.

This relationship is essential for the inventory management system because different batches of the same product can have different expiration dates and quantities.

### Batch -> StockMovement

One batch can contain multiple stock movements. Each movement identifies the affected batch, quantity, movement type, and movement date.

---

## FEFO Inventory Management

The inventory system uses FEFO (First Expired, First Out) as its stock rotation strategy.

When a product is removed from inventory, the system prioritizes active and valid batches with the earliest expiration date. Expired batches are excluded from regular stock outflows.

For example:

```text
Paracetamol

Batch A
Quantity: 50
Expiration: 2026-09-10

Batch B
Quantity: 100
Expiration: 2027-01-15

Batch C
Quantity: 80
Expiration: 2027-08-20
```

If 60 units are removed:

```text
Batch A -> 50 removed
Batch B -> 10 removed
```

Remaining stock:

```text
Batch A -> 0
Batch B -> 90
Batch C -> 80
```

This approach is particularly appropriate for perishable and pharmaceutical products.

---

## Inventory Business Rules

The system implements the following business rules:

- Stock entries create new batches instead of modifying existing batches.
- Every stock entry generates an `Entry` stock movement.
- Available stock only includes active batches with a positive quantity that have not expired.
- Products remain valid throughout their expiration date.
- Regular stock outflows cannot use expired batches.
- Stock outflows follow FEFO.
- Outflows affecting multiple batches generate one movement for each affected batch.
- Expired stock remains stored until explicitly discarded.
- Disposal operations generate `Disposal` stock movements.
- Products can define a minimum stock level.
- Stock equal to or below the configured minimum is considered low stock.
- A minimum stock value of zero disables the low-stock alert.
- Product prices must be greater than zero.
- Categories and products with related historical data are protected from invalid deletion operations.
- Batches can be administratively deactivated according to the batch rules.
- Invalid business operations are represented by domain-specific exceptions.

### Expiration and Disposal

A batch remains valid throughout its expiration date and becomes expired on the following day.

Expired stock is excluded from available stock and cannot be used in regular outflows, but it remains stored until an explicit disposal operation is performed.

When expired stock is disposed, the affected batch quantities are reduced and `Disposal` stock movements preserve the operation in the inventory history.

### Minimum Stock

- `MinimumStock <= 0`: low-stock alert disabled
- `AvailableStock > MinimumStock`: normal stock
- `AvailableStock <= MinimumStock`: low stock
- `AvailableStock == 0`: out of stock

Only active and non-expired stock with a positive quantity is considered available.

### Domain Exceptions

Services use domain-specific exceptions for invalid business operations.

These exceptions cover scenarios such as:

- Invalid names or required values
- Duplicate category names or product barcodes
- Invalid prices
- Invalid quantities
- Invalid expiration or receiving dates
- Missing categories, products, or batches
- Insufficient available stock
- Missing expired stock during disposal operations
- Protected deletion or deactivation operations

Queries that are designed to search for optional data may still return `null` or empty collections when appropriate.

---

## Inventory Queries and Monitoring

The inventory service also provides query operations for stock monitoring, including:

- Retrieve all stock movements
- Retrieve movements by batch
- Retrieve movements by product
- Calculate the available quantity of a product
- Check whether enough stock is available for an operation
- Detect low-stock products
- Retrieve expired batches
- Retrieve batches near expiration
- Calculate the number of days until a batch expires

---

## Database

The application uses SQLite as its database provider through Entity Framework Core.

The database file is:

```text
MedicalStock.db
```

Entity Framework Core maps the C# entities to the relational database and manages schema evolution through migrations.

---

## Entity Framework Core

The project uses Entity Framework Core migrations to manage database changes.

The initial database can be created and updated through the Package Manager Console:

```powershell
Update-Database
```

When the model changes, a new migration can be created and applied:

```powershell
Add-Migration MigrationName
Update-Database
```

The project currently contains the migrations required to build the database schema used by the application.

---

## EF Core Configuration

The SQLite database is configured through `AppDbContext`.

The context contains:

- `DbSet<Product>`
- `DbSet<Batch>`
- `DbSet<Category>`
- `DbSet<StockMovement>`

Entity relationships are explicitly configured using EF Core Fluent API.

Current relationships:

```text
Category 1:N Product
Product 1:N Batch
Batch 1:N StockMovement
```

The project also includes database constraints and configuration such as:

- Required entity fields
- Maximum string lengths where applicable
- Unique category names
- Unique product barcodes
- Decimal precision for product prices
- Foreign key relationships
- Restricted deletion behavior for related entities

`AppDbContext` also supports externally supplied `DbContextOptions`, allowing automated tests to use isolated SQLite in-memory databases instead of the application's main database file.

---

## Testing

The project includes automated tests using xUnit.

The test suite covers the main service behaviors and business rules, including:

- Category creation, update, search, and deletion
- Category validation and duplicate-name protection
- Product creation, update, search, and deletion
- Product validation and barcode uniqueness
- Batch creation, update, lookup, FEFO ordering, and deactivation
- Stock entries
- Stock outflows
- Stock removal across multiple batches
- Stock movement creation and history
- Available-stock calculations
- Minimum-stock monitoring
- Expiration rules
- Expired-stock disposal
- Domain exception handling

### Test Database

Tests use SQLite in-memory databases.

Each test creates an isolated database connection and schema, allowing tests to execute against SQLite behavior without modifying the application's main `MedicalStock.db`.

Conceptually:

```text
Test starts
    |
    v
Open SQLite in-memory connection
    |
    v
Create AppDbContext
    |
    v
EnsureCreated()
    |
    v
Execute test
    |
    v
Dispose context and connection
    |
    v
Temporary database is removed
```

This keeps tests independent and prevents data from one test from affecting another.

### Testing Technologies

- xUnit
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.NET.Test.Sdk
- xunit.runner.visualstudio
- coverlet.collector

### Test Organization

Tests are separated by service:

```text
MedicalStock.Tests
|
|-- CategoryServiceTests
|-- ProductServiceTests
|-- BatchServiceTests
`-- InventoryServiceTests
```

Test names follow a behavior-oriented convention:

```text
Method_Scenario_ExpectedResult
```

For example:

```text
OutflowStock_InsufficientStock_ThrowsInsufficientStockException
GetBatchesByFEFO_WhenBatchesExist_ReturnsBatches
IsLowStock_StockBelowMinimum_ReturnsTrue
```

The test suite also uses both `[Fact]` and `[Theory]` with `[InlineData]` when the same expected behavior must be validated against multiple input values.

In addition to automated service tests, the main inventory workflow was manually validated end-to-end using scenarios that combine category creation, product creation, multiple stock entries, stock outflow, FEFO behavior, movement history, expiration, and disposal.

---

## Implemented Features

### Product Management

- [x] Create products
- [x] List products
- [x] Search products by ID and barcode
- [x] Update products
- [x] Delete products with relationship protection
- [x] Validate product data
- [x] Enforce unique barcodes
- [x] Configure minimum stock

### Category Management

- [x] Create categories
- [x] List categories
- [x] Search categories
- [x] Update categories
- [x] Delete categories with relationship protection
- [x] Enforce unique category names

### Batch Management

- [x] Register product batches
- [x] Track batch quantities
- [x] Track expiration dates
- [x] Track batch receiving dates
- [x] Prevent invalid quantities and dates
- [x] Identify expired batches
- [x] Order valid stock using FEFO
- [x] Administratively deactivate batches

### Inventory

- [x] Add stock
- [x] Remove stock through outflow operations
- [x] Automatic FEFO stock selection
- [x] Calculate available stock
- [x] Prevent insufficient stock removal
- [x] Identify batches near expiration
- [x] Detect low-stock products
- [x] Dispose of expired stock
- [x] Ignore expired stock when calculating available inventory

### Stock History

- [x] Track stock entries
- [x] Track stock removals
- [x] Track expired-stock disposals
- [x] Record movement dates and quantities
- [x] Track affected batches
- [x] Retrieve movements by batch and product

### Automated Testing

- [x] Category service tests
- [x] Product service tests
- [x] Batch service tests
- [x] Inventory service tests
- [x] SQLite in-memory test databases
- [x] Domain exception scenarios
- [x] Manual end-to-end workflow validation

---

## Possible Future Improvements

This version intentionally focuses on the console application, domain rules, persistence, and testing.

Possible future extensions or related projects include:

- ASP.NET Core Web API
- REST endpoints
- DTOs
- Dependency Injection through ASP.NET Core
- Swagger / OpenAPI
- SQL Server
- Authentication and authorization
- Logging
- Improved inventory reports
- Barcode scanner integration
- Graphical or web user interface
- API and integration testing

The next learning project is planned to revisit the MedicalStock domain as a Web API, using the knowledge gained from this console version as its foundation.

---

## Learning Objectives

This project was used to study and practice:

- Object-Oriented Programming
- Encapsulation
- Classes and relationships
- Collections
- LINQ
- CRUD
- Relational database design
- Primary and foreign keys
- Entity Framework Core
- Fluent API
- Migrations
- SQLite
- Database constraints
- Inventory business rules
- FEFO stock management
- Stock movement history
- Expiration and disposal management
- Minimum-stock monitoring
- Domain exceptions
- Automated testing with xUnit
- SQLite in-memory testing
- Test isolation
- Behavior-oriented test naming
- Git and GitHub
- Clean and maintainable code

---

## Project Development

The project was developed incrementally, with each part implemented only after the previous concepts and business rules were understood and tested.

The development process evolved through:

```text
Domain entities
    |
    v
Database relationships
    |
    v
CRUD services
    |
    v
Inventory rules
    |
    v
Stock movement history
    |
    v
Expiration and disposal
    |
    v
Domain exceptions
    |
    v
Automated tests
    |
    v
Final validation
```

This incremental approach allowed the project to evolve from a simple CRUD application into a stock-management system with explicit domain rules, persistence, inventory history, and automated validation.

---

## Version

**v1.0.0**

First stable version of the MedicalStock console application.

---

## Author

Developed as a C#/.NET learning and portfolio project.
