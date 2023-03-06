var AddVideoTextView = Class.extend({
    Video: null,
    VideoInput: null,
    Reader: null,
    InitializeControls: function () {
        var self = this;
        self.VideoInput = $('#videoInput');
        self.Reader = new FileReader();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        self.Reader.addEventListener("load", () => {
            $("#videoPlaceholder")[0].src = self.Reader.result;
        }, false);

        self.VideoInput.on('change', function () {
            self.Reader.readAsDataURL(self.VideoInput[0].files[0]);
        });

        $('#addVideoCancelButton').on('click', function () {
            self.Close();
        });
        $('#addVideoConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Close: function () {
        $("#addVideoTextPlaceholder").empty();
    },
    Save: function () {
        debugger;
        var rowCount = $("#videoTextContainer .row").length;
        var colCount = $("#videoTextContainer .col-md-3").length;

        $(".popup-content-custom .row #videoPlaceholder").removeAttr('id');

        var videoToMove = $(".popup-content-custom .add-video-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3)) {
            $("#videoTextContainer").append(addTextView.RowAddConstString + addTextView.ColAddConstString + videoToMove + addTextView.DivAddConstString + addTextView.DivAddConstString);
        }
        else {
            $("#videoTextContainer .row").last().append(addTextView.ColAddConstString + videoToMove + addTextView.DivAddConstString);
        }

        addTextView.AfterAddingVideo();
        $("#addVideoTextPlaceholder").empty();
    },
});

