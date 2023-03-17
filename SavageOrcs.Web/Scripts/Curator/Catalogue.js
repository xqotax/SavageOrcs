var CatalogueCuratorView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        //$(".curator-box").click(function () {
        //    window.location.href = window.location.origin + "/Curator/Revision?id=" + $(this).attr("value");
        //});
    },
    ShowDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        if (descriptionContainer.css('display') === 'flex') {
            descriptionContainer.css('display', 'none');
        } else {
            descriptionContainer.css('display', 'flex');
        }
    },
    HideDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        descriptionContainer.css({ display: 'none' });
    }
})