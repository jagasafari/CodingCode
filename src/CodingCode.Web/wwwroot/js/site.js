function disableSubmit() {
    $("button:submit").click(function () {
        $(this).text("Please Wait");
    });
    $("form").on("submit", function () {
        $(this).find("button:submit").prop("disabled", true);
    });
}
