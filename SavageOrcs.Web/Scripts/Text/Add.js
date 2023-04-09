var AddTextView = Class.extend({
    IsNew: null,
    ToDelete: false,
    CuratorName: null,
    CuratorId: null,

    UkrTextName: null,
    UkrTextId: null,

    Blocks: null,
    OldData: null,

    Curators: null,
    CuratorIds: null,
    CuratorNames: null,
    SearchSelectDropdownCurators: null,

    UkrTexts: null,
    UkrTextIds: null,
    UkrTextNames: null,
    SearchSelectDropdownUkrTexts: null,

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

        self.SearchSelectDropdownUkrTexts = new SearchSelect('#dropdown-input-for-ukrText', {
            data: [],
            filter: SearchSelect.FILTER_CONTAINS,
            sort: undefined,
            inputClass: 'form-control-Select mobile-field',
            maxOpenEntries: 5,
            searchPosition: 'top',
            onInputClickCallback: null,
            onInputKeyDownCallback: null,
        });

        self.InitializeUkrTexts(self.UkrTexts);

        if (self.UkrTextName !== '') {
            var selected = $($("#ukrText .searchSelect--Result")[0]);
            selected.removeClass("#ukrText searchSelect--Placeholder");
            selected.html(self.UkrTextName);

            $.each($("#ukrText .searchSelect--Option"), function (index, element) {
                if ($(element).text() === self.UkrTextName) {
                    $(element).addClass("#ukrText searchSelect--Option--selected")
                }
            });

            $("#dropdown-input-for-ukrText").val(self.UkrTextName);
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
                            "#000000",
                            "#DEDCDC"
                        ],
                        defaultColor: "#FF1300",
                        type: "text"
                    }
                },
                Marker: {
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
                            "#000000",
                            "#DEDCDC"
                        ],
                        defaultColor: '#FFBF00',
                        type: 'marker',
                    }
                },
                clearStyles: {
                    class: ClearStylesTool,
                    inlineToolbar: true,
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

        $('#EnglishVersion').change(function () {
            self.ChangeUkrRow();
        });
        self.ChangeUkrRow();

        //$('#dropdown-input-for-curator').addClass("display-8-custom");
    },

    ChangeUkrRow: function () {
        if ($("#EnglishVersion").is(":checked")) {
            $('#ukrTextRow').show();
        } else {
            $('#ukrTextRow').hide();
        }
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
    InitializeUkrTexts: function (data) {
        var self = this;
        self.UkrTextNames = [];
        self.UkrTextIds = [];

        $.each(data, function (index, element) {
            self.UkrTextNames.push(element.name);
            self.UkrTextIds.push(element.id);
        });

        self.SearchSelectDropdownUkrTexts.setData(self.UkrTextNames);
    },

    Save: function () {
        var self = this;

        self.Editor.save().then((output) => {
            self.Data = output;

            var curatorId = self.CuratorIds[self.CuratorNames.indexOf($("#dropdown-input-for-curator").val())];
            curatorId = curatorId === "" ? null : curatorId;

            var ukrTextId = self.UkrTextIds[self.UkrTextNames.indexOf($("#dropdown-input-for-ukrText").val())];
            ukrTextId = ukrTextId === "" ? null : ukrTextId;

            var saveTextViewModel = {
                Id: $("#Id").val() === "" ? null : $("#Id").val(),
                CuratorId: curatorId,
                UkrTextId: ukrTextId,
                EnglishVersion: $('#EnglishVersion').is(":checked"),
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

class ClearStylesTool {
    static get isInline() {
        return true;
    }

    static get title() {
        return "Clear Styles";
    }

    static get icon() {
        return '<svg width="12" height="12" viewBox="0 0 12 12" xmlns="http://www.w3.org/2000/svg"><path d="M9.5 2C9.5 1.17157 8.82843 0.5 8 0.5H4C3.17157 0.5 2.5 1.17157 2.5 2V2.5H0.5V3.5H2.5V10.5H3.5V3.5H5.5V2.5H3.5V2C3.5 1.72386 3.72386 1.5 4 1.5H8C8.27614 1.5 8.5 1.72386 8.5 2V2.5H9.5V2ZM7.5 3.5V10.5H4.5V3.5H7.5Z"/></svg>';
    }

    //surround(range) {
    //    const element = this.selection.findParentTag(this.tag);
    //    if (element) {
    //        element.outerHTML = element.innerHTML;
    //    }
    //}

    surround(range) {
        const selectedText = range.extractContents();
        const span = document.createElement("span");
        span.textContent = selectedText.textContent;
        range.insertNode(span);
    }

    checkState() { }

    render() {
        this.button = document.createElement("div");
        this.button.classList.add("color-fire-btn");
        this.button.innerHTML = this.constructor.icon;
        this.button.title = this.constructor.title;
        this.button.addEventListener("click", () => {
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);
                this.surround(range);
            }
        });
        return this.button;
    }
}
