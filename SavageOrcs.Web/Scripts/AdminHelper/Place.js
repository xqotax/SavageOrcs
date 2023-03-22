var PlaceView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#addPlace").on('click', function () {
            self.Add();
        });

        $("#search").on('click', function () {
            self.Search();
        });

        $("#save").on('click', function () {
            self.Save();
        });

        $("#clear").on('click', function () {
            self.Clear();
        });
    },
    Add: function () {
        var text = $("#placeToAdd").val();
        var textEng = $("#placeToAddEng").val();
        if (text.lenght === 0)
            return;

        $("#placeContainer").prepend("<div class=\"place-row pb-2\"><input type=\"hidden\" value=\"\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            text + "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            textEng + "\"><button class=\"btn btn-dark-custom\" onclick=\"placeView.Remove(this)\">Видалити</button></div>");
        $("#placeToAdd").val("");
        $("#placeToAddEng").val("");
    },
    Search: function () {
        var filter = $("#filter").val().toLowerCase();

        $('#placeContainer .place-row').each(function (index, element) {
            var place = $(element).find(".text-box-custom").eq(0).val().toLowerCase();
            var placeEng = $(element).find(".text-box-custom").eq(1).val().toLowerCase();

            $(element).css({ display: 'flex' });
            if (place.indexOf(filter) === -1 && placeEng.indexOf(filter) === -1)
                $(element).css({ display: 'none' });
        });

    },
    Save: function () {
        var self = this;
        var dataArray = [];
        $('.place-row').each(function () {
            var id = $(this).find('input[type="hidden"]').eq(0).val();

            if (id === undefined || id === null)
                id = "";
            var name = $(this).find('input[type="text"]').eq(0).val();
            var nameEng = $(this).find('input[type="text"]').eq(1).val();
            var obj = {
                Id: id,
                Name: name,
                NameEng: nameEng
            };
            dataArray.push(obj);
            console.log(dataArray);
        });

        $.ajax({
            type: 'POST',
            url: "/AdminHelper/SavePlaces",
            data: JSON.stringify(dataArray),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                location.reload();
            }
        });
    },
    Remove: function (el) {
        var row = $(el).parent();
        row.remove();
    },
    Clear: function () {
        $('#placeContainer .place-row').each(function (index, element) {
            $(element).css({ display: 'flex' });
        });

        $("#filter").val("");
    }
});

