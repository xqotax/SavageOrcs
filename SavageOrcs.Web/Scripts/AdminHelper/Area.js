var AreaView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#addArea").on('click', function () {
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
        var name = $("#addName").val();
        var comunnity = $("#addCommunity").val();
        var region = $("#addRegion").val();
        if (name.lenght === 0 || comunnity.lenght === 0 || region.lenght === 0)
            return;

        $("#areaContainer").prepend("<div class=\"area-row pb-2\"><div class=\"area-row-first\">" +
            "<input type=\"hidden\" value=\"\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            name + "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            comunnity + "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            region + "\"></div><div class=\"area-row-second\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            "\"><button class=\"btn btn-dark-custom\" onclick=\"areaView.Remove(this)\">Видалити</button></div></div>");
        $("#addName").val("");
        $("#addCommunity").val("");
        $("#addRegion").val("");
    },
    Search: function () {
        var filterName = $("#filterName").val().toLowerCase();
        var filterCommunity = $("#filterCommunity").val().toLowerCase();
        var filterRegion = $("#filterRegion").val().toLowerCase();

        $('#areaContainer .area-row').each(function (index, element) {
            var name = $(element).find(".text-box-custom").eq(0).val().toLowerCase();
            var community = $(element).find(".text-box-custom").eq(1).val().toLowerCase();
            var region = $(element).find(".text-box-custom").eq(2).val().toLowerCase();

            var nameEng = $(element).find(".text-box-custom").eq(3).val().toLowerCase();
            var communityEng = $(element).find(".text-box-custom").eq(4).val().toLowerCase();
            var regionEng = $(element).find(".text-box-custom").eq(5).val().toLowerCase();

            var toShowName = false;
            var toShowCommunity = false;
            var toShowRegion = false;
            if (filterName === '')
                toShowName = true;

            if (filterCommunity === '')
                toShowCommunity = true;

            if (filterRegion === '')
                toShowRegion = true;

            if (name !== '' && name.indexOf(filterName) !== -1)
                toShowName = true;

            if (nameEng !== '' && nameEng.indexOf(filterName) !== -1)
                toShowName = true;

            if (community !== '' && community.indexOf(filterCommunity) !== -1)
                toShowCommunity = true;

            if (communityEng !== '' && communityEng.indexOf(filterCommunity) !== -1)
                toShowCommunity = true;

            if (region !== '' && region.indexOf(filterRegion) !== -1)
                toShowRegion = true;

            if (regionEng !== '' && regionEng.indexOf(filterRegion) !== -1)
                toShowRegion = true;

            if (toShowName && toShowCommunity && toShowRegion)
                $(element).css({ display: 'flex' });
            else
                $(element).css({ display: 'none' });
        });

    },
    Save: function () {
        var self = this;
        var dataArray = [];
        $('.area-row').each(function () {
            var id = $(this).find('input[type="hidden"]').eq(0).val();

            if (id === undefined || id === null)
                id = "";

            var name = $(this).find('input[type="text"]').eq(0).val();
            var community = $(this).find('input[type="text"]').eq(1).val();
            var region = $(this).find('input[type="text"]').eq(2).val();

            var nameEng = $(this).find('input[type="text"]').eq(3).val();
            var communityEng = $(this).find('input[type="text"]').eq(4).val();
            var regionEng = $(this).find('input[type="text"]').eq(5).val();
            var obj = {
                Id: id,
                Name: name,
                NameEng: nameEng,
                CommunityEng: communityEng,
                Community: community,
                Region: region,
                RegionEng: regionEng,
            };
            dataArray.push(obj);
            console.log(dataArray);
        });

        $.ajax({
            type: 'POST',
            url: "/AdminHelper/SaveAreas",
            data: JSON.stringify(dataArray),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                location.reload();
            }
        });
    },
    Remove: function (el) {
        var row = $(el).parent().parent();
        row.remove();
    },
    Clear: function () {
        $('#areaContainer .area-row').each(function (index, element) {
            $(element).css({ display: 'flex' });
        });

        $("#filterName").val("");
        $("#filterCommunity").val("");
        $("#filterRegion").val("");
    }
});

