﻿var AddCuratorImageView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
    InitializeControls: function () {
        var self = this;
        self.ImageInput = $('#imageInput');
        self.Reader = new FileReader();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        self.Reader.addEventListener("load", () => {
            $("#imagePlaceholder")[0].src = self.Reader.result;
        }, false);

        self.ImageInput.on('change', function () {
            self.Reader.readAsDataURL(self.ImageInput[0].files[0]);
        });

        $('#addImageCancelButton').on('click', function () {
            self.Close();
        });
        $('#addImageConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Close: function () {
        $("#addImagePlaceholder").empty();
    },
    Save: function () {
        var self = this;
        debugger;
        $("#imageContainer").empty();

        var element = $(".popup-content-custom .add-image-placeholder-custom").html();

        $("#imageContainer").append(element);

        $("#addImagePlaceholder").empty();
    },
});