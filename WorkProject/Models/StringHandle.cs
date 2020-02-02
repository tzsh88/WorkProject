using System.Linq;


namespace WorkProject.Models
{
    /// <summary>
    /// 常用的字符串处理
    /// </summary>
    public static class StringHandle
    {

        /// <summary>
        /// 分割固定字符，生产string[]
        /// </summary>
        /// <param name="str">待处理字符串</param>
        /// <param name="specificstr">特定字符</param>
        /// <returns></returns>
        public static string[] StrSplitBySpecificStr(string str, string specificstr)
        {
            char[] c = specificstr.ToArray();
            return str.Split(c).ToArray();
        }
    }
}