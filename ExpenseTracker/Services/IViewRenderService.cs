using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(ControllerContext controllerContext, string viewName, object model);
    }
}
