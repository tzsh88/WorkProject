//var host = location.host; //主机号+端口号
$.ajaxSettings.async = false; //防止objson出现undefined,设置为同步
var topUrl = '/Views/login.html';
//////查询用户名信息是否到session中，不在的话调回login
//$.ajax({
//    url: '/api/UserLogin/5',
//    type: "get",
//    dataType: 'json',
//    async: false,
//    success: function (strjson) {
//        if (strjson == "session is not exist") {
//            var a = $("<a href='login.html' target='_self' ></a>").get(0);
//            var e = document.createEvent('MouseEvents');
//            e.initEvent('click', true, true);
//            a.dispatchEvent(e);

//        }
//        else {
//            console.log("用户导入成功");
//        }

//    },
//    //error: function (e) {
//    //    console.log(e.responseJSON);
//    //}
//})
//'fa fa-bus'
var iconArr = ['fa fa-edit', 'fa fa-map', 'fa fa-fw fa-bar-chart', 'fa fa-pie-chart', 'fa fa-paper-plane' ,'fa fa-thumb-tack'];//菜单栏icon
$.ajax({
    url: '../api/treeitem/Get',
    datatype: 'json',
    cache: false,
    success: function (strjson) {
        var firstChildren = strjson.children;
        menuFirstTree(firstChildren);//第一级菜单
    },
    error: function (res) {
        //修改start
        if (res.responseJSON.messages == 'login information is useless') {
           
            top.location.href = topUrl;

        }
            //修改end
    }
});
//一级菜单
function menuFirstTree(childrenData) {
    for (var i = 0; i < childrenData.length; i++) {
        var treeData =
            '<li  class="treeview">' +
            '<a href="#">' +
            '<i class=\'' + iconArr[i] + '\'></i>' +
            '<span> ' + childrenData[i].name + ' </span>' +
            '<span class="pull-right-container">' +
            '<i class="fa fa-angle-left pull-right"></i>' +
            ' </span>' +
            ' </a>' +

            menuSecTree(childrenData[i].children) +

            '</li>';

        $('#menuSideBar').append(treeData);
    }
  

}
//二级菜单
function menuSecTree(secData) {
    var treeData = '<ul class="treeview-menu">';
    for (var i = 0; i < secData.length; i++) {
        if (secData[i].children == null) {
            treeData +=
                '<li mid=\'' + secData[i].id + '\' funurl=' + secData[i].url + ' class="treeview">' +
                '<a href="javascript:void(0)">' +
                '<i class="fa fa-circle-o"></i>' +
                '<span> ' + secData[i].name + ' </span>' +
                '</a>' +
                '</li>';
        } else {
            treeData +=
                '<li class="treeview">' +
                '<a href="#">' +
                '<i class="fa fa-circle-o"></i>' +
                '<span> ' + secData[i].name + ' </span>' +

                '<i class="fa fa-angle-left pull-right"></i>' +

                '</a>' +
                menuThirdTree(secData[i].children) +
                '</li>';
        }
        
    }
    treeData += '</ul>';
    return treeData;

}
//三级菜单
function menuThirdTree(thirdData) {
    var treeData = '<ul class="treeview-menu">';
    for (var i = 0; i < thirdData.length; i++) {
        treeData +=
            '<li mid=\'' + thirdData[i].id + '\' funurl=' + thirdData[i].url + ' class="treeview">' +
            '<a href="javascript:void(0)">' +
            '<i class="fa fa-circle-o"></i>' + thirdData[i].name +
            '</a>' +
            '</li>';
    }

    treeData += '</ul>';
    return treeData;
}




