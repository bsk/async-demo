using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;

namespace AsyncDemo.Filters
{
    public class UseStopwatchAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            filterContext.Controller.ViewData["stopWatch"] = stopWatch;
            filterContext.Controller.ViewBag.stopWatch = stopWatch;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var stopWatch = (Stopwatch) filterContext.Controller.ViewBag.stopWatch;
            stopWatch.Stop();

            filterContext.Controller.ViewBag.elapsedTime = stopWatch.Elapsed.ToString();
        }
    }
}