using Avarez.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Avarez
{
    public static class SendToSamie
    {
        public static void Send(int CollectionId,int MunId)
        {
            try{
                SamieSrv.Service1 ss = new SamieSrv.Service1();
                var res = ss.DoSend(CollectionId, MunId);
                
                //cartaxEntities p = new cartaxEntities();
                //var mun = p.sp_MunicipalitySelect("fldid", MunId.ToString(), 0, 1, "").Where(k => k.fldSamieUser != null).FirstOrDefault();
                //if(mun!=null)
                //{
                //    string BaseUrl = "http://s1.saamie.ir/Complication/Complication/CreateComplication";
                //    var q = p.sp_CollectionSelect("fldid",CollectionId.ToString(),0,1,"").FirstOrDefault();
                //    string token = loginTken(mun.fldSamieUser, mun.fldSamiePass);

                //    if(q!=null)
                //    {

                //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(BaseUrl);
                //        httpWebRequest.ContentType = "application/json-patch+json";
                //        httpWebRequest.Method = "POST";
                //        httpWebRequest.Headers.Add("Authorization", token);
                //        string json = "", s = "";
                //        var car = p.sp_SelectCarDetilsByCarFileID(q.fldCarFileID).FirstOrDefault();
                //        var plq = car.fldPlaquNumber.Split('|');
                //        CreateComplication viewModel = new CreateComplication()
                //        {
                //            vin = new Vin() { value = car.fldVIN },
                //            productYear = new Year() { value = (int)car.fldModel },
                //            plaqueTwoLeftDigit = plq[2].Replace(" ", "").Substring(4, 2),
                //            plaqueLetter = plq[2].Replace(" ", "").Substring(3, 1),
                //            plaqueThreeDigit = plq[2].Replace(" ", "").Substring(0, 3),
                //            plaqueTwoRightDigit = plq[1],
                //            carType = new CarType2 { value = car.fldCarSystemName + " " + car.fldCarModel + " " + car.fldCarClassName },
                //            datePament = MyLib.Shamsi.Shamsi2miladiString(q.fldCollectionDate),
                //            carComplicationYear = new Year() { value = Convert.ToInt32(q.fldCollectionDate.Substring(0, 4)) },
                //            paymentPriceByOwner = q.fldPrice,
                //            paymentCode = q.fldID.ToString(),
                //            destinationBankName = "",
                //            bankAccount = "",
                //            municipalId = mun.fldSamieGUID
                //        };
                //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                //        {
                //            json = new JavaScriptSerializer().Serialize(viewModel);
                //            streamWriter.Write(json);
                //            streamWriter.Flush();
                //            streamWriter.Close();
                //        }
                //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //        {
                //            s = streamReader.ReadToEnd().Replace("\"", "");
                //            p.sp_CollectionUpdateSamie(q.fldID, s, 1);
                //            //System.IO.File.AppendAllText(@"F:\RasaCo Programs\Avarez Team\سامانه سمیع\send.txt", "boin_" + item.fldID + "_" + s);
                //        }
                //    }
                //}
            }
            catch(Exception x){
                Models.cartaxEntities Car = new Models.cartaxEntities();
                System.Data.Entity.Core.Objects.ObjectParameter Eid = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                Car.sp_ErrorProgramInsert(Eid, InnerException, 1, x.Message, DateTime.Now, "");
                //return Json(new { MsgTitle = "خطا", Msg = "خطایی با شماره: " + Eid.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.", Er = 1 });
            }
        }
        static string loginTken(string user, string pass)
        {
            string BaseUrl = "http://s1.saamie.ir/MunicipalTollPaymentPortal/Identity/sign-in";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BaseUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            SignIn login = new SignIn()
            {
                /*username = new UserName() { value = "94290917" },
                password = new Password() { value = "45697059" }//برف انبار*/
                //username = new UserName() { value = "338488501" },
                //password = new Password() { value = "794746483" }//بوئین
                username = new UserName() { value = user },
                password = new Password() { value = pass }
            };
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(login);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string s = streamReader.ReadToEnd();//.Replace("\"", "");
                //FillCatalog("Bearer " + s);
                var o = JsonConvert.DeserializeObject<Login>(s);
                return "Bearer " + o.jwt.accessToken;
            }
        }
    }
    
    public class SignIn
    {
        public UserName username { get; set; }
        public Password password { get; set; }
    }
    public class UserName
    {
        public string value { get; set; }
    }
    public class Password
    {
        public string value { get; set; }
    }
    public class CreateComplication
    {
        public Vin vin { get; set; }
        public Year productYear { get; set; }
        public string plaqueTwoLeftDigit { get; set; }
        public string plaqueLetter { get; set; }
        public string plaqueThreeDigit { get; set; }
        public string plaqueTwoRightDigit { get; set; }
        public CarType2 carType { get; set; }
        public string datePament { get; set; }
        public Year carComplicationYear { get; set; }
        public int paymentPriceByOwner { get; set; }
        public string paymentCode { get; set; }
        public string destinationBankName { get; set; }
        public string bankAccount { get; set; }
        public string municipalId { get; set; }
    }
    public class Vin
    {
        public string value { get; set; }
    }
    public class Year
    {
        public int value { get; set; }
    }
    public class CarType2
    {
        public string value { get; set; }
    }
    public class Login
    {
        public jwt jwt { get; set; }
    }
    public class jwt
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string expires { get; set; }
        public string id { get; set; }
        public claims claims { get; set; }
    }
    public class claims
    {
        public string MunicipalId { get; set; }
    }
}