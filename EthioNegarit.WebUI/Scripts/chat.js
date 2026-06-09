//show the last time the chat state was fetched from the server
function ShowLastRefresh() {
    var dt = new Date();
    var time = dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
    $("#LastRefresh").text("Last Refresh - " + time);
}
function ScrollChat() {
    //   window.scrollTo(0, document.body.scrollHeight);
    var wtf = $('#chat-panel');
    var height = wtf[0].scrollHeight;
    wtf.scrollTop(height);
}
function ShowLogMessage() {
    setTimeout(function () {
        $("#logonMessage").delay(5000).fadeOut();
    }, 5000)
}
var AddMoreMessages = function (showMoreMessageLink, moreMessages) {
    showMoreMessageLink.before(moreMessages);
}
/**
 * Ajax Form Data
 * ====================================================
 */
var AjaxChatError = function (message) {
    ShowUserMessage(message);
    AjaxChatFinish();
}
var AjaxChatBegin = function () {
    var messageButton = $("#chatmessagebutton");
    if (messageButton.length > 0) {
        messageButton.attr("disabled", true);
    }
}
var AjaxChatFinish = function () {
    var messageButton = $("#chatmessagebutton");
    // Re-enable the button    
    if (messageButton.length > 0) {
        messageButton.attr("disabled", false);
    }
}
var AjaxChatSuccess = function () {
    var chatmessageHolder = $("#chatmessagemarker");
    chatmessageHolder.after("<span id=\"chatmessagemarker\"></span>");
    chatmessageHolder.attr("id", "imabebeduretulamo");
   // $(".message").innerHTML = "";
    $('#message').val('').focus();
    // Clear the reply div
    var replyToDiv = $(".showchatreplyto");
    replyToDiv.html("");
    replyToDiv.hide();
    AjaxChatFinish();
  //  ScrollChat();
    const container = document.querySelector('.chat-panel');
    container.scrollTop = container.scrollHeight;
}
/**
 * Chat Lobby
 * ====================================================
 */
//called every 5 seconds
function Refresh() {
    var room_Id = $("input.roomId").val();
    var user_Id = $("input.userId").val();
    var loggedOn = false;
   if (user_Id.length !== 0) {
        loggedOn = true;
    }
   if (room_Id.length > 0) {
        cache: false;
        $.ajax({
            url: app_base + "Chat/LogOn",
            type: "POST",
            data: { roomId: room_Id, logOn: loggedOn },
            success: function (data) {               
              //  LoadChatMessages();
               GetMembers();
                ShowLastRefresh();
              //  ScrollChat();
            }
        });       
    } else {
    	clearInterval(refreshInterval);
    }
}
function LoadChatMessages() {
    var my_Chat = false;
    if (localStorage.getItem("messageview") !== null && localStorage.getItem("messageview") === "mychat") {
        my_Chat = true;
    }
	var room_Id = $("input.roomId").val();
	if (room_Id.length !== 0) {
        cache: false;
        $.ajax({
            url: app_base + "Chat/Messages",
            type: "POST",
            data: { roomId: room_Id, myChat: my_Chat },
            success: function (data) {
                $("#chat-panel").empty();
                $("#chat-panel").html(data);
                //   ScrollChat();
                Refresh();
            }
        });
	} else {
		return;
	}
}
function GetMembers() {
    var room_Id = $("input.roomId").val();
        cache: false;
        $.ajax({
            url: app_base + "Chat/RoomMembers",
            type: "POST",
            data: { roomId: room_Id, amountToShow: 5 },
            success: function (data) {
                $("#roomMembers").empty();
                $("#roomMembers").html(data);
            }
        });
}
function GetRooms() {
    cache: false;
    $.ajax({
        url: app_base + "Chat/Index",
        type: "GET",
        success: function (data) {
            $("#chatRooms").empty();
            $("#chatRooms").html(data);
        }
    });
}
/**
 * Chat Message Click Events
 * ====================================================
 */
