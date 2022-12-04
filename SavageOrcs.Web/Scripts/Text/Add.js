var AddTextView = Class.extend({
    IsNew: null,
    CuratorName: null,
    CuratorId: null,

    Curators: null,
    CuratorIds: null,
    CuratorNames: null,
    SearchSelectDropdownCurators: null,

    InitializeControls: function () {
        var self = this;

        self.SearchSelectDropdownCurators = new SearchSelect('#dropdown-input-for-curator', {
            data: [],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 9,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: null,
        });

        self.InitializeCurators(self.Curators);

       

        if (self.CuratorName !== '') {
            var selected = $($("#Curator .searchSelect--Result")[0]);
            selected.removeClass("#Curator searchSelect--Placeholder");
            selected.html(self.CuratorName);

            $.each($("#Curator .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.CuratorName) {
                    $(element).addClass("#Curator searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-curator").val(self.CuratorName);
        }

        self.SubscribeEvents();

        if (self.ToDelete) {
            self.DeleteText();
        }
    },
    SubscribeEvents: function () {
        var self = this;

        $('#saveText').on('click', function () {
            self.Save();
        });

        $('#dropdown-input-for-curator').addClass("display-8-custom");
    },

    InitializeCurators: function (data) {
        var self = this;
        self.CuratorNames = [];
        self.CuratorIds = [];

        $.each(data, function (index, element) {
            self.CuratorNames.push(element.name);
            self.CuratorIds.push(element.id);
        });

        self.SearchSelectDropdownCurators.setData(self.CuratorNames);
    },

    Save: function () {
        var self = this;

        debugger;
        var curatorId = self.CuratorIds[self.CuratorNames.indexOf($("#dropdown-input-for-curator").val())];
        curatorId = curatorId === "" ? null : curatorId;

        var saveTextViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            CuratorId: curatorId,
            Name: $("#Name").val(),
            Subject: $("#Subject").val(),
            Content: "not complete"
        };

        $.ajax({
            type: 'POST',
            url: "/Text/SaveText",
            data: JSON.stringify(saveTextViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);
            }
        });

    },
    
    DeleteText: function () {
        $.ajax({
            type: 'POST',
            url: "/Text/DeleteText",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteTextPlaceholder').html(src);
            }
        });
    }
})