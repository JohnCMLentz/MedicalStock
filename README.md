# Medical Stock Management System

A medical and pharmaceutical stock management system developed in C# with .NET, Entity Framework Core, and SQLite.

This project was created as a learning and portfolio project, with a focus on object-oriented programming, relational database modeling, CRUD operations, Entity Framework Core, and inventory management business rules.

---

## Project Status

**In Development**

The project is currently under active development. The core database structure, entity relationships, CRUD services, stock movements, FEFO stock rotation, expiration rules, minimum-stock monitoring, disposal operations, and domain exception handling have been implemented.

The next development stage will focus on automated testing with xUnit, followed by further application and interface improvements.

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
- Develop a structured and maintainable application
- Build a portfolio project following common software development practices

---

## Technologies

- C#
- .NET
- Entity Framework Core
- SQLite
- LINQ
- Visual Studio
- Git / GitHub

---

## Current Architecture

The project currently follows a simple separation of responsibilities:

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
```

The structure may evolve as new features are implemented.

---

## Domain Model

The current database model consists of four main entities.

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
`-- ReceivedAt
```

A product can have multiple batches, allowing different quantities and expiration dates to be tracked independently.

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

When a product is removed from inventory, the system prioritizes valid batches with the earliest expiration date. Expired batches are excluded from regular stock outflows.

For example:

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

If 60 units are removed:

Batch A -> 50 removed
Batch B -> 10 removed

Remaining stock:

Batch A -> 0
Batch B -> 90
Batch C -> 80

This approach is particularly appropriate for perishable and pharmaceutical products.

---

## Inventory Business Rules

The system currently implements the following business rules:

- Stock entries create new batches instead of modifying existing batches.
- Every stock entry generates an `Entry` stock movement.
- Available stock only includes batches with a positive quantity that have not expired.
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
- Entities with historical relationships are protected from deletion.
- Invalid business operations are represented by domain-specific exceptions.

### Expiration and Disposal

A batch remains valid throughout its expiration date and becomes expired on the following day. Expired stock is excluded from available stock and cannot be used in regular outflows, but it remains stored until an explicit disposal operation is performed.

### Minimum Stock

- `MinimumStock <= 0`: low-stock alert disabled
- `AvailableStock > MinimumStock`: normal stock
- `AvailableStock <= MinimumStock`: low stock
- `AvailableStock == 0`: out of stock

Only non-expired stock is considered available.

### Domain Exceptions

Services use domain-specific exceptions instead of relying only on `false` or `null` results. These exceptions cover invalid data, duplicate barcodes, invalid quantities or dates, missing entities, insufficient stock, missing expired stock for disposal, and protected deletion operations.

---

## Database

The application currently uses SQLite as its database provider through Entity Framework Core.

The database file is:

MedicalStock.db

Entity Framework Core is responsible for mapping the C# entities to the relational database.

---

## Entity Framework Core

The project uses Entity Framework Core migrations to manage database changes.

The initial database was created using:

Add-Migration InitialCreate
Update-Database

Future changes to the data model will be managed through additional migrations.

Example:

Add-Migration AddNewFeature
Update-Database

---

## Current EF Core Configuration

The SQLite database is configured through AppDbContext.

The current context contains:

- DbSet<Product>
- DbSet<Batch>
- DbSet<Category>
- DbSet<StockMovement>

Entity relationships are explicitly configured using EF Core Fluent API.

Current relationships:

Category 1:N Product
Product 1:N Batch

The project also includes database constraints such as:

- Required product fields
- Maximum string lengths
- Unique product barcodes
- Decimal precision for product prices
- Foreign key relationships
- Restricted deletion behavior for related entities

---

## Planned Features

### Product Management

- [x] Create products
- [x] List products
- [x] Search products
- [x] Update products
- [x] Delete products with relationship protection
- [x] Barcode validation

### Category Management

- [x] Create categories
- [x] List categories
- [x] Update categories
- [x] Delete categories with relationship protection

### Batch Management

- [x] Register product batches
- [x] Track batch quantities
- [x] Track expiration dates
- [x] Track batch receiving dates
- [x] Prevent invalid quantities
- [x] Identify expired batches

### Inventory

- [x] Add stock
- [x] Remove stock through outflow operations
- [x] Automatic FEFO stock selection
- [x] Calculate available stock
- [x] Prevent insufficient stock removal
- [x] Identify batches near expiration
- [x] Detect low-stock products
- [x] Dispose of expired stock
- [ ] Graphical expiration warnings

### Stock History

- [x] Track stock entries
- [x] Track stock removals and disposals
- [x] Record dates and quantities
- [x] Track affected batches

### User Interface

- [ ] Graphical user interface
- [ ] Input validation
- [ ] Clear domain error messages
- [ ] Low-stock alerts
- [ ] Expiration and disposal alerts

---

## Future Improvements

Possible improvements after the initial version:

- SQL Server support
- Improved application architecture
- Repository/Service patterns where appropriate
- Automated tests with xUnit
- Logging
- Better inventory reports
- Low-stock alerts
- Expiration alerts
- Barcode scanner integration
- Graphical user interface
- API development

---

## Learning Objectives

This project is also being used to study and practice:

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
- Domain exceptions
- Clean and maintainable code
- Automated testing with xUnit

---

## Project Development

This project is being developed incrementally.

The current development process is focused on understanding each component before implementing the next one, rather than building the entire application at once.

The current backend includes CRUD services, stock movement tracking, FEFO logic, expiration rules, minimum-stock monitoring, disposal operations, and domain exception handling.

The next development stage will focus on automated tests with xUnit before expanding the application further.

---

## Author

Developed as a C#/.NET learning and portfolio project.
