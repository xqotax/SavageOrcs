var CatalogueCuratorView = Class.extend({
    SelectedText: null,
    MultiselectAll: null,
    CuratorTextPlaceholder: null,
    SearchTextCurator: null,

    InitializeControls: function () {
        var self = this;

        var curatorsOptions = {
            placeholder: self.CuratorTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextCurator.value,
            height: window.innerWidth > 950 ? "300px" : "150px",
            Id: "curatorsMultiselect"
        }

        MultiselectDropdown(curatorsOptions);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#curatorsMultiselect").on('change', function () {
            self.OnCuratorsChange();
        });
    },
    Search: function () {
        var self = this;

        
        var curatorIds = $("#curatorsMultiselect").val();

        $.ajax({
            type: 'POST',
            url: "/Curator/GetCurators",
            data: JSON.stringify(curatorIds),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".curator-data-col-container").html(data);
            }
        });
    },
    OnCuratorsChange: function () {
        var self = this;
        self.Search();
    },
    ShowDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        var bottomArrow = $(el).find(".bottomArrow");
        var topArrow = $(el).find(".topArrow");

        if (descriptionContainer.css('display') === 'flex') {
            descriptionContainer.css('display', 'none');
            topArrow.css('display', 'none');
            bottomArrow.css('display', 'block');

        } else {
            descriptionContainer.css('display', 'flex');
            bottomArrow.css('display', 'none');
            topArrow.css('display', 'block');
        }
    },
    HideDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        descriptionContainer.css({ display: 'none' });
    }
})