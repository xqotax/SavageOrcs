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
        var videoToMove = $(".popup-content-custom .add-video-placeholder-custom").html();

        var content = $(videoToMove).attr('src');
        $.ajax({
            type: 'POST',
            url: "/Text/VideoToInsert",
            data: JSON.stringify(content),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $("#videoTextContainer").append(src);
            }
        });

        $("#addVideoTextPlaceholder").empty();
    },
});

