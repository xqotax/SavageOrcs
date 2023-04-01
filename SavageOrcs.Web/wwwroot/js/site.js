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

function ResultPopUp(success, text, url, id) {
    var mainText = "Успіх";
    if (!success)
        mainText = "Помилка";
    var resultUrl = url;

    if (resultUrl.endsWith("{id}"))
        resultUrl = resultUrl.replace("/{id}", "?id=" + id.toString());

    var stringToAppend = "<div id=\"alert-custom\"><div class=\"row display-8-custom\">";
    stringToAppend += mainText;
    stringToAppend += "</div><div class=\"row\">";
    stringToAppend += text;
    stringToAppend += "</div><div class=\"row\"><a id=\"customPopUpGoTo\" href=\"";
    stringToAppend += resultUrl;
    stringToAppend += "\" class=\"btn btn-dark-custom\">Перейти</a></div></div>";

    $("body").append(stringToAppend);

    if (success)
        $("#alert-custom").addClass("alert-custom-success");
    else
        $("#alert-custom").addClass("alert-custom-error");
    setTimeout(function () {
        $("#alert-custom").remove();
    }, 4000);
};
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
            content: "�������",
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

        if (self.Images !== null) {
            self.AddImages();
        }

        if (!self.IsNew) {
            self.SetMark();
        }

        var placesOptions = {
            placeholder: "Виберіть локацію",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "placesMultiselect"
        }

        MultiselectDropdown(placesOptions);


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
    OnPlacesChange: function () {
        var el = $("#placesMultiselect");
        console.log(el);
        console.log(el.val());
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

        $('#save').on('click', function () {
            self.Save();
        });

        $('#delete').on('click', function () {
            self.DeleteMark();
        });


        $('#removeImages').on('click', function () {
            self.RemoveImages();
        });

        $("#placesMultiselect").on('change', function () {
            self.OnPlacesChange();
        });

        //$('#dropdown-input-for-mark').addClass("display-8-custom");
        //$('#dropdown-input-for-cluster').addClass("display-8-custom");
        

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

        var saveMarkViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: areaId,
            ClusterId: clusterId,
            CuratorId: curatorId,
            Name: $("#Name").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            ResourceUrl: $("#ResourceUrl").val(),
            ResourceName: $("#ResourceName").val(),
            ResourceNameEng: $("#ResourceNameEng").val(),
            SelectedPlaceIds: $("#placesMultiselect").val(),
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
    AddImages: function () {
        //var self = this;

        //$.each(self.Images, function (index, element) {
        //    var rowCount = $("#imageContainer .row").length;
        //    var colCount = $("#imageContainer .col-md-3").length;

        //    $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        //    if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3)) {
        //        $("#imageContainer").append(self.RowAddConstString + self.ColAddConstString + "<img src=\"" + element + "\" height=\"200\">" + self.DivAddConstString + self.DivAddConstString);
        //    }
        //    else {
        //        $("#imageContainer .row").last().append(self.ColAddConstString + "<img src=\"" + element + "\" height=\"200\">" + self.DivAddConstString);
        //    }
        //});

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

var AddImageView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
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
        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        var content = $(imageToMove).attr('src');
        $.ajax({
            type: 'POST',
            url: "/Mark/ImageToInsert",
            data: JSON.stringify(content),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $("#imageContainer").append(src);
            }
        });

        $("#addImagePlaceholder").empty();
    },
});


