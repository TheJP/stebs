using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using ProcessorDispatcher;
using System.Text;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Stebs5.Controllers
{
    public class HomeController : Controller
    {
        private IDispatcher Dispatcher { get; }

        private const int DownloadSleepTime = 2000;
        private const string LogisimHeader = "v2.0 raw";

        public HomeController()
        {
            this.Dispatcher = UnityConfiguration.Container.Resolve<IDispatcher>();
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        /// <summary>
        /// Provides the ram of the given processor as download.
        /// </summary>
        /// <param name="processorId"></param>
        /// <returns></returns>
        public async Task<ActionResult> DownloadMemory(string processorId)
        {
            //To prevent processor id guessing, each request has to wait here
            await Task.Delay(DownloadSleepTime);
            try {
                //Convert guid
                var guid = Guid.Parse(processorId);
                //Get dispatcher item
                var ram = Dispatcher[guid].Processor.Ram.Data;
                var output = new StringBuilder(LogisimHeader);
                output.AppendLine();
                //The order of dictionaries is non deterministic, so the conversion has to be done this way.
                for (int i = 0; i <= byte.MaxValue; ++i)
                {
                    output.Append(ram[(byte)i].ToString("X2"));
                    output.Append(' ');
                }
                output.AppendLine();
                //Return the generated file as result
                var result = output.ToString();
                var fileName = $"stebs-logisim-export-{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}.data";
                return File(Encoding.ASCII.GetBytes(result), MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception)
            {
                return View("DownloadFailed");
            }
        }
    }
}