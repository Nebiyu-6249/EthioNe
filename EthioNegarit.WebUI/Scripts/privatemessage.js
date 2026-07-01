$(function () {
      // LoadMessages();
    blockMember();
    ShowMorePm();
});
function LoadMessages() {
    cache: false;
    $.ajax({
        url: app_base + "PrivateMessage/Inbox",
        type: "GET",
        success: function (data) {
            $("#privateMessage").empty();
            $("#privateMessage").html(data);
        }
    });
}
var ShowMorePm = function () {
    var smp = $(".showmorepms");
    if (smp.length > 0) {
        smp.click(function (e) {
            e.preventDefault();
            var userId = $("#userId").val();
            var pageIndex = $("#pageIndex");
            var totalPages = parseInt($("#totalPages").val());
            var activeText = $("span.smpmactive");
            var loadingText = $("span.smpmloading");
            var showMoreLink = $(this);
            activeText.hide();
            loadingText.show();
            var getMoreViewModel = new Object();
            getMoreViewModel.UserId = userId;
            getMoreViewModel.PageIndex = pageIndex.val();
            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(getMoreViewModel);
            $.ajax({
                url: app_base + "PrivateMessage/AjaxMore",
                type: "POST",
                dataType: "html",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    // Now add the new posts
                    showMoreLink.before(data);
                    // Update the page index value
                    var newPageIdex = (parseInt(pageIndex.val()) + parseInt(1));
                    pageIndex.val(newPageIdex);
                    // If the new pageindex is greater than the total pages, then hide the show more button
                    if (newPageIdex > totalPages) {
                        showMoreLink.hide();
                    }
                    // Lastly reattch the click events
                    //  deletePrivateMessages();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                    activeText.show();
                    loadingText.hide();
                }
            });

        });
    }
}
$(document).on("click", ".privatemessagesdeleteall", function () {
    var deleteallmessages = $(this);
    var deleteconfirmationText = deleteallmessages.data("confirmation");
    var userId = deleteallmessages.data("id");
    var deleteAllMessagesViewModel = new Object();
    deleteAllMessagesViewModel.UserId = userId;
    // Ajax call to post the view model to the controller
    var strung = JSON.stringify(deleteAllMessagesViewModel);
    var pmMessage = $(this).closest(".message-item");
    if (confirm(deleteconfirmationText)) {
    $.ajax({
        url: app_base + "PrivateMessage/DeleteAll",
        type: "POST",
        cache: false,
        data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            // Just fade out anyway
            pmMessage.fadeOut("fast");
            LoadMessages();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
    }
});
$(document).on("click", ".privatemessagedelete", function () { 
    var privatemessagedelete = $(this);
   // var messageId = $(this).data("id");
   var messageId = privatemessagedelete.data("id");
    var deletePrivateMessageViewModel = new Object();
        deletePrivateMessageViewModel.Id = messageId;
       // Ajax call to post the view model to the controller
      var strung = JSON.stringify(deletePrivateMessageViewModel);
    // Just fade out anyway
    var pmHolder = privatemessagedelete.closest(".message-block");
            pmHolder.fadeOut("fast");
            $.ajax({
                url: app_base + "PrivateMessage/Delete",
                type: "POST",
                cache: false,
                data: strung,
               contentType: "application/json; charset=utf-8",
               success: function (data) {
                   LoadMessages();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
    });
$(document).on("click", ".markstarlink", function (e) {
    e.preventDefault();
    var starLink = $(this);
    var messageId = starLink.data("id");
    var markPrivateMessageViewModel = new Object();
    markPrivateMessageViewModel.Id = messageId;
    // Ajax call to post the view model to the controller
    var strung = JSON.stringify(markPrivateMessageViewModel);
   var starIcon = starLink.closest("li").find(".message-star");
    $.ajax({
        url: app_base + "PrivateMessage/MessageStar",
        type: "POST",
        cache: false,
        data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
           if (starIcon.hasClass("fas fa-star")) {
                starIcon.removeClass("fas fa-star").addClass("far fa-star");
            }
            else {
                starIcon.removeClass("far fa-star").addClass("fas fa-star");
            }
            LoadMessages();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".attachmentlink", function () {
    var attachmentLink = $(this);
    var message_Id = attachmentLink.data("id");
    $("#modalBodyDiv").show();
    $("#attachement").modal("show");
    $.ajax({
      url: app_base + "File/Attachments",
        type: "GET",
        data: { messageId: message_Id },
        async: true,
        cache: false,
        success: function (data) {
            $("#modalBodyDiv").html("");
            $("#modalBodyDiv").html(data)
        }

    });
});
$(document).on("click", ".show-messages", function (e) {
    e.preventDefault();
    $(".show-messages").unbind("click");
    var messagesLink = $(this);
    var fromId = messagesLink.data("id");
    var title = messagesLink.data("name");
    slideOutPanel(title);
    $.ajax({
        url: app_base + "PrivateMessage/View",
        type: "GET",
        async: true,
        cache: false,
        data: { from: fromId },
        // data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            // Load the Html into the slide Out panel
            slideOutPanel(title, data);
            // Trigger Validation
            $.validator.unobtrusive.parse(document);
            // Delete private messages
          //  deletePrivateMessages();
            blockMember();
            ShowMorePm();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
var blockMember = function () {
    var blockMemberButton = $(".pm-block");
    if (blockMemberButton.length > 0) {
        blockMemberButton.click(function (e) {
            e.preventDefault();
            var pmButton = $(this);
            var thisButton = $(this);
            var blockText = pmButton.data("blocktext");
            var blockedText = pmButton.data("blockedtext");
            var isBlocked = pmButton.data("isblocked");
            var userid = pmButton.data("userid");
            var blockIcon = thisButton.closest("li").find(".blockicon");
            if (isBlocked) {
                pmButton.text(blockText);
            } else {
                pmButton.text(blockedText);
            }
            var viewModel = new Object();
            viewModel.MemberToBlockOrUnBlock = userid;
            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(viewModel);
            $.ajax({
                url: app_base + "Block/BlockOrUnBlock",
                type: "POST",
                data: strung,
                cache: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    // Just update attribute to opposite
                    if (isBlocked) {
                        pmButton.data("isblocked", false);
                    } else {
                        pmButton.data("isblocked", true);
                    }
                    ///////////////                    
                    if (blockIcon.hasClass("fa fa-user-slash")) {
                        blockIcon.removeClass("fa fa-user-slash").addClass("fa fa-user");
                    } else {
                        blockIcon.removeClass("fa fa-user").addClass("fa fa-user-slash");
                    }
                    ////////////////////////
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    // Switch back if error
                    if (isBlocked) {
                        pmButton.text(blockedText);
                    } else {
                        pmButton.text(blockText);
                    }
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });

        });
    }
};
var AjaxPmSuccess = function () {
    $("#fileProgress").hide();
    $("progress").hide();
    var createmessageholder = $(".createmessageholder");
    var messagesentText = createmessageholder.data("messagesent");
    $("#lblMessage").html(messagesentText)
    $('#FileUpload').val("");
    $(".removeattachment").attr("style", "display:none");
    // Grab the span the newly added message is in
    var pmHolder = $("#newpmmarker");
    // Now add a new span after with the key class
    // In case the user wants to add another ajax message straight after
    pmHolder.after("<span id=\"newpmmarker\"></span>");
    // Finally chnage the name of this element so it doesn't insert it into the same one again
    pmHolder.attr("id", "imabebe");
    // And more finally clear the message box
    $(".rte").val("");
  /*  if ($(".bbeditorholder textarea").length > 0) {
        $(".bbeditorholder textarea").data("sceditor").val("");
    }
    if ($(".wmd-input").length > 0) {
        $(".wmd-input").val("");
        $(".wmd-preview").html("");
    }*/
    if (typeof tinyMCE != "undefined") {
        tinyMCE.activeEditor.setContent('');
    }
    AjaxPmFinish();
    setTimeout(function () {
        $("#lblMessage").delay(5000).fadeOut();
    }, 5000)
}
var AjaxPmFinish = function () {
    var pmButton = $("#createpmbutton");
    if (pmButton.length > 0) {
        pmButton.attr("disabled", false);
    }
    LoadMessages();
}
var AjaxPmBegin = function () {
    var pmButton = $("#createpmbutton");
    if (pmButton.length > 0) {
        pmButton.attr("disabled", true);
    }
       if (window.FormData !== undefined) {
            var fileUpload = $("#FileUpload").get(0);
            var files = fileUpload.files;
            // Create FormData object
            var fileData = new FormData();
          var  other_data = $('#postcodeform').serializeArray();
            if (files.length > 0) {
                $.each(other_data, function (key, input) {
                    fileData.append(input.name, input.value);
                })
                // Looping over all files and add it to FormData object
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }
            }
            $.ajax({
                url: app_base + "PrivateMessage/Create",
                type: "POST",
                cache: false,
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (data) {

                },
                xhr: function () {
                    var fileXhr = $.ajaxSettings.xhr();
                    if (fileXhr.upload) {
                        $("progress").show();
                        fileXhr.upload.addEventListener("progress", function (e) {
                            if (e.lengthComputable) {
                                $("#fileProgress").attr({
                                    value: e.loaded,
                                    max: e.total
                                });
                            }
                        }, false);
                    }
                    return fileXhr;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });       
        } else {
            alert("FormData is not supported.");
        }
}
