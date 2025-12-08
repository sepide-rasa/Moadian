using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Avarez.Controllers.CarTax
{
    public class ListImageInTreeController : Controller
    {
        //
        // GET: /ListImageInTree/

        public ActionResult Index(int carid)
        {//بارگذاری صفحه اصلی 
            ViewBag.carid = carid;
                return PartialView();
            
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.cartaxEntities p = new Models.cartaxEntities();
            var pic = p.sp_DigitalArchive_DetailSelect("fldId", id.ToString(), 0).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldPic != null)
                {
                    return File((byte[])pic.fldPic, "jpg");
                }
            }
            return null;
        }
        public string listImage(int id,int carid)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            string Pics = "";
            var carfiles = p.sp_CarFileSelect("fldCarId", carid.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            foreach (var item1 in carfiles)
            {
                var q = p.sp_DigitalArchiveSelect("fldDigitalTreeId", id.ToString(), 0).Where(l => l.fldCarFileId == item1.fldID).ToList();
                foreach (var item2 in q)
                {
                    var m = p.sp_DigitalArchive_DetailSelect("fldDigitalArchiveId", item2.fldID.ToString(), 0).ToList();
                    foreach (var item in m)
                    {
                        string p1 = Url.Content("~/ListImageInTree/Image/" + item.fldId);
                        Pics += "<img class='preview' id='img" + item.fldId + "' src='" + p1 + "' alt='' width='120px'/></br>";
                    }
                }
            }
           
            return (Pics);
        }
        
        public ActionResult Preview(int id)
        {
            ViewBag.id = id;
            return PartialView();
        }

        public JsonResult _ProductTree(int? id)
        {
            try
            {
                var p = new Models.cartaxEntities();

                if (id != null)
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0)
                                select new
                                {
                                    id = k.fldId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("PId", id.ToString(), 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var rols = (from k in p.sp_tblDigitalTreeSelect("", "", 0)
                                select new
                                {
                                    id = k.fldId,
                                    Name = k.fldName,
                                    hasChildren = p.sp_tblDigitalTreeSelect("", "", 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.cartaxEntities m = new Models.cartaxEntities();
            var q = m.sp_BankSelect(_fiald[Convert.ToInt32(field)], searchtext, top, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
