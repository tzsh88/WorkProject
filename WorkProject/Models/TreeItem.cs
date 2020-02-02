using System.Collections.Generic;
using System.Linq;


namespace WorkProject.Models
{
    /// <summary>
    /// 生产菜单栏的树形结构数据
    /// </summary>
    public class TreeItem
    {
        public string id;
        public string pid;
        public string name;
        public string url;

    }

    public class JieDian
    {
        public string name = "";
        public string url = "";
        public string id = "";
        public JieDian[] children = null;


    }
    public class Cliet
    {
        WorkDataClassesDataContext tt = new WorkDataClassesDataContext();//linq to sql 类

        //根据pid获取相应的子目录集合
        public TreeItem[] GetTheItems(string pid)
        {
            int userRoleRank = UserSessionInfo.UserRoleRank();

            //根据父节点获取选项集合
            var data = from n in tt.Function
                       //用户可以查看比自己当前级别数字小的对应目录，数字越大权限越大
                       where n.LineN <=userRoleRank && n.ParentNode == pid && n.Show == 1
                       orderby n.FunctionID
                       select n;

            List<TreeItem> items = new List<TreeItem>();
            foreach (var function in data)
            {
                TreeItem i = new TreeItem();
                i.id = function.FunctionID;
                i.name = function.FunctionName;
                i.pid = function.ParentNode;
                i.url = function.Address;
                items.Add(i);
            }

            return items.ToArray();
            //返回

        }


        //生成树的方法
        public void creatTheTree(string pid, JieDian jd)
        {
            //获取
            TreeItem[] items = GetTheItems(pid);
            if (items.Length == 0)
                return;    //如果没有子节点了，那就返回空
            List<JieDian> jdList = new List<JieDian>();
            for (int i = 0; i < items.Length; i++)
            {
                JieDian jiedian = new JieDian();
                jiedian.id= items[i].id;
                jiedian.name = items[i].name;
                jiedian.url = items[i].url;
                creatTheTree(items[i].id.ToString(), jiedian);
                jdList.Add(jiedian);
            }
            jd.children = jdList.ToArray();    //由于对象是引用类型，因为可以改变参数的值
        }
    }
}