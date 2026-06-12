<div align="center">

# ExpenseTracker

<p>
  <strong>Beautifully organized personal finance tracking built with ASP.NET Core MVC.</strong>
</p>

<p>
  Track income, expenses, wallets, budgets, recurring transactions, and reports in one secure account-scoped dashboard.
</p>

</div>

---

## Overview

ExpenseTracker is a full-stack personal finance web application designed to help users understand where their money goes, how balances move over time, and whether spending stays within budget.

It combines authentication, wallet-level tracking, category management, budgeting, recurring entries, dashboards, and reports into a clean MVC experience. Each user works in their own authenticated space, so financial data stays separated and organized.

## Highlights

<table>
  <tr>
    <td><strong>Wallets</strong><br/>Manage multiple wallets or accounts for different money sources.</td>
    <td><strong>Income & Expenses</strong><br/>Track transactions with clear categories and dates.</td>
  </tr>
  <tr>
    <td><strong>Budgets</strong><br/>Set monthly budgets and compare them against real spending.</td>
    <td><strong>Recurring Entries</strong><br/>Automate repeated transactions like rent, subscriptions, or salary.</td>
  </tr>
  <tr>
    <td><strong>Reports</strong><br/>Review charts, summaries, and date-range reports.</td>
    <td><strong>Secure Login</strong><br/>User authentication is powered by ASP.NET Core Identity.</td>
  </tr>
</table>

## Core Features

- Multi-wallet and account management
- Income and expense tracking
- Category-based organization
- Monthly budget creation and monitoring
- Recurring transaction handling
- Dashboard, charts, and reporting views
- Authenticated user-specific financial data

## Tech Stack

<table>
  <tr>
    <td><strong>Framework</strong></td>
    <td>ASP.NET Core MVC on .NET 10</td>
  </tr>
  <tr>
    <td><strong>Database</strong></td>
    <td>SQL Server with Entity Framework Core</td>
  </tr>
  <tr>
    <td><strong>Authentication</strong></td>
    <td>ASP.NET Core Identity</td>
  </tr>
  <tr>
    <td><strong>UI</strong></td>
    <td>Bootstrap + Razor Views</td>
  </tr>
</table>

## App Areas

- `Account` - login, registration, and access control
- `Wallet` - wallet/account management
- `Expense` - expense tracking, dashboard, charts, and monthly summary
- `Income` - income tracking
- `Category` - expense categories
- `IncomeCategory` - income categories
- `Budget` - budget creation and editing
- `RecurringTransaction` - recurring transaction management
- `Report` - report generation and date-range views
- `Home` - landing pages and app entry points

## How It Works

1. A user signs up or logs in through the account pages.
2. The user creates wallets, categories, income categories, and budgets.
3. Income and expense transactions are added against the correct wallet and category.
4. Recurring entries can be managed separately for repeat use cases.
5. Dashboards, summaries, charts, and reports help the user understand financial patterns quickly.

## Project Structure

```text
ExpenseTracker/
├── Controllers/
├── Data/
├── Models/
├── Services/
├── Views/
├── wwwroot/
├── Program.cs
└── appsettings.json
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server
- EF Core tools

### Setup

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Update the connection string in [`ExpenseTracker/appsettings.json`](ExpenseTracker/appsettings.json)
4. Apply the database migrations
5. Run the application

### Commands

```bash
dotnet restore
dotnet build
dotnet ef database update --project ExpenseTracker
dotnet run --project ExpenseTracker
```

## Configuration

The app reads its database connection from the `DefaultConnection` entry in [`ExpenseTracker/appsettings.json`](ExpenseTracker/appsettings.json).

Identity is configured in `Program.cs`, including the custom authentication paths:

- Login path: `/Account/Login`
- Access denied path: `/Account/AccessDenied`

## Demo Account

If the database is empty, the app seeds a demo user automatically:

- Email: `demo@example.com`
- Password: `Demo@123`

Use this account to explore the app after the first database setup.

## Notes

- The app uses Razor views and Bootstrap for a responsive MVC interface.
- Financial data is scoped to authenticated users.
- Recurring transactions are handled through a dedicated service layer.

## License

No license has been specified yet. Add one if you want to publish or share the project publicly.
