using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Guest
{
    public class QR_MafasaController : Controller
    {
        //
        // GET: /QR_Mafasa/

        public ActionResult Get(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public FileResult print(string id)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.Sp_MafasaSelect_Image(id).FirstOrDefault();
            if (q != null)
                return File(q.fldimage, "application/pdf","Mafasa.pdf");
            else
                return null;
        }
    }
}
