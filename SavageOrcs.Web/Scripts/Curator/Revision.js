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