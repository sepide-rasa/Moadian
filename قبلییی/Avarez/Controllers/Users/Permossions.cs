using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez.Controllers.Users
{
    public class Permossions
    {
        public static bool haveAccess(int userId, int RolId)  
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var q = p.sp_PermissionSelect("HaveAcces", RolId, userId, "").Any(); 
            return q;
        }   
    }
}