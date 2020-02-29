const tableUrl = "/api/ImPort/GetCheckImport";//列表数据
const checkRightUrl = "/api/CheckImport/GetMessage";
//const submitUrl = "/api/CheckImport/GetMessage"
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
            sortName: 'wName',//初始化的时候排序的字段
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
            uniqueId: "wName",//每一行的唯一标识，一般为主键列
            showToggle: false,//是否显示详细视图和列表视图的切换按钮
            cardView: false,//是否显示详细视图
            detailView: false,//是否显示父子表
            columns: [
			{
					title : '序号',									
					formatter:function (value, row, index) {
					   //获取每页显示的数量
						let pageSize=$('#table').bootstrapTable('getOptions').pageSize; 
						//获取当前是第几页
						let pageNumber=$('#table').bootstrapTable('getOptions').pageNumber;
						//返回序号，注意index是从0开始的，所以要加上1
						let res=pageSize *(pageNumber - 1) + index + 1;
						return res;
					}
			    },
                {
                    field: 'wSiteName',
                    title: '工地',
                    sortable: true,

                    //formatter: operateFormatter //自定义方法，添加操作按钮
                },
                {
                    field: 'wName',
                    title: '姓名',
                    sortable: true,

                },
                {
                    field: 'wDate',
                    title: '核对日期',
                    sortable: true,

                },
                {
                    field: 'Weather',
                    title: '天气',
                    sortable: true,

                },
                {
                    field: 'work',
                    title: '出勤',
                    sortable: true,

                },
                {
                    field: 'workMore',
                    title: '加班',
                    sortable: true,

                },
                {
                    field: 'workQua',
                    title: '工作日志',
                    sortable: true,

                }
            ]

        });
    };

    return oTableInit;
};

//得到查询的参数
function queryParams(params) {

    let date = $("#date").val();
    let workSite = $("#selectWorkSite").val();
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        workSite: workSite,
        date: date,
        sort: params.sort,      //排序列名  
        sortOrder: params.order //排位命令（desc，asc） 
    };

    return temp;

};

//按钮click事件函数,分工地情况
function workSiteInfo() {

    let date = $("#date").val();
    let workSite = $("#selectWorkSite").val();
    let opt = {
        url: tableUrl,

        query: {
            workSite: workSite,
            date: date
        }
    };

    $("#table").bootstrapTable('refresh', opt);
}


function checkAllRight() {

    if (confirm("确认后，无法更改请核实") == true) {

        let date = $('#date').val();
      
        $.ajax({
            url: checkRightUrl, //所需要的列表接口地址
            type: "get",
            data:
            { 'date': date
            },//发送到服务器的数据，key/value形式
            dataType: "json",
            success: function (data) {

                if (data == "ok") {
                    alert('该稽核日期数据已完成稽核');
                }
                else {
                    alert('请勿重复导入');
                }

            },
            error: function (e) {
                console.log(e.responseText);
            }
        });

    }
    else {
        return;
    }


}

function checkError() {

    if (confirm("填写有误人员信息") == true) {


        $("#check").css("display", "block");
    }
    else {
        $("#check").css("display", "block");
    }


}

function submitCheck() {
    let date = $('#date').val();
    let workSite = $('#workSite').val();
    let workName = $('#workName').val();
    $.ajax({
        url: checkRightUrl, //所需要的列表接口地址
        type: "get",
        data:
        {
            'workName': workName, 'date': date, 'workSite': workSite
        },//发送到服务器的数据，key/value形式
        dataType: "json",
        success: function (data) {

            if (data == "ok") {
                alert('未稽核数据已导入');
            }
            else {
                alert('请勿重复导入');
            }

        },
        error: function (e) {
            console.log(e.responseText);
        }
    });
}