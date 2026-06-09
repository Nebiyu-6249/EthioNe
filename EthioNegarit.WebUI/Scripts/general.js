$(function () {
    // Sort the date of the member
    SortWhosOnline();
    // Subscription Methods
    emailsubscription();
    emailunsubscription();
    // Setup the upload field styles
    SetUpUploadStyle();
});

/*------------------ General Ajax Form Class ----------------------*/

var submitAjaxForm = function () {
    var submitFormElement = $(".ajaxform form");
    submitFormData(submitFormElement);
};
/*------------------ GENERAL SUBMIT FORMS METHOD -------------------*/
var submitFormData = function (formElement) {
    formElement.submit(function (e) {
        e.preventDefault();
        $(this).validate();
        if ($(this).valid()) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                dataType: "json; multipart/form-data",
                cache: false,
                success: function (result) {
                    if (result.Success) {
                        ShowUserMessage(result.ReturnMessage);
                    } else {
                        ShowUserMessage(result.ReturnMessage);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        }
        return false;
    });
};
var SetUpUploadStyle = function () {
   // $(".removeattachment").attr("style", "display:none");
    $(document).on("change", ".btn-file :file", function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger("fileselect", [numFiles, label]);
    });

    $(".btn-file :file").on("fileselect", function (event, numFiles, label) {
        var input = $(this).parents(".input-group").find(":text"),
            log = numFiles > 1 ? numFiles + " files selected" : label;

        if (input.length) {
            input.val(log);
        } else {
            if (log) alert(log);
        }
    });
};

