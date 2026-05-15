# ExpenseTracker (ASP.NET Core MVC)

A full-stack personal finance management web application built with ASP.NET Core MVC, Entity Framework Core, SQL Server, and ASP.NET Core Identity.

ExpenseTracker helps each user securely manage:
- Expenses and income
- Budget goals
- Wallets/accounts and running balances
- Recurring transactions
- Visual analytics and reports
- Production-grade exports (server-side PDF + Excel)

---

## Table of Contents

1. [What This App Does](#what-this-app-does)
2. [Core Feature Set](#core-feature-set)
3. [Architecture Overview](#architecture-overview)
4. [Technology Stack](#technology-stack)
5. [Project Structure](#project-structure)
6. [Data Model](#data-model)
7. [Authentication and Data Isolation](#authentication-and-data-isolation)
8. [Business Logic Notes](#business-logic-notes)
9. [Export System (PDF and Excel)](#export-system-pdf-and-excel)
10. [UI and Theme System](#ui-and-theme-system)
11. [Local Setup](#local-setup)
12. [How to Run](#how-to-run)
13. [Default Seed User](#default-seed-user)
14. [Common Workflows](#common-workflows)
15. [Troubleshooting](#troubleshooting)
16. [Current Limitations / Roadmap](#current-limitations--roadmap)

---

## What This App Does

ExpenseTracker is a multi-module finance application where authenticated users can track money movement across categories and accounts, monitor spending patterns, and generate sharable reports.

The system is designed so each user's data is scoped by `UserId`, which keeps records isolated per account.

---

## Core Feature Set

### 1) Expense Management
- Create, edit, delete expense entries
- Category and account linkage per transaction
- Automatic account balance decrease on expense creation
- Monthly summary by category
- Expense dashboard and chart visualizations

### 2) Income Management
- Create, edit, delete income entries
- Income category and account linkage
- Automatic account balance increase on income creation

### 3) Budget Planning
- Monthly budgets by expense category
- Budget status calculations against actual spending

### 4) Wallet / Account Management
- Multiple account types: Cash, Bank, Credit Card, Savings, Investment, Other
- Initial vs current balance tracking
- Protective delete behavior (prevents deleting accounts with existing transactions)

### 5) Recurring Transactions
- Configure recurring rules for expense or income
- Frequency support: Daily, Weekly, Monthly, Yearly
- Automatic processing updates actual Expense/Income tables and balances

### 6) Reporting
- Full period report with start/end date filtering
- Combined expense + income report view

### 7) Exports
- Server-side PDF exports (Chromium via Playwright)
- Excel exports (ClosedXML)
- Fallback to screenshot-style export currently retained for compatibility

---

## Architecture Overview

ExpenseTracker follows a classic ASP.NET Core MVC layered pattern:

- **Controllers**: Handle request routing and orchestration
- **Views (Razor)**: Present UI and report templates
- **Entity Models**: Domain objects mapped to SQL via EF Core
- **DbContext**: Single data access gateway
- **Services**:
  - `RecurringTransactionService` for automation logic
  - `ViewRenderService` for rendering Razor templates to HTML strings
  - `ExportService` for PDF and Excel generation

The app also uses ASP.NET Core Identity for authentication and user persistence.

---

## Technology Stack

### Backend
- ASP.NET Core 10.0 (MVC)
- Entity Framework Core 10
- SQL Server provider
- ASP.NET Core Identity

### Frontend
- Razor Views
- Bootstrap 5
- jQuery
- Chart.js
- Lucide icons

### Export / Reporting
- Microsoft.Playwright (Chromium PDF generation)
- ClosedXML (Excel generation)
- html2pdf.js (legacy fallback path)

---

## Project Structure

```text
ExpenseTracker/
  Controllers/
    AccountController.cs
    ExpenseController.cs
    IncomeController.cs
    BudgetController.cs
    WalletController.cs
    RecurringTransactionController.cs
    ReportController.cs
    ExportController.cs
    ...

  Data/
    ApplicationDbContext.cs

  Models/
    Entities/
      Account.cs
      Expense.cs
      Income.cs
      Category.cs
      IncomeCategory.cs
      Budget.cs
      RecurringTransaction.cs
      ApplicationUser.cs
    ViewModels/
      DashboardViewModel.cs
      MonthlySummaryViewModel.cs
      FullReportViewModel.cs
      Exports/

  Services/
    RecurringTransactionService.cs
    ViewRenderService.cs
    ExportService.cs

  Views/
    Shared/
    Expense/
    Income/
    Budget/
    Wallet/
    Report/
    ExportTemplates/

  wwwroot/
    css/site.css
    js/site.js
    lib/

  Program.cs
  appsettings.json
```

---

## Data Model

Primary entities:

- `ApplicationUser` (Identity user)
- `Account`
  - `InitialBalance`, `CurrentBalance`, `Type`, `IsActive`
- `Expense`
  - `CategoryId`, `AccountId`, `Amount`, `ExpenseDate`, `UserId`
- `Income`
  - `IncomeCategoryId`, `AccountId`, `Amount`, `IncomeDate`, `UserId`
- `Category` (expense category)
- `IncomeCategory`
- `Budget`
  - Category budget by month/year
- `RecurringTransaction`
  - Rule-based automation for generating future expense/income rows

`ApplicationDbContext` exposes `DbSet<>` for all core entities.

---

## Authentication and Data Isolation

### Authentication
- Implemented with ASP.NET Core Identity
- Cookie-based auth configured in `Program.cs`
- Paths:
  - Login: `/Account/Login`
  - Access denied: `/Account/AccessDenied`

### Data isolation
- Most feature controllers inherit `BaseController` and use `UserId`
- Queries are scoped with user filtering (`Where(x => x.UserId == UserId)`) to keep tenant-like separation

---

## Business Logic Notes

### Account balance integrity
Balance is updated with each transaction mutation:

- Expense create: `CurrentBalance -= Amount`
- Expense delete: revert by `+ Amount`
- Income create: `CurrentBalance += Amount`
- Income delete: revert by `- Amount`
- Edit flows revert old value and apply new value

### Recurring engine behavior
`RecurringTransactionService.ProcessPendingTransactions(userId)`:
- Runs pending recurring rules
- Creates real `Expense` or `Income` rows
- Updates account balance accordingly
- Updates `LastProcessedDate`

This is triggered from dashboard loading, which keeps rules processed opportunistically.

---

## Export System (PDF and Excel)

### New server-side exports
Implemented through `ExportController`:

- `GET /Export/ExpensesPdf`
- `GET /Export/ExpensesExcel`
- `GET /Export/MonthlySummaryPdf?month={m}&year={y}`
- `GET /Export/MonthlySummaryExcel?month={m}&year={y}`

### How PDF export works
1. A strongly typed export view model is created.
2. Razor template in `Views/ExportTemplates/` is rendered to HTML.
3. Playwright Chromium renders HTML and outputs A4 PDF bytes.
4. File is streamed as download.

### How Excel export works
- ClosedXML creates workbook in memory
- Styled headers + totals
- Numeric/date formatting
- Streamed as `.xlsx`

### Fallback behavior
If server PDF export is unavailable, UI can fall back to the legacy client-side screenshot export path (temporary compatibility mode).

---

## UI and Theme System

- Shared shell: sidebar + topbar layout in `Views/Shared/_Layout.cshtml`
- Theme variables and adaptive classes in `wwwroot/css/site.css`
- Theme toggle persists preference in `localStorage`
- Uses both custom theme attributes and Bootstrap theme behavior for consistent light/dark switching

---

## Local Setup

### Prerequisites
- .NET SDK 10.0+
- SQL Server (Express / LocalDB / full instance)
- (For PDF export) Playwright Chromium runtime

### Configure connection string
Edit `ExpenseTracker/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ExpenseTrackerDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Install dependencies
```bash
dotnet restore
```

### Apply EF migrations
```bash
dotnet ef database update --project ExpenseTracker
```

### Install Playwright browser binaries (required for server PDF)
```bash
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

If needed, build once before this command so Playwright tooling is generated.

---

## How to Run

From repository root:

```bash
dotnet run --project ExpenseTracker
```

Default development URLs from launch profile:
- `https://localhost:7139`
- `http://localhost:5015`

---

## Default Seed User

On startup, if there are no users, app seeds:

- Email: `demo@example.com`
- Password: `Demo@123`

This allows immediate sign-in during first run.

---

## Common Workflows

### Record an expense
1. Open `Expense > Add`
2. Select category + account
3. Save
4. Account balance updates automatically

### Generate monthly statement
1. Open `Expense > Monthly Summary`
2. Pick month/year
3. Download PDF or Excel using export actions

### Configure recurring payment
1. Open `Automations`
2. Create recurring rule
3. Open dashboard periodically (or as part of usage) to process pending entries

---

## Troubleshooting

### 1) PDF export returns unavailable / falls back
Cause: Playwright Chromium is not installed or not executable.

Fix:
1. Build project
2. Run Playwright install command for Chromium
3. Restart app

### 2) Database connection fails
- Verify SQL Server instance name
- Validate `DefaultConnection`
- Ensure trusted connection/certificate settings match local environment

### 3) Migrations errors
- Ensure `dotnet-ef` tool is installed
- Run command from repo root with correct `--project` path

### 4) Build output locked
If DLL copy fails with "file in use", stop IIS Express / Visual Studio debugging session and rebuild.

---

## Current Limitations / Roadmap

- Some controllers still mix direct context operations with business logic; can be further service-layered.
- Recurring processing currently executes during dashboard flow; background scheduling can be added for stricter automation.
- Export module can be extended with:
  - Date-range transaction exports
  - Audit logs for export events
  - Branded templates per tenant
- Legacy screenshot export fallback should be retired after server export path is fully validated in all environments.

---

If you are onboarding as a contributor, start with:
1. `Program.cs` for service registration and app pipeline
2. `ApplicationDbContext.cs` + `Models/Entities`
3. `ExpenseController` and `IncomeController` for accounting flow
4. `ExportController` + `Services/ExportService` for reporting/export path