var RevisionMarkView = Class.extend({
    InitializeControls: function () {
        var self = this;

       
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#back").on('click', () => {
            window.history.back();
        })
    },
    //ToFullScreen: function (data) {
    //    var self = this;
    //    $.ajax({
    //        type: 'POST',
    //        url: "/Mark/RevisionImage",
    //        data: JSON.stringify(data),
    //        contentType: 'application/json; charset=utf-8',
    //        success: function (html) {
    //            $("#imageFullScreenPlaceholder").html(html);
    //        }
    //    });
    //}
})
var CatalogueMarkView = Class.extend({
    Marks: null,
    OnEnglish: false,

    InitializeControls: function () {
        var self = this;
       

        var areasOptions = {
            placeholder: "Місця",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "areasMultiselect",
            //MaxElementsToShow: 2
        }

        var keyWordsAndMarksOptions = {
            placeholder: "Ключові слова",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "namesMultiselect",
            //MaxElementsToShow: 1
        }

        var placesOptions = {
            placeholder: "Локації",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "placesMultiselect",
            //MaxElementsToShow: 2
        }

        MultiselectDropdown(keyWordsAndMarksOptions);
        MultiselectDropdown(placesOptions);
        MultiselectDropdown(areasOptions);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#placesMultiselect").on('change', function () {
            self.OnPlacesChange();
        });

        $("#areasMultiselect").on('change', function () {
            self.OnAreasChange();
        });

        $("#namesMultiselect").on('change', function () {
            self.OnNamesChange();
        });

        $("#tableDetail").css("display", "none");
        $("#showMore").css("display", "none");

        $("#search").click(function () {
            $(".table-body-short").empty();
            $(".table-body-detail").empty();
            self.From = null;
            self.Search();
        });

        $("#clearFilters").click(function () {
            $('#keyWordsMultiselect option').attr('selected', ''),
            $("#AreaName").val(''); 
            $("#MarkName").val(''); 
            $("#MarkDescription").val('');
        });

        var firstElement = $(".data-row-container .data-row")[0];
        if (firstElement !== undefined)
            self.Show(firstElement);
    },
    Search: function () {
        var self = this;

        var names = $('#namesMultiselect').val() || [];



        var filters = {
            SelectedKeyWordIds: [],
            SelectedClusterIds: [],
            SelectedMarkIds: [],
            SelectedAreaIds: $("#areasMultiselect").val(),
            SelectedPlaceIds: $("#placesMultiselect").val(),
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
            url: "/Mark/GetMarks",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".data-row-container").html(data);
                var firstElement = $(".data-row-container .data-row")[0];
                if (firstElement !== undefined)
                    self.Show(firstElement);
            }
        });
    },
    Show: function (el) {
        $('.data-row').each(function (idex, element) {
            $(element).css('opacity', '0.3');
            $(element).removeClass("data-row-selected");
        });
        $(el).css('opacity', '1');
        $(el).addClass("data-row-selected");
        var fullId = $(el).find("input:first-child").attr('id')
        id = fullId.substring(fullId.length - 36);
        var index = fullId.substring(0, fullId.length - 36);
        var isCluster = $(el).find("input").eq(1).val() == 'True';
        

        $(".slideshow-container").empty();
        $.ajax({
            type: 'POST',
            url: "/Mark/GetImages?id=" + id + "&isCluster=" + isCluster + "&index=" + index,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                $(".slideshow-container").html(result);
                var containerTop = $(".data-row-container").offset().top;
                var rowTop = $(el).offset().top;
                var topToSet = rowTop - containerTop;
                $(".slideshow-container").css({ "margin-top": topToSet +'px' });
            }
        });
    },
    OnPlacesChange: function () {
        var self = this;
        self.Search();
    },
    OnAreasChange: function () {
        var self = this;
        self.Search();

    },
    OnNamesChange: function () {
        var self = this;
        self.Search();
    },
    
});
var DeleteMarkView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteMarkCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteMarkConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Mark/DeleteConfirm?id=" + $("#Id").val(),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    var currentUrl = window.location.href;
                    window.location.href = currentUrl.substring(0, currentUrl.indexOf('/', 8)) + '/Mark/Catalogue';
                    location.reload();
                }
                else
                    ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteMarkPlaceholder").empty();
    },
});


