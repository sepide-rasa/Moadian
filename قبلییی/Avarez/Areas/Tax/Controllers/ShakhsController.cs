using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Avarez.Areas.Tax.Models;
using System.IO;

namespace Avarez.Areas.Tax.Controllers
{
    public class ShakhsController : Controller
    {
        //
        // GET: /Tax/Shakhs/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        public ActionResult Index()
        {//باز شدن تب جدید
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            return new Ext.Net.MVC.PartialViewResult();


        }
        public ActionResult GetShakhsType()
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
                var q = p.prs_tblTypeShakhsSelect("", "", 0).ToList().OrderBy(l=>l.fldId).Select(l => new { ID = l.fldId, fldName = l.fldNameTypeShakhs });
                return this.Store(q);
           
        }
        public ActionResult New(int id,string typeShakhs)
        {//باز شدن پنجره
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.typeShakhs = typeShakhs;
            return PartialView;
        }
        public ActionResult TarafGharardad(int id)
        {//باز شدن پنجره
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            var gharardadId = 0;
            var code = "";
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var k=p.prs_tblTarfGharardadSelect("fldShakhsId", id.ToString(), 0).FirstOrDefault();
            if (k != null)
            {
                gharardadId = k.fldId;
                code = k.fldUniqId;
            }
            PartialView.ViewBag.gharardadId = gharardadId;
            PartialView.ViewBag.code = code;
            return PartialView;
        }

