# 🪙 ExpensePro: Next-Gen Financial Management SaaS

ExpensePro is a premium, multi-tenant ASP.NET Core MVC application designed to transform how individuals manage their finances. Built with a focus on **Security**, **Automation**, and **Stunning UI/UX**, ExpensePro offers a seamless experience for tracking wealth across multiple wallets and budgets.

---

## ✨ Key Features

### 🔐 Enterprise-Grade Security
- **Multi-Tenant Architecture**: Every user's data is strictly isolated using ASP.NET Core Identity and scoped database queries.
- **Robust Authentication**: Secure login, registration, and session management.
- **Access Control**: Role-based redirection and "Access Denied" protection.

### 💰 Financial Intelligence
- **Multi-Wallet Support**: Manage multiple bank accounts, cash, and digital wallets in one place.
- **Budget Planning**: Set monthly goals per category and track your "Budget Health" with real-time visual indicators.
- **Automated Recurring Transactions**: Set once, forget forever. The system automatically calculates and applies periodic income or expenses.
- **Dynamic Dashboard**: A high-end financial cockpit providing net balance, monthly trends, and spending breakdowns.

### 🚀 Premium UI/UX
- **Expand-on-Hover Sidebar**: A space-saving, modern navigation system with Lucide iconography.
- **Glassmorphism Design**: Beautiful, translucent components that feel modern and premium.
- **Dynamic Topbar**: Sleek header with personalized user cards and secure logout workflows.
- **Responsive Layout**: Fully optimized for desktops and tablets.

### 🤖 Advanced Modules
- **AI Receipt Scanning (Simulation)**: Experience the future of expense entry with an AI-driven receipt scanner simulation that auto-populates forms.
- **Financial Reporting**: Generate comprehensive reports with custom date filters.
- **PDF Export**: Export your financial summaries into professional PDF documents with a single click.

---

## 🛠️ Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | ASP.NET Core 10.0 (MVC) |
| **Database** | SQL Server / Entity Framework Core |
| **Identity** | Microsoft Identity Framework |
| **Frontend** | Razor Pages, JavaScript (ES6+), Bootstrap 5 |
| **Icons** | Lucide Icons |
| **Visualizations** | Chart.js |
| **Utilities** | html2pdf.js, jQuery |

---

## 🚦 Getting Started

### Prerequisites
- .NET 10.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 / VS Code

### Installation
1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/ExpenseTracker.git
   cd ExpenseTracker
   ```

2. **Update Connection String**
   Open `appsettings.json` and ensure the `DefaultConnection` points to your local SQL Server instance.

3. **Apply Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

### 🧪 Demo Account
The application automatically seeds a demo account for immediate exploration:
- **Email**: `demo@example.com`
- **Password**: `Demo@123`

---

## 📂 Project Structure

- **Controllers/**: Centralized logic for financial operations and multi-tenant scoping via `BaseController`.
- **Data/**: EF Core context and Identity configuration.
- **Models/Entities/**: Domain models including `Account`, `Budget`, `Expense`, `Income`, and `RecurringTransaction`.
- **Services/**: Business logic services (e.g., `RecurringTransactionService` for automation).
- **Views/**: Premium Razor templates with a unified `_Layout`.
- **wwwroot/**: Custom CSS/JS and third-party libraries.

---

## 🛡️ Data Privacy
ExpensePro ensures data privacy by enforcing a `UserId` filter on every database query. No user can ever see another user's financial data, even if they guess a direct URL.

---

## 📜 License
Distributed under the MIT License. See `LICENSE` for more information.

---

**Built with ❤️ for better financial futures.**