var RevisionImageView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $(".popup-custom-image-fullScreen").click(function () {
            $("#imageFullScreenPlaceholder").empty();
        });
    }
})
var CatalogueUserView = Class.extend({
    IsGlobalAdmin: null,

    TableRowStartConstString: "<div class=\"table-body-catalogue-user-row justify-content-center d-flex\"><div class=\"table-body-catalogue-user-column-number\">",
    TableRowFullNameConstString: "</div><div class=\"table-body-catalogue-user-column-name\">",
    TableRowLinkConstString: "<a class=\"table-body-catalogue-user-column-name-link\" href=\"/User/Revision?id=",
    TableRowEmailConstString: "</div><div class=\"table-body-catalogue-user-column-email\">",
    TableRowEndConstString: "</div></div>",
    
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
            $("#Name").val('');
            $("#Email").val('');
        });
    },
    Search: function () {
        var self = this;

        var filters = {
            Name: $("#Name").val(),
            Email: $("#Email").val()
        };

        $.ajax({
            type: 'POST',
            url: "/User/GetUsers",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".table-body-catalogue-user").empty();

                toAdd = "";
                $.each(data, function (index, element) {

                    toAdd += self.TableRowStartConstString + (index + 1);
                    toAdd += self.TableRowFullNameConstString
                    if (self.IsGlobalAdmin) {

                        toAdd += self.TableRowLinkConstString  + element.id + "\">" + element.fullName + "</a>";
                    }
                    else {
                        toAdd += element.fullName;
                    }
                    
                    toAdd += self.TableRowEmailConstString + element.email;
                    toAdd += self.TableRowEndConstString;
                });

                $(".table-body-catalogue-user").append(toAdd);
            }
        });
    }
})
var RevisionUserView = Class.extend({
    UserId: null,
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#saveUser").click(function () {
            self.Save();
        });

        $("#search").click(function () {
            self.Search();
        });

        $("#clearFilters").click(function () {
            $("#Name").val('');
            $("#Email").val('');
        });

    },
    Save: function () {
        var self = this;

        var saveUserViewModel = {
            Id: self.UserId,
            FirstName: $("#FirstName").val(),
            LastName: $("#LastName").val(),
            Email: $("#Email").val(),
            RoleIds: [],
        }
        $(".check-box-row").each(function (index, element) {
            if ($(element).is(":checked"))
            {
                saveUserViewModel.RoleIds.push($(element).val());
            }
        });

        $.ajax({
            type: 'POST',
            url: "/User/SaveUser",
            data: JSON.stringify(saveUserViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);
            }
        });
    }
})
var AddCuratorImageView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
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
        $("#imageContainer").empty();

        var element = $(".popup-content-custom .add-image-placeholder-custom").html();

        $("#imageContainer").append(element);

        $("#addImagePlaceholder").empty();
    },
});
var ClusterAddView = Class.extend({
    ToDelete: null,
    IsNew: null,

    Areas: null,
    AreaIds: null,
    AreaNames: null,
    SearchSelectDropdownAreas: null,

    Curators: null,
    CuratorIds: null,
    CuratorNames: null,
    SearchSelectDropdownCurators: null,


    Areas: null,
    AreaName: null,
    AreaId: null,

    Curators: null,
    CuratorName: null,
    CuratorId: null,

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

        var placesOptions = {
            placeholder: "Виберіть локацію",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "placesMultiselect",
            //MaxElementsToShow: 2
        }

        MultiselectDropdown(placesOptions);

        self.SearchSelectDropdownAreas = new SearchSelect('#dropdown-input-for-area', {
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
            self.DeleteCluster();
        }
    },
    SubscribeEvents: function () {
        var self = this;

        $("#setCoordinates").click(function () {
            $("#Lng").val(self.LastLng);
            $("#Lat").val(self.LastLat);
        });

        $('#saveCluster').on('click', function () {
            self.Save();
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

        let cluster = new google.maps.Marker({
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

        self.Map = new google.maps.Map(document.getElementById("mapClusterAdd"), {
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
        var self = this;

        var areaId = self.AreaIds[self.AreaNames.indexOf($("#dropdown-input-for-area").val())];
        var curatorId = self.CuratorIds[self.CuratorNames.indexOf($("#dropdown-input-for-curator").val())];
        areaId = areaId === "" ? null : areaId;
        curatorId = curatorId === "" ? null : curatorId;

        var saveClusterViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: areaId,
            CuratorId: curatorId,
            Name: $("#Name").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            ResourceUrl: $("#ResourceUrl").val(),
            ResourceName: $("#ResourceName").val(),
            ResourceNameEng: $("#ResourceNameEng").val(),
            SelectedPlaceIds: $("#placesMultiselect").val()
        };

        $.ajax({
            type: 'POST',
            url: "/Cluster/Save",
            data: JSON.stringify(saveClusterViewModel),
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
    DeleteCluster: function () {
        $.ajax({
            type: 'POST',
            url: "/Cluster/DeleteCluster",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteClusterPlaceholder').html(src);
            }
        });
    }
});

var RevisionClusterView = Class.extend({
    Lat: null,
    Lng: null,
    Marks: null,

    Map: null,
    OnEnglish: false,

    TableClusterMarksRowStartConstString: "<div class=\"table-body-revision-cluster-marks-row justify-content-center d-flex\"><div class=\"table-body-revision-cluster-marks-column-number flex-container-center-custom\">",
    TableClusterMarksRowNameConstString: "</div><div class=\"table-body-revision-cluster-marks-column-name flex-container-center-custom\"><a href=\"/Mark/Revision?id=",
    TableClusterMarksRowDescriptionConstString: "</a></div><div class=\"table-body-revision-cluster-marks-column-description flex-container-center-custom ukr-description\">",
    TableClusterMarksRowDescriptionEngConstString: "</div><div class=\"table-body-revision-cluster-marks-column-description flex-container-center-custom eng-description\">",
    TableClusterMarksRowLinkConstString: "</div><div class=\"table-body-revision-cluster-marks-column-photo flex-container-center-custom\"><a href=\"",
    TableClusterMarksRowPhotoConstString: "\"><img class=\"table-body-revision-cluster-marks-column-photo-item\" src=\"",
    TableClusterMarksRowEndConstString: "\"></a></div></div>",
    InitializeControls: function () {
        var self = this;

        self.InitTable();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#flagGB').on('click', function () {
            self.OnEnglish = true;
            $('#flagUA').removeClass("box-shadow-grey-custom");
            $("#flagGB").addClass("box-shadow-grey-custom");


            $(".ukr-description").addClass("display-none-custom");
            $(".eng-description").removeClass("display-none-custom");

        });

        $('#flagUA').on('click', function () {
            self.OnEnglish = false;
            $('#flagGB').removeClass("box-shadow-grey-custom");
            $("#flagUA").addClass("box-shadow-grey-custom");

            $(".eng-description").addClass("display-none-custom");
            $(".ukr-description").removeClass("display-none-custom");
        });
    },
    InitMap: function () {
        var self = this;

        self.Map = new google.maps.Map(document.getElementById("mapClusterRevision"), {
            center: {
                lat: parseFloat(self.Lat),
                lng: parseFloat(self.Lng)
            },
            zoom: 8,
            options: {
                gestureHandling: 'greedy'
            },
            disableDefaultUI: true
        });

        self.SetMark();
    },
    SetMark: function () {
        var self = this;

        let cluster = new google.maps.Marker({
            position: {
                lat: parseFloat(self.Lat),
                lng: parseFloat(self.Lng)
            },
            map: self.Map,
            title: self.AreaName,
            icon: {
                url: "../images/clusterIcon.png",
                scaledSize: new google.maps.Size(24, 24),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(12, 12)
            }
        });
    },
    InitTable: function () {
        var self = this;

        if (self.Marks.length === 0) {
            $("#revisionClusterTable").addClass("display-none-custom");
            return;
        }
        var toAdd = "";

        $.each(self.Marks, function (index, element) {

            toAdd += self.TableClusterMarksRowStartConstString + (index + 1);
            toAdd += self.TableClusterMarksRowNameConstString + element.id + "\">" + element.name;
            toAdd += self.TableClusterMarksRowDescriptionConstString + element.description;
            toAdd += self.TableClusterMarksRowDescriptionEngConstString + element.descriptionEng;

            toAdd += self.TableClusterMarksRowLinkConstString + element.resourceUrl;
            toAdd += self.TableClusterMarksRowPhotoConstString;
            if (element.image !== null)
                toAdd += element.image;
            toAdd += self.TableClusterMarksRowEndConstString;
        });
        $(".table-body-revision-cluster-marks").append(toAdd);
        
        if (self.OnEnglish)
            $(".ukr-description").addClass("display-none-custom");
        else
            $(".eng-description").addClass("display-none-custom");
    }
})
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
var DeleteClusterView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteClusterCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteClusterConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Cluster/DeleteConfirm?id=" + $("#Id").val() + "&withMarks=" + $("#IncludeMarks").is(":checked"),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteClusterPlaceholder").empty();
    }
});


var RevisionTextView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

       
    }
})
var CatalogueTextView = Class.extend({
    Curators: null,
    SearchSelectDropdown: null,

    InitializeControls: function () {
        var self = this;

        var textNamesOptions = {
            placeholder: "Виберіть назву",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "textNamesMultiselect"
        }

        var curatorsOptions = {
            placeholder: "Виберіть автора",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "curatorsMultiselect"
        }

        MultiselectDropdown(textNamesOptions);
        MultiselectDropdown(curatorsOptions);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        //$('#search').on('click', function () {
        //    self.Search();
        //});


        //$("#clearFilters").click(function () {
        //    //$("#KeyWord").val('');
        //    $("#TextName").val('');
        //    $("#TextSubject").val('');
        //    $('#curatorMultiselect option').attr('selected', 'selected');
        //});

        var firstElement = $(".text-search-data-table .text-data-row")[0];
        if (firstElement !== undefined)
            self.Show(firstElement);
        
    },
    Show: function (el) {
        $('.text-data-row').each(function (idex, element) {
            $(element).css('opacity', '0.3');
            $(element).removeClass("data-row-selected");
        });
        $(el).css('opacity', '1');
        $(el).addClass("data-row-selected");
        var fullId = $(el).find("input:first-child").attr('id')
        id = fullId.substring(fullId.length - 36);

        $("#textContentPlaceholder").empty();
        $.ajax({
            type: 'POST',
            url: "/Text/GetText?id=" + id ,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                $("#textContentPlaceholder").html(result);
            }
        });
    },
    Search: function () {
        $(".table-body-catalogue-text").empty();

        var self = this;
        var filters = {
            CuratorIds: $("#curatorMultiselect").val(),
            TextName: $("#TextName").val(),
            TextSubject: $("#TextSubject").val()
            //KeyWord: $("#KeyWord").val(),
        };

        $.ajax({
            type: 'POST',
            url: "/Text/GetTexts",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

                var toAdd = "";

                $.each(data, function (index, element) {
                    toAdd += self.TableTextStartConstString + (index + 1);
                    toAdd += self.TableTextNameConstString + element.id + "\">" + element.name;
                    toAdd += self.TableTextSubjectConstString + element.subject;
                    if (element.curator !== null) {
                        toAdd += self.TableTextCuratorConstString + element.curator.id + "\">" + element.curator.name;
                        toAdd += self.TableTextEndConstString;
                    }
                    else {
                        toAdd += "</div><div class=\"table-body-catalogue-text-column-curator\"></div></div>";
                    }
                });

                $(".table-body-catalogue-text").append(toAdd);
            }
        });
    }
})
var AddTextView = Class.extend({
    IsNew: null,
    ToDelete: false,
    CuratorName: null,
    CuratorId: null,

    Blocks: null,
    OldData: null,

    Curators: null,
    CuratorIds: null,
    CuratorNames: null,
    SearchSelectDropdownCurators: null,

    Editor: null,
    Data: null,

    InitializeControls: function () {
        var self = this;

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



        if (self.CuratorName !== '') {
            var selected = $($("#Curator .searchSelect--Result")[0]);
            selected.removeClass("#Curator searchSelect--Placeholder");
            selected.html(self.CuratorName);

            $.each($("#Curator .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.CuratorName) {
                    $(element).addClass("#Curator searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-curator").val(self.CuratorName);
        }

        self.OldData = {
            time: Date.now(),
            version: "2.26.1",
            blocks: []
        };
        if (self.Blocks !== null) {
            var fullLength = self.Blocks.paragraphs.length + self.Blocks.headers.length
                + self.Blocks.listes.length + self.Blocks.images.length + self.Blocks.raws.length;

            for (var i = 0; i < fullLength; i++) {
                var IsFind = false;
                $.each(self.Blocks.paragraphs, function (index, element) {
                    if (element.index === i) {
                        self.OldData.blocks.push({
                            data: {
                                text: element.text
                            },
                            id: element.id,
                            type: "paragraph"
                        });
                        IsFind = true;
                        return false;
                    }
                });

                if (!IsFind) {
                    $.each(self.Blocks.headers, function (index, element) {
                        if (element.index === i) {
                            self.OldData.blocks.push({
                                data: {
                                    text: element.text,
                                    level: element.level
                                },
                                id: element.id,
                                type: "header"
                            });
                            IsFind = true;
                            return false;
                        }
                    });
                }

                if (!IsFind) {
                    $.each(self.Blocks.listes, function (index, element) {
                        if (element.index === i) {
                            self.OldData.blocks.push({
                                data: {
                                    style: element.style,
                                    items: element.items
                                },
                                id: element.id,
                                type: "list"
                            });
                            IsFind = true;
                            return false;
                        }
                    });
                }

                if (!IsFind) {
                    $.each(self.Blocks.raws, function (index, element) {
                        if (element.index === i) {
                            self.OldData.blocks.push({
                                data: {
                                    html: element.text
                                },
                                id: element.id,
                                type: "raw"
                            });
                            IsFind = true;
                            return false;
                        }
                    });
                }

                if (!IsFind) {
                    $.each(self.Blocks.images, function (index, element) {
                        if (element.index === i) {
                            self.OldData.blocks.push({
                                data: {
                                    src: element.src,
                                    caption: element.caption
                                },
                                id: element.id,
                                type: "image"
                            });
                            IsFind = true;
                            return false;
                        }
                    });
                }

                if (!IsFind) {
                    $.each(self.Blocks.videos, function (index, element) {
                        if (element.index === i) {
                            self.OldData.blocks.push({
                                data: {
                                    src: element.src,
                                    caption: element.caption
                                },
                                id: element.id,
                                type: "video"
                            });
                            IsFind = true;
                            return false;
                        }
                    });
                }
            }
        }

        self.Editor = new EditorJS({
            holder: 'editorjs',
            data: self.OldData,
            tools: {
                header: {
                    class: Header,
                    inlineToolbar: true
                },
                list: {
                    class: List,
                    inlineToolbar: true
                },
                raw: RawTool,
                //checklist: {
                //    class: Checklist,
                //    inlineToolbar: true
                //},
                Color: {
                    class: window.ColorPlugin,
                    config: {
                        colorCollections: [
                            "#FF1300",
                            "#EC7878",
                            "#9C27B0",
                            "#673AB7",
                            "#3F51B5",
                            "#0070FF",
                            "#03A9F4",
                            "#00BCD4",
                            "#4CAF50",
                            "#8BC34A",
                            "#CDDC39",
                            "#FFF",
                            "#000000"
                        ],
                        defaultColor: "#FF1300",
                        type: "text"
                    }
                },
                Marker: {
                    class: window.ColorPlugin,
                    config: {
                        defaultColor: '#FFBF00',
                        type: 'marker',
                    }
                },

                image: {
                    class: SimpleImage
                },
                video: {
                    class: SimpleVideo
                }
            }
        });

        self.SubscribeEvents();

        if (self.ToDelete) {
            self.DeleteText();
        }
    },
    SubscribeEvents: function () {
        var self = this;

        $('#save').on('click', function () {
            self.Save();
        });

        $('#delete').on('click', function () {
            self.DeleteText();
        });

        $('#addPhoto').on('click', function () {
            self.AddImage();
        });

        $('#removePhotos').on('click', function () {
            self.RemoveImages();
        });

        $('#addVideo').on('click', function () {
            self.AddVideo();
        });

        $('#removeVideos').on('click', function () {
            self.RemoveVideos();
        });

        $('#dropdown-input-for-curator').addClass("display-8-custom");
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

    Save: function () {
        var self = this;

        self.Editor.save().then((output) => {
            self.Data = output;

            var curatorId = self.CuratorIds[self.CuratorNames.indexOf($("#dropdown-input-for-curator").val())];
            curatorId = curatorId === "" ? null : curatorId;

            var saveTextViewModel = {
                Id: $("#Id").val() === "" ? null : $("#Id").val(),
                CuratorId: curatorId,
                Name: $("#Name").val(),
                Subject: $("#Subject").val(),
                Blocks: {
                    Headers: [],
                    Images: [],
                    CheckBoxes: [],
                    Listes: [],
                    Paragraphs: [],
                    Raws: [],
                    Videos: []
                }
            };

            $.each(self.Data.blocks, function (index, element) {
                if (element.type === "paragraph") {
                    saveTextViewModel.Blocks.Paragraphs.push({
                        Id: element.id,
                        Text: element.data.text,
                        Index: index
                    });
                }
                else if (element.type === "header") {
                    saveTextViewModel.Blocks.Headers.push({
                        Id: element.id,
                        Text: element.data.text,
                        Level: element.data.level,
                        Index: index
                    });
                }
                else if (element.type === "image") {
                    saveTextViewModel.Blocks.Images.push({
                        Id: element.id,
                        Src: element.data.src,
                        Caption: element.data.caption,
                        Index: index
                    });
                }
                else if (element.type === "video") {
                    saveTextViewModel.Blocks.Videos.push({
                        Id: element.id,
                        Src: element.data.src,
                        Caption: element.data.caption,
                        Index: index
                    });
                }
                //else if (element.type === "checklist") {
                //    saveTextViewModel.Blocks.CheckBoxes.push({
                //        Id: element.id,
                //        Items: element.data.items,
                //        Index: index
                //    });
                //}
                else if (element.type === "list") {
                    saveTextViewModel.Blocks.Listes.push({
                        Id: element.id,
                        Items: element.data.items,
                        Style: element.data.style,
                        Index: index
                    });
                }
                else if (element.type === "raw") {
                    saveTextViewModel.Blocks.Raws.push({
                        Id: element.id,
                        Text: element.data.html,
                        Index: index
                    });
                }
            });

            $.ajax({
                type: 'POST',
                url: "/Text/SaveText",
                data: JSON.stringify(saveTextViewModel),
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    ResultPopUp(result.success, result.text, result.url, result.id);
                }
            });
        }).catch((error) => {
            
        });


        

    },

    DeleteText: function () {
        if ($("#Id").val() === "")
            return;
        $.ajax({
            type: 'POST',
            url: "/Text/DeleteText",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteTextPlaceholder').html(src);
            }
        });
    },

    AddImage: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Text/AddImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImageTextPlaceholder').html(src);


            }
        });
    },

    CopyTextToClipboard: function (text) {
        if (!navigator.clipboard) {
            //fallbackCopyTextToClipboard(text);
            return;
        }
        navigator.clipboard.writeText(text);
    },
    RemoveImages: function () {
        $("#imageTextContainer").empty();
    },

    AddVideo: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Text/AddVideo",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addVideoTextPlaceholder').html(src);
            }
        });
    },

    RemoveVideos: function () {
        $("#videoTextContainer").empty();
    },

    CopyTimeId: function (el) {
        var time = $(el).parent().find("input").val();
        var self = this;
        self.CopyTextToClipboard(time);
    }

});

