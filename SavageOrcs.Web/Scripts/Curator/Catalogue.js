var CatalogueCuratorView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $(".curator-box").click(function () {
            window.location.href = window.location.origin + "/Curator/Revision?id=" + $(this).attr("value");
        });
    }
})