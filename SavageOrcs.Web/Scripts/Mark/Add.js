var MarkAddView = Class.extend({
    IsNew: null,
    Images: null,
    AreaName: null,
    ClusterName: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    AreaId: null,

    Map: null,
    InfoWindow: null,
    LastLat: null,
    LastLng: null,

    Areas: null,
    AreaIds: null,
    AreaNames: null,
    SearchSelectDropdownAreas: null,

    Clusters: null,
    ClusterIds: null,
    ClusterNames: null,
    SearchSelectDropdownClusters: null,

    Curators: null,
    CuratorIds: null,
    CuratorNames: null,
    SearchSelectDropdownCurators: null,

    IsInitializate: null,
    OldDataInput: null,
    SearchAreasViewModel: null,

    InitializeControls: function () {
        var self = this;
        const myLatlng = { lat: parseFloat(self.Lat), lng: parseFloat(self.Lng) };

        this.InfoWindow = new google.maps.InfoWindow({
            content: "Нажми, щоб отримати координати",
            position: myLatlng,
        });

        self.IsInitializate = true;
        //self.InfoWindow.open(self.Map);

        if (!self.IsNew) {
            self.SetMark();
        }

        //var placesOptions = {
        //    placeholder: "Виберіть локацію",
        //    txtSelected: "вибрано",
        //    txtAll: "Всі",
        //    txtRemove: "Видалити",
        //    txtSearch: "Пошук",
        //    height: "300px",
        //    Id: "placesMultiselect"
        //}

        //MultiselectDropdown(placesOptions);


        self.SearchSelectDropdownAreas = new SearchSelect('#dropdown-input-for-mark', {
            data: [],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 9,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: function (ev) { self.GetAreas() },
        });

        self.InitializeAreas(self.Areas);

        self.SearchSelectDropdownClusters = new SearchSelect('#dropdown-input-for-cluster', {
            data: [],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 9,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: null,
        });

        self.InitializeClusters(self.Clusters);

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

        if (self.AreaName !== '') {
            var selected = $($("#Area .searchSelect--Result")[0]);
            selected.removeClass("#Area searchSelect--Placeholder");
            selected.html(self.AreaName);


            $.each($("#Area .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.AreaName) {
                    $(element).addClass("#Area searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-mark").val(self.AreaName);

        }

        if (self.ClusterName !== '') {
            var selected = $($("#Cluster .searchSelect--Result")[0]);
            selected.removeClass("#Cluster searchSelect--Placeholder");
            selected.html(self.ClusterName);

            $.each($("#Cluster .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.ClusterName) {
                    $(element).addClass("#Cluster searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-cluster").val(self.ClusterName);
        }

        if (self.CuratorName !== '') {
            var selected = $($("#Curator .searchSelect--Result")[0]);
            selected.removeClass("#Curator searchSelect--Placeholder");
            selected.html(self.CuratorName);

            $.each($("#Cluster .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.CuratorName) {
                    $(element).addClass("#Curator searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-curator").val(self.CuratorName);
        }

        self.SubscribeEvents();

        if (self.ToDelete) {
            self.DeleteMark();
        }
    },
    //OnPlacesChange: function () {
    //    var el = $("#placesMultiselect");
    //},
    SubscribeEvents: function () {
        var self = this;

        $("#setCoordinates").click(function () {
            $("#Lng").val(self.LastLng);
            $("#Lat").val(self.LastLat);
        });

        $('#addImage').on('click', function () {
            self.AddImage();
        });

        $('#save').on('click', function () {
            self.Save();
        });

        $('#delete').on('click', function () {
            self.DeleteMark();
        });


        $('#removeImages').on('click', function () {
            self.RemoveImages();
        });

        //$("#placesMultiselect").on('change', function () {
        //    self.OnPlacesChange();
        //});


        self.Map.addListener("click", (mapsMouseEvent) => {
            self.InfoWindow.close();

            self.InfoWindow = new google.maps.InfoWindow({
                position: mapsMouseEvent.latLng,
            });
            var latLng = mapsMouseEvent.latLng.toJSON();
            self.LastLat = latLng.lat.toFixed(9).toString();
            self.LastLng = latLng.lng.toFixed(9).toString();
            self.InfoWindow.setContent(
                "&#8645;: " + self.LastLat + ",   &#8644;: " + self.LastLng
            );
            self.InfoWindow.open(self.Map);
        });
    },
    InitializeAreas: function (data)
    {
        var self = this;
        self.AreaNames = [];
        self.AreaIds = [];

        $.each(data, function (index, element) {
            self.AreaNames.push(element.name);
            self.AreaIds.push(element.id);
        });

        self.SearchSelectDropdownAreas.setData(self.AreaNames);
    },

    InitializeClusters: function (data) {
        var self = this;
        self.ClusterNames = [];
        self.ClusterIds = [];

        $.each(data, function (index, element) {
            self.ClusterNames.push(element.name);
            self.ClusterIds.push(element.id);
        });

        self.SearchSelectDropdownClusters.setData(self.ClusterNames);
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
    SetMark: function () {
        var self = this;

        let marker = new google.maps.Marker({
            position: {
                lat: parseFloat(self.Lat),
                lng: parseFloat(self.Lng)
            },
            map: self.Map,
            title: self.AreaName,
            icon: {
                url: "../images/redCircle.png",
                scaledSize: new google.maps.Size(24, 24),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(12, 12)
            }
        });
    },
    InitMap: function () {
        var self = this;

        self.Map = new google.maps.Map(document.getElementById("mapMarkAdd"), {
            center: {
                lat: parseFloat(self.Lat),
                lng: parseFloat(self.Lng)
            },
            zoom: parseFloat(self.Zoom),
            options: {
                gestureHandling: 'greedy'
            },
            disableDefaultUI: true
        });
    },
    
    Save: function () {

        //$("#placesMultiselect").val(null)
        //$('#Place input[type="checkbox"]').prop('checked', false);

        var self = this;

        var areaId = self.AreaIds[self.AreaNames.indexOf($("#dropdown-input-for-mark").val())];
        var clusterId = self.ClusterIds[self.ClusterNames.indexOf($("#dropdown-input-for-cluster").val())];
        var curatorId = self.CuratorIds[self.CuratorNames.indexOf($("#dropdown-input-for-curator").val())];
        areaId = areaId === "" ? null : areaId;
        clusterId = clusterId === "" ? null : clusterId;
        curatorId = curatorId === "" ? null : curatorId;
        debugger;
        var saveMarkViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: areaId,
            ClusterId: clusterId,
            CuratorId: curatorId,
            Name: $("#Name").val(),
            NameEng: $("#NameEng").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            ResourceUrl: $("#ResourceUrl").val(),
            ResourceName: $("#ResourceName").val(),
            ResourceNameEng: $("#ResourceNameEng").val(),
            //SelectedPlaceIds: $("#placesMultiselect").val(),
            Images: []
        };

        $("#imageContainer img").each(function (index, element) {
            saveMarkViewModel.Images.push(
                {
                    Content: element.src,
                    IsVisible: $(element).parent().parent().find("input").is(":checked")
                }
            );
        });

        $.ajax({
            type: 'POST',
            url: "/Mark/SaveMark",
            data: JSON.stringify(saveMarkViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    window.location.href = 'https://' + window.location.host + '/Mark/Catalogue';
                }
                else {
                    ResultPopUp(result.success, result.text, result.url, result.id);
                }
            }
        });
        
    },
    GetAreas: function () {
        var self = this;


        if (self.IsInitializate) {
            self.IsInitializate = false;
            return;
        }

        self.SearchAreasViewModel = { Text: $("#Area .form-control-Select-Bar").val() };

        self.OldDataInput = Date.now();


        if (self.SearchAreasViewModel.Text.length < 3)
            return;
        else {
            setTimeout(function () {
                var newDataInput = Date.now();
                if (newDataInput - self.OldDataInput < 2000)
                    return;
                else {
                    $.ajax({
                        type: 'POST',
                        url: "/Mark/GetAreas",
                        data: JSON.stringify(self.SearchAreasViewModel),
                        contentType: "application/json; charset=utf-8",
                        async: false,
                        success: function (data) {
                            self.InitializeAreas(data);
                        }
                    });
                }
            }, 2000);
        }
    },
    DeleteMark: function () {
        $.ajax({
            type: 'POST',
            url: "/Mark/DeleteMark",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteMarkPlaceholder').html(src);
            }
        });
    },



    //for image
    AddImage: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Mark/AddImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImagePlaceholder').html(src);
            }
        });
    },
    RemoveImage: function (el) {
        var row = $(el).parent().parent().parent();
        console.log(row);
        row.remove();
    },
    RemoveImages: function () {
        $("#imageContainer").empty();
    }
});
