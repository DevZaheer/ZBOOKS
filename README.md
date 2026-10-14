# ZBOOKS

An online bookstore built as a full-stack portfolio project. A layered **ASP.NET Core Web API** with **SQL Server** powers the backend, and a **React + TypeScript** single-page app (Vite) provides the storefront.

Readers can browse and search books, save them to a wishlist, add them to a cart, pay, read, review and track their reading progress. An admin account manages the catalogue.

## Features

- **Authentication**: register and log in with JWT bearer tokens; protected routes on the frontend and role-aware endpoints on the API
- **Catalogue**: books with authors, categories and chapters; search, filter, sort and pagination
- **Cart and payments**: cart on the client, batch payment on the server; payments complete purchases server-side
- **Purchase integrity**: price and status are set by the server (never trusted from the client) and duplicate purchases are blocked
- **Library and orders**: a personal library of purchased books and an order history
- **Reviews**: star ratings and reviews, with a rating summary on each book
- **Wishlist** and **reading progress** tracking
- **Global exception handling** middleware that returns consistent error responses

## Tech stack

| Layer | Technology |
|-------|------------|
| API | ASP.NET Core Web API (C#) |
| Data | Entity Framework Core, SQL Server |
| Auth | JWT bearer authentication |
| Frontend | React, TypeScript, Vite, ESLint |

## Architecture

The backend follows a layered design so each part has one job:

```
HTTP request
   │
   ▼
Controller      → handles HTTP, validates input, returns status codes
   │
   ▼
Service         → business rules (pricing, duplicate checks, payment flow)
   │
   ▼
DbContext (EF)  → talks to SQL Server
```

Controllers never touch the database directly. They call services through interfaces (registered with dependency injection), and data crosses the API boundary as **DTOs** rather than raw entity models. This keeps the database shape private and makes the business logic easy to test and change.

### Project structure

```
ZBooksApi/
├── Backend/
│   ├── Controllers/     # API endpoints
│   ├── Services/        # Business logic (interfaces + implementations)
│   ├── DTOs/            # Request and response shapes
│   ├── Models/          # EF Core entities and ZBooksDbContext
│   ├── Data/            # Admin account seeding
│   ├── Middleware/      # Global exception handling
│   ├── Migrations/      # EF Core migrations
│   └── Program.cs       # App setup, DI and authentication
└── frontend/
    └── src/
        ├── api/         # API client and resource calls
        ├── components/  # Reusable UI (BookCard, Pagination, SearchBar, ...)
        ├── context/     # Auth state
        ├── pages/       # Books, cart, payment, library, orders, wishlist, auth
        ├── types/       # Shared TypeScript types
        └── utils/       # Cart helpers
```

### Database

12 tables in SQL Server: `Users`, `Books`, `Authors`, `Categories`, `Chapters`, `Purchases`, `Payments`, `Wishlists`, `Reviews`, `ReadingProgress`, plus the `BookAuthors` and `BookCategories` junction tables (modelled as many-to-many navigation properties in EF Core).

## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS)
- SQL Server or SQL Server Express
- The EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Clone the repo

```bash
git clone https://github.com/DevZaheer/ZBOOKS.git
cd ZBOOKS
```

### 2. Configure the database

The default connection string in `Backend/appsettings.json` uses Windows authentication against a local SQL Server Express instance:

```
Server=localhost\SQLEXPRESS01;Database=ZBOOKS;Trusted_Connection=True;TrustServerCertificate=True;
```

Change the server name if your instance is different.

### 3. Set your secrets

Secrets are **not** stored in the repo. They live in [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) on your machine. Run these inside the `Backend` folder:

```bash
cd Backend
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "any-long-random-string-of-32-or-more-characters"
dotnet user-secrets set "Admin:Email" "admin@example.com"
dotnet user-secrets set "Admin:Password" "choose-a-strong-password"
```

The API will not start without `Jwt:Key`. The admin email and password are used to seed the first admin account.

### 4. Create the database and run the API

```bash
dotnet ef database update
dotnet run
```

The console prints the URL the API is listening on. You can try the endpoints with the requests in `Backend/ZBooksApi.http`.

### 5. Run the frontend

In a second terminal:

```bash
cd frontend
npm install
npm run dev
```

The app opens at `http://localhost:5173`. The frontend talks to the API through `src/api/client.ts`, so make sure the base URL there matches the address the API printed in step 4.

## API overview

| Controller | Purpose |
|------------|---------|
| Auth | Register and log in, issue JWTs |
| Books | Browse, search, filter, sort, paginate; manage books |
| Authors, Categories | Catalogue metadata and book linking |
| Chapters | Book chapters |
| Purchases, Payments | Buying books and completing payments (single and batch) |
| Reviews | Ratings and reviews |
| Wishlists | Saved books |
| ReadingProgresses | Per-user reading progress |
| Users | User management |

## What I learned

- Designing a relational schema (including many-to-many relationships) and mapping it with EF Core
- Structuring an API in layers with dependency injection and DTOs
- Keeping business-critical values (price, purchase status) on the server
- Handling secrets safely with user secrets
- Building a typed React frontend that consumes a secured API

## Roadmap

- Automated tests for the service layer
- Deployment (API and frontend)
- Screenshots and a live demo link

## Author

**Zaheer**: backend developer focused on C# and ASP.NET Core. [GitHub](https://github.com/DevZaheer)
