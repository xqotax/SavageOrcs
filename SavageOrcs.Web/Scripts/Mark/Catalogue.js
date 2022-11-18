var CatalogueMarkView = Class.extend({
    Marks: null,
    DetailedView: false,
    From: null,
    CountConst: 4,
    WereDataInDetailedTable: null,
    WereDataInShortTable: null,
    
    TableShortRowStartConstString: "<div class=\"table-body-short-row justify-content-center d-flex\"><div class=\"table-body-short-column-number\">",
    TableShortRowNameConstString: "</div><div class=\"table-body-short-column-name\"><a href=\"/Mark/Revision?id=",
    TableShortRowDescriptionConstString: "</a></div><div class=\"table-body-short-column-description\">",
    TableShortRowAreaConstString: "</div><div class=\"table-body-short-column-area\">",
    TableShortRowAreaButtonConstString: "<button class=\"button-short-column-area\" value=\"",
    TableShortRowEndConstString: "</div></div>",

    TableDetailRowStartConstString: "<div class=\"table-body-detail-row justify-content-center d-flex\"><div class=\"table-body-detail-column-number flex-container-center-custom\">",
    TableDetailRowNameConstString: "</div><div class=\"table-body-detail-column-name flex-container-center-custom\"><a href=\"/Mark/Revision?id=",
    TableDetailRowDescriptionConstString: "</a></div><div class=\"table-body-detail-column-description flex-container-center-custom\">",
    TableDetailRowAreaConstString: "</div><div class=\"table-body-detail-column-area flex-container-center-custom\">",
    TableDetailRowAreaButtonConstString: "<button class=\"button-detail-column-area\" value=\"",
    TableDetailRowLinkConstString: "</div><div class=\"table-body-detail-column-photo flex-container-center-custom\"><a href=\"",
    TableDetailRowPhotoConstString: "\"><img class=\"table-body-detail-column-photo-item\" src=\"",
    TableDetailRowEndConstString: "\"></a></div></div>",
    InitializeControls: function () {
        var self = this;
       

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
            $("#KeyWord").val('');
            $("#Area").val(''); 
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
    },
    Search: function () {
        var self = this;

        var filters = {
            Area: $("#Area").val(),
            KeyWord: $("#KeyWord").val(),
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
                debugger;
                if (self.DetailedView) {
                    self.WereDataInShortTable = false;
                    self.WereDataInDetailedTable = true;
                    
                    var toAdd = "";

                    $.each(data, function (index, element) {

                        toAdd += self.TableDetailRowStartConstString + (self.From + index + 1);
                        toAdd += self.TableDetailRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableDetailRowDescriptionConstString + element.description;
                        if (element.area.id !== null) {
                            toAdd += self.TableDetailRowAreaConstString + self.TableDetailRowAreaButtonConstString;
                            toAdd += element.area.id + "\">" + element.area.name + "</button>";
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
                        $("#Area").val(this.innerText);
                        self.Search();
                    });

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
                        if (element.area.id !== null) {
                            toAdd += self.TableShortRowAreaConstString + self.TableShortRowAreaButtonConstString;
                            toAdd += element.area.id + "\">" + element.area.name + "</button>";
                        }
                        else {
                            toAdd += self.TableShortRowAreaConstString;
                        }
                        toAdd += self.TableShortRowEndConstString;
                    });

                    $(".table-body-short").append(toAdd);

                    $(".button-short-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#Area").val(this.innerText);
                        self.Search();
                    });
                }
            }
        });
    }
});