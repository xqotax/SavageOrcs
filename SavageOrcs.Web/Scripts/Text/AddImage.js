var AddImageTextView = Class.extend({
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
        $("#addImageTextPlaceholder").empty();
    },
    Save: function () {
        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        var content = $(imageToMove).attr('src');
        $.ajax({
            type: 'POST',
            url: "/Text/ImageToInsert",
            data: JSON.stringify(content),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $("#imageTextContainer").append(src);
            }
        });

        $("#addImageTextPlaceholder").empty();
    },
});

