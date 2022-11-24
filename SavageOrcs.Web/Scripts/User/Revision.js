var RevisionUserView = Class.extend({
    UserId: null,
    CuratorId: null,
    IsCurator: null,
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#saveMark").click(function () {
            self.Save();
        });

        $("#addCuratorImage").click(function () {
            self.AddCuratorImage();
        });

        $("#IsCurator").change(function () {
            if (this.checked) {
                $(".curatorRow").css({ 'visibility': 'visible' });
            }
            else {
                $(".curatorRow").css({ 'visibility': 'hidden' });
            }
        });

        $("#search").click(function () {
            self.Search();
        });

        $("#clearFilters").click(function () {
            $("#Name").val('');
            $("#Email").val('');
        });

    },
    Save: function () {
        var self = this;

        var saveUserViewModel = {
            Id: self.UserId,
            FirstName: $("#FirstName").val(),
            LastName: $("#LastName").val(),
            Email: $("#Email").val(),
            IsCurator: $("#IsCurator").is(":checked"),
            RoleIds: [],
            CuratorId: $("#CuratorInfo_Id").val(),
            DisplayName: $("#CuratorInfo_DisplayName").val(),
            Description: $("#CuratorInfo_Description").val(),
            Image: $("#imagePlaceholder").attr('src')
        }
        $(".check-box-row").each(function (index, element) {
            if ($(element).is(":checked"))
            {
                saveUserViewModel.RoleIds.push($(element).val());
            }
        });

        $.ajax({
            type: 'POST',
            url: "/User/SaveUser",
            data: JSON.stringify(saveUserViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);
            }
        });
    },
    AddCuratorImage: function () {
        $.ajax({
            type: 'POST',
            url: "/User/AddCuratorImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImagePlaceholder').html(src);
            }
        });
    }
})