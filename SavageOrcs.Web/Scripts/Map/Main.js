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
            disableDefaultUI: true
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
                    url: element.isApproximate ? "images/markIconApproximate.png" : "images/markIcon.png",
                    scaledSize: new google.maps.Size(20, 20),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 10)
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
                    url: "images/clusterIcon.png",
                    scaledSize: new google.maps.Size(30, 30),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(15, 15)
                }
            });

            marker.addListener("click", () => {
                self.MarkOnClick(marker, element, true);
            });

            self.MapClusters.push({ id: element.id, marker: marker });
        });
        
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
