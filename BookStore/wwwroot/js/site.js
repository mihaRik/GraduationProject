$(document).ready(function () {
    $(document).on("click", "#favorites", function () {
        $("#favorites-container").toggle(100);
        $("#favorites-container").toggleClass("show-favs");
        if ($("#favorites-container").hasClass("show-favs")) {
            $("#favorites-container").append('<img src="/images/loader.gif" height="100" class="mx-auto d-block" />');
            $.ajax({
                url: "/favorite/getuserfavorites",
                success: function (res) {
                    $("#favorites-container").html("");
                    $("#favorites-container").html(res);
                }
            })
        }
    })

    function calculateCount() {
        $.ajax({
            url: "/favorite/favcount",
            success: function (res) {
                $("#favorites-count").text(res);
            }
        })
    }

    calculateCount();
})