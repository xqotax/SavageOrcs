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
    AfterAddingVideo: function () {
        var self = this;

        $("video").click(function () {
            self.CopyTextToClipboard($(this).attr("src"));
        });
    },
    RemoveVideos: function () {
        $("#videoTextContainer .row").empty();
    },

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