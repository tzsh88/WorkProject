autodivheight();
function autodivheight() { //函数：获取尺寸
    
   // var w = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    var h = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    document.getElementById("main").style.height = h  + "px";
}
window.onresize = autodivheight; //浏览器窗口发生变化时同时变化DIV高度