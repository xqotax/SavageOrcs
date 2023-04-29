function ArrayToMultiselectOptions(arr) {
    var str = ""
    const optionStart = "<option value=\"";
    const optionEnd = "</option>";
    $.each(arr, function (index, element) {
        str += optionStart + element.id + "\">" + element.name + optionEnd;
    });

    return str;
};

function ClearMultiSelect(selectId) {
    if (!($('#' + selectId).next().find('.multiselect-dropdown-custom-all-selector input[type="checkbox"]').is(":checked")))
        $('#' + selectId).next().find('.multiselect-dropdown-custom-all-selector').trigger('click');
    $('#' + selectId).next().find('.multiselect-dropdown-custom-all-selector').trigger('click');
}


function MultiselectDropdown(options) {
    var config = {
        search: true,
        height: options.height,
        placeholder: options.placeholder,
        txtSelected: options.txtSelected,
        txtAll: options.txtAll,
        txtRemove: options.txtRemove,
        txtSearch: options.txtSearch,
        id: options.Id
    };
    function newEl(tag, attrs) {
        var e = document.createElement(tag);
        if (attrs !== undefined) Object.keys(attrs).forEach(k => {
            if (k === 'class') { Array.isArray(attrs[k]) ? attrs[k].forEach(o => o !== '' ? e.classList.add(o) : 0) : (attrs[k] !== '' ? e.classList.add(attrs[k]) : 0) }
            else if (k === 'style') {
                Object.keys(attrs[k]).forEach(ks => {
                    e.style[ks] = attrs[k][ks];
                });
            }
            else if (k === 'text') { attrs[k] === '' ? e.innerHTML = '&nbsp;' : e.innerText = attrs[k] }
            else e[k] = attrs[k];
        });
        return e;
    }


    document.querySelectorAll("select[multiple]").forEach((el, k) => {
        if ($(el).attr('id') !== config.id)
            return;
        var div = newEl('div', { class: 'multiselect-dropdown-custom', style: { width: '100%', minHeight: window.innerWidth >= 700 ? '44px' : '24px', padding: '' } });
        el.style.display = 'none';
        el.parentNode.insertBefore(div, el.nextSibling);
        var listWrap = newEl('div', { class: 'multiselect-dropdown-custom-list-wrapper' });
        var list = newEl('div', { class: 'multiselect-dropdown-custom-list', style: { height: config.height } });
        var search = newEl('textarea', { class: ['multiselect-dropdown-custom-search'], style: { width: '100%', display: el.attributes['multiselect-search']?.value === 'true' ? 'block' : 'none' }, placeholder: config.txtSearch });
        listWrap.appendChild(search);
        div.appendChild(listWrap);
        listWrap.appendChild(list);

        el.loadOptions = () => {
            list.innerHTML = '';

            if (el.attributes['multiselect-select-all']?.value == 'true') {
                var op = newEl('div', { class: 'multiselect-dropdown-custom-all-selector' })
                var ic = newEl('input', { type: 'checkbox' });
                op.appendChild(ic);
                op.appendChild(newEl('label', { text: config.txtAll }));

                op.addEventListener('click', () => {
                    op.classList.toggle('checked');
                    op.querySelector("input").checked = !op.querySelector("input").checked;

                    var ch = op.querySelector("input").checked;
                    list.querySelectorAll(":scope > div:not(.multiselect-dropdown-custom-all-selector)")
                        .forEach(i => { if (i.style.display !== 'none') { i.querySelector("input").checked = ch; i.optEl.selected = ch } });

                    document.getElementById(config.id).parentNode.querySelector('.multiselect-dropdown-custom-search').value = '';
                    list.querySelectorAll(":scope div:not(.multiselect-dropdown-custom-all-selector)").forEach(d => {
                        var txt = d.querySelector("label").innerText.toUpperCase();
                        d.style.display = txt.includes(search.value.toUpperCase()) ? 'block' : 'none';
                    });
                    el.dispatchEvent(new Event('change'));
                });
                ic.addEventListener('click', (ev) => {
                    ic.checked = !ic.checked;
                    document.getElementById(config.id).parentNode.querySelector('.multiselect-dropdown-custom-search').value = '';
                    list.querySelectorAll(":scope div:not(.multiselect-dropdown-custom-all-selector)").forEach(d => {
                        var txt = d.querySelector("label").innerText.toUpperCase();
                        d.style.display = txt.includes(search.value.toUpperCase()) ? 'block' : 'none';
                    });
                });
                el.addEventListener('change', (ev) => {
                    let itms = Array.from(list.querySelectorAll(":scope > div:not(.multiselect-dropdown-custom-all-selector)")).filter(e => e.style.display !== 'none')
                    let existsNotSelected = itms.find(i => !i.querySelector("input").checked);
                    if (ic.checked && existsNotSelected) ic.checked = false;
                    else if (ic.checked == false && existsNotSelected === undefined) ic.checked = true;
                    document.getElementById(config.id).parentNode.querySelector('.multiselect-dropdown-custom-search').value = '';
                    list.querySelectorAll(":scope div:not(.multiselect-dropdown-custom-all-selector)").forEach(d => {
                        var txt = d.querySelector("label").innerText.toUpperCase();
                        d.style.display = txt.includes(search.value.toUpperCase()) ? 'block' : 'none';
                    });
                });

                list.appendChild(op);
            }

            Array.from(el.options).map(o => {
                var op = newEl('div', { class: o.selected ? 'checked' : '', optEl: o })
                var ic = newEl('input', { type: 'checkbox', checked: o.selected });
                op.appendChild(ic);
                op.appendChild(newEl('label', { text: o.text }));

                op.addEventListener('click', () => {
                    op.classList.toggle('checked');
                    op.querySelector("input").checked = !op.querySelector("input").checked;
                    op.optEl.selected = !!!op.optEl.selected;
                    el.dispatchEvent(new Event('change'));
                });
                ic.addEventListener('click', (ev) => {
                    ic.checked = !ic.checked;
                });
                o.listitemEl = op;
                list.appendChild(op);
            });
            div.listEl = listWrap;

            div.refresh = () => {
                div.querySelectorAll('span.optext, span.multiselect-placeholder').forEach(t => div.removeChild(t));
                var sels = Array.from(el.selectedOptions);
                if (sels.length > (el.attributes['multiselect-max-items']?.value ?? 1)) {
                    div.appendChild(newEl('span', { class: ['optext', 'maxselected'], text: sels.length + ' ' + config.txtSelected }));
                }
                else {
                    sels.map(x => {
                        var c = newEl('span', { class: 'optext', text: x.text, srcOption: x });
                        if ((el.attributes['multiselect-hide-x']?.value !== 'true'))
                            c.appendChild(newEl('span', { class: 'optdel', text: '🗙', title: config.txtRemove, onclick: (ev) => { c.srcOption.listitemEl.dispatchEvent(new Event('click')); div.refresh(); ev.stopPropagation(); } }));

                        div.appendChild(c);
                    });
                }
                div.appendChild(newEl('span', { class: 'multiselect-placeholder', text: el.attributes['placeholder']?.value ?? config.placeholder }));
            };
            div.refresh();
        }
        el.loadOptions();

        search.addEventListener('input', () => {
            list.querySelectorAll(":scope div:not(.multiselect-dropdown-custom-all-selector)").forEach(d => {
                var txt = d.querySelector("label").innerText.toUpperCase();
                d.style.display = txt.includes(search.value.toUpperCase()) ? 'block' : 'none';
            });
        });

        div.addEventListener('click', (element) => {
            var searchWrap = listWrap.querySelector('.multiselect-dropdown-custom-list');
            searchWrap.style.display = 'block';
            searchWrap.style.zIndex = 4;
            search.focus();
            search.select();
            if ($(event.target).parent().parent().parent().find("#namesMultiselect").length == 1
                || $(event.target).parent().parent().parent().find("#textNamesMultiselect").length == 1) {
                //$(document).find(".map-container").find("#areasMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "none" });
                setTimeout(() => {
                    if ($(document).find(".map-container").length == 1)
                        $(document).find(".map-container").find("#areasMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "none" });
                    if ($(document).find(".table-col").length == 1)
                        $(document).find(".table-col").find("#areasMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "none" });
                    if ($(document).find(".text-table-col").length == 1)
                        $(document).find(".text-table-col").find("#curatorsMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "none" });

                }, 10);
            }
        });

        document.addEventListener('click', function (event) {
            if (!div.contains(event.target)) {
                var searchWrap = listWrap.querySelector('.multiselect-dropdown-custom-list');
                if ($(document).find(".map-container").length == 1 && $(event.target).closest('.filter-multiselect-placeholder').find("#namesMultiselect").length != 1) {
                    $(document).find(".map-container").find("#areasMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "block" });
                }
                if ($(document).find(".table-col").length == 1 && $(event.target).closest('.filter-multiselect-placeholder').find("#namesMultiselect").length != 1) {
                    $(document).find(".table-col").find("#areasMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "block" });
                }
                if ($(document).find(".text-table-col").length == 1 && $(event.target).closest('.text-search-filter-multiselect-placeholder').find("#textNamesMultiselect").length != 1) {
                    $(document).find(".text-table-col").find("#curatorsMultiselect").parent().find(".multiselect-dropdown-custom").css({ "display": "block" });
                }
                searchWrap.style.display = 'none';
                searchWrap.style.zIndex = 2;
                /*listWrap.style.display = 'none';*/
                div.refresh();
            }
        });
    });
}


