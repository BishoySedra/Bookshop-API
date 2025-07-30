# Bookshop API

Bookshop API is a RESTful web service built using ASP.NET Core 8 Web API. It provides functionality for managing books and categories in a bookshop. The API supports CRUD operations, caching, pagination, and versioning, making it a robust solution for bookshop management.

---

## Agenda

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
  - [1. Clone the Repository](#1-clone-the-repository)
  - [2. Configure the Database](#2-configure-the-database)
  - [3. Run Database Migrations](#3-run-database-migrations)
  - [4. Build and Run the Application](#4-build-and-run-the-application)
  - [5. Access the API](#5-access-the-api)
- [Usage](#usage)
  - [Example Endpoints](#example-endpoints)
    - [Categories](#categories)
    - [Products](#products)
- [Docker Setup](#docker-setup)
- [Project Structure](#project-structure)
- [Testing](#testing)

---

## Features

- **CRUD Operations**: Manage books and categories with full Create, Read, Update, and Delete functionality.
- **Caching**: Response caching and memory caching for improved performance.
- **Pagination**: Efficient handling of large datasets with paginated responses.
- **API Versioning**: Support for multiple API versions using URL-based versioning.
- **Swagger Documentation**: Interactive API documentation for easy testing and exploration.
- **Entity Framework Core**: Database interaction using EF Core with migrations and seed data.
- **Docker Support**: Pre-configured `docker-compose.yml` for running the project with SQL Server.

---

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or Docker for running the database.

---

## Setup

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/Bookshop-API.git
cd Bookshop-API
```

### 2. Configure the Database
Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BookShop;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```

### 3. Run Database Migrations
Navigate to the API directory and apply migrations:
```bash
dotnet ef database update
```

### 4. Build and Run the Application
```bash
dotnet run
```

### 5. Access the API
- **API Base URL**: `https://localhost:5104` or `http://localhost:5104`
- **Swagger Documentation**: `https://localhost:5104/swagger`

---

## Usage

Use an API client like Postman to test the endpoints. Refer to the Swagger UI for detailed API documentation and testing.

### Example Endpoints

#### Categories
- `GET /api/Category`: Retrieve all categories.
- `POST /api/Category`: Create a new category.
- `PUT /api/Category/{id}`: Update an existing category.
- `DELETE /api/Category/{id}`: Delete a category.

#### Products
- `GET /api/v1/Product`: Retrieve all products (v1).
- `POST /api/v1/Product`: Create a new product (v1).
- `PATCH /api/v1/Product/{id}`: Partially update a product (v1).

---

## Docker Setup

To run the project with Docker:

1. Ensure Docker is installed and running.
2. Use the provided `docker-compose.yml` file to start the SQL Server container:
   ```bash
   docker-compose up -d
   ```
3. Update the connection string in `appsettings.json` to match the Docker container configuration.

---

## Project Structure

- **API**: Contains controllers, DTOs, and the main application logic.
- **Core**: Defines interfaces and shared parameters.
- **DataAccess**: Implements repositories, migrations, and database context.
- **Models**: Contains entity definitions and configurations.

## Testing

Unit tests for the API are located in the API.Tests project. To run the tests, use the following command:

```bash
dotnet test
```