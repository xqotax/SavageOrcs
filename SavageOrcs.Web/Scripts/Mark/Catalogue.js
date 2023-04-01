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
        
        var filters = {
            SelectedKeyWordAndMarkIds: $("#keyWordsMultiselect").val(),
            SelectedAreaIds: $("#areasMultiselect").val(),
            //AreaName: $("#AreaName").val(),
            //MarkName: $("#MarkName").val(),
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
    }
});