class SimpleImage {
    static get toolbox() {
        return {
            title: 'Image',
            icon: '<svg width="17" height="15" viewBox="0 0 336 276" xmlns="http://www.w3.org/2000/svg"><path d="M291 150V79c0-19-15-34-34-34H79c-19 0-34 15-34 34v42l67-44 81 72 56-29 42 30zm0 52l-43-30-56 30-81-67-66 39v23c0 19 15 34 34 34h178c17 0 31-13 34-29zM79 0h178c44 0 79 35 79 79v118c0 44-35 79-79 79H79c-44 0-79-35-79-79V79C0 35 35 0 79 0z"/></svg>'
        };
    }

    constructor({ data }) {
        this.data = data;
        this.wrapper = undefined;
    }

    render() {
        this.wrapper = document.createElement('div');
        this.wrapper.classList.add('simple-image');

        if (this.data && this.data.src) {
            this._createImage(this.data.src, this.data.caption);
            return this.wrapper;
        }
        const input = document.createElement('input');

        this.wrapper.classList.add('simple-image');
        this.wrapper.appendChild(input);

        input.placeholder = 'Insert copied photo';
        input.value = this.data && this.data.src ? this.data.src : '';

        input.addEventListener('paste', (event) => {
            var time = event.clipboardData.getData('text');
            var input = $('input').filter(function () {
                return $(this).val() === time;
            });

            if (input.length) {
                var src = input.parent().parent().find('.text-add-image').attr("src");
                if (!src || !src.length || !src.includes("data") || !src.includes("base64"))
                    return;
                this._createImage(src);
            } 
        });

        return this.wrapper;
    }

