using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers
{
    [Authorize]
    public class CountryTreeController : Controller
    {
        //
        // GET: /CountryTree/

        public ActionResult Index()
        {
            return View();
        }        

    }
}
