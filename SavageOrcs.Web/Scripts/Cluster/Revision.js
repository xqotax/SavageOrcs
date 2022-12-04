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