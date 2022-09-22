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
            content: "Нажми, щоб отримати координати",
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

