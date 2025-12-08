<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
      Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
      Avarez.DataSet.DataSet1TableAdapters.sp_CharacterPersianPlaqueSelectTableAdapter character = new Avarez.DataSet.DataSet1TableAdapters.sp_CharacterPersianPlaqueSelectTableAdapter();
      sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
      character.Fill(dt.sp_CharacterPersianPlaqueSelect, "", "", 0, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
      Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
      Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
      var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
      var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\CharacterPersianPlaque.frx");
      WebReport1.RegisterData(dt,"complicationsCarDBDataSet");
      WebReport1.Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
      var time=DateTime.Now;
      WebReport1.Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
      WebReport1.Report.SetParameterValue("MunicipalityName", mnu.fldName);
      WebReport1.Report.SetParameterValue("StateName", State.fldName);
      WebReport1.Report.SetParameterValue("AreaName", Session["area"].ToString());
      WebReport1.Report.SetParameterValue("OfficeName", Session["office"].ToString());
      WebReport1.Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
      WebReport1.Prepare();      
      
  } %>
<form id="Form1" runat="server" dir="ltr" >
    <cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" Style="direction: ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>