/*------------ SUBSCRIPTION METHODS --------------------*/
var emailsubscription = function () {
    var esub = $(".emailsubscription");
    if (esub.length > 0) {
        esub.click(function (e) {
            e.preventDefault();
            var entityId = $(this).data("id");
            var subscriptionType = $(this).data("type");
          $(this).hide();
            var subscribeEmailViewModel = new Object();
            subscribeEmailViewModel.Id = entityId;
            subscribeEmailViewModel.SubscriptionType = subscriptionType;
            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(subscribeEmailViewModel);
            $.ajax({
                url: app_base + "Email/Subscribe",
                type: "POST",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                   $(".emailunsubscription").fadeIn();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        });
    }
};
var emailunsubscription = function () {
    var eunsub = $(".emailunsubscription");
    if (eunsub.length > 0) {
        eunsub.click(function (e) {
            e.preventDefault();
            var thisLink = $(this);
            var entityId = $(this).data("id");
            var subscriptionType = $(this).data("type");
            thisLink.hide();
            var unSubscribeEmailViewModel = new Object();
            unSubscribeEmailViewModel.Id = entityId;
            unSubscribeEmailViewModel.SubscriptionType = subscriptionType;
            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(unSubscribeEmailViewModel);
            $.ajax({
                url: app_base + "Email/UnSubscribe",
                type: "POST",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                	var emailsubscription = $(".emailsubscription");
                	if (emailsubscription) {
                		emailsubscription.fadeIn();
                	}
                	// We might be on the following page, so hide the items we need
                	var topicrow = thisLink.closest(".topicrow");
                	if (topicrow) {
                		topicrow.fadeOut("fast");
                	}
                //	var categoryBox = thisLink.closest(".categorySubscribed");
                	var categoryBox = thisLink.closest("li").find(".categorySubscribed");
                	if (categoryBox) {
                		categoryBox.fadeOut("fast");
                    }                  
                    var chatroomBox = thisLink.closest(".chatroom");
                    if (chatroomBox) {
                    	chatroomBox.fadeOut("fast");
                    }              
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        });
    }
};

/*------------ DARK MODE --------------------*/
window.addEventListener("load", function () {
    // Set the default theme to light if no setting is found in local storage
    var switchElement = document.getElementById("darkSwitch");
    switchElement.checked = localStorage.getItem("theme") !== null && localStorage.getItem("theme") === "dark";
    if (localStorage.getItem("theme") !== null && localStorage.getItem("theme") === "dark") {
        $('body').addClass("dark-mode");
        $('nav').removeClass("navbar-light").addClass("navbar-dark");
        $('aside').removeClass("sidebar-light-primary").addClass("sidebar-dark-primary");

    } else {
        localStorage.setItem("theme", "light");
        $('body').removeClass("dark-mode");
        $('nav').removeClass("navbar-dark").addClass("navbar-light");
        $('aside').removeClass("sidebar-dark-primary").addClass("sidebar-light-primary");
    }
    switchElement.addEventListener('change', function () {
        if (this.checked) {
            localStorage.setItem("theme","dark");
            $('body').addClass("dark-mode");
            $('nav').removeClass("navbar-light").addClass("navbar-dark");
            $('aside').removeClass("sidebar-light-primary").addClass("sidebar-dark-primary");
        } else {
            localStorage.setItem("theme", "light");
            $('body').removeClass("dark-mode");
            $('nav').removeClass("navbar-dark").addClass("navbar-light");
            $('aside').removeClass("sidebar-dark-primary").addClass("sidebar-light-primary");           
        }
    });
});
// Whos online
var SortWhosOnline = function () {
    $.getJSON(app_base + "Members/LastActiveCheck");
}
/**
 * Side Panel
 * ====================================================
 */

$(document).on("click", ".push-panel", function (e) {
    var closePanelButton = $(".close-Panel");
    e.preventDefault();
    var thisButton = $(this);
    const $body = $('body');
    $body.addClass("side-panel-open")
    thisButton.hide();
    closePanelButton.show();
});
$(document).on("click", ".close-panel", function (e) {
    e.preventDefault();
    var thisButton = $(this);
    const $body = $('body');
    $body.removeClass("side-panel-open")
    var pushPanelButton = $(".push-panel");
    pushPanelButton.show();
});
/**
 * Side Panel End
 * ====================================================
 */
/*------------ NOTIFICATIONS --------------------*/
$(document).on("click", ".enablenotification", function () {
    var enablenotification = $(".enablenotification");
    var disablenotification = $(".disablenotification");
    var notificationHolder = $(".notifications");
    var showNotification = $(".shownotification");
    var notificationIcon = showNotification.closest("li").find(".notificationicon");
    var notificationbadge = showNotification.closest("li").find(".newnotification");
    $.ajax({
        url: app_base + "Notification/Desktop",
        type: "POST",
        async: true,
        cache: false,
        success: function () {
            notificationIcon.removeClass("far fa-bell-slash").addClass("far fa-bell")
            showNotification.addClass("btn-wobble")
            notificationbadge.fadeIn();
            notificationHolder.show();
            disablenotification.fadeOut();
            enablenotification.fadeIn();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".disablenotification", function () {
    var enablenotification = $(".enablenotification");
    var notificationHolder = $(".notifications");
    var showNotification = $(".shownotification");
    var notificationIcon = showNotification.closest("li").find(".notificationicon");
    var notificationbadge = showNotification.closest("li").find(".newnotification");
    $(this).fadeOut();
    $.ajax({
        url: app_base + "Notification/Desktop",
        type: "POST",
        async: true,
        cache: false,
        success: function () {
            notificationIcon.removeClass("far fa-bell").addClass("far fa-bell-slash");
            showNotification.removeClass("btn-wobble")
            notificationbadge.fadeOut("slow");
            notificationHolder.hide();   
            enablenotification.fadeIn();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".shownotification", function () {
            var thisButton = $(this);
            var notificationIcon = thisButton.closest("li").find(".notificationicon");
            if (notificationIcon.hasClass("fas fa-bell")) {
                notificationIcon.removeClass("fas fa-bell").addClass("far fa-bell");
            } else {
                notificationIcon.removeClass("far fa-bell").addClass("fas fa-bell");
            }
            var notificationbadge = thisButton.closest("li").find(".newnotification");
            notificationbadge.fadeOut("slow");
            $.ajax({
                url: app_base + "Notification/Update",
                type: "POST",
                async: true,
                cache: false,
                success: function () {
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        });
$(document).on("change", "#desktopnotification", function (e) {
    e.preventDefault();
    var thisButton = $(this);
    if (thisButton.length > 0) {
        $.ajax({
            url: app_base + "Notification/Desktop",
            type: "POST",
            cache: false,
            success: function (data) {
               // GetNotifications();
                var showNotification = $(".shownotification");

                var notificationIcon = showNotification.closest("li").find(".notificationicon");
                if (notificationIcon.hasClass("far fa-bell-slash")) {
                    notificationIcon.removeClass("far fa-bell-slash").addClass("far fa-bell");
                }
                else {
                    notificationIcon.removeClass("far fa-bell").addClass("far fa-bell-slash");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowUserMessage("Error: " + xhr.status + " " + thrownError);
            }
        });
    }

});
$(document).on("change", "#emailnotification", function (e) {
    e.preventDefault();
    var thisButton = $(this);
    if (thisButton.length > 0) {
        $.ajax({
            url: app_base + "Notification/Email",
            type: "POST",
            cache: false,
            success: function (data) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowUserMessage("Error: " + xhr.status + " " + thrownError);
            }
        });
    }

});
$(document).on("click", ".rsslink", function () {
    var rsslink = $(this);
    var actionlink = rsslink.data("action");
    var categorySlug = rsslink.data("slug");
    $("#copylink").modal("show");
    $.ajax({
        url: app_base + "Home/CopyUrl",
        type: "POST",
        data: { action: actionlink, slug: categorySlug },
        async: true,
        cache: false,
        success: function (data) {
            $("#modal-copylink").html("");
            $("#modal-copylink").html(data)
        }
    });
});
$(document).on("click", ".close", function () {
    $(this).closest('.ad').hide();
});
function setItem(key, value, ttlMilliseconds) {
    const now = new Date();
    // Create an object with the value and the calculated expiry date
    const item = {
        value: value,
        expiry: now.getTime() + ttlMilliseconds
    };
    // Store the object as a JSON string
    localStorage.setItem(key, JSON.stringify(item));
}
function getItem(key) {
    const itemStr = localStorage.getItem(key);
    // If the item doesn't exist, return null
    if (!itemStr) {
        return null;
    }
    const item = JSON.parse(itemStr);
    const now = new Date();
    // Compare the current time with the expiry time
    if (now.getTime() > item.expiry) {
        // If expired, remove the item and return null
        localStorage.removeItem(key);
        return null;
    }
    // Otherwise, return the value
    return item.value;
}