var pairs = 1;

$(function () {
    $('.ajaxAddPair').click(function () {
        console.log('it works');
        $.ajax({
            url: "/Pair/AjaxAddPair",
            type: "GET",
            data: "pairs=" + pairs,
            success: function (result) {
                $('#result').append(result);
                pairs += 1;
            }
        });
    });
});