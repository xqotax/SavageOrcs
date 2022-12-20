var CatalogueMarkView = Class.extend({
    Marks: null,
    DetailedView: false,
    From: null,
    CountConst: 4,
    WereDataInDetailedTable: null,
    WereDataInShortTable: null,
    OnEnglish: false,
    
    TableShortRowStartConstString: "<div class=\"table-body-short-row justify-content-center d-flex\"><div class=\"table-body-short-column-number\">",
    TableShortRowNameConstString: "</div><div class=\"table-body-short-column-name\"><a href=\"/Mark/Revision?id=",
    TableShortRowDescriptionConstString: "</a></div><div class=\"table-body-short-column-description ukr-description\">",
    TableShortRowDescriptionEngConstString: "</div><div class=\"table-body-short-column-description eng-description\">",
    TableShortRowAreaConstString: "</div><div class=\"table-body-short-column-area\">",
    TableShortRowAreaButtonConstString: "<button class=\"button-short-column-area\" value=\"",
    TableShortRowEndConstString: "</div></div>",

    TableDetailRowStartConstString: "<div class=\"table-body-detail-row justify-content-center d-flex\"><div class=\"table-body-detail-column-number flex-container-center-custom\">",
    TableDetailRowNameConstString: "</div><div class=\"table-body-detail-column-name flex-container-center-custom\"><a href=\"/Mark/Revision?id=",
    TableDetailRowDescriptionConstString: "</a></div><div class=\"table-body-detail-column-description flex-container-center-custom ukr-description\">",
    TableDetailRowDescriptionEngConstString: "</div><div class=\"table-body-detail-column-description flex-container-center-custom eng-description\">",
    TableDetailRowAreaConstString: "</div><div class=\"table-body-detail-column-area flex-container-center-custom\">",
    TableDetailRowAreaButtonConstString: "<button class=\"button-detail-column-area\" value=\"",
    TableDetailRowLinkConstString: "</div><div class=\"table-body-detail-column-photo flex-container-center-custom\"><a href=\"",
    TableDetailRowPhotoConstString: "\"><img class=\"table-body-detail-column-photo-item\" src=\"",
    TableDetailRowEndConstString: "\"></a></div></div>",
    InitializeControls: function () {
        var self = this;
       

        var options = {
            placeholder: "Виберіть ключове слово",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "keyWordsMultiselect"
        }

        MultiselectDropdown(options);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#tableDetail").css("display", "none");
        $("#showMore").css("display", "none");

        $("#search").click(function () {
            $(".table-body-short").empty();
            $(".table-body-detail").empty();
            self.From = null;
            self.Search();
        });

        $("#clearFilters").click(function () {
            $('#keyWordsMultiselect option').attr('selected', ''),
            $("#AreaName").val(''); 
            $("#MarkName").val(''); 
            $("#MarkDescription").val('');
        });

        $("#showMore").click(function () {
            self.Search();
        });

        $("#fullData").click(function () {
            if (self.DetailedView) {
                self.DetailedView = false;

                $("#tableDetail").css("display", "none");
                $("#tableShort").css({ 'display': '' });
                $(".table-body-detail").empty();

                if (self.WereDataInDetailedTable == true) {
                    self.Search();
                }

                $("#fullData").text("Ввімкнути детальний перегляд");
                $("#showMore").css("display", "none");
            }
            else {
                self.DetailedView = true;

                $("#tableShort").css("display", "none");
                $("#tableDetail").css({ 'display': ''});
                $(".table-body-short").empty();

                if (self.WereDataInShortTable == true) {
                    self.Search();
                }

                $("#fullData").text("Ввімкнути спрощений перегляд");
                $("#showMore").css({ 'display': '' });
            }
            //if (!$("#table").is(':empty')) {
            //    self.Search();
            //}
        });

        $('#flagGB').on('click', function () {
            self.OnEnglish = true;
            $('#flagUA').removeClass("box-shadow-grey-custom");
            $("#flagGB").addClass("box-shadow-grey-custom");


            $(".ukr-description").addClass("display-none-custom");
            $(".eng-description").removeClass("display-none-custom");

        });

        $('#flagUA').on('click', function () {
            self.OnEnglish = false;
            $('#flagGB').removeClass("box-shadow-grey-custom");
            $("#flagUA").addClass("box-shadow-grey-custom");

            $(".eng-description").addClass("display-none-custom");
            $(".ukr-description").removeClass("display-none-custom");
        });
    },
    Search: function () {
        var self = this;
        
        var filters = {
            KeyWordIds: $("#keyWordsMultiselect").val(),
            AreaName: $("#AreaName").val(),
            MarkName: $("#MarkName").val(),
            MarkDescription: $("#MarkDescription").val(),
            NotIncludeCluster: $("#NotIncludeCluster").is(":checked"),
            FullData: self.DetailedView,
            From: self.From,
            Count: self.CountConst
        };

        $.ajax({
            type: 'POST',
            url: "/Mark/GetMarks",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (self.DetailedView) {
                    self.WereDataInShortTable = false;
                    self.WereDataInDetailedTable = true;
                    
                    var toAdd = "";

                    $.each(data, function (index, element) {

                        toAdd += self.TableDetailRowStartConstString + (self.From + index + 1);
                        toAdd += self.TableDetailRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableDetailRowDescriptionConstString + element.description;
                        toAdd += self.TableDetailRowDescriptionEngConstString + element.descriptionEng;
                        
                        if (element.area !== null) {
                            if (element.area.id !== null) {
                                toAdd += self.TableDetailRowAreaConstString + self.TableDetailRowAreaButtonConstString;
                                toAdd += element.area.id + "\">" + element.area.name + "</button>";
                            }
                        }
                        else {
                            toAdd += self.TableDetailRowAreaConstString;
                        }

                        toAdd += self.TableDetailRowLinkConstString + element.resourceUrl;
                        toAdd += self.TableDetailRowPhotoConstString + element.images[0]; //NOT COMPLETED
                        toAdd += self.TableDetailRowEndConstString;
                    });

                    self.From += self.CountConst;

                    $(".table-body-detail").append(toAdd);

                    $(".button-detail-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#AreaName").val(this.innerText);
                        self.Search();
                    });

                    if (self.OnEnglish)
                        $(".ukr-description").addClass("display-none-custom");
                    else
                        $(".eng-description").addClass("display-none-custom");
                }
                else {
                    self.WereDataInShortTable = true;
                    self.WereDataInDetailedTable = false;

                    self.From = 0;

                    var toAdd = "";

                    $.each(data, function (index, element) {
                        toAdd += self.TableShortRowStartConstString + (index + 1);
                        toAdd += self.TableShortRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableShortRowDescriptionConstString + element.description;
                        toAdd += self.TableShortRowDescriptionEngConstString + element.descriptionEng;
                        

                        if (element.area !== null) {
                            if (element.area.id !== null) {
                                toAdd += self.TableShortRowAreaConstString + self.TableShortRowAreaButtonConstString;
                                toAdd += element.area.id + "\">" + element.area.name + "</button>";
                            }
                        }
                        else {
                            toAdd += self.TableShortRowAreaConstString;
                        }
                        toAdd += self.TableShortRowEndConstString;
                    });

                    $(".table-body-short").append(toAdd);

                    $(".button-short-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#AreaName").val(this.innerText);
                        self.Search();
                    });

                    if (self.OnEnglish)
                        $(".ukr-description").addClass("display-none-custom");
                    else
                        $(".eng-description").addClass("display-none-custom");
                }
            }
        });
    }
});