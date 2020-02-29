const tableUrl = "/api/AttendanceData/GetCost";

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
                    field: 'WorkSiteName',
                    title: '工地',
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
                    title: '加班工时',
                    sortable: true,

                },
                {
                    field: 'totalWork',
                    title: '总工时（天）',
                    sortable: true,

                },
                {
                    field: 'spend',
                    title: '人工费',
                    sortable: true,

                },
                {
                    field: 'WorkQuality',
                    title: '工作日志',
                    sortable: true,

                },
				{
                    field: 'WorkManage',
                    title: '管理员',
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
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();   
    let swage = $("#swage").val(); let bwage = $("#bwage").val();
    temp = {//这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
        limit: params.limit,                         //页面大小
        offset: params.offset,
        sort: params.sort,      //排序列名  
        sortOrder: params.order, //排位命令（desc，asc）
        worker: worker,
        workSite: workSite,
        year: year,
        mon: mon,
        day: day,
        swage: swage,
        bwage: bwage,
    };

    return temp;

};
//按钮click事件函数
function workerInfo() {

    let worker = $("#selectName").val();
    let workSite = $("#selectWorkSite").val();
    let year = $("#selectYear").val();
    let mon = $("#selectMon").val(); let day = $("#selectDay").val();
    let swage = $("#swage").val(); let bwage = $("#bwage").val();
    let opt = {
        url: tableUrl,
        query: {
            worker: worker,
            workSite: workSite,
            year: year,
            mon: mon,
            day: day,
            swage: swage,
            bwage: bwage
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
    let swage = $("#swage").val(); let bwage = $("#bwage").val();
    let excelUrl = "/api/CostExport/GetExportData?worker=" + worker + "&workSite=" + workSite +
        "&year=" + year + "&mon=" + mon + "&day=" + day + "&swageStr=" + swage + "&bwageStr=" + bwage;
    var oReq = new XMLHttpRequest();
    oReq.open("Get", excelUrl, true);
    oReq.responseType = "blob";
    send_data = { 'worker': worker, 'workSite': workSite, 'year': year, 'mon': mon, 'day': day };

    oReq.onload = function () {
        var content = oReq.response;
        var elink = document.createElement('a');
        var fileName = "成本明细_" + workSite + `_${new Date().toLocaleDateString()}.xls`; // 保存的文件名
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




