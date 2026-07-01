//$(document).ready(function () {
    $(function () {
  //  paymentNotice();
    //  setInterval(showPaymentNotice, 1000 * 60);
    const now = new Date();
    const delay = 3600000 - (now.getMinutes() * 60 + now.getSeconds()) * 1000 - now.getMilliseconds();
    setTimeout(() => {
        showPaymentNotification();
        // Subsequent executions every hour
        setInterval(showPaymentNotification, 3600000);
    }, delay);
});
$(document).ready(function () {
    $("#searchpayment").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#paymenttable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});
function LoadPayments() {
    cache: false;
    $.ajax({
        url: app_base + "Payment/Index",
        type: "GET",      
        success: function (data) {
            $("#payments").empty();
            $("#payments").html(data);       
        }
    });
}
/**
 * Payment List Click Events
 * ====================================================
 */
function ShowConfirmationMessage() {
    setTimeout(function () {
        $("#confirmationMessage").delay(5000).fadeOut();
    }, 5000)
}
$(document).on("click", ".paymentconfirmationlink", function () {
    var thislink = $(this);
    var paymentId = $(this).data("id");
    var rejectionlink = $(".paymentrejectionlink");
    var paymentConfirmationViewModel = new Object();
    paymentConfirmationViewModel.Id = paymentId;
    var strung = JSON.stringify(paymentConfirmationViewModel);
    $.ajax({
        url: app_base + "Payment/Confirm",
        type: "POST",
        cache: false,
        data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {          
            $("#confirmationMessage").text("Payment Confirmed");
            ShowConfirmationMessage();
            thislink.fadeOut();
          rejectionlink.show();
          LoadPayments();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".paymentrejectionlink", function () {
    var rejection_link = $(this);
  //  var confirmation_link = $(".paymentconfirmationlink");
  
    var paymentId = $(".paymentrejectionlink").data('id');
    var paymentRejectionViewModel = new Object();

    paymentRejectionViewModel.Id = paymentId;
    var strung = JSON.stringify(paymentRejectionViewModel);
    $.ajax({
        url: app_base + "Payment/Reject",
        type: "POST",
        cache: false,
        //data: { paymentId: payment_Id },
        data: strung,
          contentType: "application/json; charset=utf-8",
        success: function (data) {
            rejection_link.hide();
            $("#confirmationMessage").text("Confirmation Rejected");
            ShowConfirmationMessage();
           // confirmation_link.show();
            LoadPayments();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
    // });
    //}

});
$(document).on("click", ".paymentdeletelink", function () {
    var paymentId = $(this).data("id");
    var deleteconfirmationText = $(this).data("deleteconfirmation");
    var deletedText = $(this).data("paymentdeleted");
    var paymentDeleteViewModel = new Object();
    paymentDeleteViewModel.Id = paymentId;
    var strung = JSON.stringify(paymentDeleteViewModel);
    var row = $(this).closest("tr");
    if (confirm(deleteconfirmationText)) {
        $.ajax({
            url: app_base + "Payment/Delete",
            type: "POST",
            cache: false,
            data: strung,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                row.fadeOut("fast");
                $("#confirmationMessage").text(deletedText);
               ShowConfirmationMessage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowUserMessage("Error: " + xhr.status + " " + thrownError);
            }
        });
    }
});
var showPaymentNotification = function () {
    //Getting an item   
    const paymentNotification = getItem('paymentNotification');
    if (paymentNotification) {
        return null;
    } else {
        $.ajax({
            url: app_base + "Payment/Notification",
            type: "POST",
            cache: false,
            success: function (data) {
                if (data == null) {
                    return;
                } else {
                    $(".notification").show();
                    $("div.notification").delay(250 * 60).fadeIn();
                    $(".notification").html(data);
                    $("div.notification").delay(250 * 60).fadeOut();
                    //To store Payment Notification that expires in 1 hour (3,600,000 milliseconds):
                    setItem('paymentNotification', 'paymentNotification', 3600000);
                }
            },
        });
    }
};
