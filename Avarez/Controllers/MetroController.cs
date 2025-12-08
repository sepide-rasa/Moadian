using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers
{
    public class MetroController : Controller
    {
        //
        // GET: /Metro/

        public ActionResult error()
        {
            return PartialView();
        }

        public ActionResult YesNomsg(string id,string url)
        {
            ViewBag.ID = id;
            ViewBag.URL = url;
            return PartialView();
        }
        public ActionResult DuplicatedFishYesNomsg(string From, string To, string url)
        {
            ViewBag.From = From;
            ViewBag.To = To;
            ViewBag.URL = url;
            return PartialView();
        }
        public ActionResult FishYesNomsg(int id, string url,int mablagh)
        {
            ViewBag.URL = url;
            ViewBag.id = id;
            ViewBag.mablagh = mablagh;
            return PartialView();
        }

        public ActionResult Window()
        {
            return PartialView();
        }
    }
}
