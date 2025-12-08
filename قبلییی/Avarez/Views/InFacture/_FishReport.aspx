<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   فیش
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      Avarez.DataSet.DataSet1 dt = new Avarez.DataSet.DataSet1();
      Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter sp_pic = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelectTableAdapter();
      Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter sp_pic1 = new Avarez.DataSet.DataSet1TableAdapters.sp_PictureSelect1TableAdapter();
      Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter fish = new Avarez.DataSet.DataSet1TableAdapters.sp_PeacockerySelectTableAdapter();
      
      sp_pic1.Fill(dt.sp_PictureSelect1, "fldMunicipalityPic", Session["UserMnu"].ToString(), 1, 1, "");
      

      fish.Fill(dt.sp_PeacockerySelect, "fldId", Session["FishId"].ToString(), 1, 1, "");
      Avarez.Models.cartaxEntities car = new Avarez.Models.cartaxEntities();
      var _fish = car.sp_PeacockerySelect("fldId", Session["FishId"].ToString(), 1, 1, "").FirstOrDefault();

      sp_pic.Fill(dt.sp_PictureSelect, "fldBankPic", _fish.fldBankId.ToString(), 1, 1, "");
      var mnu = car.sp_MunicipalitySelect("fldId", Session["UserMnu"].ToString(), 1, 1, "").FirstOrDefault();

      var UpReportSelect = car.sp_UpReportSelect(Convert.ToInt32(Session["CountryType"]), Convert.ToInt32(Session["CountryCode"])).FirstOrDefault();
      var FishReport = car.sp_ReportsSelect("fldId", UpReportSelect.fldReportsID.ToString(), 1, Convert.ToInt32(Session["UserId"]), Session["UserPass"].ToString()).FirstOrDefault();
      System.IO.MemoryStream Stream = new System.IO.MemoryStream(FishReport.fldReport);
      WebReport1.Report.Load(Stream);
      WebReport1.RegisterData(dt, "complicationsCarDBDataSet");
      WebReport1.Report.SetParameterValue("MunicipalityName", mnu.fldName);
      WebReport1.Report.SetParameterValue("Barcode", Session["Barcode"].ToString());
      WebReport1.Report.SetParameterValue("ShGhabz", Session["ShGhabz"].ToString());
      WebReport1.Report.SetParameterValue("ShPardakht", Session["ShPardakht"].ToString());
      WebReport1.Report.SetParameterValue("SalAvarez", Session["SalAvarez"].ToString());
      WebReport1.Report.SetParameterValue("Mohlat", Session["Mohlat"].ToString());    
      
      WebReport1.Prepare();
      Session.Remove("FishId");
      Session.Remove("ShGhabz");
      Session.Remove("ShPardakht");
      Session.Remove("Barcode");
      Session.Remove("SalAvarez");
      Session.Remove("Mohlat");
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="800" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="False" AutoHeight="False" />
</form>

</asp:Content>
