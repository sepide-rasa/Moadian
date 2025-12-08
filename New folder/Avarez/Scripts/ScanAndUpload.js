Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', Dynamsoft_OnReady);

function Dynamsoft_OnReady() {
    DWObject = Dynamsoft.WebTwainEnv.GetWebTwain('dwtcontrolContainer');
    DWObject.RegisterEvent("OnPostAllTransfers", Dynamsoft_OnPostAllTransfers);
}

function AcquireImage() {    
    if (DWObject) {
        DWObject.IfDisableSourceAfterAcquire = true;
        DWObject.IfShowUI = true;
        if (DWObject.SourceCount > 0) {
            DWObject.SelectSource();
            DWObject.AcquireImage();
        }
        else
            alert("No TWAIN compatible drivers detected.");
    }
}

function SaveImage() {
    if (DWObject) {
        DWObject.IfShowFileDialog = true;
        if (DWObject.HowManyImagesInBuffer > 0)
            DWObject.SaveAllAsMultiPageTIFF("WebTWAINImage.tiff");
    }
}

/// <summary>
/// This is an asynchronous upload. OnSuccess and OnFailure are callback functions.
/// OnSuccess is the callback function for successful loads while OnFailure is for failed ones.
/// </summary>

function OnSuccess() {
    console.log('successful');

}
function OnFailure(errorCode, errorString) {
    alert(errorString);
}

function LoadImage() {
    if (DWObject) {
        DWObject.IfShowFileDialog = true;
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_ALL, OnSuccess, OnFailure);
    }
}

/// <summary>
/// This is an asynchronous upload. OnHttpUploadSuccess and OnHttpUploadFailure are callback functions.
/// OnHttpUploadSuccess is the callback function for successful uploads while OnHttpUploadFailure is for failed ones.
/// function Dynamsoft_OnPostAllTransfers is registered to the event 'OnPostAllTransfers', which would fire after all transfers end.
/// Please check HomeController -> SaveToFile to learn how to process uploaded files on server side.
/// </summary>

function OnHttpUploadSuccess() {
    //console.log('successful');
    $("#error").remove();
    windowAppend("body", "/metro/error");
    $("#message").html("آپلود با موفقیت انجام شد.");
    $("#error .wintitle").html("ذخیره موفق");
}

function OnHttpUploadFailure(errorCode, errorString, sHttpResponse) {
    $("#error").remove();
    alert(errorString + sHttpResponse);
    windowAppend("body", "/metro/error");
    $("#message").html("آپلود با موفقیت انجام نشد.");
    $("#error .wintitle").html("خطا");
}

function Dynamsoft_OnPostAllTransfers() {
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer > 0) {
            for (var i = 0; i < DWObject.HowManyImagesInBuffer; i++) {
                var strHTTPServer = location.hostname;
                DWObject.HTTPPort = location.port == "" ? 80 : location.port;
                var CurrentPathName = unescape(location.pathname); // get current PathName in plain ASCII
                var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
                var strActionPage = "/TempArchive/SaveToFile?carfile=" + carfile;//CurrentPath + "Home/SaveToFile";
                var uploadfilename = "WebTWAINImage"+i;

                // DWObject.HTTPUploadAllThroughPostAsPDF(strHTTPServer, strActionPage, uploadfilename, OnHttpUploadSuccess, OnHttpUploadFailure);

                DWObject.HTTPUploadThroughPost(strHTTPServer, DWObject.CurrentImageIndexInBuffer, strActionPage, uploadfilename + ".jpg", OnHttpUploadSuccess, OnHttpUploadFailure);

            }

        }
    }
}