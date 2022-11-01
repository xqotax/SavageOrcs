var MarkAddView = Class.extend({
    Map: null,
    InfoWindow: null,
    LastLat: null,
    LastLng: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    AreaIds: null,
    AreaNames: null,
    SearchSelectDropdown: null,
    IsInitializate: null,
    //InputText: true,
    InitializeControls: function () {
        const myLatlng = { lat: 50.5077456, lng: 31.018623 };

        this.InfoWindow = new google.maps.InfoWindow({
            content: "Нажми, щоб отримати координати",
            position: myLatlng,
        });

        var self = this;

        self.IsInitializate = true;
        //self.InfoWindow.open(self.Map);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#setCoordinates").click(function () {
            $("#Lng").val(self.LastLng);
            $("#Lat").val(self.LastLat);
        });


        $('#dropdown-input').keyup(function (event) { console.log(event.key); });

        
        //$('#AreaText').keyup(function () {
        //    var areaId = self.LastAreaId;
        //    self.GetAreas();
        //    setTimeout(() => {
        //        $.each(self.Areas, function (index, element) {
        //            if (element.id == areaId) {
        //                $("#AreaId").val(areaId);
        //                return false;
        //            }
        //        })
        //    }, 1000);
           
        //});

        //$('#AreaId').change(function () {
        //    self.LastAreaId = $("#AreaId").val();
        //});


        self.SearchSelectDropdown = new SearchSelect('#dropdown-input', {
            data: ["2" , "3"],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 9,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: function (ev) { self.GetAreas() },
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
        var self = this;

        var saveMarkViewModel = {
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: self.AreaIds[self.AreaNames.indexOf($("#dropdown-input").val())],
            Name: $("#Name").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            ResourceUrl: $("#ResourceUrl").val(),
            Images: []
        };

        $("#imageContainer img").each(function (index, element) {
            saveMarkViewModel.Images.push(element.src);
        });

        $.ajax({
            type: 'POST',
            url: "/Mark/SaveMark",
            data: JSON.stringify(saveMarkViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (!result.success) {
                    //error
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
        var searchAreasViewModel = { Text: $(".form-control-Select-Bar").val() };

        if (searchAreasViewModel.Text.length < 3)
            return;
        else {
            $.ajax({
                type: 'POST',
                url: "/Mark/GetAreas",
                data: JSON.stringify(searchAreasViewModel),
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    self.AreaNames = [];
                    self.AreaIds = [];

                    $.each(data, function (index, element) {
                        self.AreaNames.push(element.name);
                        self.AreaIds.push(element.id);
                    });

                    self.SearchSelectDropdown.setData(self.AreaNames);
                }
            });
        }
    }
});
