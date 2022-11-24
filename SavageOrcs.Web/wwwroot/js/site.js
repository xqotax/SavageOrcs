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
    debugger;
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
    MapMarks: null,
    MapId: null,
    MapName: null,
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
        $.each(self.Marks, function (index, element) {

            let marker = new google.maps.Marker({
                position: {
                    lat: parseFloat(element.lat),
                    lng: parseFloat(element.lng)
                },
                map: map,
                title: element.name,
                icon: {
                    url: "images/markIcon.png",
                    scaledSize: new google.maps.Size(25, 25),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(0, 0)
                }
            });

            marker.addListener("click", () => {
                self.MarkOnClick(marker, element);
            });

            self.MapMarks.push({ id: element.id, marker: marker });
        });
        
    },
    MarkOnClick: function (marker, element) {
        var self = this;
        self.Map.setZoom(12);
        self.Map.setCenter(marker.getPosition());

        self.InfoWindow.close();

        self.InfoWindow = new google.maps.InfoWindow({
            position: marker.getPosition(),
        });

        var link = "<a href=\"";
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
    }
})
var CatalogueMarkView = Class.extend({
    Marks: null,
    DetailedView: false,
    From: null,
    CountConst: 4,
    WereDataInDetailedTable: null,
    WereDataInShortTable: null,
    
    TableShortRowStartConstString: "<div class=\"table-body-short-row justify-content-center d-flex\"><div class=\"table-body-short-column-number\">",
    TableShortRowNameConstString: "</div><div class=\"table-body-short-column-name\"><a href=\"/Mark/Revision?id=",
    TableShortRowDescriptionConstString: "</a></div><div class=\"table-body-short-column-description\">",
    TableShortRowAreaConstString: "</div><div class=\"table-body-short-column-area\">",
    TableShortRowAreaButtonConstString: "<button class=\"button-short-column-area\" value=\"",
    TableShortRowEndConstString: "</div></div>",

    TableDetailRowStartConstString: "<div class=\"table-body-detail-row justify-content-center d-flex\"><div class=\"table-body-detail-column-number flex-container-center-custom\">",
    TableDetailRowNameConstString: "</div><div class=\"table-body-detail-column-name flex-container-center-custom\"><a href=\"/Mark/Revision?id=",
    TableDetailRowDescriptionConstString: "</a></div><div class=\"table-body-detail-column-description flex-container-center-custom\">",
    TableDetailRowAreaConstString: "</div><div class=\"table-body-detail-column-area flex-container-center-custom\">",
    TableDetailRowAreaButtonConstString: "<button class=\"button-detail-column-area\" value=\"",
    TableDetailRowLinkConstString: "</div><div class=\"table-body-detail-column-photo flex-container-center-custom\"><a href=\"",
    TableDetailRowPhotoConstString: "\"><img class=\"table-body-detail-column-photo-item\" src=\"",
    TableDetailRowEndConstString: "\"></a></div></div>",
    InitializeControls: function () {
        var self = this;
       

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
            $("#KeyWord").val('');
            $("#Area").val(''); 
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
    },
    Search: function () {
        var self = this;

        var filters = {
            Area: $("#Area").val(),
            KeyWord: $("#KeyWord").val(),
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
                debugger;
                if (self.DetailedView) {
                    self.WereDataInShortTable = false;
                    self.WereDataInDetailedTable = true;
                    
                    var toAdd = "";

                    $.each(data, function (index, element) {

                        toAdd += self.TableDetailRowStartConstString + (self.From + index + 1);
                        toAdd += self.TableDetailRowNameConstString + element.id + "\">" + element.name;
                        toAdd += self.TableDetailRowDescriptionConstString + element.description;
                        if (element.area.id !== null) {
                            toAdd += self.TableDetailRowAreaConstString + self.TableDetailRowAreaButtonConstString;
                            toAdd += element.area.id + "\">" + element.area.name + "</button>";
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
                        $("#Area").val(this.innerText);
                        self.Search();
                    });

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
                        if (element.area.id !== null) {
                            toAdd += self.TableShortRowAreaConstString + self.TableShortRowAreaButtonConstString;
                            toAdd += element.area.id + "\">" + element.area.name + "</button>";
                        }
                        else {
                            toAdd += self.TableShortRowAreaConstString;
                        }
                        toAdd += self.TableShortRowEndConstString;
                    });

                    $(".table-body-short").append(toAdd);

                    $(".button-short-column-area").click(function () {
                        $("#KeyWord").val("");
                        $("#Area").val(this.innerText);
                        self.Search();
                    });
                }
            }
        });
    }
});
var CatalogueUserView = Class.extend({
    IsGlobalAdmin: null,

    TableRowStartConstString: "<div class=\"table-body-row justify-content-center d-flex\"><div class=\"table-body-column-number\">",
    TableRowFullNameConstString: "</div><div class=\"table-body-column-name\">",
    TableRowLinkConstString: "<a class=\"table-body-column-name-link\" href=\"/User/Revision?id=",
    TableRowCuratorStatusConstString: "</div><div class=\"table-body-column-curator-status\">",
    TableRowEmailConstString: "</div><div class=\"table-body-column-email\">",
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
                $(".table-body").empty();

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

                $(".table-body").append(toAdd);
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
        debugger;
        $("#imageContainer").empty();

        var element = $(".popup-content-custom .add-image-placeholder-custom").html();

        $("#imageContainer").append(element);

        $("#addImagePlaceholder").empty();
    },
});