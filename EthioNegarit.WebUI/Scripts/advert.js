$(function () {
   FileUploadClickHandler();
   setInterval(showTopAd, 1000 * 60 * 17);
   setInterval(showBottomAd, 1000 * 60 * 13);
   setInterval(showBottomLeft, 1000 * 60 * 5);
   setInterval(showBottomRight, 1000 * 60 * 9);   
   const now = new Date();
   const delay = 3600000 - (now.getMinutes() * 60 + now.getSeconds()) * 1000 - now.getMilliseconds();
   setTimeout(() => {
       showAdReminder();
       // Subsequent executions every hour
      setInterval(showAdReminder, 3600000);
   }, delay);
});
/* 
* ====================Show Advert================================
*/
var showTopAd = function () {
    $.ajax({   
        url: app_base + "Ad/Top",
        type: "POST",
        cache: false,
        success: function (data) {
            if (data == null) {
                return;
            } else {
                $(".popup-top").show();
                $("div.popup-top").delay(250 * 60).slideDown();
                $(".popup-top").html(data);
                $("div.popup-top").delay(250 * 60).slideUp();              
            }
        },

    });
};
var showBottomAd = function () {
    $.ajax({
        url: app_base + "Ad/Bottom",
        type: "POST",
        cache: false,
        success: function (data) {
            if (data != null) {
                $(".popup-bottom").show();
                $("div.popup-bottom").delay(250 * 60).slideDown();
                $(".popup-bottom").html(data);
                $("div.popup-bottom").delay(250 * 60).slideUp();
            } else {
                return;
            }        
        },
    });
};
var showBottomLeft = function () {
    $.ajax({
        url: app_base + "Ad/BottomLeft",
        type: "POST",
        cache: false,
        success: function (data) {
            if (data != null) {
                $(".bottom-left").show();
                $("div.bottom-left").delay(1000 * 60).slideDown();
                $(".bottom-left").html(data);
                $("div.bottom-left").delay(1000 * 60).slideUp();
                
            } else {
                return;
            }
        },
    });
};
var showBottomRight = function () {
    $.ajax({
        url: app_base + "Ad/BottomRight",
        type: "POST",
        cache: false,
        success: function (data) {
            if (data != null) {
                $(".bottom-right").show();
                $("div.bottom-right").delay(250 * 60).slideDown();
                $(".bottom-right").html(data);
                $("div.bottom-right").delay(250 * 60).slideUp();
            } else {
                return;
            }
        },
      
    });
};
var showAdReminder = function () {
    //Getting an item   
    const adReminder = getItem('adReminder');
    if (adReminder) {
        return null;
    } else {
        $.ajax({
            url: app_base + "Ad/Reminder",
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
                    //To store Ad Reminder that expires in 1 hour (3,600,000 milliseconds):
                    setItem('adReminder', 'adReminder', 3600000);
                }
            },
        });
    }
};
$(document).on("click", ".showhidebutton", function (e) {
    e.preventDefault();
    var thisButton = $(this);
    var showhideIcon = thisButton.closest("li").find(".showhideicon");
    if (showhideIcon.hasClass("fas fa-angle-down")) {
        showhideIcon.removeClass("fas fa-angle-down").addClass("fas fa-angle-up");
    } else {
        showhideIcon.removeClass("fas fa-angle-up").addClass("fas fa-angle-down");
    }
});
$(document).on("click", ".addlocationlink", function () {
    var locationLink = $(this);
    var ad_Id = locationLink.data("id");
    $("#modal-location").show();
    $("#location").modal("show");
    $.ajax({
        // url: app_base + "Ad/Location",
        url: app_base + "Location/Get",
        type: "GET",
        data: { advertId: ad_Id },
        async: true,
        cache: false,
        success: function (data) {
            $("#modal-location").html("");
            $("#modal-location").html(data)
        }
    });
});
// var isAuthenticated = '@Request.IsAuthenticated';
/* 
* ====================Ad File Attachement================================
*/
var FileUploadClickHandler = function () {
    var attachButton = $(".advertshowattach");
    if (attachButton.length > 0) {
        attachButton.click(function (e) {
            e.preventDefault();
            var advertHolder = $(this).closest("div.advert");
            var uploadHolder = advertHolder.find("div.advertuploadholder");
            uploadHolder.toggle();
        });
    }
}
$(document).ready(function () {
    $("#searchad").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#adtable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});



/*
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
*/
//index.js file - Server Side Code
/*
const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {
    if (req.url === '/') {
        fs.readFile(__dirname + '/index.html', (err, data) => {
            if (err) {
                res.writeHead(500);
                res.end('Error loading index.html');
            } else {
                res.writeHead(200, { 'Content-Type': 'text/html' });
                res.end(data);
            }
        });
    }
});

const io = require('socket.io')(server);
const port = 5000;

io.on('connection', (socket) => {
    socket.on('send name', (user) => {
        io.emit('send name', user);
    });

    socket.on('send message', (chat) => {
        io.emit('send message', chat);
    });
});

server.listen(port, () => {
    console.log(`Server is listening at the port: ${port}`);
});
*/