using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Avarez.Controllers.Users;
using System.Text.RegularExpressions;
using System.Globalization;
using Avarez.Models;
using System.Web.Configuration;


namespace Avarez.Areas.NewVer.Controllers.Users
{
    public class User_NewController : Controller
    {
        //
        // GET: /NewVer/User_New/

        public ActionResult Index(string containerId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Avarez.Models.OnlineUser.UpdateUrl(Session["UserId"].ToString(), "مدیریت کاربران->کاربران نرم افزار");
            SignalrHub hub = new SignalrHub();
            hub.ReloadOnlineUser();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        bool invalid = false;
        public bool checkEmail(string Email)
        {

            if (String.IsNullOrEmpty(Email))
                invalid = false;

            else
            {
                Email = Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalid = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return invalid;
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public ActionResult New(int Id)
        {//باز شدن پنجره
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult Help()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        public ActionResult NodeLoadGroup(string nod, int UserId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            Models.cartaxEntities p = new Models.cartaxEntities();
            if (nod == "0")
            {
                var q = p.sp_UserGroupSelect("ByUserId", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.Checked = false;
                    var f = p.sp_User_GroupSelect("fldUserSelectID", UserId.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    if (f.Count() != 0)
                    {
                        foreach (var _item in f)
                        {
                            if (_item.fldUserGroupID == item.fldID && _item.fldUserSelectID == UserId)
                            {
                                asyncNode.Checked = true;
                            }
                        }
                    }
                    nodes.Add(asyncNode);
                }

            }
            return this.Direct(nodes);
        }

        public ActionResult Upload()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
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

                if (Request.Files[0].ContentType == "image/jpeg" || Request.Files[0].ContentType == "image/png")
                {
                    if (Request.Files[0].ContentLength <= 102400)
                    {
                        HttpPostedFileBase file = Request.Files[0];
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 100 کیلوبایت باشد."
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
                        Message = "فایل انتخاب شده غیر مجاز است."
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

        public ActionResult ShowPic(string dc)
        {//برگرداندن عکس 
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Avarez.Models.sp_SelectUserByUserId> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Avarez.Models.sp_SelectUserByUserId> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
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
                        case "fldMelliCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMelliCode";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                        case "fldCountryDivisionsName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCountryDivisionsName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)

                        data1 = m.sp_SelectUserByUserId(field, searchtext, 100,"", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                    else
                        data = m.sp_SelectUserByUserId(field, searchtext, 100,"", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.sp_SelectUserByUserId("", "", 100,"", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
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
                            //return !oValue.ToString().Contains(value.ToString());
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<Avarez.Models.sp_SelectUserByUserId> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult NodeLoadTreeCountry(string nod)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            NodeCollection nodes = new Ext.Net.NodeCollection();
            string url = Url.Content("~/Content/images/");
            var p = new Models.cartaxEntities();
            var treeidid = p.sp_SelectTreeNodeID(Convert.ToInt32(Session["UserId"])).FirstOrDefault();
            string CountryDivisionTempIdUserAccess = treeidid.fldID.ToString();

            if (nod == "0" || nod == null)
            {
                var child = p.sp_TableTreeSelect("fldId", CountryDivisionTempIdUserAccess, 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            else
            {
                var child = p.sp_TableTreeSelect("fldPId", nod.ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldNodeName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.IconFile = url + ch.fldNodeType + ".png";
                    childNode.DataPath = ch.fldNodeType.ToString();
                    childNode.Cls = ch.fldSourceID.ToString();
                    nodes.Add(childNode);
                }
            }
            return this.Direct(nodes);
        }

        public ActionResult LoadPath(string Path, byte jj, string treeid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });

            Models.cartaxEntities Car = new Models.cartaxEntities();
            List<string> a = null;
            if (jj == 0)
            {
                var ind = Path.IndexOf(treeid);
                if (ind != -1)
                {
                    a = Path.Substring(ind).Split('/').ToList();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "لطفا با پشتیبان تماس بگیرید."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            else
            {
                a = Path.Split('/').Skip(1).ToList();
            }

            NodeCollection nodes = new Ext.Net.NodeCollection();
            var node = new Ext.Net.Node();
            var node2 = new Ext.Net.Node();

            string url = Url.Content("~/Content/images/");
            int m = 0;
            for (var i = 0; i < a.Count - 1; i++)
            {
                var p = new Models.cartaxEntities();
                var child = p.sp_TableTreeSelect("fldPId", a[i].ToString(), 0, 0, 0).Where(w => w.fldNodeType != 9).OrderBy(l => l.fldNodeName).ToList();
                if (i == 0)
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        nodes.Add(childNode);
                    }
                }
                else
                {
                    for (int j = 0; j < child.Count; j++)
                    {
                        Node childNode = new Node();
                        if (child[j].fldID == Convert.ToInt32(a[i + 1])) { node2 = childNode; }
                        childNode.Text = child[j].fldNodeName;
                        childNode.NodeID = child[j].fldID.ToString();
                        childNode.IconFile = url + child[j].fldNodeType + ".png";
                        childNode.DataPath = child[j].fldNodeType.ToString();
                        childNode.Cls = child[j].fldSourceID.ToString();
                        node.Children.Add(childNode);
                    };
                    node = node2;
                }
            }
            return this.Direct(nodes);
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 266))
                {
                    Models.cartaxEntities Car = new Models.cartaxEntities();
                        Car.sp_UserDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                        return Json(new
                        {
                            MsgTitle = "حذف موفق",
                            Msg = "حذف با موفقیت انجام شد.",
                            Er = 0
                        });
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public int checks(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }
        }
        public ActionResult Save(Models.sp_UserSelect User, List<Models.sp_User_GroupSelect> UserGroup, string fldImage)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {

                Models.cartaxEntities Car = new Models.cartaxEntities();
                var codeMeli = Car.sp_UserSelect("fldMelliCode", User.fldMelliCode, 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var userName = Car.sp_UserSelect("fldUserName", User.fldUserName, 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                bool checkmail;
                if (User.fldDesc == null)
                    User.fldDesc = "";
                bool fldStatus = true;
                byte[] image = null;
                if (User.fldID == 0 && codeMeli!=null)
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "کد ملی وارد شده قبلا در سیستم ثبت شده است.",
                        Er = 1
                    });
                }
                else if(User.fldID == 0 && userName!=null)
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "نام کاربری وارد شده قبلا در سیستم ثبت شده است.",
                        Er = 1
                    });
                }
                if (User.fldID != 0 && codeMeli != null && User.fldID!=codeMeli.fldID)
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "کد ملی وارد شده قبلا در سیستم ثبت شده است.",
                        Er = 1
                    });
                }
                //else if (User.fldID != 0 && userName != null && User.fldID != codeMeli.fldID)
                //{
                //    return Json(new
                //    {
                //        MsgTitle = "خطا",
                //        Msg = "نام کاربری وارد شده قبلا در سیستم ثبت شده است.",
                //        Er = 1
                //    });
                //}
                if (User.fldUserType == false)
                {
                    var chck = checks(User.fldMelliCode);

                    if (chck != 1)
                    {
                        return Json(new
                        {
                            Msg = "کد ملی وارد شده نامعتبر است.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (User.fldEmail != null && User.fldEmail != "")
                {
                    checkmail = checkEmail(User.fldEmail);
                    if (checkmail == false)
                    {
                        return Json(new
                        {
                            Msg = "ایمیل وارد شده نامعتبر است.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (User.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 265))
                    {
                        
                       DateTime Sdate = MyLib.Shamsi.Shamsi2miladiDateTime(User.fldStartDate);
                        
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            image = stream.ToArray();
                        }
                       
                        //if (fldImage != null)
                        //    image = Avarez.Helper.ClsCommon.Base64ToImage(fldImage);

                        System.Data.Entity.Core.Objects.ObjectParameter id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                        Car.sp_UserInsert(id, User.fldName, User.fldFamily, User.fldStatus, User.fldPassword.GetHashCode().ToString(), User.fldUserName, User.fldMelliCode,
                            User.fldEmail, User.fldNumberAgoTel, User.fldTel, User.fldMobile, Sdate, User.CountryType, User.CountryCode,
                           Convert.ToInt32(Session["UserId"]), User.fldDesc, image,User.fldOfficeUserKey, Session["UserPass"].ToString(),User.fldUserType);
                        if (UserGroup != null)
                        {
                            foreach (var item in UserGroup)
                            {
                                Car.sp_User_GroupInsert(item.fldUserGroupID, Convert.ToInt64(id.Value), Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                            }
                        }
                        if (Session["savePath"] != null)
                        {
                            string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            Session.Remove("FileName");
                            System.IO.File.Delete(physicalPath);
                        }
                        return Json(new
                        {
                            MsgTitle = "ذخیره موفق",
                            Msg = "ذخیره با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 267))
                    {
                        DateTime Sdate = MyLib.Shamsi.Shamsi2miladiDateTime(User.fldStartDate);
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            image = stream.ToArray();
                            //FileName = Session["FileName"].ToString();
                        }
                        else
                        {
                            var pic = Car.sp_PictureSelect("fldUserPic", User.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                            if (pic != null)
                                image = pic.fldPic;
                        }
                        Car.sp_UserUpdate(User.fldID, User.fldName, User.fldFamily, User.fldStatus, "", "", User.fldMelliCode,
                               User.fldEmail, User.fldNumberAgoTel, User.fldTel, User.fldMobile, Sdate, User.CountryType, User.CountryCode,
                               Convert.ToInt32(Session["UserId"]), User.fldDesc, image, User.fldOfficeUserKey, Session["UserPass"].ToString(),User.fldUserType);

                         Car.sp_User_GroupDelete(Convert.ToInt32(User.fldID), Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
                         if (UserGroup != null)
                         {
                             foreach (var item in UserGroup)
                             {
                                 Car.sp_User_GroupInsert(item.fldUserGroupID, User.fldID, Convert.ToInt32(Session["UserId"]), "", Session["UserPass"].ToString());
                            }
                        }
                        if (Session["savePath"] != null)
                        {
                            string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            Session.Remove("FileName");
                            System.IO.File.Delete(physicalPath);
                        }
                        return Json(new
                        {
                            MsgTitle = "ویرایش موفق",
                            Msg = "ویرایش با موفقیت انجام شد.",
                            Er = 0
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Er = 1
                        });
                    }
                }
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }

        public ActionResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                long? sid = 0;
                string Path = "/";
                int? type = 0;
                int LastNodeId = 0;
                int treeid=0;
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter type1 = new System.Data.Entity.Core.Objects.ObjectParameter("Type", sizeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter code1 = new System.Data.Entity.Core.Objects.ObjectParameter("Code", sizeof(int));
                var User = Car.sp_UserSelect("fldId", id.ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var Userlog = Car.sp_UserSelect("fldId", Session["UserId"].ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var a = Car.sp_CountryDivisionsSelect("fldId", User.fldCountryDivisionsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                var pic = Car.sp_PictureSelect("fldUserPic", User.fldID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                if (Userlog.fldCountryDivisionsID!=1)
                {
                    var rec = Car.sp_GET_TypeAndCodeCountryDivisions(Userlog.fldCountryDivisionsID, type1, code1);
                    treeid = Car.sp_TableTreeSelect("fldSourceID", code1.Value.ToString(), 0, 0, 0).Where(l => l.fldNodeType == Convert.ToInt32(type1.Value)).FirstOrDefault().fldID;
                }


                string image = null;
                if (pic != null && pic.fldPic!=null)
                    image = Convert.ToBase64String(pic.fldPic);
                var _checked = Car.sp_User_GroupSelect("fldUserSelectID", User.fldID.ToString(), 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).ToList();
                int[] checkedNodes = new int[_checked.Count];
                for (int i = 0; i < _checked.Count; i++)
                {
                    checkedNodes[i] = Convert.ToInt32(_checked[i].fldUserGroupID);
                }

                if (a != null)
                {
                    if (a.fldAreaID != null)
                    {
                        sid = a.fldAreaID;
                        type = 7;
                    }
                    else if (a.fldCityID != null)
                    {
                        sid = a.fldCityID;
                        type = 4;
                    }
                    else if (a.fldCountyID != null)
                    {
                        sid = a.fldCountyID;
                        type = 2;
                    }
                    else if (a.fldLocalID != null)
                    {
                        sid = a.fldLocalID;
                        type = 6;
                    }
                    else if (a.fldMunicipalityID != null)
                    {
                        sid = a.fldMunicipalityID;
                        type = 5;
                    }
                    else if (a.fldOfficesID != null)
                    {
                        sid = a.fldOfficesID;
                        type = 8;
                    }
                    else if (a.fldStateID != null)
                    {
                        sid = a.fldStateID;
                        type = 1;
                    }
                    else if (a.fldZoneID != null)
                    {
                        sid = a.fldZoneID;
                        type = 3;
                    }
                    LastNodeId = Car.sp_TableTreeSelect("fldSourceID", sid.ToString(),0, 0, 0).Where(l => l.fldNodeType == type).FirstOrDefault().fldID;
                    var nodes = Car.sp_SelectUpTreeCountryDivisions(LastNodeId, 1, "").ToList();
                    foreach (var item in nodes)
                    {
                        Path = Path + item.fldID + "/";
                    }
                    Path = Path.Substring(0, Path.Length - 1);
                    if (sid == 0)
                    {
                        Path = "/1";
                    }
                }

                string status = "0";
                if (User.fldStatus)
                    status = "1";
                string userType = "0";
                if (User.fldUserType)
                    userType = "1";
                return Json(new
                {
                    fldID = User.fldID,
                    fldName = User.fldName,
                    fldFamily = User.fldFamily,
                    fldStatus = status,
                    //fldPassword=User.fldPassword,
                    //fldUserName = User.fldUserName,
                    fldMelliCode = User.fldMelliCode,
                    fldEmail = User.fldEmail,
                    countryId = LastNodeId,
                    fldSDate = User.fldStartDate,
                    fldNumberAgoTel = User.fldNumberAgoTel,
                    fldTel = User.fldTel,
                    fldMobile = User.fldMobile,
                    fldStartDate = User.fldStartDate,
                    fldType = User.CountryType,
                    fldCode = User.CountryCode,
                    Path = Path,
                    image = image,
                    Er=0,
                    fldDesc = User.fldDesc,
                    treeid=treeid,
                    checkedNodes = checkedNodes,
                    fldOfficeUserKey=User.fldOfficeUserKey,
                    fldUserType = userType
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
        public ActionResult ResetPass(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Session["UserId"] == null)
                return RedirectToAction("LogOn", "Account_New", new { area = "NewVer" });
            try
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                var User = Car.sp_UserSelect("fldId", id.ToString(), 1, "", Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
                Car.sp_UserPassUpdate(id, User.fldUserName.GetHashCode().ToString());
                return Json(new
                {
                    MsgTitle = "عملیات موفق",
                    Msg = "کلمه عبور به نام کاربری تغییر کرد.",
                    Er = 0
                });
                
            }
            catch (Exception x)
            {
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", sizeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, Convert.ToInt32(Session["UserId"]), x.Message, DateTime.Now, Session["UserPass"].ToString());
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
        }
    }
}