$(document).on("click", ".reactionlink", function () {
    var reactionLink = $(this);
    var messageId = $(this).data('id');
    var reactionType = reactionLink.data("reactiontype");
    var holdingLi = reactionLink.closest("li");
    var holdingUl = holdingLi.closest("ul");
    var reactionCounts = holdingLi.find(".count");

    var hasReaction = reactionLink.data("hasreaction");
    var link_icon = holdingLi.find(".linkicon");
    var spam_icon = holdingLi.find(".spamicon");

    var ChatReactionViewModel = new Object();
    ChatReactionViewModel.MessageId = messageId;
    if (reactionType == "negative") {
        spam_icon.removeClass("fas fa-flag").addClass("far fa-flag");
        link_icon.removeClass("far fa-heart").addClass("fas fa-heart");

    }
    var reactionUrl = "Chat/Report";
    var otherReactionLink = holdingUl.find(".reactionlink[data-reactiontype='positive']");
    if (reactionType == "positive") {
        reactionUrl = "Chat/Like";
        link_icon.removeClass("fas fa-heart").addClass("far fa-heart");
        spam_icon.removeClass("far fa-flag").addClass("fas fa-flag");
        otherReactionLink = holdingUl.find(".reactionlink[data-reactiontype='negative']");
    }
    // We do the show hide/change of votes here for speed in the UI
    // Change the number up or down
    var currentCounts = parseInt(reactionCounts.text());
    if (hasReaction) {
        reactionCounts.text((currentCounts - 1));

        // So show the other link
        otherReactionLink.show();
        link_icon.removeClass("fas fa-heart").addClass("far fa-heart");

        // Change has favourited to false
        reactionLink.data('hasreaction', false);
    } else {
        //  adding a favourited reaction
        reactionCounts.text((currentCounts + 1));

        // Hide the other link
        otherReactionLink.hide();

        // Change has favourited to true
        link_icon.removeClass("far fa-heart").addClass("fas fa-heart");

        reactionLink.data('hasreaction', true);
    }
    // Ajax call to post the view model to the controller
    var strung = JSON.stringify(ChatReactionViewModel);

    $.ajax({
        url: app_base + reactionUrl,
        type: "POST",
        cache: false,
        data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".chatreplylink", function () {
	var message_Id = $(this).data('id'); 
	var room_Id = $(this).data('roomid');
    
    $.ajax({
        url: app_base + "Chat/CreateMessage",
        type: "GET",
        data: { roomId: room_Id, messageId: message_Id },
        async: true,
        cache: false,
        success: function (data) {
            $(".newchatmessage").html("");
            $(".newchatmessage").html(data);
        }

    });
});
$(document).on("click", "#emailsubscription", function (e) {
    var room_Id = $(this).data('id');
    var newchatmessageHolder = $(".newchatmessage");
    var logOffButton = $(".chatlogoff");
    logOffButton.hide();
    newchatmessageHolder.show();
    $.ajax({
        url: app_base + "Chat/LogOn",
        type: "POST",
        data: { roomId: room_Id, logOn: true },
        success: function (data) {

            GetMembers();
            refreshInterval = setInterval("Refresh();", 5 * 1000);
        }
    });
});
$(document).on("click", "#emailunsubscription", function (e) {
    var room_Id = $(this).data('id');
    var newchatmessageHolder = $(".newchatmessage");
    var loginButton = $(".chatlogin");
    var logOffButton = $(".chatlogoff");
    loginButton.hide();
    logOffButton.hide();
    newchatmessageHolder.hide();
    $.ajax({
        url: app_base + "Chat/LogOn",
        type: "POST",
        data: { roomId: room_Id, logOn: false },
        success: function (data) {
            GetMembers();
            clearInterval(refreshInterval);
        }
    });

});
$(document).on("click", ".responselink", function (e) {
    e.preventDefault();
    var responseLink = $(this);
    var messageId = responseLink.data("id");
    var responseType = responseLink.data("responsetype");
    var holdingLi = responseLink.closest("li");
    var holdingUl = holdingLi.closest("ul");
    var responsePoints = holdingLi.find(".count");
    var responseText = responseLink.data("responsetext");
    var responsedText = responseLink.data("responsedtext");
    var hasResponse = responseLink.data("hasresponse");

    var chatResponseViewModel = new Object();
    chatResponseViewModel.Message = messageId;

    var responseUrl = "Chat/Report";
    var otherResponseLink = holdingUl.find(".responselink[data-responsetype='positive']");
    if (responseType == "positive") {
        responseUrl = "Chat/Favourite";
        otherResponseLink = holdingUl.find(".responselink[data-responsetype='negative']");
    }

    // We do the show hide/change of votes here for speed in the UI
    // Change the number up or down
    var currentPoints = parseInt(responsePoints.text());
    if (hasResponse) {
        // They are removing their vote
        responsePoints.text((currentPoints - 1));
        responseLink.text(responseText);

        // So show the other link
        otherResponseLink.show();

        // Change has voted to false
        responseLink.data('hasresponse', false);
    } else {
        // They add adding a vote
        responsePoints.text((currentPoints + 1));
        responseLink.text(responsedText);

        // Hide the other link
        otherResponseLink.hide();

        // Change has voted to false
        responseLink.data('hasresponse', true);
    }

    // Ajax call to post the view model to the controller
    var strung = JSON.stringify(chatResponseViewModel);

    $.ajax({
        url: app_base + responseUrl,
        type: "POST",
        cache: false,
        data: strung,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
        }
    });
});
$(document).on("click", ".showmoremessages", function (e) {
    e.preventDefault();
   // var my_Chat = $("#myChat").val();
    var my_Chat = false;
    if (localStorage.getItem("messageview") !== null && localStorage.getItem("messageview") === "mychat") {
        my_Chat = true;
    }
    var roomId = $("#roomId").val();
    var pageIndex = $("#pageIndex");
    var totalPages = parseInt($("#totalPages").val());
    var activeText = $("span.spmactive");
    var loadingText = $("span.spmloading");

    var showMoreMessageLink = $(this);
    activeText.hide();
    loadingText.show();
    var chatMessagesViewModel = new Object();
    chatMessagesViewModel.MyChat = my_Chat;
    chatMessagesViewModel.RoomId = roomId;
    chatMessagesViewModel.PageIndex = pageIndex.val();
    var strung = JSON.stringify(chatMessagesViewModel);
    $.ajax({
        url: app_base + 'Chat/AjaxMoreMessages',
        type: 'POST',
        dataType: 'html',
        data: strung,
        cache: false,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            // Now add the more messages
            AddMoreMessages(showMoreMessageLink, data);
            // Update the page index value
            var newPageIdex = (parseInt(pageIndex.val()) + parseInt(1));
            pageIndex.val(newPageIdex);

            // If the new pageindex is greater than the total pages, then hide the show more button
            if (newPageIdex > totalPages) {
                showMoreMessageLink.hide();
            }
            activeText.show();
            loadingText.hide();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ShowUserMessage("Error: " + xhr.status + " " + thrownError);
            activeText.show();
            loadingText.hide();
        }
    });
});
$(document).on("click", ".deletechatmessage", function (e) {
	e.preventDefault();
	var linkClicked = $(this);            
            var messageId = linkClicked.data("id");
            var deleteChatMessageViewModel = new Object();
            deleteChatMessageViewModel.Id = messageId;
            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(deleteChatMessageViewModel);
            // Just fade out anyway
            var messageHolder = linkClicked.closest(".chat-message");
            messageHolder.fadeOut("fast");
            $.ajax({
                url: app_base + "Chat/Delete",
                type: "POST",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                //	messageHolder.fadeOut("fast");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
});
/**
 * Chat Log
 * ====================================================
 */
$(document).on("click", ".chatlogin", function (e) {
    e.preventDefault();
    var room_Id = $(this).data("id");
    var logOffButton = $(".chatlogoff");
    var newchatmessageHolder = $(".newchatmessage");
    var showmoremessageHolder = $(".showmoremessages");
    var subscriptionholder = $(".subscription");
    subscriptionholder.fadeOut();
    $(this).fadeOut();
    logOffButton.fadeIn();
    showmoremessageHolder.hide();
    newchatmessageHolder.fadeIn();
    $.ajax({
        url: app_base + "Chat/LogOn",
        type: "POST",
        data: { roomId: room_Id, logOn: true },
        success: function (data) {
           refreshInterval = setInterval("Refresh();", 10 * 1000);
            $("#logonMessage").text("Chat Start");
            ShowLogMessage();
          // window.location.reload();
        }
    });
});
$(document).on("click", ".chatlogoff", function (e) {
    e.preventDefault();
    var room_Id = $(this).data('id');
    var newchatmessageHolder = $(".newchatmessage");
    var showmoremessageHolder = $(".showmoremessages");
    var subscriptionholder = $(".subscription");
    var loginButton = $(".chatlogin");
    $(this).fadeOut();
    loginButton.fadeIn();
    showmoremessageHolder.fadeIn();
    subscriptionholder.fadeIn();
    newchatmessageHolder.fadeOut();
    $.ajax({
        url: app_base + "Chat/LogOn",
        type: "POST",
        data: { roomId: room_Id, logOn: false },
        success: function (data) {
            $("input.userId").val("");
            $('#userId').val('');
            clearInterval(refreshInterval);
            $("#logonMessage").text("Chat Closed");
            ShowLogMessage();
          //  Refresh();
           setTimeout(function () {
         	    window.location.reload();
           }, 1000); // 5000 milliseconds = 5 seconds*/

        }
    });
});
$(document).on("click", ".mychat", function (e) {
    e.preventDefault();
    var allchat = $(".allchat");
    localStorage.setItem("messageview", "mychat");
    $(this).fadeOut();
    allchat.fadeIn();
    LoadChatMessages();
});
$(document).on("click", ".allchat", function (e) {
    e.preventDefault();
    var mychat = $(".mychat");
    localStorage.removeItem("messageview");
    $(this).fadeOut();
    mychat.fadeIn();
    LoadChatMessages();
});

/**
 * Chat Log
 * ====================================================
 */