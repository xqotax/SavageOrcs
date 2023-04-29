var CatalogueTextView = Class.extend({
    Curators: null,
    SelectedText: null,
    MultiselectAll: null,

    CuratorTextPlaceholder: null,
    NameTextPlaceholder: null,

    SearchTextName: null,
    SearchTextCurator: null,

    InitializeControls: function () {
        var self = this;

        var textNamesOptions = {
            placeholder: self.NameTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextName.value,
            height: "300px",
            Id: "textNamesMultiselect"
        }

        var curatorsOptions = {
            placeholder: self.CuratorTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextCurator.value,
            height: "300px",
            Id: "curatorsMultiselect"
        }

        MultiselectDropdown(textNamesOptions);
        MultiselectDropdown(curatorsOptions);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#curatorsMultiselect").on('change', function () {
            self.OnCuratorsChange();
        });

        $("#textNamesMultiselect").on('change', function () {
            self.OnNamesChange();
        });

        var firstElement = $(".text-search-data-table .text-data-row")[0];
        if (firstElement !== undefined)
            self.Show(firstElement);
        
    },
    Show: function (el) {
        var self = this;
        

        $('.text-data-row').each(function (idex, element) {
            $(element).css('opacity', '0.3');
            $(element).removeClass("data-row-selected");
        });

        if (window.innerWidth <= 1000 && $(el).next().attr('id') === 'textMobileContentPlaceholder') {
            $("#textMobileContentPlaceholder").remove();
            return;
        }

        $(el).css('opacity', '1');
        $(el).addClass("data-row-selected");
        var fullId = $(el).find("input:first-child").val();
        id = fullId.substring(fullId.length - 36);

        $("#textContentPlaceholder").empty();

        $.ajax({
            type: 'POST',
            url: "/Text/GetText?id=" + id ,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (window.innerWidth <= 1000) {
                    self.ShowMobile(el, result);
                }
                else {
                    $("#textContentPlaceholder").html(result);
                }
            }
        });
    },
    Search: function () {
        var self = this;

        var filters = {
            TextIds: $("#textNamesMultiselect").val(),
            CuratorIds: $("#curatorsMultiselect").val(),
        };

        $.ajax({
            type: 'POST',
            url: "/Text/GetTexts",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".text-search-data-table").html(data);
                var firstElement = $(".text-search-data-table .text-data-row")[0];
                if (firstElement !== undefined)
                    self.Show(firstElement);
            }
        });
    },
    OnCuratorsChange: function () {
        var self = this;
        self.Search();
    },
    OnNamesChange: function () {
        var self = this;
        self.Search();
    },
    ShowMobile: function (el, result) {
        $("#textMobileContentPlaceholder").remove();
        $(el).after("<div class=\"text-data-row\" id=\"textMobileContentPlaceholder\">" + result + "</div>")
    }
})