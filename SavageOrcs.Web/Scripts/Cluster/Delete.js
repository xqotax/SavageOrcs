var DeleteClusterView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteClusterCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteClusterConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Cluster/DeleteConfirm?id=" + $("#Id").val() + "&withMarks=" + $("#IncludeMarks").is(":checked"),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteClusterPlaceholder").empty();
    }
});

