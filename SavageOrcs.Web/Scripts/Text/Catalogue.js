var CatalogueTextView = Class.extend({
    Curators: null,
    SearchSelectDropdown: null,

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

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        //$('#search').on('click', function () {
        //    self.Search();
        //});


        //$("#clearFilters").click(function () {
        //    //$("#KeyWord").val('');
        //    $("#TextName").val('');
        //    $("#TextSubject").val('');
        //    $('#curatorMultiselect option').attr('selected', 'selected');
        //});

        var firstElement = $(".text-search-data-table .text-data-row")[0];
        if (firstElement !== undefined)
            self.Show(firstElement);
        
    },
    Show: function (el) {
        $('.text-data-row').each(function (idex, element) {
            $(element).css('opacity', '0.3');
            $(element).removeClass("data-row-selected");
        });
        $(el).css('opacity', '1');
        $(el).addClass("data-row-selected");
        var fullId = $(el).find("input:first-child").attr('id')
        id = fullId.substring(fullId.length - 36);

        $("#textContentPlaceholder").empty();
        $.ajax({
            type: 'POST',
            url: "/Text/GetText?id=" + id ,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                $("#textContentPlaceholder").html(result);
            }
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