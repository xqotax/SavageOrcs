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

        var textNamesOptions = {
            placeholder: "Виберіть назву",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "textNamesMultiselect"
        }

        var curatorsOptions = {
            placeholder: "Виберіть автора",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "curatorsMultiselect"
        }

        MultiselectDropdown(textNamesOptions);
        MultiselectDropdown(curatorsOptions);

        //self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#search').on('click', function () {
            self.Search();
        });


        $("#clearFilters").click(function () {
            //$("#KeyWord").val('');
            $("#TextName").val('');
            $("#TextSubject").val('');
            $('#curatorMultiselect option').attr('selected', 'selected');
        });
        
    },
    Search: function () {
        $(".table-body-catalogue-text").empty();

        var self = this;
        var filters = {
            CuratorIds: $("#curatorMultiselect").val(),
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
                    if (element.curator !== null) {
                        toAdd += self.TableTextCuratorConstString + element.curator.id + "\">" + element.curator.name;
                        toAdd += self.TableTextEndConstString;
                    }
                    else {
                        toAdd += "</div><div class=\"table-body-catalogue-text-column-curator\"></div></div>";
                    }
                });

                $(".table-body-catalogue-text").append(toAdd);
            }
        });
    }
})