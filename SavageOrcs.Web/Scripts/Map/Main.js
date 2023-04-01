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
    InitializeControls: function () {
        var self = this;
        const myLatlng = { lat: 50.5077456, lng: 31.018623 };

        self.InfoWindow = new google.maps.InfoWindow({
            content: "Перейти",
            position: myLatlng,
        });
    },
    SubscribeEvents: function () { },
    InitMap: function () {
        var self = this;
        let map = new google.maps.Map(document.getElementById("map"), {
            center: {
                lat: parseFloat(this.Lat),
                lng: parseFloat(this.Lng)
            },
            zoom: 6,
            options: {
                gestureHandling: 'greedy'
            },
            disableDefaultUI: true,
            styles: [{ "featureType": "water", "elementType": "geometry.fill", "stylers": [{ "color": "#d3d3d3" }] },
                { "featureType": "transit", "stylers": [{ "color": "#808080" }, { "visibility": "off" }] },
                { "featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{ "visibility": "on" }, { "color": "#b3b3b3" }]},
                { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
                { "featureType": "road.local", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ffffff" }, { "weight": 1.8 }] },
                { "featureType": "road.local", "elementType": "geometry.stroke", "stylers": [{ "color": "#d7d7d7" }] },
                { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ebebeb" }] },
                { "featureType": "administrative", "elementType": "geometry", "stylers": [{ "color": "#a7a7a7" }] },
                { "featureType": "road.arterial", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
                { "featureType": "road.arterial", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }] },
                { "featureType": "landscape", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#efefef" }] },
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
                    scaledSize: new google.maps.Size(20, 20),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 10)
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
        self.Map.setZoom(12);
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
        link += "\" class=\"btn btn-dark-custom\">" + element.name + "</a>";
        self.InfoWindow.setContent(link);
        self.InfoWindow.open(self.Map);
    }

});
//parseFloat(self.Lat),
//function initMap() {
//    let map = new google.maps.Map(document.getElementById("map"), {
//        center: {
//            lat: 12,
//            lng: 12
//        },
//        zoom: 12
//    });

//    let marker = new google.maps.Marker({
//        position: {
//            lat: 48.6683328,
//            lng: 25.4827080
//        },
//        map: map,
//        title: "My Home",
//        icon: {
//            url: "photo/1.jpg",
//            scaledSize: new google.maps.Size(15, 15),
//            origin: new google.maps.Point(0, 0),
//            anchor: new google.maps.Point(0, 0)
//        }
//    });
//}
//var src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDuw7bPRLxL2yVBd9YArtpb47myhmUePGY&callback=initMap"
