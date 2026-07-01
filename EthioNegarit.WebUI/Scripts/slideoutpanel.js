$(function () {
    hideSlideOutPanel();
});


/**********************************************/
/* 
* Slide Out Panel
* ====================================================
*/

var largeSpinnerBlock = "<div class=\"loaderholder\"><img src=\"" + largeSpinnerBlockImage + "\" alt=\"\"/></div>";
var AddLargeSpinner = function (elementToPlaceInside) {
    elementToPlaceInside.html(largeSpinnerBlock);
};

var slideOutPanel = function (title) {
    slideOutPanel(title, spinnerBlock);
}

var slideOutPanel = function (title, content) {
    // Get panel
    const $body = $('body')
    const $html = $('html')
    var panel = $(".slide-out-panel");
    //  var sidepanel = $(".side-panel");

    // Get content div and add in content
    var contentDiv = panel.find(".slide-out-panel-content");
    contentDiv.html(content);
    var titleDiv = panel.find(".slide-out-panel-header h6");
    titleDiv.html(title);

    //Hide Side Panel
    //  sidepanel.hide();

    // Show Slide Out Panel
    panel.addClass("is-visible");
    // $body.addClass("slide-out-panel-open")

    // Add overflow hidden to the body tag
    $("body").css("overflow", "hidden").css("height", "100%");
    $("html").css("overflow", "hidden").css("height", "100%");

    // $("div.slide-panel").delay(6600).fadeOut();
};


var hideSlideOutPanel = function () {
    //  const $body = $('body')
    var panel = $(".slide-out-panel");
    // var sidepanel = $(".side-panel");
    panel.on("click", function (event) {
        //    sidepanel.show();
        if ($(event.target).is(".slide-out-panel") || $(event.target).is(".slide-out-close")) {
            // Confirm they want to close it
            var closeconfirmationText = panel.data("confirmationtext");

            if (typeof tinyMCE != "undefined") {
                var pmMessage = tinyMCE.activeEditor.getContent();
                if (pmMessage != "") {
                    if (confirm(closeconfirmationText)) {
                        hardPanelClose(event, panel);
                    }
                } else {
                    hardPanelClose(event, panel);
                }
            } else {
                hardPanelClose(event, panel);
            }
            return false;
        }
    });
};


var hardPanelClose = function (event, panel) {
    event.preventDefault();

    //  const $body = $('body')
    //  var panel = $(".slide-out-panel");
    panel.removeClass("is-visible");
    //  var sidepanel = $(".side-panel");
    // $body.removeClass("slide-out-panel-open")
    //Clear fields
    var contentDiv = panel.find(".slide-out-panel-content");
    contentDiv.html(largeSpinnerBlock);

    var titleDiv = panel.find(".slide-out-panel-header h6");
    titleDiv.html("");

    $("body").css("overflow", "").css("height", "");
    $("html").css("overflow", "").css("height", "");
    //remove onbeforeunload registered by TinyMCE
    window.onbeforeunload = function () { };
};

var closeSlideOutPanel = function () {
    var panel = $(".slide-out-panel");
    panel.trigger("click");
};
/****************Slide Out Panel******************************/