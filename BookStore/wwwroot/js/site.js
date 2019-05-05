
$(document).ready(function () {
    $(document).on("click", "#favorites", function () {
        $('.modal')
            .modal('')
            .modal('setting', 'transition', 'vertical flip')
            .modal('show');
    })
})