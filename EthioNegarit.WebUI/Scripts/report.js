//$(function () {
/*$(document).ready(function () {
    var input = document.getElementById("reason");
    input.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            document.getElementById("submitreport").click();
         //   $("#submitreport").click();
            //    $('#message').val('').focus();
        }
    });
});*/
//auto click when enter is pressed
$('#reason').keydown(function (e) {
    if (e.keyCode == 13) {
        e.preventDefault();
        $("#submitreport").click();
    }
})
var AjaxReportBegin = function () {
    var submitreport = $("#submitreportbutton");
    if (submitreport.length > 0) {
        submitreport.attr("disabled", true);
    }
}
var AjaxReportFinish = function () {
    var submitreport = $("#submitreport");
    if (submitreport.length > 0) {
        submitreport.attr("disabled",false);
    }

    var postHolder = $(this).closest("div.post");
    var reportHolder = postHolder.find("div.newreport");

    if (reportHolder.length > 0) {
        reportHolder.hide();
       
    }

    var reportformHolder = $(".newreport");
    if (reportformHolder.length > 0) {
        reportformHolder.hide();

    }
}
var AjaxReportError = function (message) {
    ShowUserMessage(message);
    AjaxReportFinish();
}
var AjaxReportSuccess = function () {
    $(".reason").val("");
  //  document.getElementById("postreport").innerHTML = "Reported";
    document.getElementById("postreport").fadeOut();
    document.getElementById("postreported").fadeIn();
}
/**
 * Post Report
 * ====================================================
 */
$(document).on("click", "#submitreport", function (e) {
    var thisLink = $(this);
    var id = thisLink.data("id");
    var sentconfirmationText = thisLink.data("confirmation");
    $("#post-" + id).fadeOut("slow");
    $("#postreport-" + id).hide();
    $("#postreported-" + id).show();
    $("#confirmationMessage").text(sentconfirmationText);
    ShowConfirmationMessage();
});
function ShowConfirmationMessage() {
    setTimeout(function () {
        $("#confirmationMessage").delay(20000).fadeOut();
    }, 20000)
}
/**
 * Member
 * ====================================================
 */
$(document).on("click", ".memberreportlink", function () {
    var reportLink = $(this);
    var user_Id = reportLink.data("id");
    $("#modal-memberreport").show();
    $("#memberreport").modal("show");
    $.ajax({
        url: app_base + "Report/MemberReport",
        type: "GET",
        data: { userId: user_Id },
        async: true,
        cache: false,
        success: function (data) {
            $("#modal-memberreport").html("");
            $("#modal-memberreport").html(data)
        }
    });
});


