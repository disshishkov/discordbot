"use strict";

var url = require("system").args[1];
var clip = null;

if (url.indexOf("heroesfire.com") >= 0)
{
    clip = { top: 857, left: 383, width: 370, height: 481 };
}
else if (url.indexOf("blizzardheroes.ru") >= 0)
{
    clip = { top: 860, left: 15, width: 370, height: 525 };
}
else if (url.indexOf("psionic-storm.com") >= 0)
{
    clip = { top: 630, left: 20, width: 400, height: 660 };
}
else 
{
    phantom.exit();
}

var render = function()
{
    console.log(page.renderBase64("JPEG"));
    phantom.exit();
};

var page = require("webpage").create();

page.onConfirm = page.onPrompt = page.onError = function empty() {};
page.viewportSize = { width: 600, height: 600 };
page.clipRect = clip;

page.open(url, function (status) 
{
    if (status !== "success") 
    {
        phantom.exit();
    } 
    else 
    {
        page.onLoadFinished = function() { render(); };
        window.setTimeout(function () { render(); }, 10000);
    }
});
