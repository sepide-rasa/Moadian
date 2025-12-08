using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Guest
{
    public class OfficeReciptController : Controller
    {
        //
        // GET: /OfficeRecipt/

        public ActionResult Get(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public FileResult print(string id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.Sp_tblOfficeReciptSelect_Image(id).FirstOrDefault();
            if (q != null)
                return File(q.fldImage, "application/pdf", "OfficeRecipt.pdf");
            else
                return null;
        }
    }
}
