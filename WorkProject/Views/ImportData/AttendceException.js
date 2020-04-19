const generateUrl = "/api/AttendceException/Generate";//数据异常
const deleteUrl = "/api/AttendceException/MonRemove";//删除异常数据（字段handled=false）
const tableUrl = "/api/AttendceException/ExceptionShow";//异常数据显示
const updateUrl = "/api/AttendceException/UpdateWork";//异常数据更新
let initPageSize = 16;
$.ajaxSettings.async = false;
$(function () {
    //初始化Table
    var oTable = new TableInit();
    oTable.Init();
});
var TableInit = function () {
    var oTableInit = new Object();
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
            sortOrder: "asc",//排序方式
            queryParams: queryParams,//传递参数（*）
            sidePagination: "server",//分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,//初始化加载第一页，默认第一页
            pageSize: initPageSize,//每页的记录行数（*）
            pageList: [13, 16, 20],//可供选择的每页的行数（*）
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
                    formatter: function (value, row, index) {
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
                    field: 'RecordTime',
                    title: '生成日期',
                    sortable: true,

                },  
                {
                    field: 'WorkDate',
                    title: '出勤日期',
                    sortable: true,

                },                
                {
                    field: 'WorkTime',
                    title: '日工',
                    sortable: true,

                },
                {
                    field: 'WorkMore',
                    title: '加班',
                    sortable: true,

                },
                {
                    field: 'totalWork',
                    title: '总工',
                    sortable: true,

                }, 
                {
                    field: 'WorkSiteCnt',
                    title: '工地数',
                    sortable: true,

                },  
                {
                    field: 'Handled',
                    title: '处理',
                    sortable: true,

                },
                {
                    title: '异常修复',
                    formatter: option
                },
                
            ],
            //onLoadSuccess: function (data) {//数据成功加载完成触发的方法

            //    //前后两行数据人名一致标红
            //    ChangeSameNameColor(data.rows);

            //},

        });
    };

    return oTableInit;
};

//得到查询的参数
function queryParams(params) {

    let year = $("#selectYear").val(); let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    let worker = $("#selectName").val(); 
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        worker: worker,
        year: year,
        mon: mon,
        day: day,
        sort: params.sort,      //排序列名  
        sortOrder: params.order //排位命令（desc，asc） 
    };

    return temp;

};

//操作栏的格式化
// onclick=\"ChangeSameNameColor('" + row.RouteName + "','" + row.Direcition + "','" + index + "')\"
// 定义删除、更新操作
function option(value, row, index) {
    var htm =
        " <button id ='dupUser' onclick =\"updUser('" + row.WorkName + "','" + row.WorkDate + "')\">复位</button> ";
    return htm;
}

//数据生成
function exceptionGenerate() {
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val();
    $.ajax({
        url: generateUrl, //所需要的列表接口地址
        type: "get",
        data: { 'year': year, 'mon': mon },
        dataType: "json",
        success: function (data) {

            if (data == 'ok') {
                alert(year + mon + "月度数据已生成");
            }
            else if (data == 'error') {
                alert(year + mon + "月度数据生成有误请排查");
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
    workerInfo();
}
//数据删除
function deleteException() {

    let year = $("#selectYear").val();
    let mon = $("#selectMon").val();
    $.ajax({
        url: deleteUrl, //所需要的列表接口地址
        type: "get",
        data: { 'year': year, 'mon': mon },
        dataType: "json",
        success: function (data) {

            if (data == 'ok') {
                alert(year + mon + "月度数据已删除");
            }
            else if (data == 'nothing is deleted') {
                alert('nothing is deleted');
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
    workerInfo();
}

//异常数据修复
function updUser(name,date) {
    $.ajax({
        url: updateUrl, //所需要的列表接口地址
        type: "get",
        data: { 'worker': name, 'workDate': date },
        dataType: "json",
        success: function (data) {

            if (data == 'ok') {
                alert(name + date + "异常数据已复位");
            }
            else if (data == 'error') {
                alert(year + mon + "异常数据复位失败");
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
    workerInfo();
}



//按钮click事件函数
function workerInfo() {

    
    let worker = $("#selectName").val();

    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();

    let opt = {
        url: tableUrl,
        query: {
            worker: worker,
            year: year,
            mon: mon,
            day: day
        }
    };
    $("#table").bootstrapTable('refresh', opt);
}