$(function () {

    $('[rel="popover"]').popover({
        container: "body",
        html: true,
        content: function() {
            const clone = $($(this).data("popover-content")).clone(true).removeClass("d-none");
            return clone;
        }
    }).click(function(e) {
        e.preventDefault();
    });

    function getNotification() {
        let res = "<ul class='list-group'>";
        $.ajax({
            url: "/Notification/getNotification",
            method: "GET",
            success: function (result) {

                if (result.count !== 0) {
                    $("#notificationCount").html(result.count);
                    $("#notificationCount").show("slow");
                } else {
                    $("#notificationCount").html();
                    $("#notificationCount").hide(0);
                    $("#notificationCount").popover("hide");
                }

                const notifications = result.userNotification;
                notifications.forEach(element => {
                    res = `${res}<li class='list-group-item notification-text' data-id='${element.notificationId}'>${
                        element
                        .notification.text}</li>`;
                });

                res = res + "</ul>";

                $("#list-popover").html(res);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

    function readNotification(id, target) {
        $.ajax({
            url: "/Notification/readNotification",
            method: "GET",
            data: { notificationId: id },
            success: function () {
                //console.log("readNotification query", $(target));
                $(target).fadeOut("slow");
                getNotification();
            },
            error: function (error) {
                console.log("error:", error);
            }
        });
    }

    $("#list-popover").on("click", "li.notification-text", function(e) {
        const target = e.target;
        const id = $(target).data("id");

        readNotification(id, target);
        //console.log(`read notification on click: id = ${id}, target`, target);
    });


    getNotification();

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalServer")
        .build();
    connection.on("displayNotification", () => { getNotification() });

    connection.start();
}); 
