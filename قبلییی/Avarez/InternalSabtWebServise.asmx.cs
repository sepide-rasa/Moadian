using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;

namespace Avarez
{
    /// <summary>
    /// Summary description for InternalSabtWebServise
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class InternalSabtWebServise : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetData(string CodeMeli,string Tarikhtavalod,string pass)
        {
            if (pass != "CArtaXWebSrv")
                return "";
            var url = ConfigurationManager.AppSettings["Avarez_SabtWs_SabtWebService"];
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(url);

                var response = (HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SabtWs.SabtWebService a = new SabtWs.SabtWebService();
                    SabtWs.Sp_PersonsSelect p = a.GetData(CodeMeli, Tarikhtavalod);
                    if (p != null)
                        return p.name + "_" + p.family;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;

            }
        }
    }
}
