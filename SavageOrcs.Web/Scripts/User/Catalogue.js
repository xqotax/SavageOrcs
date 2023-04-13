var CatalogueUserView = Class.extend({
    IsGlobalAdmin: null,

    TableRowStartConstString: "<div class=\"table-body-catalogue-user-row justify-content-center d-flex\"><div class=\"table-body-catalogue-user-column-number\">",
    TableRowFullNameConstString: "</div><div class=\"table-body-catalogue-user-column-name\">",
    TableRowLinkConstString: "<a class=\"table-body-catalogue-user-column-name-link\" href=\"/User/Revision?id=",
    TableRowEmailConstString: "</div><div class=\"table-body-catalogue-user-column-email\">",
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

        var userCatalogueFilter = {
            Name: $("#Name").val(),
            Email: $("#Email").val()
        };

        $.ajax({
            type: 'POST',
            url: "/User/GetUsers",
            data: JSON.stringify(userCatalogueFilter),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".table-body-catalogue-user").empty();

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
                    
                    toAdd += self.TableRowEmailConstString + element.email;
                    toAdd += self.TableRowEndConstString;
                });

                $(".table-body-catalogue-user").append(toAdd);
            }
        });
    }
})