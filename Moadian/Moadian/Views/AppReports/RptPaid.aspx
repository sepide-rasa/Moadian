<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    گزارش فیش های پرداخت شده
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
      Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
      Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter Peacockery = new Avarez.DataSet.DataSet1TableAdapters.sp_RptPeacockeryTableAdapter();
      sp_pic.Fill(dt.sp_PictureSelect, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString());
      Peacockery.Fill(dt.sp_RptPeacockery, "Paid", Convert.ToInt32(Session["UserMnu"]), MyLib.Shamsi.Shamsi2miladiDateTime(Session["start"].ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(Session["end"].ToString()),1);
      Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
      Avarez.Models.MyTablighat MyTablighat = new Avarez.Models.MyTablighat(Session["UserMnu"].ToString());
      var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
      var State = car.sp_StateSelect("fldId", Session["UserState"].ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();

      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Rpt_Paid.frx");
      WebReport1.RegisterData(dt, "carTaxDataSet");
      WebReport1.Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
      var time=DateTime.Now;
      WebReport1.Report.SetParameterValue("time", time.Hour + ":" + time.Minute + ":" + time.Second);
      WebReport1.Report.SetParameterValue("MunicipalityName", mnu.fldName);
      WebReport1.Report.SetParameterValue("StateName", State.fldName);
      WebReport1.Report.SetParameterValue("AreaName", Session["area"].ToString());
      WebReport1.Report.SetParameterValue("OfficeName", Session["office"].ToString());
      WebReport1.Report.SetParameterValue("MyTablighat", MyTablighat.Matn);
      WebReport1.Prepare();
      Session.Remove("start");
      Session.Remove("end");
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>

</asp:Content>