    _createImage(src, captionText) {
        const image = document.createElement('img');
        const caption = document.createElement('div');

        image.src = src;
        caption.contentEditable = true;
        caption.innerHTML = captionText || '';

        this.wrapper.innerHTML = '';
        this.wrapper.appendChild(image);
        this.wrapper.appendChild(caption);
    }


    validate(savedData) {
        if (!savedData.src.trim()) {
            return false;
        }

        return true;
    }

    save(blockContent) {
        const image = blockContent.querySelector('img');
        const caption = blockContent.querySelector('[contenteditable]');

        return {
            src: image.src,
            caption: caption.innerHTML || ''
        }
    }
}

class SimpleVideo {
    static get toolbox() {
        return {
            title: 'Video',
            icon: '<img src="/images/icons/video.png" class="editor-video-icon" alt="" />'
        };
    }
    constructor({ data }) {
        this.data = data;
        this.wrapper = undefined;
    }
    render() {
        this.wrapper = document.createElement('div');
        this.wrapper.classList.add('simple-video');

        if (this.data && this.data.src) {
            this._createVideo(this.data.src, this.data.caption);
            return this.wrapper;
        }
        const input = document.createElement('input');

        this.wrapper.classList.add('simple-image');
        this.wrapper.appendChild(input);

        input.placeholder = 'Insert copied video';
        input.value = this.data && this.data.src ? this.data.src : '';

        input.addEventListener('paste', (event) => {
            var time = event.clipboardData.getData('text');
            var input = $('input').filter(function () {
                return $(this).val() === time;
            });

            if (input.length) {
                var src = input.parent().parent().find('.text-add-video').attr("src");
                if (!src || !src.length || !src.includes("data") || !src.includes("base64"))
                    return;
                this._createVideo(src);
            }
        });

        return this.wrapper;
    }
    _createVideo(src, captionText) {
        const video = document.createElement('video');
        video.muted = true;
        video.controls = true;
        const caption = document.createElement('div');

        video.src = src;
        caption.contentEditable = true;
        caption.innerHTML = captionText || '';

        this.wrapper.innerHTML = '';
        this.wrapper.appendChild(video);
        this.wrapper.appendChild(caption);
    }


