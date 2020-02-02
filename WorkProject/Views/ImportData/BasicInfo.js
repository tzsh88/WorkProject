const tableUrl = "/api/BasicInfoData/GetWorkers";
const table1Url = "/api/BasicInfoData/GetWorkSites";
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
                    field: 'WorkId',
                    title: '身份证',
                    sortable: true,

                },
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

                }
                
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

                }
                
            ]

        });
    };

    return oTableInit;
};
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



