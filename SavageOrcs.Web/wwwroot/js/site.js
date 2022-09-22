function Class() { }
Class.prototype.construct = function () { };
Class.extend = function (def) {
    var classDef = function () {
        if (arguments[0] !== Class) { this.construct.apply(this, arguments); }
    };

    var proto = new this(Class);
    var superClass = this.prototype;

    for (var n in def) {
        var item = def[n];
        if (item instanceof Function) item.Base = superClass;
        proto[n] = item;
    }

    classDef.prototype = proto;

    classDef.extend = this.extend;
    return classDef;
};
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

var MarkAddView = Class.extend({
    Map: null,
    InitWindow: null,
    LastLat: null,
    LastLng: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    InitializeControls: function () {
        const myLatlng = { lat: 50.5077456, lng: 31.018623 };

        this.InfoWindow = new google.maps.InfoWindow({
            content: "�����, ��� �������� ����������",
            position: myLatlng,
        });

        var self = this;

        //self.InfoWindow.open(self.Map);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#setCoordinates").click(function () {
            $("#Lng").val(self.LastLng);
            $("#Lat").val(self.LastLat);
        });


        $('#addImage').on('click', function () {
            self.AddImage();
        });

        $('#saveMark').on('click', function () {
            self.Save();
        });

        $('#removeImages').on('click', function () {
            self.RemoveImages();
        });
        

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
    InitMap: function () {
        debugger;
        var self = this;
        this.Map = new google.maps.Map(document.getElementById("mapMarkAdd"), {
            center: {
                lat: parseFloat(self.Lat),
                lng: parseFloat(self.Lng)
            },
            zoom: parseFloat(self.Zoom)
        });
    },
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
    RemoveImages: function () {
        $("#imageContainer .row").empty();
    },
    Save: function () {
        var saveMarkViewModel = {
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: $("#AreaId").val(),
            Name: $("#Name").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEnd").val(),
            ResourceUrl: $("#ResourceUrl").val(),
            ImageMarkSaveViewModels: []
        };
        debugger;
        $("#imageContainer img").each(function (index, element) {
            saveMarkViewModel.ImageMarkSaveViewModels.push(element.src);
        });

        $.ajax({
            type: 'POST',
            url: "/Mark/SaveMark",
            data: JSON.stringify(saveMarkViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                alert("fads");
            }
        });
        
    }
});


var AddImageView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
    RowAddConstString: "<div class=\"row justify-content-md-around\">",
    ColAddConstString: "<div class=\"col-md-3\">",
    DivAddConstString: "</div>",
    InitializeControls: function () {
        var self = this;
        self.ImageInput = $('#imageInput');
        self.Reader = new FileReader();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        self.Reader.addEventListener("load", () => {
            $("#imagePlaceholder")[0].src = self.Reader.result;
        }, false);

        self.ImageInput.on('change', function () {
            self.Reader.readAsDataURL(self.ImageInput[0].files[0]);
        });
       
        $('#addImageCancelButton').on('click', function () {
            self.Close();
        });
        $('#addImageConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Close: function () {
        $("#addImagePlaceholder").empty();
    },
    Save: function () {
        var self = this;

        var rowCount = $("#imageContainer .row").length;
        var colCount = $("#imageContainer .col-md-3").length;

        $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3 )) {
            $("#imageContainer").append(self.RowAddConstString + self.ColAddConstString + imageToMove + self.DivAddConstString + self.DivAddConstString);
        }
        else {
            $("#imageContainer .row").last().append(self.ColAddConstString + imageToMove + self.DivAddConstString);
        }
        $("#addImagePlaceholder").empty();
    },
});