        public ActionResult Help()
        {//باز شدن پنجره
            //if (Session["UserId"] == null)
            //    return RedirectToAction("Logon", "Account");
            //else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                return PartialView;
            }

        }
        public ActionResult Details(int Id)
        {
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblShakhsHaghighi_HoghoghiSelect("fldId", Id.ToString(), Convert.ToInt32(Session["TaxUserId"]), 0).FirstOrDefault();

            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldFamily = q.fldFamily,
                fldCodeEghtesadi = q.fldCodeEghtesadi,
                fldCodePosti = q.fldCodePosti,
                fldCodeShobe=q.fldCodeShobe,
                fldNationalCode=q.fldNationalCode,
                fldOwnerId=q.fldOwnerId,
                fldTypeShakhsId=q.fldTypeShakhsId.ToString(),
                fldDesc = q.fldDesc

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldNationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldNameTypeShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameTypeShakhs";
                            break;
                    }
                    if (data != null)
                        data1 = p.prs_tblShakhsHaghighi_HoghoghiSelect(field, searchtext,Convert.ToInt32(Session["TaxUserId"]) ,100).ToList();
                    else
                        data = p.prs_tblShakhsHaghighi_HoghoghiSelect(field, searchtext, Convert.ToInt32(Session["TaxUserId"]),  100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.prs_tblShakhsHaghighi_HoghoghiSelect("", "", Convert.ToInt32(Session["TaxUserId"]), 100).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Models.prs_tblShakhsHaghighi_HoghoghiSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(Models.prs_tblShakhsHaghighi_HoghoghiSelect shakhs)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Models.cartaxtest2Entities m = new Models.cartaxtest2Entities();


                if (shakhs.fldDesc == null)
                    shakhs.fldDesc = "";
                if (shakhs.fldFamily == null)
                    shakhs.fldFamily = "";
                if (shakhs.fldNationalCode == null)
                    shakhs.fldNationalCode = "";
                if (shakhs.fldCodeEghtesadi == null)
                    shakhs.fldCodeEghtesadi = "";
                if (shakhs.fldCodePosti == null)
                    shakhs.fldCodePosti = "";
                if (shakhs.fldCodeShobe == null)
                    shakhs.fldCodeShobe = "";


                var q = m.prs_tblShakhsHaghighi_HoghoghiSelect("fldNationalCode", shakhs.fldNationalCode, Convert.ToInt32(Session["TaxUserId"]), 0).Where(l => l.fldOwnerId == Convert.ToInt32(Session["TarafGharardadId"])).FirstOrDefault();
                if (shakhs.fldId == 0)
                { //ذخیره
                    if (q != null)
                    {
                        Er = 1;
                        Msg = "شخص با این شناسه/کد ملی قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        m.prs_tblShakhsHaghighi_HoghoghiInsert(shakhs.fldName, shakhs.fldFamily, shakhs.fldNationalCode, shakhs.fldCodeEghtesadi, shakhs.fldTypeShakhsId,
                            shakhs.fldCodePosti, shakhs.fldCodeShobe, Convert.ToInt32(Session["TarafGharardadId"]), Convert.ToInt64(Session["TaxUserId"]), shakhs.fldDesc,IP);

                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }

                }
                else
                { //ویرایش
                    if (q != null && q.fldId != shakhs.fldId)
                    {
                        Er = 1;
                        Msg = "شخص با این شناسه/کد ملی قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        var q2 = m.prs_tblShakhsHaghighi_HoghoghiSelect("fldId", shakhs.fldId.ToString(), Convert.ToInt32(Session["TaxUserId"]), 0).FirstOrDefault();
                        if (q2.fldOwnerId == Convert.ToInt32(Session["TarafGharardadId"]))
                        {
                            m.prs_tblShakhsHaghighi_HoghoghiUpdate(shakhs.fldId, shakhs.fldName, shakhs.fldFamily, shakhs.fldNationalCode, shakhs.fldCodeEghtesadi, shakhs.fldTypeShakhsId,
                               shakhs.fldCodePosti, shakhs.fldCodeShobe, Convert.ToInt32(Session["TarafGharardadId"]), Convert.ToInt64(Session["TaxUserId"]), shakhs.fldDesc,IP);



                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
                        }
                        else
                        {
                            Er = 1;
                            Msg = "چون این شخص توسط شما تعریف نشده، امکان ویرایش وجود ندارد.";
                            MsgTitle = "خطا";
                        }
                    }
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                Er = 1;
                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadFileKey()
        {
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }

                //if (Request.Files[0].ContentType == "application/pdf")

                if (Request.Files[0].ContentLength <= 5242880)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (Path.GetExtension(file.FileName).ToLower() == ".pem")
                    {
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["FileName"] = file.FileName;
                        Session["savePath"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));

                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "فایل انتخاب شده غیر مجاز است."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }

                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult UploadFileSign()
        {
            string Msg = "";
            try
            {
                if (Session["savePathD"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathD"].ToString());
                    Session.Remove("savePathD");
                    Session.Remove("FileNameD");
                    System.IO.File.Delete(physicalPath);
                }

                //if (Request.Files[0].ContentType == "application/pdf")
               
                    if (Request.Files[1].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[1];
                        if (Path.GetExtension(file.FileName).ToLower() == ".crt")
                        {
                        string savePath = Server.MapPath(@"~\Uploaded\" + "D" + file.FileName);
                        file.SaveAs(savePath);
                        Session["FileNameD"] = file.FileName;
                        Session["savePathD"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Title = "خطا",
                                Message = "فایل انتخاب شده غیر مجاز است."
                            });
                            DirectResult result = new DirectResult();
                            return result;
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
               
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public FileContentResult DownloadFile(int state,string gharardadId)
        {
           
            if (Session["TaxUserId"] == null)
                return null;

            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            var q = p.prs_tblTarfGharardadSelect("fldId", gharardadId.ToString(), 1).FirstOrDefault();
            var ff = q.fldPrivateKey;
            var pass = ".pem";
            if (state == 2)
            {
                ff = q.fldSignatureCertificate;
                pass = ".crt";
            }
            if (q != null)
            {
                MemoryStream st = new MemoryStream(ff);
                return File(st.ToArray(), MimeType.Get(pass), "DownloadFile" + pass);
            }
            return null;
        }
        public ActionResult SaveTarafGharardad(int Id, int gharardadId, string code)
        {

            if (Session["TaxUserId"] == null)
                return RedirectToAction("Login", "AccountTax", new { area = "Tax" });

            byte[] file = null; string e = ""; int IdFile = 0; string FileName = ""; byte Er = 0; string Msg = ""; string MsgTitle = "";

            byte[] fileD = null; string eD = ""; int IdFileD = 0; string FileNameD = "";
            Models.cartaxtest2Entities p = new Models.cartaxtest2Entities();
            try
            {
                if (Session["savePath"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    file = stream.ToArray();
                    e = Path.GetExtension(Session["savePath"].ToString());
                    FileName = Session["FileName"].ToString();
                }
                else
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = "لطفا فایل کلید خصوصی را انتخاب کنید.",
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (Session["savePathD"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathD"].ToString()));
                    fileD = stream.ToArray();
                    eD = Path.GetExtension(Session["savePathD"].ToString());
                    FileNameD = Session["FileNameD"].ToString();
                }
                else
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = "لطفا فایل گواهی امضا را انتخاب کنید.",
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }


                if (gharardadId == 0)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter GId = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));

                    p.prs_tblTarfGharardadInsert(GId,Id, code, file, fileD, Convert.ToInt64(Session["TaxUserId"]), "", IP);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    gharardadId = Convert.ToInt32(GId.Value);
                }
                else
                {
                    p.prs_tblTarfGharardadUpdate(gharardadId,Id, code, file, fileD, Convert.ToInt64(Session["TaxUserId"]), "", IP);
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";

                    var path = base.Server.MapPath(@"~\Uploaded\privateKey" + gharardadId.ToString() + ".pem");
                    var path2 = base.Server.MapPath(@"~\Uploaded\certificate" + gharardadId.ToString() + ".crt");

                    System.IO.File.Delete(path);
                    System.IO.File.Delete(path2);
                }


                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                if (Session["savePathD"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathD"].ToString());
                    Session.Remove("savePathD");
                    Session.Remove("FileNameD");
                    System.IO.File.Delete(physicalPath);
                }

                return Json(new
                {
                    Er = Er,
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    gharardadId = gharardadId
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                return Json(new
                {
                    Er = 1,
                    Msg = Msg,
                    MsgTitle = "خطا"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
