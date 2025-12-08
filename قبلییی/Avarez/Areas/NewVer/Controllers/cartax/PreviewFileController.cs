using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace Avarez.Areas.NewVer.Controllers.cartax
{
    public class PreviewFileController : Controller
    {
        //
        // GET: /NewVer/PreviewFile/

        public ActionResult Index(string State,string Id)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        [Authorize]
        public FileContentResult showFile(string dc,long id, string type)
        {//برگرداندن عکس 
           
            Models.cartaxEntities p = new Models.cartaxEntities();
            int fileid = 0;
            switch (type)
            {
                case "CarEx":
                    var carEx = p.sp_CarExperienceSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    fileid = (int)carEx.fldFileId;
                    break;
                case "Collection":
                    var Fish = p.sp_CollectionSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (Fish.fldFileId != null)
                        fileid = (int)Fish.fldFileId;
                    break;
                case "Bargsabz":
                    //var car = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file = p.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (file.fldBargSabzFileId != null) 
                        fileid = (int)file.fldBargSabzFileId;
                    break;
                case "Cart":
                    //var car1 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file1 = p.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (file1.fldCartFileId != null)
                        fileid = (int)file1.fldCartFileId;
                    break;
                case "CartBack":
                    //var car3 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file3 = p.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (file3.fldCartBackFileId != null) 
                        fileid = (int)file3.fldCartBackFileId;
                    break;
                case "Sanad":
                    //var car2 = p.sp_SelectCarDetils(id).FirstOrDefault();
                    var file2 = p.sp_CarFileSelect("fldid", id.ToString(), 0, 1, "").FirstOrDefault();
                    if (file2.fldSanadForoshFileId != null) 
                        fileid = (int)file2.fldSanadForoshFileId;
                    break;
            }
            var image = p.Sp_FilesSelect(fileid).FirstOrDefault();
            if (image != null)
            {
                if (image.fldImage != null)
                {
                    return File((byte[])image.fldImage, ".jpg");
                }
            }
            return null;
        }
    }
}
