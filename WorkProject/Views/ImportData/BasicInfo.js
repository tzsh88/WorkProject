const tableUrl = "/api/BasicInfoData/GetWorkers";
const table1Url = "/api/BasicInfoData/GetWorkSites";
const updateWorkerUrl = "/api/BasicInfoData/UpdateWorkerVisual";
const updateWorkSiteUrl = "/api/BasicInfoData/UpdateWorkSIteFinshish";
let initPageSize = 16;
$.ajaxSettings.async = false;
$(function () {
    //初始化Table
    var oTable = new TableInit();
    oTable.Init();

    //初始化Table1
    var oTable1 = new TableInit1();
    oTable1.Init();
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
            sortName: 'WorkName',//初始化的时候排序的字段
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
            uniqueId: "WorkId",//每一行的唯一标识，一般为主键列
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
                //{
                //    field: 'IC',
                //    title: '身份证',
                //    sortable: true,

                //},
                {
                    field: 'WorkName',
                    title: '姓名',
                    sortable: true,

                },
                {
                    field: 'Sex',
                    title: '性别',
                    sortable: true,
                    formatter: operateFormatter //自定义方法，添加操作按钮
                },
                {
                    field: 'Phone',
                    title: '手机',
                    sortable: true,

                },
                {
                    field: 'WorkType',
                    title: '工种',
                    sortable: true,

                },
                {
                    field: 'CCBPayCard',
                    title: '建行卡号',
                    sortable: true,

                },
                {
                    field: 'Affiliation',
                    title: '归属',
                    sortable: true,

                }, 
                {
                    field: 'Visual',
                    title: '可见',
                    sortable: true,

                },
                {
                    title: '可见控制',
                    formatter: workerOption
                },

            ]

        });
    };

    return oTableInit;
};
function operateFormatter(value, row, index) {

    if (value == 1) {
        return "<span title='" + value + "'>" + '男' + "</span>"
    } else if (value == 0) {
        return "<span title='" + value + "'>" + '女' + "</span>"
    }
}

//操作栏的格式化
// onclick=\"ChangeSameNameColor('" + row.RouteName + "','" + row.Direcition + "','" + index + "')\"
// 定义删除、更新操作
function workerOption(value, row, index) {
    var htm =
        " <button class ='dupUser' onclick =\"updUser('" + row.WorkName + "')\">复位</button> ";
    return htm;
}

function updUser(name) {
    $.ajax({
        url: updateWorkerUrl, //所需要的列表接口地址
        type: "get",
        data: { 'worker': name },
        dataType: "json",
        success: function (data) {

            if (data == 'ok') {
                alert(name + date + "数据可见性已修改");
            }
            else if (data == 'error') {
                alert(year + mon + "数据可见性修改失败");
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
    workerInfo();
}



//得到查询的参数
function queryParams(params) {


    let worker = $("#selectName").val();
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        worker: worker,
        sort: params.sort,      //排序列名  
        sortOrder: params.order //排位命令（desc，asc） 
    };

    return temp;

};

//第二个表格
var TableInit1 = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $("#table1").bootstrapTable({
            url: table1Url,//请求后台的URL（*）
            method: 'get',//请求方式（*）
            striped: true,//是否显示行间隔色
            cache: false,//是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,//是否显示分页（*）
            sortName: 'WorkSiteId',//初始化的时候排序的字段
            sortable: true,//是否启用排序
            sortOrder: "asc",//排序方式
            queryParams: queryParams1,//传递参数（*）
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
            uniqueId: "WorkSiteId",//每一行的唯一标识，一般为主键列
            showToggle: false,//是否显示详细视图和列表视图的切换按钮
            cardView: false,//是否显示详细视图
            detailView: false,//是否显示父子表
            columns: [
                {
                    field: 'WorkSiteId',
                    title: '工地ID',
                    sortable: true,

                    //formatter: operateFormatter //自定义方法，添加操作按钮
                },
                {
                    field: 'WorkSiteName',
                    title: '工地名称',
                    sortable: true,

                },
                {
                    field: 'WorkManage',
                    title: '管理员',
                    sortable: true,

                },
                {
                    field: 'Company',
                    title: '归属公司',
                    sortable: true,

                },
                {
                    field: 'CompanyBoss',
                    title: '老板',
                    sortable: true,

                },               
                {
                    field: 'WorkSiteFinished',
                    title: '完工',
                    sortable: true,

                },
                {
                    title: '完工控制',
                    formatter: workSiteOption
                },

            ]

        });
    };

    return oTableInit;
};
function workSiteOption(value, row, index) {
    var htm =
        " <button class ='dupUser' onclick =\"updWorkSite('" + row.WorkSiteId+"')\">复位</button> ";
    return htm;
}


function updWorkSite(id) {
    $.ajax({
        url: updateWorkSiteUrl, //所需要的列表接口地址
        type: "get",
        data: { 'workSiteId': id },
        dataType: "json",
        success: function (data) {

            if (data == 'ok') {
                alert(name + date + "工地可见性已修改");
            }
            else if (data == 'error') {
                alert(year + mon + "工地可见性修改失败");
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
    workSiteInfo();
}

//得到查询的参数
function queryParams1(params) {
    let workSite = $("#selectWorkSite").val();
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        workSite: workSite,
        sort: params.sort,      //排序列名  
        sortOrder: params.order //排位命令（desc，asc） 
    };

    return temp;

};
function workerInfo() {


    let worker = $("#selectName").val();
    let opt = {
        url: tableUrl,

        query: {
            worker: worker,
        }
    };

    $("#table").bootstrapTable('refresh', opt);
}

//按钮click事件函数,分工地情况
function workSiteInfo() {


    let workSite = $("#selectWorkSite").val();
    let opt = {
        url: table1Url,

        query: {
            workSite: workSite,
        }
    };

    $("#table1").bootstrapTable('refresh', opt);
}



