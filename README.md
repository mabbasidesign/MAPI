# MAPI

This is a .NET 8 Web API project for managing product orders.

## Project Structure

- **Controllers/**: Contains API controllers (e.g., `ProductOrder.cs`).
- **Data/**: Entity Framework Core database context (`AppDbContext.cs`).
- **Helper/**: Custom attributes and helpers (e.g., `StatusValidationAttribute.cs`).
- **IServices/**: Service interfaces (e.g., `IProductService.cs`).
- **Models/**: Data models (e.g., `Products.cs`).
- **Services/**: Service implementations (e.g., `ProductService.cs`).
- **Properties/**: Project properties and launch settings.
- **appsettings.json**: Application configuration.
- **Program.cs**: Main entry point and API setup.

## Features

- Product management (CRUD operations)
- Order management
- Status validation with custom attributes
- Entity Framework Core integration

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or another supported database (update connection string in `appsettings.json`)

### Build and Run
1. Restore dependencies:
   ```sh
   dotnet restore
   ```
2. Build the project:
   ```sh
   dotnet build
   ```
3. Run the API:
   ```sh
   dotnet run
   ```
4. The API will be available at the URL specified in `launchSettings.json` (default: `https://localhost:5001`).

### API Endpoints
- See `Controllers/ProductOrder.cs` for available endpoints.
- Test endpoints using tools like [Postman](https://www.postman.com/) or [curl](https://curl.se/).

## Configuration
- Update database connection strings in `appsettings.json` and `appsettings.Development.json` as needed.

## License
This project is licensed under the MIT License.
