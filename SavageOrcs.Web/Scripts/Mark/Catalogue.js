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