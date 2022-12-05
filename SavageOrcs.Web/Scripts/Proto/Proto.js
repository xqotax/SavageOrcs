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