using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers
{
    public class CommonWinController : Controller
    {
        //
        // GET: /CommonWin/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Index1()
        {
            return PartialView();
        }
        public ActionResult Index2()
        {
            return PartialView();
        }
    }
}
