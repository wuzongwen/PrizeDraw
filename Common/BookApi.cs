using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{
    public static class BookApi
    {
        /// <summary>
        /// 根据ISBN码从豆瓣API获取书籍详细信息
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="bookInfo"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool getBookInfo(string isbn, out BookInfo bookInfo)
        {
            try
            {
                //豆瓣API
                string uri = "https://api.douban.com/v2/book/isbn/" + isbn;
                //获取书籍详细信息，Json格式
                string json = doGet(uri, "utf-8");
                //将获取到的Json格式的文件转换为定义的类
                bookInfo = toMap(json);
                return true;
            }
            catch
            {
                //信息获取失败
                bookInfo = null;
                return false;
            }
        }

        /// <summary>
        /// HTTP的GET请求，获取书籍详细信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private static string doGet(string url, string charset)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null)
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding(charset));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            throw new Exception();
        }


        /// <summary>
        /// Json解析
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private static BookInfo toMap(string jsonString)
        {
            try
            {
                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                return JsonConvert.DeserializeObject<BookInfo>(jsonString);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 豆瓣API返回的信息很多，可以根据自己的需要重新定义该类
    /// </summary>
    public class BookInfo
    {
        //标题
        public string title { get; set; }
        //作者
        public string[] author { get; set; }
        //出版社
        public string publisher { get; set; }
        //封面图片的url
        public string image { get; set; }
        //isbn10
        public string isbn10 { get; set; }
        //isbn13
        public string isbn13 { get; set; }
        //出版日期
        public string pubdate { get; set; }
        //概述
        public string summary { get; set; }
        //页数
        public string pages { get; set; }
        //价格
        public string price { get; set; }
        //获取失败的返回信息
        public string msg { get; set; }
        public string code { get; set; }
    }
}
