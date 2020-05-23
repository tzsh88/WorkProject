var initUrl = "/Views/login.html";


//导航区域项目点击增加标签页处理
var currentWidth = $('#rightop').width();
$('a', $('#menuSideBar')).on('click', function (e) {
    //e.stopPropagation();//不禁掉的话下来菜单没有用；

    var li = $(this).closest('li');
    var menuId = $(li).attr('mid');
    if (menuId != undefined) {
        var url = $(li).attr('funurl');
        var title = $(this).text();
            
        $('#mainFrameTabs').bTabsAdd(menuId, title, url);

    }


});



//删除
$('button', $('.navTabsCloseBtn')).on('click', function (e) {

    $('#mainFrameTabs').bTabsClose();
});

//插件的初始化
$('#mainFrameTabs').bTabs({
    //登录界面URL，用于登录超时后的跳转
    'loginUrl': initUrl,
    //用于初始化主框架的宽度高度等，另外会在窗口尺寸发生改变的时候，也自动进行调整
    'resize': function () {
        //这里只是个样例，具体的情况需要计算
        var tabContentHeight = $('.content-wrapper').height() - $('#tips').height();
        var tabContentWidth = $('#rightop').width();
        $('#mainFrameTabs').height(tabContentHeight);
        $('#mainFrameTabs').width(tabContentWidth);
    }
});


$('#zoomWidth').on('click', function () {

    setTimeout("ResetWidth()", 300);

});
//检测设备断pc端还是移动端。
function JugePC() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone",
        "SymbianOS", "Windows Phone",
        "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}
//当设备为移动设备时
if (!JugePC()) {
    $("body").addClass("sidebar-collapse"); // 追加样式
    setTimeout("ResetWidth()", 200);
}
function ResetWidth() {
    var tabContentWidth = $('#rightop').width();
    $('#mainFrameTabs').width(tabContentWidth);
}

function checkDevTools(options) {
    const isFF = ~navigator.userAgent.indexOf("Firefox");
    let toTest = '';
    if (isFF) {
        toTest = /./;
        toTest.toString = function () {
            options.opened();
        }
    } else {
        toTest = new Image();
        //__defineGetter__  方法已经弃用
        toTest.__defineGetter__('id', function () {
            options.opened();
        });

      
        //方法一
        //toTest = {
        //    get id()
        //    {
        //         options.opened();
        //    }
        //};
        

        //方法二
        Object.defineProperty(toTest, 'id',
            {
                get: 
                    function() {
                        options.opened();
                    }

            });
       
    }
    setInterval(function () {
        options.offed();
        console.log(toTest);
        console.clear && console.clear();
    }, 500);
}

//checkDevTools({
//    opened: function () {
//        $(".wrapper").remove();
//        window.location.href = initUrl;
//    },
//    offed: function () {

//        //document.body.innerHTML = 'Dev Tools is off';
//    }
//});



document.onkeydown = function () {
    var e = window.event || arguments[0];
    //屏蔽F12  
    if (e.keyCode == 123) {
        return false;
        //屏蔽Ctrl+Shift+I  
    } else if ((e.ctrlKey) && (e.shiftKey) && (e.keyCode == 73)) {
        return false;
        //屏蔽Shift+F10  
    } else if ((e.shiftKey) && (e.keyCode == 121)) {
        return false;
    }
};
//屏蔽右键单击  
document.oncontextmenu = function () {
    return false;
}

