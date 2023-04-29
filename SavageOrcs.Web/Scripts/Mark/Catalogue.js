var CatalogueMarkView = Class.extend({
    Marks: null,
    SelectedText: null,
    MultiselectAll: null,

    AreaTextPlaceholder: null,
    NameTextPlaceholder: null,

    SearchTextName: null,
    SearchTextArea: null,

    IsClear: false,

    InitializeControls: function () {
        var self = this;
       

        var areasOptions = {
            placeholder: self.AreaTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextArea.value,
            height: "300px",
            Id: "areasMultiselect",
            //MaxElementsToShow: 2
        }

        var keyWordsAndMarksOptions = {
            placeholder: self.NameTextPlaceholder.value,
            txtSelected: self.SelectedText.value,
            txtAll: self.MultiselectAll.value,
            txtRemove: "Видалити",
            txtSearch: self.SearchTextName.value,
            height: "300px",
            Id: "namesMultiselect", 
            //MaxElementsToShow: 2
        }

        MultiselectDropdown(keyWordsAndMarksOptions);
        MultiselectDropdown(areasOptions);

        if ($("#areasMultiselect").val().length !== 0 || $('#namesMultiselect').val().length !== 0)
            self.Search();

        self.SubscribeEvents();
    },
    SubscribeEvents: function () {
        var self = this;
        $("#filter-big-text-info").on('click', function () {
            self.Search();
        });

        $(".clear-button-container").on('click', function () {
            self.Clear();
        });
        
        $("#namesMultiselect").on('change', function () {
            self.OnMultiselectChange();
        });

        $("#areasMultiselect").on('change', function () {
            self.OnMultiselectChange();
        });

        var firstElement = $(".data-row-container .data-row")[0];
        if (firstElement !== undefined)
            self.Show(firstElement);
    },
    Search: function () {
        var self = this;
        if (self.IsClear)
            return;

        var names = $('#namesMultiselect').val() || [];

        var filters = {
            SelectedKeyWordIds: [],
            SelectedClusterIds: [],
            SelectedMarkIds: [],
            SelectedAreaIds: $("#areasMultiselect").val(),
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
        var self = this;

        $('.data-row').each(function (idex, element) {
            $(element).css('opacity', '0.3');
            $(element).removeClass("data-row-selected");
        });

        if (window.innerWidth <= 1000 && $(el).next().attr('id') === 'markMobileSlideshowPlaceholder') {
            $("#markMobileSlideshowPlaceholder").remove();
            return;
        }

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
                if (window.innerWidth <= 1000) {
                    self.ShowMobile(el, result);
                }
                else {
                    $(".slideshow-container").html(result);
                    var containerTop = $(".data-row-container").offset().top;
                    var rowTop = $(el).offset().top;
                    var topToSet = rowTop - containerTop;
                    $(".slideshow-container").css({ "margin-top": topToSet + 'px' });
                }
            }
        });
    },
    OnAreasChange: function () {
        var self = this;
        self.Search();
    },
    OnNamesChange: function () {
        var self = this;
        self.Search();
    },
    Clear: function () {
        var self = this;
        self.IsClear = true;
        ClearMultiSelect("namesMultiselect");
        ClearMultiSelect("areasMultiselect");
        self.IsClear = false;
        document.activeElement.blur();
        self.Search();
    },
    OnMultiselectChange: function () {
        var areaSelectedLenght = $("#areasMultiselect").val().length;
        var nameSelectedLenght = $("#namesMultiselect").val().length;

        if (areaSelectedLenght > 0 || nameSelectedLenght > 0) {
            $("#filter-big-text-info").css({ "color": "#FF2929" });
            $("#clear-button").css({ "color": "#FF2929" });
            $(".clear-button-img img").attr("src", "/images/icons/clearRed.png");
        }
        else {
            $("#filter-big-text-info").css({ "color": "#0F0F0F" });
            $("#clear-button").css({ "color": "#0F0F0F" });
            $(".clear-button-img img").attr("src", "/images/icons/clear.png");
        }
    },
    ShowMobile: function (el, result) {
        $("#markMobileSlideshowPlaceholder").remove();
        $(el).after("<div class=\"data-row\" id=\"markMobileSlideshowPlaceholder\">" + result + "</div>")
    }
});