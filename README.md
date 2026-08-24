# Medical Stock Management System

A medical and pharmaceutical stock management system developed in C# with .NET, Entity Framework Core, and SQLite.

This project was created as a learning and portfolio project, with a focus on object-oriented programming, relational database modeling, CRUD operations, Entity Framework Core, and inventory management business rules.

---

## Project Status

**In Development**

The project is currently in the initial development stage. The database structure and entity relationships have been defined, and the SQLite database has been successfully created using Entity Framework Core migrations.

New features and improvements will be added progressively.

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

MedicalStock
|
|-- Data
|   `-- AppDbContext.cs
|
|-- Models
|   |-- Product.cs
|   |-- Batch.cs
|   `-- Category.cs
|
|-- Migrations
|   `-- InitialCreate
|
|-- MedicalStock.db
|
`-- Program.cs

The structure may evolve as new features are implemented.

---

## Domain Model

The current database model consists of three main entities.

### Category

Represents a category used to organize products.

Category
|-- Id
`-- Name

### Product

Represents a product stored in the medical/pharmaceutical inventory.

Product
|-- Id
|-- Name
|-- Barcode
|-- Manufacturer
|-- Price
`-- CategoryId

A product belongs to one category and can have multiple batches.

### Batch

Represents a specific batch of a product.

Batch
|-- Id
|-- ProductId
|-- Quantity
|-- ExpirationDate
`-- ReceivedAt

A product can have multiple batches, allowing different quantities and expiration dates to be tracked independently.

---

## Entity Relationships

The current relationships are:

Category
   |
   | 1:N
   v
Product
   |
   | 1:N
   v
Batch

### Category -> Product

One category can contain multiple products.

### Product -> Batch

One product can contain multiple batches.

This relationship is essential for the inventory management system because different batches of the same product can have different expiration dates and quantities.

---

## FEFO Inventory Management

The inventory system will use FEFO (First Expired, First Out) as its stock rotation strategy.

When a product is removed from inventory, the system will prioritize the batch with the earliest expiration date.

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

- [ ] Create products
- [ ] List products
- [ ] Search products
- [ ] Update products
- [ ] Delete products
- [ ] Barcode validation

### Category Management

- [ ] Create categories
- [ ] List categories
- [ ] Update categories
- [ ] Delete categories

### Batch Management

- [ ] Register product batches
- [ ] Track batch quantities
- [ ] Track expiration dates
- [ ] Track batch receiving dates
- [ ] Prevent invalid quantities
- [ ] Identify expired batches

### Inventory

- [ ] Add stock
- [ ] Remove stock
- [ ] Automatic FEFO stock selection
- [ ] Calculate available stock
- [ ] Prevent insufficient stock removal
- [ ] Expiration warnings

### Stock History

- [ ] Track stock entries
- [ ] Track stock removals
- [ ] Record dates and quantities
- [ ] Track affected batches

### User Interface

- [ ] Console-based menu
- [ ] Input validation
- [ ] Error handling
- [ ] Clear status messages

---

## Future Improvements

Possible improvements after the initial version:

- SQL Server support
- Improved application architecture
- Repository/Service patterns where appropriate
- Automated tests
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
- Clean and maintainable code

---

## Project Development

This project is being developed incrementally.

The current development process is focused on understanding each component before implementing the next one, rather than building the entire application at once.

Future updates will expand the database model, business logic, CRUD functionality, and inventory management features.

---

## Author

Developed as a C#/.NET learning and portfolio project.
