<div align="center"> 
    
# ExpenseTracker

A premium personal finance web app built with ASP.NET Core MVC.

</div>

## Overview
ExpenseTracker helps users manage day-to-day finances with secure account-scoped data, modern dashboards, budget tracking, income/expense logs, and recurring transaction automation.

## Core Features
- Multi-wallet/account management
- Expense and income tracking
- Category-based monthly budgets
- Recurring transaction automation
- Date-range financial report views
- Light/dark adaptive UI

## Tech Stack
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core + SQL Server
- ASP.NET Core Identity
- Bootstrap + Razor Views

## Quick Start
1. Update connection string in `ExpenseTracker/appsettings.json`
2. Run:

```bash
dotnet restore
dotnet build
dotnet ef database update --project ExpenseTracker
dotnet run --project ExpenseTracker
```

## Project Structure
```text
ExpenseTracker/
├── ExpenseTracker.slnx
└── ExpenseTracker/
    ├── Controllers/
    ├── Data/
    ├── Models/
    ├── Services/
    ├── Views/
    ├── wwwroot/
    └── Program.cs
```

## Demo Account
If the database is fresh, the app seeds:
- Email: `demo@example.com`
- Password: `Demo@123`
