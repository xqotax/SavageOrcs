var MarkAddView = Class.extend({
    IsNew: null,
    Images: null,
    Areas: null,
    AreaName: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    AreaId: null,

    Map: null,
    InfoWindow: null,
    LastLat: null,
    LastLng: null,
    AreaIds: null,
    AreaNames: null,
    SearchSelectDropdown: null,
    IsInitializate: null,
    
    OldDataInput: null,
    SearchAreasViewModel: null,


    RowAddConstString: "<div class=\"row justify-content-md-around\">",
    ColAddConstString: "<div class=\"col-md-3\">",
    DivAddConstString: "</div>",


    InitializeControls: function () {
        var self = this;
        const myLatlng = { lat: parseFloat(self.Lat), lng: parseFloat(self.Lng) };

        this.InfoWindow = new google.maps.InfoWindow({
            content: "Нажми, щоб отримати координати",
            position: myLatlng,
        });

        self.IsInitializate = true;
        //self.InfoWindow.open(self.Map);

        if (self.Images !== null) {
            self.AddImages();
        }

        if (!self.IsNew) {
            self.SetMark();
        }

        self.SearchSelectDropdown = new SearchSelect('#dropdown-input', {
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

        if (self.AreaName !== '') {
            var selected = $($(".searchSelect--Result")[0]);
            selected.removeClass("searchSelect--Placeholder");
            selected.html(self.AreaName);

            $.each($(".searchSelect--Option"), function (index, element) {
                debugger;
                if ($(element).text() === self.AreaName) {
                    $(element).addClass("searchSelect--Option--selected")
                }
            });
            
        }

        self.SubscribeEvents();

        if (self.ToDelete) {
            self.DeleteMark();
        }
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

        $('#dropdown-input').addClass("display-8-custom");
        

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

        self.SearchSelectDropdown.setData(self.AreaNames);
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
                url: "../images/markIcon.png",
                scaledSize: new google.maps.Size(25, 25),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(0, 0)
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
    AddImages: function () {
        var self = this;

        $.each(self.Images, function (index, element) {
            var rowCount = $("#imageContainer .row").length;
            var colCount = $("#imageContainer .col-md-3").length;

            $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

            if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3)) {
                $("#imageContainer").append(self.RowAddConstString + self.ColAddConstString + "<img src=\"" + element + "\" height=\"200\">" + self.DivAddConstString + self.DivAddConstString);
            }
            else {
                $("#imageContainer .row").last().append(self.ColAddConstString + "<img src=\"" + element + "\" height=\"200\">" + self.DivAddConstString);
            }
        });
        
    },
    RemoveImages: function () {
        $("#imageContainer .row").empty();
    },
    Save: function () {
        var self = this;

        var saveMarkViewModel = {
            Id: $("#Id").val(),
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
                ResultPopUp(result.success, result.text, result.url, result.id);
            }
        });
        
    },
    GetAreas: function () {
        var self = this;

        if (self.IsInitializate) {
            self.IsInitializate = false;
            return;
        }

        self.SearchAreasViewModel = { Text: $(".form-control-Select-Bar").val() };

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
    }
});
