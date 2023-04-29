var KeyWordView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#addKeyWord").on('click', function () {
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
        var text = $("#keyWordToAdd").val();
        var textEng = $("#keyWordToAddEng").val();
        if (text.lenght === 0 || textEng.lenght === 0)
            return;

        $("#keyWordContainer").prepend("<div class=\"keyWord-row pb-2\"><input type=\"hidden\" value=\"\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            text + "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            textEng + "\"><button class=\"btn btn-dark-custom\" onclick=\"keyWordView.Remove(this)\">Видалити</button></div>");
        $("#keyWordToAdd").val("");
        $("#keyWordToAddEng").val("");
    },
    Search: function () {
        var filter = $("#filter").val().toLowerCase();

        $('#keyWordContainer .keyWord-row').each(function (index, element) {
            var keyWord = $(element).find(".text-box-custom").eq(0).val().toLowerCase();
            var keyWordEng = $(element).find(".text-box-custom").eq(1).val().toLowerCase();

            $(element).css({ display: 'flex' });
            if (keyWord.indexOf(filter) === -1 && keyWordEng.indexOf(filter) === -1)
                $(element).css({ display: 'none' });
        });

    },
    Save: function () {
        var self = this;
        var dataArray = [];
        $('.keyWord-row').each(function () {
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
        });

        $.ajax({
            type: 'POST',
            url: "/AdminHelper/SaveKeyWords",
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
        $('#keyWordContainer .keyWord-row').each(function (index, element) {
            $(element).css({ display: 'flex' });
        });

        $("#filter").val("");
    }
});

