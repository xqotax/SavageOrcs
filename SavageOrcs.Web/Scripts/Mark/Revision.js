var RevisionMarkView = Class.extend({
    InitializeControls: function () {
        var self = this;

       
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $(".image-revision-mark-custom").click(function () {
            self.ToFullScreen($(this).attr("src"));
        });

        $('#flagGB').on('click', function () {
            $('#flagUA').removeClass("box-shadow-grey-custom");
            $("#flagGB").addClass("box-shadow-grey-custom");

            $("#textGB").removeClass("display-none-custom");
            $("#textUA").addClass("display-none-custom");
           
        });

        $('#flagUA').on('click', function () {
            $('#flagGB').removeClass("box-shadow-grey-custom");
            $("#flagUA").addClass("box-shadow-grey-custom");

            $("#textUA").removeClass("display-none-custom");
            $("#textGB").addClass("display-none-custom");
        });
    },
    ToFullScreen: function (data) {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Mark/RevisionImage",
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (html) {
                $("#imageFullScreenPlaceholder").html(html);
            }
        });
    }
})