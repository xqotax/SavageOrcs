var CatalogueMarkView = Class.extend({
    Marks: null,
    FullData: false,
    RowAddConstString: "<div class=\"row justify-content-md-around\">",
    ColAddConstString: "<div class=\"col-md-3\">",
    DivAddConstString: "</div>",
    InitializeControls: function () {
        var self = this;
       

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $("#search").click(function () {
            self.Search();
        });

        $("#fullData").click(function () {
            debugger;
            if (self.FullData) {
                self.FullData = false;
                $("#fullData").text("Ввімкнути детальний перегляд");
            }
            else {
                self.FullData = true;
                $("#fullData").text("Ввімкнути спрощений перегляд");
            }

            if (!$("#table").is(':empty')) {
                self.Search();
            }
        });
    },
    Search: function () {
        var self = this;
        $("#table").empty();

        var filters = {
            Area: $("#Area").val(),
            KeyWord: $("#KeyWord").val(),
            FullData: self.FullData
        };

        $.ajax({
            type: 'POST',
            url: "/Mark/GetMarks",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                debugger;
            }
        });

        debugger;
    }
});