    validate(savedData) {
        if (!savedData.src.trim()) {
            return false;
        }

        return true;
    }

    save(blockContent) {
        const video = blockContent.querySelector('video');
        const caption = blockContent.querySelector('[contenteditable]');

        return {
            src: video.src,
            caption: caption.innerHTML || ''
        }
    }
}
var AddImageTextView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
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
        $("#addImageTextPlaceholder").empty();
    },
    Save: function () {
        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        var content = $(imageToMove).attr('src');
        $.ajax({
            type: 'POST',
            url: "/Text/ImageToInsert",
            data: JSON.stringify(content),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $("#imageTextContainer").append(src);
            }
        });

        $("#addImageTextPlaceholder").empty();
    },
});


var AddVideoTextView = Class.extend({
    Video: null,
    VideoInput: null,
    Reader: null,
    InitializeControls: function () {
        var self = this;
        self.VideoInput = $('#videoInput');
        self.Reader = new FileReader();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        self.Reader.addEventListener("load", () => {
            $("#videoPlaceholder")[0].src = self.Reader.result;
        }, false);

        self.VideoInput.on('change', function () {
            self.Reader.readAsDataURL(self.VideoInput[0].files[0]);
        });

        $('#addVideoCancelButton').on('click', function () {
            self.Close();
        });
        $('#addVideoConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Close: function () {
        $("#addVideoTextPlaceholder").empty();
    },
    Save: function () {
        var videoToMove = $(".popup-content-custom .add-video-placeholder-custom").html();

        var content = $(videoToMove).attr('src');
        $.ajax({
            type: 'POST',
            url: "/Text/VideoToInsert",
            data: JSON.stringify(content),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $("#videoTextContainer").append(src);
            }
        });

        $("#addVideoTextPlaceholder").empty();
    },
});


