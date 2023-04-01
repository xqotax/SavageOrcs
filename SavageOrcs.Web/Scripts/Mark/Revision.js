var RevisionMarkView = Class.extend({
    InitializeControls: function () {
        var self = this;

       
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#back").on('click', () => {
            window.history.back();
        })
    },
    //ToFullScreen: function (data) {
    //    var self = this;
    //    $.ajax({
    //        type: 'POST',
    //        url: "/Mark/RevisionImage",
    //        data: JSON.stringify(data),
    //        contentType: 'application/json; charset=utf-8',
    //        success: function (html) {
    //            $("#imageFullScreenPlaceholder").html(html);
    //        }
    //    });
    //}
})