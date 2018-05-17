//helpers

var logado = false;


function DataDeHoje() {
    var d = new Date();
    return d.getFullYear() + "-" + d.getMonth() + "-" + d.getDate();
}

function FormataData(dt) {
    var d = new Date(dt);
    return d.getDate() + "/" + d.getMonth() + "/" + d.getFullYear();
}

function Hoje() {
    var d = new Date();
    d = Date(d.getFullYear(), d.getMonth(), d.getDate());
    return d;
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

function getUrlLastSlash() {
    var pathArray = window.location.pathname.split('/');
    return pathArray[pathArray.length-1];
}


function formataDinheiro(n) {
    if (n == null) {
        n = 0;
    } 
    return "R$ " + n.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d{3})+\,)/g, "$1.");
}

function formataPorcentagem(n) {
    if (n == null) {
        n = 0;
    } 
    return n.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d{3})+\,)/g, "$1.") + "%";
}

function formataDec2(n) {
    if (n == null) {
        n = 0;
    } 
    return n.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d{3})+\,)/g, "$1.");
}

function unicodeEscape(str) {
    return str.replace(/[\s\S]/g, function (escape) {
        return '\\u' + ('0000' + escape.charCodeAt().toString(16)).slice(-4);
    });
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}



function IsRegistered(param) {
    var _uid = getCookie("uid");
    var retorno = null;
    //alert(_uid);
    if (_uid != null) {
        //check if exists
        
        var urlcall = "/api/islogged?uid=" + _uid;
        var jqxhr = $.ajax(urlcall)
            .done(function (data) {
                //retorno = true;
                logado = true;
                //return retorno;
                console.log(logado);
                //callback(logado);
            });
    }
    else {
        //
    }
    
}