var DeleteTextView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteTextCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteTextConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Text/DeleteConfirm?id=" + $("#Id").val(),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteTextPlaceholder").empty();
    },
});


var RevisionCuratorView = Class.extend({
    Texts: null,

    TableCuratorTextsRowStartConstString: "<div class=\"table-body-curator-texts-row justify-content-center d-flex\"><div class=\"table-body-curator-texts-column-number flex-container-center-custom\">",
    TableCuratorTextsRowNameConstString: "</div><div class=\"table-body-curator-texts-column-name flex-container-center-custom\"><a href=\"/Text/Revision?id=",
    TableCuratorTextsRowSubjectConstString: "</a></div><div class=\"table-body-curator-texts-column-subject flex-container-center-custom\">",
    TableCuratorTextsRowEndConstString: "</div></div>",

    InitializeControls: function () {
        var self = this;

        self.InitTable();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
    },
    InitTable: function () {
        var self = this;

        if (self.Texts.length === 0) {
            $("#revisionCuratorTable").addClass("display-none-custom");
            return;
        }
        var toAdd = "";

        $.each(self.Texts, function (index, element) {

            toAdd += self.TableCuratorTextsRowStartConstString + (index + 1);
            toAdd += self.TableCuratorTextsRowNameConstString + element.id + "\">" + element.name;
            toAdd += self.TableCuratorTextsRowSubjectConstString + element.subject;
            
            toAdd += self.TableCuratorTextsRowEndConstString;
        });

        $(".table-body-curator-texts").append(toAdd);
    }
})
var CatalogueCuratorView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        //$(".curator-box").click(function () {
        //    window.location.href = window.location.origin + "/Curator/Revision?id=" + $(this).attr("value");
        //});
    },
    ShowDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        if (descriptionContainer.css('display') === 'flex') {
            descriptionContainer.css('display', 'none');
        } else {
            descriptionContainer.css('display', 'flex');
        }
    },
    HideDescription: function (el) {
        var container = $(el).parent().parent().parent();
        var descriptionContainer = container.find(".curator-content-detailed");
        descriptionContainer.css({ display: 'none' });
    }
})
var CuratorAddView = Class.extend({
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#addImage').on('click', function () {
            self.AddImage();
        });

        $('#saveCurator').on('click', function () {
            self.Save();
        });
        $('#delete').on('click', function () {
            self.Delete();
        });
    },
    Delete: function () {
        if ($("#Id").val() === "")
            return;
        $.ajax({
            type: 'POST',
            url: "/Curator/DeleteCurator",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#deleteCuratorPlaceholder').html(src);
            }
        });
    },

    AddImage: function () {
        $.ajax({
            type: 'POST',
            url: "/Curator/AddImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImagePlaceholder').html(src);
            }
        });
    },
    Save: function () {
        var self = this;

        var saveCuratorViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            DisplayName: $("#DisplayName").val(),
            Description: $("#Description").val(),
            DescriptionEng: $("#DescriptionEng").val(),
            Image: $("#curatorImage").attr('src')
        };

        

        $.ajax({
            type: 'POST',
            url: "/Curator/SaveCurator",
            data: JSON.stringify(saveCuratorViewModel),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    window.location.href = 'https://' + window.location.host + '/Curator/Catalogue';
                }
                else {
                    ResultPopUp(result.success, result.text, result.url, result.id);
                }
            }
        });

    }
});
var AddImageCuratorView = Class.extend({
    Image: null,
    ImageInput: null,
    Reader: null,
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
        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        var src = $(imageToMove).attr('src');


        $("#curatorImagePlaceholder").empty();
        $("#curatorImagePlaceholder").html("<img id=\"curatorImage\" src=\"" + src +"\" />")

        $("#addImagePlaceholder").empty();
    },
});


