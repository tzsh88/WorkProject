const tableUrl = "/api/AttendanceData/GetAttendancs";
let initPageSize = 16;
$.ajaxSettings.async = false;
$(function () {
    //初始化Table
    let oTable = new TableInit();
    oTable.Init();
});

let TableInit = function () {
    let oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $("#table").bootstrapTable({
            url: tableUrl,//请求后台的URL（*）
            method: 'get',//请求方式（*）
            striped: true,//是否显示行间隔色
            cache: false,//是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,//是否显示分页（*）
            sortName: 'WorkDate',//初始化的时候排序的字段
            sortable: true,//是否启用排序
            sortOrder: "desc",//排序方式
            queryParams: queryParams,//传递参数（*）
            sidePagination: "server",//分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,//初始化加载第一页，默认第一页
            pageSize: initPageSize,//每页的记录行数（*）
            pageList: [12, 16, 20],//可供选择的每页的行数（*）
            search: false,//是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
            showColumns: false,//是否显示所有的列
            showRefresh: false,//是否显示刷新按钮
            minimumCountColumns: 2,//最少允许的列数
            clickToSelect: true,//是否启用点击选中行
            //height: 700,//行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
            uniqueId: "WorkName",//每一行的唯一标识，一般为主键列
            showToggle: false,//是否显示详细视图和列表视图的切换按钮
            cardView: false,//是否显示详细视图
            detailView: false,//是否显示父子表
            columns: [
                {
                    title: '序号',
                    formatter: ordersFormatter

                },
                {
                    field: 'WorkSiteName',
                    title: '工地',
                    sortable: true,

                },
                {
                    field: 'WorkName',
                    title: '姓名',
                    sortable: true,
                    formatter: function (value, row, index) {
                        return "<span id=" + " wn" + index + " > " + value + "</span >"
                    }
                },
                {
                    field: 'Sex',
                    title: '性别',
                    sortable: true,
                    formatter: operateFormatter //自定义方法，添加操作按钮
                },
                {
                    field: 'WorkType',
                    title: '工种',
                    sortable: true,

                },
                {
                    field: 'WorkDate',
                    title: '出勤日期',
                    sortable: true,

                },
                {
                    field: 'Weather',
                    title: '天气',
                    sortable: true,

                },

                {
                    field: 'WorkTime',
                    title: '工日',
                    sortable: true,

                },
                {
                    field: 'WorkMore',
                    title: '加班',
                    sortable: true,

                },
                {
                    field: 'totalWork',
                    title: '总工时',
                    sortable: true,

                },
                {
                    field: 'WorkQuality',
                    title: '工作日志',
                    sortable: true,

                },
                {
                    field: 'Affiliation',
                    title: '工人归属',
                    sortable: true,

                },
                {
                    title: '操作',               
                    formatter: option
                }
            ],
            onLoadSuccess: function (data) {//数据成功加载完成触发的方法

                //前后两行数据人名一致标红
                ChangeSameNameColor(data.rows);

            },

        });
    };

    return oTableInit;
};

//操作栏的格式化
// onclick=\"ChangeSameNameColor('" + row.RouteName + "','" + row.Direcition + "','" + index + "')\"
// 定义删除、更新操作
function option(value, row, index) {
    var htm = "<button id='delUser' onclick=\"delUser('" + row.WorkName + "','" + row.WorkSiteName + "','" + row.WorkDate +"')\">删除</button> " +
        " <button id ='dupUser' onclick =\"updUser('" + row.WorkName +"','"+ row.WorkSiteName + "','"+ row.WorkDate +"','"+row.Weather+"','"+row.WorkTime +"','"+ row.WorkMore+"','"+ row.WorkQuality +"')\">修改</button>";
    return htm;
}

const deleteUrl = "/api/ModifyAttendance/DeleteWork";
const updateUrl = "/api/ModifyAttendance/UpdateWork";

