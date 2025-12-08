using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;

namespace Avarez
{
    /// <summary>
    /// Summary description for Estelam
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Estelam : System.Web.Services.WebService
    {
        public static string GenerateHash(string value)
        {
            SHA1 sha1 = SHA1.Create();
            //convert the input text to array of bytes
            if (value == null)
            {
                return "رمز عبور ضروری است.";
            }
            byte[] hashData = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString("X2"));
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        [WebMethod]
        public clsCar GetCar(string VIN,string User,string pass)
        {
            Models.cartaxEntities p = new Models.cartaxEntities();
            var user = p.prs_TblWebSrvUser(User, GenerateHash(pass)).Any();
            if (user)
            {
                var car = p.sp_CarSelect("fldvin", VIN, 0, 1, "").FirstOrDefault();
                clsCar c = new clsCar();
                if (car != null)
                {
                    var carfile = p.sp_CarFileSelect("fldCarId", car.fldID.ToString(), 1, 1, "").FirstOrDefault();
                    if (carfile != null)
                    {
                        var cc = p.sp_SelectCarDetils(car.fldID).FirstOrDefault();
                        c.CarType = cc.fldCarSystemName + " " + cc.fldCarModel + " " + cc.fldCarClassName;
                        c.Model = (short)cc.fldModel;
                        c.ChasiNum = cc.fldShasiNumber;
                        c.MotorNum = cc.fldMotorNumber;
                        c.VIN = cc.fldVIN;
                        c.Mafasa = new List<Mafasa>();
                        c.Exp = new List<Exprience>();
                        c.Pay = new List<Pay>();
                        var varizi = p.rpt_Receipt(car.fldID, 2).ToList();
                        foreach (var item in varizi)
                        {
                            c.Pay.Add(new Pay
                            {
                                PayDate = item.fldCollectionDate,
                                Price = item.fldPrice,
                                MunName = item.fldMunName
                            });
                        }

                        var exp = p.sp_CarExperienceSelect("fldCarID", car.fldID.ToString(), 30, 0, "").ToList();
                        foreach (var item in exp)
                        {
                            var file = p.Sp_FilesSelect(item.fldFileId).FirstOrDefault();
                            byte[] image = null;
                            //if (file != null)
                            //    image = file.fldImage;
                            c.Exp.Add(new Exprience
                            {
                                FromDate = item.fldStartDate,
                                ToDate = item.fldEndDate,
                                MunName = item.fldName,
                                Image = image
                            });
                        }


                        var mafasa = p.Sp_MafasaSelect(car.fldID).ToList();
                        foreach (var item in mafasa)
                        {
                            var mun = p.sp_MunicipalitySelect("fldid", item.fldMunId.ToString(), 0, 1, "").FirstOrDefault();
                            var image = p.Sp_MafasaSelect_Image1(item.fldRef.ToString()).FirstOrDefault();
                            byte[] img = null;
                            //if (image != null)
                            //    img = image.fldimage;
                            c.Mafasa.Add(new Mafasa
                            {
                                Date = item.fldLetterDate,
                                MunName = mun.fldName,
                                Image = img,
                                RefCode = item.fldRef
                            });
                        }

                        return c;
                    }
                    return null;
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
    
}
