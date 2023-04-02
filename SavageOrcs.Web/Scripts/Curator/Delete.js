var DeleteCuratorView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteCuratorCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteCuratorConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Curator/DeleteConfirm?id=" + $("#Id").val(),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteCuratorPlaceholder").empty();
    },
});