// 删除用户
function delUser(name,site,date) {
    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    var mymessage = confirm("确认删除嘛？");
    if (mymessage == true) {
        $.ajax({
            type: "get",
            url: deleteUrl,
            data: { 'name': name, 'site': site, 'date': date},
            dataType: "json",
            success: function (data) {
                if (data == "ok") {
                    alert(date + " " + name + " 在" +site+"出勤数据删除成功");
                    let opt = {
                        url: tableUrl,
                        query: {
                            worker: worker,
                            workSite: workSite,
                            year: year,
                            mon: mon,
                            day: day
                        }
                    };

                    //删除成功后，刷新表格
                    $("#table").bootstrapTable('refresh', opt);
                }
              
            },
            error: function (data) {
                alert("服务器繁忙,暂时无法删除")
            }
        });
    }
}

function workerUpdate(name, site, date) {
    let work = $("#work").val(); let more = $("#more").val(); let detail = $("#detail").val();
    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    $.ajax({
        type: "get",
        url: updateUrl,
        data: { 'name': name, 'site': site, 'date': date, 'work': work, 'more': more, 'detail': detail },
        dataType: "json",
        success: function (data) {
            if (data == "ok") {
                alert(date + " " + name + " 在" + site + "出勤数据修改成功");
                let opt = {
                    url: tableUrl,
                    query: {
                        worker: worker,
                        workSite: workSite,
                        year: year,
                        mon: mon,
                        day: day
                    }
                };

                //删除成功后，刷新表格
                $("#table").bootstrapTable('refresh', opt);
            }

        },
        error: function (data) {
            alert("服务器繁忙,暂时无法更新")
        }
    });
}

// 编辑用户
function updUser(name, site, date, weather, work, more, detail) {

  
    //页面层
    layer.open({
        type: 1,
        title: ['数据修改', 'text-align:center;font-size:1.5em;font-weight:600'],
        skin: 'layui-layer-rim', //加上边框
        area: ['420px', '240px'], //宽高
        content: "<div class='form-group' style='margin:2% 30%;'>" +
                    "<div class='form-inline' style='margin-bottom:2%'>"  +
                    "<span style='font-size: 1em;' id='name'>姓&nbsp;&nbsp;&nbsp;&nbsp;名："+name +"</span>"  +         
                    "</div>" +
                    "<div class='form-inline' style='margin-bottom:2%'>"  +
                    "<span style='font-size: 1em;' id = 'site'>工&nbsp;&nbsp;&nbsp;&nbsp;地："+site+" </span>"  +         
                    "</div>" +
                    "<div class='form-inline' style='margin-bottom:2% '>"  +
                    "<span style='font-size: 1em;' id = 'date' > 日 &nbsp;&nbsp;&nbsp;&nbsp;期："+ date +"</span >"  +         
                    "</div>" +
                    "<div class='form-inline' style='margin-bottom:2%'>" +
                    "<span style='font-size: 1em;' id = 'weather' > 天 &nbsp;&nbsp;&nbsp;&nbsp;气：" + weather +"</span >"   +
                    "</div>" +
                    "<div class='form-inline' style='margin-bottom:2%'>" +
                    "<span style=’font-size: 1em;'>工&nbsp;&nbsp;&nbsp;&nbsp;日：</span>"  +
                    "<input type='text'id='work' class='form-control' value='"+ work+"' style='width:5em; height: 2em;'>"  +
                    "</div>"  +
                    "<div class='form-inline' style='margin-bottom:2%'>" +
                    "<span style='font-size: 1em; '>加&nbsp;&nbsp;&nbsp;&nbsp;班：</span>"  +
                    "<input type='text' id='more' class='form-control' value='"+ more+"' style='width: 5em; height: 2em;'>"  +
                    "</div>" +
                    "<div class='form-inline' style='margin-bottom:2%'>" +
                          "<span style='font-size:1em;'>日&nbsp;&nbsp;&nbsp;&nbsp;志：</span>" +
                           "<textarea type='text' id='detail' class='form-control' style='width:10em;'  rows='3'>" + detail +"</textarea>"+            
                     "</div>" +

                    "<div class='form-inline' style='margin-top:5%;margin-bottom:2%;margin-left:20%' >" +
            "<button type='button' class='btn' style='background: #1caf9a; color: #FFFFFF; border: none; '  onclick =\"workerUpdate('" + name + "','" + site + "','" + date + "')\">数据提交</button>" +
                     "</div>" +
                     "</div>"
    });
}

