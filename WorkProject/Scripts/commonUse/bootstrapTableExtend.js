/**
 * 前提网页使用boostrap table
 * 当文本框获得焦点的时候，自动将表格索引页码恢复到原始状态，
 * 不然会出bug(搜索出的结果/页面小于当前页码值)
 * ...vars不定参数数量
 * @param {txtId} txtId 搜索文本框Id
 * @param {tableId} tableId 表Id,建议只有一个的话统一用 "table"
 * @param {initPageSize} initPageSize 一页数据量
 */
function getBoostrapTableFirstIndex(...vars) {
    let len = vars.length;

    if (len == 3) {
        let txtId = vars[0]; let tableId = vars[1]; let initPageSize = vars[2];
        $("#" + txtId).val("");
        $("#" + tableId).bootstrapTable('refreshOptions', { pageNumber: 1, pageSize: initPageSize });

    } else if (len == 2) {
        let tableId = vars[0]; let initPageSize = vars[1];
        $("#" + tableId).bootstrapTable('refreshOptions', { pageNumber: 1, pageSize: initPageSize });
    } 
}

/**
  * 初始化表格高度
  * @param {toolBarH} toolBarH 表格上端菜单栏的高度
  */
function initTableHeight(toolBarH) {
    //拿到父窗口的main高度(这是iframe子页面拿到父窗口元素的方法，需要根据自己项目所使用的框架自行修改元素的id)
    var panelH = $("#mainMaster", parent.document).height();
    //计算表格container该设置的高度
    var height = panelH - toolBarH - 91;
    $(".fixed-table-container").css({ "height": height });
}
