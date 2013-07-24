using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AsyncDemo.Filters;
using AsyncDemo.Models;

namespace AsyncDemo.Controllers
{
    [UseStopwatch]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new GuidanceLinks().GetAllLinks();

            return View("View", model.ToArray());
        }

        public async Task<ActionResult> Async()
        {
            var model = await new GuidanceLinks().GetAllLinksAsync();

            return View("View", model.ToArray());
        }

        public ActionResult SyncAsync()
        {
            var model = new GuidanceLinks().GetAllLinksAsyncSafe();

            return View("View", model.Result.ToArray());
        }
    }
}