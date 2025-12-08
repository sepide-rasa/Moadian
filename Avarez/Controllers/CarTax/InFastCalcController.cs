using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avarez.Controllers.Users;

namespace Avarez.Controllers.CarTax
{
    public class InFastCalcController : Controller
    {
        //
        // GET: /FirstCalc/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 352))
            {
                Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "عوارض->محاسبه سرانگشتی");
                SignalrHub hub = new SignalrHub();
                hub.ReloadOnlineUser();
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public JsonResult GetCascadeMake()
        {
            List<SelectListItem> No = new List<SelectListItem>();        
            SelectListItem item = new SelectListItem();
            item.Text = "شمسی";
            item.Value = "1";
            No.Add(item);
            item = new SelectListItem();
            item.Text = "میلادی";
            item.Value = "2";
            No.Add(item);
            return Json(No.Select(c => new { fldID = c.Value, fldName = c.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeAccount(int cboCarMake)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarAccountTypeSelect("fldCarMakeID", cboCarMake.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l=>l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeCabin(int cboCarAccountTypes)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CabinTypeSelect("fldCarAccountTypeID", cboCarAccountTypes.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeSystem(int cboCarCabin)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarSystemSelect("fldCabinTypeID", cboCarCabin.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeModel(int cboSystem)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarModelSelect("fldCarSystemID", cboSystem.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeClass(int cboModel)
        {
            Models.cartaxEntities car = new Models.cartaxEntities();
            var County = car.sp_CarClassSelect("fldCarModelID", cboModel.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
            return Json(County.Select(p1 => new { fldID = p1.fldID, fldName = p1.fldName }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModel(int Noo)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = 1350; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = 1950; i <= DateTime.Now.Year + 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.OrderByDescending(l => l.Text).Select(p1 => new { fldID = p1.Value, fldName = p1.Text }).OrderByDescending(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFromYear(int Noo,int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)) ; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(value.ToString()+"/01/01" )).Substring(0, 4)); i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetToYear(int Noo, int value)
        {
            List<SelectListItem> sal = new List<SelectListItem>();
            if (Noo == 1)
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            else
            {
                for (int i = value; i <= Convert.ToInt32(MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4)); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    sal.Add(item);
                }
            }
            return Json(sal.Select(p1 => new { fldID = p1.Value, fldName = p1.Text }).OrderBy(l => l.fldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDateText(string year)
        {
            if (Convert.ToInt32(year) < 1900)
                return Json(new { date = year + "/01/01" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { date = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(year + "/03/21")) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Calc(int carCode, string fromYear, string toYear, string model, string Date)
        {
              Models.cartaxEntities m = new Models.cartaxEntities();
              if (toYear == "")
                  toYear = MyLib.Shamsi.Miladi2ShamsiString(m.sp_GetDate().FirstOrDefault().CurrentDateTime).Substring(0, 4);
            string date = toYear + "/12/29";
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(toYear)))
                date = toYear + "/12/30";
          
            //System.Data.Entity.Core.Objects.ObjectParameter _year = new System.Data.Entity.Core.Objects.ObjectParameter("NullYears", typeof(string));
            //var q = m.sp_jCalcSingleBaze(6, carCode, 5, Convert.ToInt32(Session["UserMnu"]),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(fromYear + "/01/01"),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(date),
            //    MyLib.Shamsi.Shamsi2miladiDateTime(Date), DateTime.Now, Convert.ToInt32(model),_year).OrderBy(h => h.fldyear).ToList();
            string _year = "";
            var q = m.prs_newCarCalc(DateTime.Now, Convert.ToInt32(Session["CountryType"]),
                Convert.ToInt32(Session["CountryCode"]), Convert.ToInt32(Session["UserId"]), Convert.ToInt32(fromYear),
                Convert.ToInt32(toYear), Convert.ToInt32(model), MyLib.Shamsi.Shamsi2miladiDateTime(Date), carCode).ToList();
            var years = q.Where(k => k.fldPrice == null).ToList();
            foreach (var item in years)
            {
                _year += item.fldYear;
            } 
            if (_year.ToString() == "")
                return Json(new { data = q, flag = 0 }, JsonRequestBehavior.AllowGet);
            else
            {
                string s = "", msg = "";
                for (int i = 0; i < _year.ToString().Length; i += 4)
                {
                    if (i < _year.ToString().Length - 4)
                        s += _year.ToString().Substring(i, 4) + " و ";
                    else
                        s += _year.ToString().Substring(i, 4);
                }
                //msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا به مدیر سیستم گزارش دهید.";
                msg = "تعرفه سالهای " + s + " تعریف نشده است لطفا جهت اعلام به پشتیبان دکمه ارسال به پشتیبان را انتخاب و تا زمانی که نرخ توسط پشتیبان ثبت شود، منتظر بمانید، سپس دکمه دریافت از سرور را انتخاب کنید و پس از دریافت پیغام تایید، دکمه محاسبه را انتخاب کنید.";
                
                object q1 = null;
                return Json(new { data = q1, flag = 1, msg = msg, Year = s.Replace(" و ", ",") }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
