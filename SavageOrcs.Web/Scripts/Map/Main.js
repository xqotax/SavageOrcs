var MapMainView = Class.extend({
    Lat: null,
    Lng: null,
    Zoom: null,
    InitializeControls: function () {
    },
    SubscribeEvents: function () { },
    InitMap: function () {
        let map = new google.maps.Map(document.getElementById("map"), {
            center: {
                lat: parseFloat(this.Lat),
                lng: parseFloat(this.Lng)
            },
            zoom: 6
        });
        let marker = new google.maps.Marker({
            position: {
                lat: 48.6683328,
                lng: 25.4827080
            },
            map: map,
            title: "My Home",
            //icon: {
            //    url: "~/Media/Photos/1.jpg",
            //    scaledSize: new google.maps.Size(15, 15),
            //    origin: new google.maps.Point(0, 0),
            //    anchor: new google.maps.Point(0, 0)
            //}
        });
    }

});

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
