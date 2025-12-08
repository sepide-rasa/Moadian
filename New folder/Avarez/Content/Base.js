function windows (id, url) {
    $.ajax({ url: url, cache: false,async: false, type: "GET" })
    .done(function (e) { $(id).html(e); })
    .fail(function (e) { console.log(e); });
}


function windowAppend(id, url) {
    $.ajax({ url: url, cache: false, async: false,type: "GET" })
    .done(function (e) {
        $(id).append(e); 
    })
    .fail(function (e) {
        console.log(e);
     });
}



