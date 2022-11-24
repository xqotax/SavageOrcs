var CatalogueUserView = Class.extend({
    IsGlobalAdmin: null,

    TableRowStartConstString: "<div class=\"table-body-row justify-content-center d-flex\"><div class=\"table-body-column-number\">",
    TableRowFullNameConstString: "</div><div class=\"table-body-column-name\">",
    TableRowLinkConstString: "<a class=\"table-body-column-name-link\" href=\"/User/Revision?id=",
    TableRowCuratorStatusConstString: "</div><div class=\"table-body-column-curator-status\">",
    TableRowEmailConstString: "</div><div class=\"table-body-column-email\">",
    TableRowEndConstString: "</div></div>",
    
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#search").click(function () {
            self.Search();
        });

        $("#clearFilters").click(function () {
            $("#Name").val('');
            $("#Email").val('');
        });

    },
    Search: function () {
        var self = this;

        var filters = {
            Name: $("#Name").val(),
            Email: $("#Email").val()
        };

        $.ajax({
            type: 'POST',
            url: "/User/GetUsers",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".table-body").empty();

                toAdd = "";
                $.each(data, function (index, element) {

                    toAdd += self.TableRowStartConstString + (index + 1);
                    toAdd += self.TableRowFullNameConstString
                    if (self.IsGlobalAdmin) {

                        toAdd += self.TableRowLinkConstString  + element.id + "\">" + element.fullName + "</a>";
                    }
                    else {
                        toAdd += element.fullName;
                    }
                    toAdd += self.TableRowCuratorStatusConstString;
                    if (element.isCurator) {
                        toAdd += "Так";
                    }
                    else {
                        toAdd += "Ні";
                    }
                    toAdd += self.TableRowEmailConstString + element.email;
                    toAdd += self.TableRowEndConstString;
                });

                $(".table-body").append(toAdd);
            }
        });
    }
})