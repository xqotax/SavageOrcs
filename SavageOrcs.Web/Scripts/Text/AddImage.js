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
        var self = this;

        var rowCount = $("#imageTextContainer .row").length;
        var colCount = $("#imageTextContainer .col-md-3").length;

        $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3 )) {
            $("#imageTextContainer").append(addTextView.RowAddConstString + addTextView.ColAddConstString + imageToMove + addTextView.DivAddConstString + addTextView.DivAddConstString);
        }
        else {
            $("#imageTextContainer .row").last().append(addTextView.ColAddConstString + imageToMove + addTextView.DivAddConstString);
        }

        addTextView.AfterAddingImage();
        $("#addImageTextPlaceholder").empty();
    },
});

