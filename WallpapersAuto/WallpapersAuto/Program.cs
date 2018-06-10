using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace WallpapersAuto
{
    class Program
    {
        static void Main(string[] args)
        {
            saveWallpaper();
            deleteWallpapers(180);
        }

        public static string getBingWallpaperURL(int day = 0, string mkt = "zh-CN")
        {
            string bingApiUrl = "https://cn.bing.com/HPImageArchive.aspx?format=hp&n=1&pid=hp&FORM=BEHPTB&video=1&quiz=0&og=1&idx=" + day.ToString() + "&mkt=" + mkt;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(bingApiUrl);
            request.Method = "GET"; request.ContentType = "text/html;charset=UTF-8";
            string xmlDoc;
            //使用using自动注销HttpWebResponse
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                Stream stream = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    xmlDoc = reader.ReadToEnd();
                }
            }
            // 使用正则表达式解析标签（字符串）
            Regex regex = new Regex("\"url\":\"(?<Url>[^\"]*)\"", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(xmlDoc);
            // 取得匹配项列表
            string ImageUrl = "https://cn.bing.com" + collection[0].Groups["Url"].Value;
            if (true)
            {
                ImageUrl = ImageUrl.Replace("1366x768", "1920x1080");
            }
            return ImageUrl;
        }
        public static void saveWallpaper(int day = 0, string imageSavePath = @"E:\wallpapers")
        {
            /*
            #region BING_MARKETS
            string[] BING_MARKETS =
                { "ar-SA"
                , "ar-XA"
                , "bg-BG"
                , "cs-CZ"
                , "da-DK"
                , "de-AT"
                , "de-CH"
                , "de-DE"
                , "el-GR"
                , "en-AU"
                , "en-CA"
                , "en-GB"
                , "en-ID"
                , "en-IE"
                , "en-IN"
                , "en-MX"
                , "en-MY"
                , "en-NZ"
                , "en-PH"
                , "en-SG"
                , "en-US"
                , "en-XA"
                , "en-ZA"
                , "es-AR"
                , "es-CL"
                , "es-ES"
                , "es-MX"
                , "es-US"
                , "es-XL"
                , "et-EE"
                , "fi-FI"
                , "fr-BE"
                , "fr-CA"
                , "fr-CH"
                , "fr-FR"
                , "he-IL"
                , "hr-HR"
                , "hu-HU"
                , "it-IT"
                , "ja-JP"
                , "ko-KR"
                , "lt-LT"
                , "lv-LV"
                , "nb-NO"
                , "nl-BE"
                , "nl-NL"
                , "no-NO"
                , "pl-PL"
                , "pt-BR"
                , "pt-PT"
                , "ro-RO"
                , "ru-RU"
                , "sk-SK"
                , "sl-SL"
                , "sv-SE"
                , "th-TH"
                , "tr-TR"
                , "uk-UA"
                , "zh-CN"
                , "zh-HK"
                , "zh-TW"};
            #endregion
            
            foreach (string mkt in BING_MARKETS)
            {
                string url = getBingWallpaperURL(day,mkt);
                string ext = Path.GetExtension(url);
                //格式化文件名：bing2017816.jpg
                string fileName = imageSavePath + "\\bing" + DateTime.Now.ToString("yyyyMMdd") + "_" + mkt + ext;
                if (!File.Exists(fileName))
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile(url, fileName);
                }
            }
            */

            DateTime dt = DateTime.Now;
            for (int i = -1; i < 8; i++)
            {
                string url = getBingWallpaperURL(i);
                string ext = Path.GetExtension(url);
                //格式化文件名：bing2017816.jpg
                string fileName = imageSavePath + "\\bing" + dt.AddDays(0 - i).ToString("yyyyMMdd") + ext;
                if (!File.Exists(fileName))
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile(url, fileName);
                }
            }
            /*
            setWallpaperApi(fileName);
            */
        }

        public static void deleteWallpapers(int day = 31, string imageSavePath = @"E:\wallpapers")
        {
            DateTime dt = DateTime.Now;
            string[] filelist = Directory.GetFiles(imageSavePath);
            foreach (string filename in filelist)
            {
                string name = Path.GetFileNameWithoutExtension(filename);
                //int days = dt.Subtract(DateTime.ParseExact(name.Substring(4, 8), "yyyyMMdd", null)).Days + 1;
                //Console.WriteLine("距离今天 " + days.ToString() + " 天");
                if (Regex.IsMatch(name, "bing\\d{8}") && dt.Subtract(DateTime.ParseExact(name.Substring(4, 8), "yyyyMMdd", null)).Days + 1 > day)
                {
                    //Console.WriteLine(Path.GetFileNameWithoutExtension(filename));
                    File.Delete(filename);
                }
            }
        }

        //利用系统的用户接口设置壁纸
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int fuWinIni
                );
        public static void setWallpaperApi(string wallpapersPath)
        {
            SystemParametersInfo(20, 1, wallpapersPath, 1);
        }

    }

}
