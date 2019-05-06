$(document).ready(function () {
    $(document).on("click", "#favorites", function () {
        $("#favorites-container").show();
        $("#favorites-container").css("transform", "rotateX(0)");
        $("#favorites-container").addClass("active");
    })

    $(document).click(function (e) {
        if ($(e.target) != $("#favorites-container")) {
            if (!$("#favorites-container").hasClass("active")) {
                $("#favorites-container").removeClass("active");
                $("#favorites-container").hide();
                $("#favorites-container").css("transform", "rotateX(-85deg)");
            }
        }
    })
})