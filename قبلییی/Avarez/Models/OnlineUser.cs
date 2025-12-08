using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avarez.Models
{
    public static class OnlineUser
    {
        public static List<LogOnModel> userObj = new List<LogOnModel>();
        
        public static void AddOnlineUser(string strConnectionId, string strUserName,string strUserId,string strSessionId)
        {
            LogOnModel user = new LogOnModel();
            user.connectionId = strConnectionId;
            user.UserName = strUserName;
            user.userId = strUserId;
            user.newStatus = true;
            user.sessionId = strSessionId;
            userObj.Add(user);
        }
        public static void AddOnlineUser(string strUserId,string ip,string Mun)
        {
            LogOnModel user = new LogOnModel();
            Models.cartaxEntities car = new cartaxEntities();
            if (OnlineUser.userObj.Find(k => k.userId == strUserId) != null)
            {
                var userRemove = (LogOnModel)userObj.Where(item => item.userId == strUserId).FirstOrDefault();
                userObj.Remove(userRemove);
            }
            var q = car.sp_UserSelect("fldid", strUserId, 0, "", 1, "").FirstOrDefault();
            user.Name = q.fldName + " " + q.fldFamily;
            user.userId = strUserId;
            user.cboMnu = Mun;
            user.IPAdress = ip;
            user.newStatus = true;
            userObj.Add(user);
        }
        public static void UpdateUrl(string strUserId,string Url)
        {
            try
            {
                var upt = (LogOnModel)userObj.Where(item => item.userId == strUserId).FirstOrDefault();
                if (upt != null)
                {
                    upt.Url = Url;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public static void RemoveOnlineUser(string strUserId)
        {
            var userRemove =(LogOnModel) userObj.Where(item => item.userId == strUserId).FirstOrDefault();
            userObj.Remove(userRemove);           
 
        }
    }
}
