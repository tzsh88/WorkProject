var uploadedTypeURL = "/JsonData/Config/yearsArr.json";


function years_append(emementId) {
    $.getJSON(uploadedTypeURL, function (data) {
        let listPayType = "";
        for (let i = 0; i < data.length; i++) {
            listPayType += "<option value='" + data[i] + "'>" + data[i] + "</option>";

        }
        $(emementId).append(listPayType);//append 添加进去并展示

        //不能删下面俩行不然在ie下有问题
        $(emementId).selectpicker('refresh');
        $(emementId).selectpicker('render');


    })
};
