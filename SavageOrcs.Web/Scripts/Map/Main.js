var MapMainView = Class.extend({
    Lat: null,
    Lng: null,
    Map: null,
    Zoom: null,
    Marks: null,
    Clusters: null,
    MapId: null,
    MapName: null,

    MapMarks: null,
    MapClusters: null,
    InfoWindow: null,
    OldZoom: null,
    SelectedText: null,
    MultiselectAll: null,

    AreaTextPlaceholder: null,
    NameTextPlaceholder: null,

    SearchTextName: null,
    SearchTextArea: null,

    IsClear: null,
    InitializeControls: function () {
        var self = this;
        const myLatlng = { lat: 50.5077456, lng: 31.018623 };

        self.InfoWindow = new google.maps.InfoWindow({
            content: "Перейти",
            position: myLatlng,
        });

        var areasOptions = {
            placeholder: self.AreaTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextArea.value,
            height: window.innerWidth < 700 ? "150px" : "300px",
            Id: "areasMultiselect",
            //MaxElementsToShow: 2
        }

        var keyWordsAndMarksOptions = {
            placeholder: self.NameTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextName.value,
            height: window.innerWidth < 700 ? "150px" : "300px",
            Id: "namesMultiselect",
            //MaxElementsToShow: 2
        }

        MultiselectDropdown(keyWordsAndMarksOptions);
        MultiselectDropdown(areasOptions);

        if ($("#areasMultiselect").val().length !== 0 || $('#namesMultiselect').val().length !== 0)
            self.Search();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $("#filter-big-text-info").on('click', function () {
            self.Search();
        });

        $(".clear-button-col").on('click', function () {
            self.Clear();
        });

        $("#namesMultiselect").on('change', function () {
            self.OnMultiselectChange();
        });

        $("#areasMultiselect").on('change', function () {
            self.OnMultiselectChange();
        });
    },
    InitMap: function () {
        var self = this;
        let map = new google.maps.Map(document.getElementById("map"), {
            center: {
                lat: parseFloat(this.Lat),
                lng: parseFloat(this.Lng)
            },
            zoom: 6,
            minZoom: 5,
            options: {
                gestureHandling: 'greedy'
            },
            disableDefaultUI: true,
            styles: [{ "featureType": "water", "elementType": "geometry.fill", "stylers": [{ "color": "#d3d3d3" }] },
            { "featureType": "transit", "stylers": [{ "color": "#808080" }, { "visibility": "off" }] },
            { "featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{ "visibility": "on" }, { "color": "#b3b3b3" }] },
            { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
            { "featureType": "road.local", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ffffff" }, { "weight": 1.8 }] },
            { "featureType": "road.local", "elementType": "geometry.stroke", "stylers": [{ "color": "#d7d7d7" }] },
            { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ebebeb" }] },
            { "featureType": "administrative", "elementType": "geometry", "stylers": [{ "color": "#a7a7a7" }] },
            { "featureType": "road.arterial", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
            { "featureType": "road.arterial", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
            { "featureType": "landscape", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#DEDCDC" }] },
            { "featureType": "road", "elementType": "labels.text.fill", "stylers": [{ "color": "#696969" }] },
            { "featureType": "administrative", "elementType": "labels.text.fill", "stylers": [{ "visibility": "on" }, { "color": "#737373" }] },
            { "featureType": "poi", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] },
            { "featureType": "poi", "elementType": "labels", "stylers": [{ "visibility": "off" }] },
            { "featureType": "road.arterial", "elementType": "geometry.stroke", "stylers": [{ "color": "#d6d6d6" }] },
            { "featureType": "road", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] },
            {},
            { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#dadada" }] }]
        });
        self.OldZoom = 6;
        google.maps.event.addListener(map, 'zoom_changed', function () {
            self.ChangeMarkerSize();
        });

        self.Map = map;

        self.MapMarks = [];
        self.MapClusters = [];
        $.each(self.Marks, function (index, element) {

            let marker = new google.maps.Marker({
                position: {
                    lat: parseFloat(element.lat),
                    lng: parseFloat(element.lng)
                },
                map: map,
                title: element.name,
                icon: {
                    url: "images/redCircle.png",
                    scaledSize: new google.maps.Size(10, 10),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(5, 5)
                }
            });

            marker.addListener("click", () => {
                self.MarkOnClick(marker, element);
            });

            self.MapMarks.push({ id: element.id, marker: marker });
        });

        $.each(self.Clusters, function (index, element) {

            let marker = new google.maps.Marker({
                position: {
                    lat: parseFloat(element.lat),
                    lng: parseFloat(element.lng)
                },
                map: map,
                title: element.name,
                icon: {
                    url: "images/redCircle.png",
                    scaledSize: new google.maps.Size(16, 16),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(8, 8)
                }
            });

            marker.addListener("click", () => {
                self.MarkOnClick(marker, element, true);
            });

            self.MapClusters.push({ id: element.id, marker: marker });
        });

    },
    ChangeMarkerSize: function () {
        var self = this;
        var currentZoom = self.Map.getZoom();

        if (!((self.OldZoom <= 15 && currentZoom <= 15) || (self.OldZoom > 15 && currentZoom > 15))) {
            var iconSize = new google.maps.Size(10, 10);
            var iconOrigin = new google.maps.Point(0, 0);
            var iconAnchor = new google.maps.Point(5, 5);

            if (currentZoom > 15) {
                iconSize = new google.maps.Size(20, 20);
                iconOrigin = new google.maps.Point(0, 0);
                iconAnchor = new google.maps.Point(10, 10);
            }

            for (var i = 0; i < self.MapMarks.length; i++) {
                var marker = self.MapMarks[i].marker;

                marker.setIcon({
                    url: marker.getIcon().url,
                    scaledSize: iconSize,
                    origin: iconOrigin,
                    anchor: iconAnchor
                });
            }
        }
        self.OldZoom = currentZoom;

    },
    MarkOnClick: function (marker, element, isCluster = false) {
        var self = this;
        self.Map.setZoom(10);
        self.Map.setCenter(marker.getPosition());

        self.InfoWindow.close();

        self.InfoWindow = new google.maps.InfoWindow({
            position: marker.getPosition(),
        });

        var link = "<a href=\"";
        if (isCluster)
            link += "/Cluster/Revision?id=" + element.id.toString();
        else
            link += "/Mark/Revision?id=" + element.id.toString();
        link += "\" class=\"mark-map-popup\">" + element.name + "</a>";
        self.InfoWindow.setContent(link);
        self.InfoWindow.open(self.Map);
    },
    Search: function () {
        var self = this;
        console.log("1");
        if (self.IsClear)
            return;
        console.log("2");
        var names = $('#namesMultiselect').val() || [];

        var filters = {
            SelectedKeyWordIds: [],
            SelectedClusterIds: [],
            SelectedMarkIds: [],
            SelectedAreaIds: $("#areasMultiselect").val(),
        };

        names.map(function (value) {
            if (value.startsWith('C')) {
                filters.SelectedClusterIds.push(value.substr(1));
            } else if (value.startsWith('M')) {
                filters.SelectedMarkIds.push(value.substr(1));
            } else if (value.startsWith('K')) {
                filters.SelectedKeyWordIds.push(value.substr(1));
            }
        });

        $.ajax({
            type: 'POST',
            url: "/Map/GetMarks",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (json) {
                self.MapMarks.forEach((marker) => {
                    marker.marker.setMap(null);
                });
                self.MapClusters.forEach((marker) => {
                    marker.marker.setMap(null);
                });
                self.MapMarks = [];
                self.MapClusters = [];
                $.each(json, function (index, element) {
                    let marker = new google.maps.Marker({
                        position: {
                            lat: parseFloat(element.lat),
                            lng: parseFloat(element.lng)
                        },
                        map: self.Map,
                        title: element.name,
                        icon: {
                            url: "images/redCircle.png",
                            scaledSize: element.isCluster ? new google.maps.Size(16, 16) : new google.maps.Size(10, 10),
                            origin: new google.maps.Point(0, 0),
                            anchor: element.isCluster ? new google.maps.Point(8, 8) : new google.maps.Point(5, 5)
                        }
                    });

                    marker.addListener("click", () => {
                        self.MarkOnClick(marker, element, element.isCluster);
                    });
                    if (element.isCluster)
                        self.MapClusters.push({ id: element.id, marker: marker });
                    else
                        self.MapMarks.push({ id: element.id, marker: marker });
                });
            }
        });
    },
    Clear: function () {
        var self = this;
        self.IsClear = true;
        ClearMultiSelect("namesMultiselect");
        ClearMultiSelect("areasMultiselect");
        self.IsClear = false;
        document.activeElement.blur();
        self.Search();
    },
    OnMultiselectChange: function () {
        var areaSelectedLenght = $("#areasMultiselect").val().length;
        var nameSelectedLenght = $("#namesMultiselect").val().length;

        if (areaSelectedLenght > 0 || nameSelectedLenght > 0) {
            $("#filter-big-text-info").css({ "color": "#FF2929" });
            $("#clear-button").css({ "color": "#FF2929" });
            $(".clear-button-img img").attr("src", "/images/icons/clearRed.png");
        }
        else {
            $("#filter-big-text-info").css({ "color": "#0F0F0F" });
            $("#clear-button").css({ "color": "#0F0F0F" });
            $(".clear-button-img img").attr("src", "/images/icons/clear.png");
        }
    }
});

