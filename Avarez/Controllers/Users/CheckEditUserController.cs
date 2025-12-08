using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avarez.Controllers.Users
{
    public class CheckEditUserController : Controller
    {
        //
        // GET: /CheckEditUser/

        public ActionResult Index(String UserId,String UserName)
        {
            ViewBag.UserId = UserId;
            ViewBag.UserName = UserName;
            return PartialView();
        }
        public ActionResult ReloadGride(string UserId, string UserName)
        {
            Models.cartaxEntities m = new Models.cartaxEntities();
            List<Models.sp_PcPosUserSelect> groups = new List<Models.sp_PcPosUserSelect>();
            var K = UserId.Split(';');
            var S = UserName.Split(';');
            for (byte i = 0; i < K.Length - 1; i++)
            {
                Models.sp_PcPosUserSelect V = new Models.sp_PcPosUserSelect();
                V.fldIdUser = Convert.ToInt32(K[i]);
                V.fldNameFamilyUser = S[i];
                groups.Add(V);
            }
            return Json(groups, JsonRequestBehavior.AllowGet);
            //}
        }
       
    }
}
