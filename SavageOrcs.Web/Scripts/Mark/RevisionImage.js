var RevisionImageView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $(".popup-custom-image-fullScreen").click(function () {
            $("#imageFullScreenPlaceholder").empty();
        });
    }
})