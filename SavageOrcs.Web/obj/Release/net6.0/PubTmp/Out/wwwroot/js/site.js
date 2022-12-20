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

var MarkAddView = Class.extend({
    IsNew: null,
    Images: null,
    AreaName: null,
    ClusterName: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    AreaId: null,
    IsApproximate: null,

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
            content: "�����, ��� �������� ����������",
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

        $('#dropdown-input-for-mark').addClass("display-8-custom");
        $('#dropdown-input-for-cluster').addClass("display-8-custom");
        

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
        var self = this;

        var areaId = self.AreaIds[self.AreaNames.indexOf($("#dropdown-input-for-mark").val())];
        var clusterId = self.ClusterIds[self.ClusterNames.indexOf($("#dropdown-input-for-cluster").val())];
        areaId = areaId === "" ? null : areaId;
        clusterId = clusterId === "" ? null : clusterId;

        var saveMarkViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: areaId,
            ClusterId: clusterId,
            IsApproximate: $("#IsApproximate").is(":checked"),
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
        var self = this;

        var rowCount = $("#imageContainer .row").length;
        var colCount = $("#imageContainer .col-md-3").length;

        $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3 )) {
            $("#imageContainer").append(markAddView.RowAddConstString + markAddView.ColAddConstString + imageToMove + markAddView.DivAddConstString + markAddView.DivAddConstString);
        }
        else {
            $("#imageContainer .row").last().append(markAddView.ColAddConstString + imageToMove + markAddView.DivAddConstString);
        }
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

        $(".image-revision-mark-custom").click(function () {
            self.ToFullScreen($(this).attr("src"));
        });

        $('#flagGB').on('click', function () {
            $('#flagUA').removeClass("box-shadow-grey-custom");
            $("#flagGB").addClass("box-shadow-grey-custom");

            $("#textGB").removeClass("display-none-custom");
            $("#textUA").addClass("display-none-custom");
           
        });

        $('#flagUA').on('click', function () {
            $('#flagGB').removeClass("box-shadow-grey-custom");
            $("#flagUA").addClass("box-shadow-grey-custom");

            $("#textUA").removeClass("display-none-custom");
            $("#textGB").addClass("display-none-custom");
        });
    },
    ToFullScreen: function (data) {
        var self = this;
        $.ajax({
            type: 'POST',
            url: "/Mark/RevisionImage",
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (html) {
                $("#imageFullScreenPlaceholder").html(html);
            }
        });
    }
})
var CatalogueMarkView = Class.extend({
    Marks: null,
    DetailedView: false,
    From: null,
    CountConst: 4,
    WereDataInDetailedTable: null,
    WereDataInShortTable: null,
    OnEnglish: false,
    
    TableShortRowStartConstString: "<div class=\"table-body-short-row justify-content-center d-flex\"><div class=\"table-body-short-column-number\">",
    TableShortRowNameConstString: "</div><div class=\"table-body-short-column-name\"><a href=\"/Mark/Revision?id=",
    TableShortRowDescriptionConstString: "</a></div><div class=\"table-body-short-column-description ukr-description\">",
    TableShortRowDescriptionEngConstString: "</div><div class=\"table-body-short-column-description eng-description\">",
    TableShortRowAreaConstString: "</div><div class=\"table-body-short-column-area\">",
    TableShortRowAreaButtonConstString: "<button class=\"button-short-column-area\" value=\"",
    TableShortRowEndConstString: "</div></div>",

    TableDetailRowStartConstString: "<div class=\"table-body-detail-row justify-content-center d-flex\"><div class=\"table-body-detail-column-number flex-container-center-custom\">",
    TableDetailRowNameConstString: "</div><div class=\"table-body-detail-column-name flex-container-center-custom\"><a href=\"/Mark/Revision?id=",
    TableDetailRowDescriptionConstString: "</a></div><div class=\"table-body-detail-column-description flex-container-center-custom ukr-description\">",
    TableDetailRowDescriptionEngConstString: "</div><div class=\"table-body-detail-column-description flex-container-center-custom eng-description\">",
    TableDetailRowAreaConstString: "</div><div class=\"table-body-detail-column-area flex-container-center-custom\">",
    TableDetailRowAreaButtonConstString: "<button class=\"button-detail-column-area\" value=\"",
    TableDetailRowLinkConstString: "</div><div class=\"table-body-detail-column-photo flex-container-center-custom\"><a href=\"",
    TableDetailRowPhotoConstString: "\"><img class=\"table-body-detail-column-photo-item\" src=\"",
    TableDetailRowEndConstString: "\"></a></div></div>",
    InitializeControls: function () {
        var self = this;
       

        var options = {
            placeholder: "Виберіть ключове слово",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "keyWordsMultiselect"
        }

        MultiselectDropdown(options);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

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

        $("#showMore").click(function () {
            self.Search();
        });

        $("#fullData").click(function () {
            if (self.DetailedView) {
                self.DetailedView = false;

                $("#tableDetail").css("display", "none");
                $("#tableShort").css({ 'display': '' });
                $(".table-body-detail").empty();

                if (self.WereDataInDetailedTable == true) {
                    self.Search();
                }

                $("#fullData").text("Ввімкнути детальний перегляд");
                $("#showMore").css("display", "none");
            }
            else {
                self.DetailedView = true;

                $("#tableShort").css("display", "none");
                $("#tableDetail").css({ 'display': ''});
                $(".table-body-short").empty();

                if (self.WereDataInShortTable == true) {
                    self.Search();
                }

                $("#fullData").text("Ввімкнути спрощений перегляд");
                $("#showMore").css({ 'display': '' });
            }
            //if (!$("#table").is(':empty')) {
            //    self.Search();
            //}
        });

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
    Search: function () {
        var self = this;
        
        var filters = {
            KeyWordIds: $("#keyWordsMultiselect").val(),
            AreaName: $("#AreaName").val(),
            MarkName: $("#MarkName").val(),
            MarkDescription: $("#MarkDescription").val(),
            NotIncludeCluster: $("#NotIncludeCluster").is(":checked"),
            FullData: self.DetailedView,
            From: self.From,
            Count: self.CountConst
        };

        $.ajax({
            type: 'POST',
            url: "/Mark/GetMarks",
            data: JSON.stringify(filters),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (self.DetailedView) {
                    self.WereDataInShortTable = false;
                    self.WereDataInDetailedTable = true;
                    
                    var toAdd = "";

                    $.each(data, function (index, element) {

                        toAdd += self.TableDetailRowStartConstString + (self.From + index + 1);
                        toAdd += self.TableDetailRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableDetailRowDescriptionConstString + element.description;
                        toAdd += self.TableDetailRowDescriptionEngConstString + element.descriptionEng;
                        
                        if (element.area !== null) {
                            if (element.area.id !== null) {
                                toAdd += self.TableDetailRowAreaConstString + self.TableDetailRowAreaButtonConstString;
                                toAdd += element.area.id + "\">" + element.area.name + "</button>";
                            }
                        }
                        else {
                            toAdd += self.TableDetailRowAreaConstString;
                        }

                        toAdd += self.TableDetailRowLinkConstString + element.resourceUrl;
                        toAdd += self.TableDetailRowPhotoConstString + element.images[0]; //NOT COMPLETED
                        toAdd += self.TableDetailRowEndConstString;
                    });

                    self.From += self.CountConst;

                    $(".table-body-detail").append(toAdd);

                    $(".button-detail-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#AreaName").val(this.innerText);
                        self.Search();
                    });

                    if (self.OnEnglish)
                        $(".ukr-description").addClass("display-none-custom");
                    else
                        $(".eng-description").addClass("display-none-custom");
                }
                else {
                    self.WereDataInShortTable = true;
                    self.WereDataInDetailedTable = false;

                    self.From = 0;

                    var toAdd = "";

                    $.each(data, function (index, element) {
                        toAdd += self.TableShortRowStartConstString + (index + 1);
                        toAdd += self.TableShortRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableShortRowDescriptionConstString + element.description;
                        toAdd += self.TableShortRowDescriptionEngConstString + element.descriptionEng;
                        

                        if (element.area !== null) {
                            if (element.area.id !== null) {
                                toAdd += self.TableShortRowAreaConstString + self.TableShortRowAreaButtonConstString;
                                toAdd += element.area.id + "\">" + element.area.name + "</button>";
                            }
                        }
                        else {
                            toAdd += self.TableShortRowAreaConstString;
                        }
                        toAdd += self.TableShortRowEndConstString;
                    });

                    $(".table-body-short").append(toAdd);

                    $(".button-short-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#AreaName").val(this.innerText);
                        self.Search();
                    });

                    if (self.OnEnglish)
                        $(".ukr-description").addClass("display-none-custom");
                    else
                        $(".eng-description").addClass("display-none-custom");
                }
            }
        });
    }
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
    TableRowCuratorStatusConstString: "</div><div class=\"table-body-catalogue-user-column-curator-status\">",
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
                    toAdd += self.TableRowCuratorStatusConstString;
                    if (element.isCurator) {
                        toAdd += "Так";
                    }
                    else {
                        toAdd += "Ні";
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
    CuratorId: null,
    IsCurator: null,
    InitializeControls: function () {
        var self = this;

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $("#saveMark").click(function () {
            self.Save();
        });

        $("#addCuratorImage").click(function () {
            self.AddCuratorImage();
        });

        $("#IsCurator").change(function () {
            if (this.checked) {
                $(".curatorRow").css({ 'visibility': 'visible' });
            }
            else {
                $(".curatorRow").css({ 'visibility': 'hidden' });
            }
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
            IsCurator: $("#IsCurator").is(":checked"),
            RoleIds: [],
            CuratorId: $("#CuratorInfo_Id").val(),
            DisplayName: $("#CuratorInfo_DisplayName").val(),
            Description: $("#CuratorInfo_Description").val(),
            Image: $("#imagePlaceholder").attr('src')
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
    },
    AddCuratorImage: function () {
        $.ajax({
            type: 'POST',
            url: "/User/AddCuratorImage",
            contentType: 'application/json; charset=utf-8',
            success: function (src) {
                $('#addImagePlaceholder').html(src);
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
    //Images: null,
    Areas: null,
    AreaName: null,
    Lat: null,
    Lng: null,
    Zoom: null,
    AreaId: null,
    //IsApproximate: null,

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

    InitializeControls: function () {
        var self = this;
        const myLatlng = { lat: parseFloat(self.Lat), lng: parseFloat(self.Lng) };

        this.InfoWindow = new google.maps.InfoWindow({
            content: "�����, ��� �������� ����������",
            position: myLatlng,
        });

        self.IsInitializate = true;
        //self.InfoWindow.open(self.Map);

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
                if ($(element).text() === self.AreaName) {
                    $(element).addClass("searchSelect--Option--selected")
                }
            });

            $("#dropdown-input").val(self.AreaName);
            
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

        var areaId = self.AreaIds[self.AreaNames.indexOf($("#dropdown-input").val())];
        areaId = areaId === "" ? null : areaId;

        var saveClusterViewModel = {
            Id: $("#Id").val() === "" ? null : $("#Id").val(),
            Lng: $("#Lng").val(),
            Lat: $("#Lat").val(),
            AreaId: areaId,
            Name: $("#Name").val(),
            Description: $("#Description").val(),
        };

        $.ajax({
            type: 'POST',
            url: "/Cluster/Save",
            data: JSON.stringify(saveClusterViewModel),
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

    TableTextStartConstString: "<div class=\"table-body-catalogue-text-row justify-content-center d-flex\"><div class=\"table-body-catalogue-text-column-number\">",
    TableTextNameConstString: "</div><div class=\"table-body-catalogue-text-column-name\"><a href=\"/Text/Revision?id=",
    TableTextSubjectConstString: "</a></div><div class=\"table-body-catalogue-text-column-subject\">",
    TableTextCuratorConstString: "</div><div class=\"table-body-catalogue-text-column-curator\"><a href=\"/Curator/Revision?id=",
    TableTextEndConstString: "</a></div></div>",

    InitializeControls: function () {
        var self = this;

        var options = {
            placeholder: "Виберіть куратора",
            txtSelected: "вибрано",
            txtAll: "Всі",
            txtRemove: "Видалити",
            txtSearch: "Пошук",
            height: "300px",
            Id: "curatorMultiselect"
        }

        MultiselectDropdown(options);

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;

        $('#search').on('click', function () {
            self.Search();
        });


        $("#clearFilters").click(function () {
            //$("#KeyWord").val('');
            $("#TextName").val('');
            $("#TextSubject").val('');
            $('#curatorMultiselect option').attr('selected', 'selected');
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

    RowAddConstString: "<div class=\"row justify-content-md-around\">",
    ColAddConstString: "<div class=\"col-md-3\">",
    DivAddConstString: "</div>",

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

        $('#saveText').on('click', function () {
            self.Save();
        });

        $('#addPhoto').on('click', function () {
            self.AddImage();
        });

        $('#removePhotos').on('click', function () {
            self.RemoveImages();
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
                    Raws: []
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
            debugger;
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
    AfterAddingImage: function () {
        var self = this;

        $("img").click(function () {
            self.CopyTextToClipboard($(this).attr("src"));
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
        $("#imageTextContainer .row").empty();
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
            this._createImage(event.clipboardData.getData('text'));
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
        var self = this;

        var rowCount = $("#imageTextContainer .row").length;
        var colCount = $("#imageTextContainer .col-md-3").length;

        $(".popup-content-custom .row #imagePlaceholder").removeAttr('id');

        var imageToMove = $(".popup-content-custom .add-image-placeholder-custom").html();

        if ((rowCount === 0) || (colCount !== 0 && Math.floor(colCount / rowCount) === 3 )) {
            $("#imageTextContainer").append(addTextView.RowAddConstString + addTextView.ColAddConstString + imageToMove + addTextView.DivAddConstString + addTextView.DivAddConstString);
        }
        else {
            $("#imageTextContainer .row").last().append(addTextView.ColAddConstString + imageToMove + addTextView.DivAddConstString);
        }

        addTextView.AfterAddingImage();
        $("#addImageTextPlaceholder").empty();
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

        $(".curator-box").click(function () {
            window.location.href = window.location.origin + "/Curator/Revision?id=" + $(this).attr("value");
        });
    }
})