//前后两行数据人名一致标红,同时保证页面最大数为偶数，不然下面代码有bug
function ChangeSameNameColor(rows) {//赋予的参数
    if (rows.length > 1) {
        for (let index = 0; index < rows.length; index++) {
            if (index % 2 == 0) {    //获取序号为i的数据
                if (index + 1 < rows.length && rows[index].WorkName == rows[index + 1].WorkName) {
                    $("#wn" + index + "").css("color", "#ef4f4f");//点击过的名字颜色改变
                  
                }
                if (index != 0 && rows[index].WorkName == rows[index - 1].WorkName) {                   
                    $("#wn" + index + "").css("color", "#ef4f4f");//点击过的名字颜色改变
                }
            }         
            if (index % 2 != 0) {    //获取序号为i的数据
                if (rows[index].WorkName == rows[index - 1].WorkName)
                    $("#wn" + index + "").css("color", "#ef4f4f");//点击过的名字颜色改变
                if (index + 1 < rows.length &&rows[index].WorkName == rows[index + 1].WorkName )
                    $("#wn" + index + "").css("color", "#ef4f4f");//点击过的名字颜色改变  

            }
        }
    }
}

function operateFormatter(value, row, index) {

    if (value == 1) {
        return "<span title='" + value + "'>" + '男' + "</span>"
    } else if (value == 0) {
        return "<span title='" + value + "'>" + '女' + "</span>"
    }
}

function ordersFormatter(value, row, index) {
  
    //获取每页显示的数量,第一次加载无法获取
    let pageSize = $('#table').bootstrapTable('getOptions').pageSize;
    if (pageSize == undefined) {
        return index + 1;
    }
    //获取当前是第几页
    let pageNumber = $('#table').bootstrapTable('getOptions').pageNumber;
    //返回序号，注意index是从0开始的，所以要加上1
    let res = pageSize * (pageNumber - 1) + index + 1;
    return res;
}
//得到查询的参数
function queryParams(params) {
    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        worker: worker,
        workSite: workSite,
        year: year,
        mon: mon,
        day: day,
        sort: params.sort,      //排序列名  
        sortOrder: params.order //排位命令（desc，asc） 
    };

    return temp;

};
//按钮click事件函数
function workerInfo() {

    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();

    let opt = {
        url: tableUrl,
        query: {
            worker: worker,
            workSite: workSite,
            year: year,
            mon: mon,
            day: day
        }
    };

    $("#table").bootstrapTable('refresh', opt);
}


//按钮excelExport click事件函数
function excelExport() {

    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    let excelUrl = "/api/AttendanceExport/GetExportData?worker=" + worker + "&workSite=" + workSite + "&year=" + year + "&mon=" + mon + "&day=" + day;
    var oReq = new XMLHttpRequest();
    oReq.open("Get", excelUrl, true);
    oReq.responseType = "blob";
    send_data = { 'worker': worker, 'workSite': workSite, 'year': year, 'mon': mon, 'day': day };

    oReq.onload = function (oEvent) {
        var content = oReq.response;
        var elink = document.createElement('a');
        var fileName = "工日_" + workSite + `_${new Date().toLocaleDateString()}.xls`; // 保存的文件名
        elink.download = fileName;
        elink.style.display = 'none';

        var blob = new Blob([content]);
        elink.href = URL.createObjectURL(blob);

        document.body.appendChild(elink);
        elink.click();

        document.body.removeChild(elink);
    };

    oReq.send(null);


}




