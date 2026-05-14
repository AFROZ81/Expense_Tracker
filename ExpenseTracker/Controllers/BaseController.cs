using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class BaseController : Controller
    {
        protected string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
