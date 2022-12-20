var CatalogueClusterView = Class.extend({
    TableClusterRowStartConstString: "<div class=\"table-body-catalogue-cluster-row justify-content-center d-flex\"><div class=\"table-body-catalogue-cluster-column-number\">",
    TableClusterRowNameConstString: "</div><div class=\"table-body-catalogue-cluster-column-name\"><a href=\"/Cluster/Revision?id=",
    TableClusterRowDescriptionConstString: "</a></div><div class=\"table-body-catalogue-cluster-column-description\">",
    TableClusterRowMarksCountConstString: "</div><div class=\"table-body-catalogue-cluster-column-markCount\">",
    TableClusterRowAreaConstString: "</div><div class=\"table-body-catalogue-cluster-column-area\">",
    TableClusterRowAreaButtonConstString: "<button class=\"button-catalogue-cluster-column-area\" value=\"",
    TableClusterRowEndConstString: "</div></div>",
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
            $("#KeyWord").val('');
            $("#AreaName").val('');
            $("#ClusterName").val('');
            $("#ClusterDescription").val('');
        });
    },
    Search: function () {
        $(".table-body-catalogue-cluster").empty();

        var self = this;

        var filters = {
            AreaName: $("#AreaName").val(),
            ClusterName: $("#ClusterName").val(),
            ClusterDescription: $("#ClusterDescription").val(),
            KeyWord: $("#KeyWord").val(),
            MinCountOfMarks: $("#MinCountOfMarks").val() === '' ? 0 : parseInt($("#MinCountOfMarks").val())
        };

        $.ajax({
            type: 'POST',
            url: "/Cluster/GetClusters",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

                var toAdd = "";

                $.each(data, function (index, element) {
                    toAdd += self.TableClusterRowStartConstString + (index + 1);
                    toAdd += self.TableClusterRowNameConstString + element.id + "\">" + element.name;
                    toAdd += self.TableClusterRowDescriptionConstString + element.description;
                    toAdd += self.TableClusterRowMarksCountConstString + element.markCount;


                    if (element.area.id !== null) {
                        toAdd += self.TableClusterRowAreaConstString + self.TableClusterRowAreaButtonConstString;
                        toAdd += element.area.id + "\">" + element.area.name + "</button>";
                    }
                    else {
                        toAdd += self.TableClusterRowAreaConstString;
                    }
                    toAdd += self.TableClusterRowEndConstString;
                });

                $(".table-body-catalogue-cluster").append(toAdd);

                $(".button-catalogue-cluster-column-area").click(function () {
                    $("#KeyWord").val("");
                    $("#AreaName").val(this.innerText);
                    $("#ClusterName").val('');
                    $("#ClusterDescription").val('');
                    self.Search();
                });
            }
        });
    }
});