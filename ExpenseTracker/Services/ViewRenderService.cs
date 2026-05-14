using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ExpenseTracker.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public ViewRenderService(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> RenderToStringAsync(ControllerContext controllerContext, string viewName, object model)
        {
            var actionContext = controllerContext;

            using var writer = new StringWriter();
            var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);
            if (!viewResult.Success)
            {
                viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);
            }
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Could not find view '{viewName}'.");
            }

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                tempData,
                writer,
                new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }
    }
}
