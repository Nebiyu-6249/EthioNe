$(function () {
    LatestUsers();
    LowestPointUsers();
    LowestPointPosts();
    HighestViewedTopics();
    TodaysTopics();
    RecentTopics();
});

function LatestUsers() {
    $.post(app_base + "Admin/Dashboard/LatestUsers",
    function (data) {
        $(".dashboardlatestusers").html(data);
    });
}

function LowestPointUsers() {
    $.post(app_base + "Admin/Dashboard/LowestPointUsers",
    function (data) {
        $(".dashboardlowestpointusers").html(data);
    });
}

function LowestPointPosts() {
    $.post(app_base + "Admin/Dashboard/LowestPointPosts",
    function (data) {
        $(".dashboardlowestpointposts").html(data);
    });
}

function TodaysTopics() {
    $.post(app_base + "Admin/Dashboard/TodaysTopics",
    function (data) {
        $(".dashboardtodaystopics").html(data);
    });
}

function HighestViewedTopics() {
    $.post(app_base + "Admin/Dashboard/HighestViewedTopics",
    function (data) {
        $(".dashboardhighestviewedtopics").html(data);
    });
}
function RecentTopics() {
    $.post(app_base + "Admin/Dashboard/RecentTopics",
    function (data) {
        $(".dashboardrecenttopics").html(data);
    });
}