var CatalogueTextView = Class.extend({
    Curators: null,
    SearchSelectDropdown: null,

    TableTextStartConstString: "<div class=\"table-body-catalogue-text-row justify-content-center d-flex\"><div class=\"table-body-catalogue-text-column-number\">",
    TableTextNameConstString: "</div><div class=\"table-body-catalogue-text-column-name\"><a href=\"/Text/Revision?id=",
    TableTextSubjectConstString: "</a></div><div class=\"table-body-catalogue-text-column-subject\">",
    TableTextCuratorConstString: "</div><div class=\"table-body-catalogue-text-column-curator\"><a href=\"/Curator/Revision?id=",
    TableTextEndConstString: "</a></div></div>",

    InitializeControls: function () {
        var self = this;

        self.SearchSelectDropdown = new SearchSelect('#dropdown-input', {
            data: [],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 9,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: null,
        });

        self.InitializeCurators(self.Curators);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#search').on('click', function () {
            self.Search();
        });

        $('#dropdown-input').addClass("display-8-custom");

        $("#clearFilters").click(function () {
            //$("#KeyWord").val('');
            $("#dropdown-input").val('');
            $("#TextName").val('');
            $("#TextSubject").val('');
        });
        
    },
    InitializeCurators: function (data) {
        var self = this;
        var curatorNames = [];

        $.each(data, function (index, element) {
            curatorNames.push(element);
        });

        self.SearchSelectDropdown.setData(curatorNames);
    },
    Search: function () {
        $(".table-body-catalogue-text").empty();

        var self = this;

        var filters = {
            CuratorName: $("#dropdown-input").val(),
            TextName: $("#TextName").val(),
            TextSubject: $("#TextSubject").val()
            //KeyWord: $("#KeyWord").val(),
        };

        $.ajax({
            type: 'POST',
            url: "/Text/GetTexts",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

                var toAdd = "";

                $.each(data, function (index, element) {
                    toAdd += self.TableTextStartConstString + (index + 1);
                    toAdd += self.TableTextNameConstString + element.id + "\">" + element.name;
                    toAdd += self.TableTextSubjectConstString + element.subject;
                    toAdd += self.TableTextCuratorConstString + element.curator.id + "\">" + element.curator.name;
                    toAdd += self.TableTextEndConstString;
                });

                $(".table-body-catalogue-text").append(toAdd);
            }
        });
    }
})