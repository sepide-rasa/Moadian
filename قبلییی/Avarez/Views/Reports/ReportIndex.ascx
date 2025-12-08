<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<%{
      Avarez.DataSet.DataSet1 a = new Avarez.DataSet.DataSet1();
   
    string Path = ViewBag.Path;
    WebReport1.Report.Load(Path);
      

    WebReport1.ID = ViewBag.RId;
	// activate Designer in WebReport
        WebReport1.DesignReport = true;
	// you can restrict transfer in browser and edit of script code
        WebReport1.DesignScriptCode = true; 
	// path to the Designer index
        WebReport1.DesignerPath = "~/WebReportDesigner/index.html"; 
        // path with R/W permissions for saving of designed reports    
	WebReport1.DesignerSavePath = "~/App_Data"; 
	// path to call-back page in your web application, we make a GET query with parameters reportID="here is webReport.ID"&reportUUID="here is saved report file name"
    WebReport1.DesignerSaveCallBack = "~/Reports/SaveDesignedReport"; 

  } %>

<form id="Form1" runat="server" dir="ltr">
    <cc1:WebReport ID="WebReport1" runat="server"  Width="100%" Height="100%"></cc1:WebReport>
    
</form>
