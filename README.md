# ZBOOKS — Online Bookstore

ZBOOKS is a full-stack online bookstore portfolio project built with **ASP.NET Core Web API, SQL Server, React, and TypeScript**. The backend follows a layered architecture, while the frontend is a single-page application built with Vite.

Readers can browse and search for books, manage a wishlist, add books to a cart, make purchases, read books online, write reviews, and track their reading progress. An administrator manages the book catalogue and related content.

## Features

- **Authentication and authorization:** User registration and login with JWT bearer tokens, protected frontend routes, protected API endpoints, and an admin account for catalogue management.
- **Book catalogue:** Books with authors, categories, and chapters, including search, filtering, sorting, and pagination.
- **Cart and payments:** Client-side cart management and server-side batch payment processing.
- **Purchase integrity:** Book prices and purchase statuses are controlled by the server, and duplicate purchases are prevented.
- **Library and orders:** A personal library of purchased books and a purchase history.
- **Reviews and ratings:** Star ratings, written reviews, and rating summaries for books.
- **Wishlist:** Save books for future consideration.
- **Reading progress:** Track a reader's progress through purchased books.
- **Global exception handling:** Middleware for consistent API error responses.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core Web API, C# |
| Database | Microsoft SQL Server |
| Data access | Entity Framework Core |
| Authentication | JWT bearer authentication |
| Frontend | React, TypeScript |
| Build tool | Vite |
| Code quality | ESLint |

## Architecture

The backend follows a layered architecture, with each component responsible for a specific part of the application.

```text
HTTP Request
     |
     v
Controller
Handles HTTP requests, input validation
and response status codes
     |
     v
Service
Implements business rules and application logic
     |
     v
DbContext (Entity Framework Core)
Manages database access
     |
     v
SQL Server
```

Controllers delegate business operations to services through interfaces registered with dependency injection. Services handle application logic, while Entity Framework Core manages communication with SQL Server.

Data crossing the API boundary is represented by **Data Transfer Objects (DTOs)** rather than exposing database entities directly. This separation helps keep the database structure independent of the API contract and makes the application easier to maintain and test.

## Project Structure

```text
ZBOOKS/
├── Backend/
│   ├── Controllers/     # API endpoints
│   ├── Services/        # Business logic and interfaces
│   ├── DTOs/            # Request and response models
│   ├── Models/          # EF Core entities and DbContext
│   ├── Data/            # Admin account seeding
│   ├── Middleware/      # Global exception handling
│   ├── Migrations/      # EF Core database migrations
│   └── Program.cs       # Application configuration,
│                        # dependency injection and authentication
│
└── frontend/
    └── src/
        ├── api/         # API client and resource calls
        ├── components/  # Reusable UI components
        ├── context/     # Authentication state
        ├── pages/       # Books, cart, payment, library,
        │                # orders, wishlist and authentication
        ├── types/       # Shared TypeScript types
        └── utils/       # Cart helpers
```

## Database

The application uses SQL Server with 12 tables:

- `Users`
- `Books`
- `Authors`
- `Categories`
- `BookAuthors`
- `BookCategories`
- `Chapters`
- `Purchases`
- `Payments`
- `Wishlists`
- `Reviews`
- `ReadingProgress`

`BookAuthors` and `BookCategories` are junction tables that represent many-to-many relationships between books and authors, and books and categories. Entity Framework Core models these relationships using navigation properties.

## Getting Started

### Prerequisites

Install the following tools:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/)
- [SQL Server or SQL Server Express](https://www.microsoft.com/sql-server)
- Entity Framework Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Clone the Repository

```bash
git clone https://github.com/DevZaheer/ZBOOKS.git
cd ZBOOKS
```

### 2. Configure the Database

The default connection string in `Backend/appsettings.json` targets a local SQL Server Express instance using Windows authentication:

```text
Server=localhost\SQLEXPRESS01;Database=ZBOOKS;Trusted_Connection=True;TrustServerCertificate=True;
```

Change the server name if your SQL Server instance is different.

### 3. Configure Secrets

Sensitive configuration, including the JWT signing key and administrator credentials, is not committed to the repository. Use [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for local development.

Run the following commands from the `Backend` folder:

```bash
cd Backend
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-long-random-secret-key"
dotnet user-secrets set "Admin:Email" "admin@example.com"
dotnet user-secrets set "Admin:Password" "choose-a-strong-password"
```

Replace the example values with your own. The JWT key should be a long random string (32 or more characters). The administrator credentials are used to seed the initial administrator account.

### 4. Apply Database Migrations and Run the API

From the `Backend` folder, run:

```bash
dotnet ef database update
dotnet run
```

The terminal displays the address on which the API is listening. Use that address when configuring the frontend API client.

You can test the endpoints with the requests in `Backend/ZBooksApi.http`.

### 5. Run the Frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Vite normally serves the frontend at:

```text
http://localhost:5173
```

The frontend communicates with the API through `src/api/client.ts`. Ensure the configured API base URL matches the actual address printed by the backend.

## API Overview

| Controller | Purpose |
|---|---|
| Auth | Register users, authenticate users, and issue JWTs |
| Books | Browse, search, filter, sort, paginate, and manage books |
| Authors | Manage authors and catalogue relationships |
| Categories | Manage book categories and catalogue relationships |
| Chapters | Access and manage book chapters |
| Purchases | Manage purchases and purchase history |
| Payments | Process payment operations, including batch payments |
| Reviews | Manage ratings and written reviews |
| Wishlists | Manage saved books |
| ReadingProgresses | Track reading progress for each user |
| Users | Manage user accounts |

The operations available to each user depend on authentication, authorization rules, and the endpoint's intended purpose.

## What I Learned

Developing ZBOOKS has helped me practise and apply:

- Designing a relational database schema with primary keys, foreign keys, and many-to-many relationships.
- Using Entity Framework Core to map database tables and relationships.
- Building a layered ASP.NET Core Web API with controllers, services, interfaces, and dependency injection.
- Using DTOs to define API request and response contracts.
- Applying server-side business rules to protect prices and purchase operations.
- Managing application secrets with .NET user secrets.
- Implementing JWT-based authentication and protecting API endpoints.
- Building a React and TypeScript frontend that communicates with a secured API.

## Roadmap

- [ ] Add automated tests for the service layer.
- [ ] Deploy the API and frontend.
- [ ] Add project screenshots and a live demo link.

## Author

**Zaheer** — Backend developer focused on C# and ASP.NET Core.

- GitHub: [@DevZaheer](https://github.com/DevZaheer)
