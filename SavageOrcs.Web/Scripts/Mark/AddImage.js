var AddImageView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
    RowAddConstString: "<div class=\"row justify-content-md-around\">",
    ColAddConstString: "<div class=\"col-md-3\">",
    DivAddConstString: "</div>",
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

        var rowCount = $("#imageContainer .row").length;
        var colCount = $("#imageContainer .col-md-3").length;

        $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3 )) {
            $("#imageContainer").append(self.RowAddConstString + self.ColAddConstString + imageToMove + self.DivAddConstString + self.DivAddConstString);
        }
        else {
            $("#imageContainer .row").last().append(self.ColAddConstString + imageToMove + self.DivAddConstString);
        }
        $("#addImagePlaceholder").empty();
    },
});