var DeleteCuratorView = Class.extend({
    
    InitializeControls: function () {
        var self = this;
        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $('#deleteCuratorCancelButton').on('click', function () {
            self.Close();
        });
        $('#deleteCuratorConfirmButton').on('click', function () {
            self.Save();
        });
    },
    Save: function () {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Curator/DeleteConfirm?id=" + $("#Id").val(),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                ResultPopUp(result.success, result.text, result.url, result.id);

                self.Close();
            }
        });
    },
    Close: function () {
        $("#deleteCuratorPlaceholder").empty();
    },
});


var KeyWordView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#addKeyWord").on('click', function () {
            self.Add();
        });

        $("#search").on('click', function () {
            self.Search();
        });

        $("#save").on('click', function () {
            self.Save();
        });

        $("#clear").on('click', function () {
            self.Clear();
        });
    },
    Add: function () {
        var text = $("#keyWordToAdd").val();
        if (text.lenght === 0)
            return;

        $("#keyWordContainer").prepend("<div class=\"keyWord-row pb-2\"><input type=\"hidden\" value=\"\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            text + "\"><button class=\"btn btn-dark-custom\" onclick=\"keyWordView.Remove(this)\">Видалити</button></div>");
        $("#keyWordToAdd").val("");
    },
    Search: function () {
        var filter = $("#filter").val().toLowerCase();

        $('#keyWordContainer .keyWord-row').each(function (index, element) {
            var keyWord = $(element).find(".text-box-custom").val().toLowerCase();

            $(element).css({ display: 'flex' });
            if (keyWord.indexOf(filter) === -1)
                $(element).css({ display: 'none' });
        });

    },
    Save: function () {
        var self = this;
        var dataArray = [];
        $('.keyWord-row').each(function () {
            var id = $(this).find('input[type="hidden"]').eq(0).val();

            if (id === undefined || id === null)
                id = "";
            var name = $(this).find('input[type="text"]').eq(0).val();
            var obj = {
                Id: id,
                Name: name
            };
            dataArray.push(obj);
        });

        $.ajax({
            type: 'POST',
            url: "/AdminHelper/SaveKeyWords",
            data: JSON.stringify(dataArray),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                location.reload();
            }
        });
    },
    Remove: function (el) {
        var row = $(el).parent();
        row.remove();
    },
    Clear: function () {
        $('#keyWordContainer .keyWord-row').each(function (index, element) {
            $(element).css({ display: 'flex' });
        });

        $("#filter").val("");
    }
});


var PlaceView = Class.extend({
    InitializeControls: function () {
        var self = this;


        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#addPlace").on('click', function () {
            self.Add();
        });

        $("#search").on('click', function () {
            self.Search();
        });

        $("#save").on('click', function () {
            self.Save();
        });

        $("#clear").on('click', function () {
            self.Clear();
        });
    },
    Add: function () {
        var text = $("#placeToAdd").val();
        var textEng = $("#placeToAddEng").val();
        if (text.lenght === 0)
            return;

        $("#placeContainer").prepend("<div class=\"place-row pb-2\"><input type=\"hidden\" value=\"\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            text + "\"><input type=\"text\" class=\"text-box-custom form-control\" value=\"" +
            textEng + "\"><button class=\"btn btn-dark-custom\" onclick=\"placeView.Remove(this)\">Видалити</button></div>");
        $("#placeToAdd").val("");
        $("#placeToAddEng").val("");
    },
    Search: function () {
        var filter = $("#filter").val().toLowerCase();

        $('#placeContainer .place-row').each(function (index, element) {
            var place = $(element).find(".text-box-custom").eq(0).val().toLowerCase();
            var placeEng = $(element).find(".text-box-custom").eq(1).val().toLowerCase();

            $(element).css({ display: 'flex' });
            if (place.indexOf(filter) === -1 && placeEng.indexOf(filter) === -1)
                $(element).css({ display: 'none' });
        });

    },
    Save: function () {
        var self = this;
        var dataArray = [];
        $('.place-row').each(function () {
            var id = $(this).find('input[type="hidden"]').eq(0).val();

            if (id === undefined || id === null)
                id = "";
            var name = $(this).find('input[type="text"]').eq(0).val();
            var nameEng = $(this).find('input[type="text"]').eq(1).val();
            var obj = {
                Id: id,
                Name: name,
                NameEng: nameEng
            };
            dataArray.push(obj);
            console.log(dataArray);
        });

        $.ajax({
            type: 'POST',
            url: "/AdminHelper/SavePlaces",
            data: JSON.stringify(dataArray),
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                location.reload();
            }
        });
    },
    Remove: function (el) {
        var row = $(el).parent();
        row.remove();
    },
    Clear: function () {
        $('#placeContainer .place-row').each(function (index, element) {
            $(element).css({ display: 'flex' });
        });

        $("#filter").val("");
    }
});

