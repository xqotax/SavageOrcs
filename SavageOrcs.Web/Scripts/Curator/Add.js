var CuratorAddView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#addImage').on('click', function () {
            self.AddImage();
        });

        $('#saveCurator').on('click', function () {
            self.Save();
        });
        $('#delete').on('click', function () {
            self.Delete();
        });
    },
    Delete: function () {
        if ($("#Id").val() === "")
            return;
        $.ajax({
            type: 'POST',
            url: "/Curator/DeleteCurator",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteCuratorPlaceholder').html(src);
            }
        });
    },

    AddImage: function () {
        $.ajax({
            type: 'POST',
            url: "/Curator/AddImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImagePlaceholder').html(src);
            }
        });
    },
    Save: function () {
        var self = this;

        var saveCuratorViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            DisplayName: $("#DisplayName").val(),
            DisplayNameEng: $("#DisplayNameEng").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            Image: $("#curatorImage").attr('src')
        };

        

        $.ajax({
            type: 'POST',
            url: "/Curator/SaveCurator",
            data: JSON.stringify(saveCuratorViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    window.location.href = 'https://' + window.location.host + '/Curator/Catalogue';
                }
                else {
                    ResultPopUp(result.success, result.text, result.url, result.id);
                }
            }
        });

    }
});