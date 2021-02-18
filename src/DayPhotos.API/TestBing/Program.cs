using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestBing
{
    class Program
    {
        const string BingUrl = "https://cn.bing.com";
        const string BingPhotoListSubUrl = "/HPImageArchive.aspx?format=js&idx=0&n=1";

        const string NGPhotoListUrl = "https://www.nationalgeographic.com/photography/photo-of-the-day/_jcr_content/.gallery.{0:yyyy-MM}.json";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start to get daily photo from Bing!");

            //Test Bing
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BingUrl);
                var response = await httpClient.GetAsync(BingPhotoListSubUrl);
                if (!response.IsSuccessStatusCode)
                {

                }
                var responseString = await response.Content.ReadAsStringAsync();
                var bingImageOutput = JsonConvert.DeserializeObject<BingImageOutput>(responseString);

                if (bingImageOutput != null && bingImageOutput.Images != null)
                {
                    foreach (var bingImage in bingImageOutput.Images)
                    {
                        var imageOutput = new ImageOutput();
                        var date = DateTime.Today;
                        DateTime.TryParseExact(bingImage.Date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out date);
                        imageOutput.Date = date;
                        imageOutput.Url = BingUrl + bingImage.Url;
                        imageOutput.Title = bingImage.Title;
                        imageOutput.Description = "";
                        imageOutput.Copyright = bingImage.Copyright;
                    }
                }
            }

            //Test National Geographic
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(string.Format(NGPhotoListUrl, DateTime.Today));
                if (!response.IsSuccessStatusCode)
                {

                }
                var responseString = await response.Content.ReadAsStringAsync();
                var ngImageOutput = JsonConvert.DeserializeObject<NGImageOutput>(responseString);

                if (ngImageOutput != null && ngImageOutput.Images != null)
                {
                    foreach (var ngImage in ngImageOutput.Images)
                    {
                        var imageOutput = new ImageOutput();
                        //var date = DateTime.Today;
                        //DateTime.TryParseExact(ngImage.Date, "MMMM dd, yyyy", null, System.Globalization.DateTimeStyles.None, out date);
                        imageOutput.Date = ngImage.Date;
                        imageOutput.Url = ngImage.ImageDetail.Url;
                        imageOutput.Title = ngImage.ImageDetail.Title;
                        imageOutput.Description = ngImage.ImageDetail.Description;
                        imageOutput.Copyright = ngImage.ImageDetail.Copyright;
                    }
                }
            }

            //Test baidu translate
            string q = "This is a test (@Roy)";
            string from = "en";
            string to = "zh";
            string appId = "";
            string secretKey = "";
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + System.Web.HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BaiduTranslateOutput>(responseString);
                var bb = System.Web.HttpUtility.HtmlDecode(result.TransResult[0].Dst);
            }

            Console.ReadKey();
        }

        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

    }

    //{"from":"en","to":"zh","trans_result":[{"src":"This is a test (@Roy)","dst":"\u8fd9\u662f\u4e00\u4e2a\u6d4b\u8bd5\uff08@Roy\uff09"}]}
    public class BaiduTranslateOutput
    {
        [JsonProperty("trans_result")]
        public List<BaiduTranslateDetailOutput> TransResult { get; set; }
    }

    public class BaiduTranslateDetailOutput
    {
        [JsonProperty("dst")]
        public string Dst { get; set; }
    }
    public class ImageOutput
    {
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
    }

    public class BingImageOutput
    {
        [JsonProperty("images")]
        public List<BingImageDetail> Images { get; set; }
    }

    public class BingImageDetail
    {
        [JsonProperty("enddate")]
        public string Date { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }
    }

    public class NGImageOutput
    {
        [JsonProperty("items")]
        public List<NGImageItem> Images { get; set; }
    }

    public class NGImageItem
    {
        [JsonProperty("publishDate")]
        [JsonConverter(typeof(NGDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("image")]
        public NGImageDetail ImageDetail { get; set; }
    }

    public class NGImageDetail
    {
        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("caption")]
        public string Description { get; set; }

        [JsonProperty("credit")]
        public string Copyright { get; set; }
    }

    public class NGDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "MMMM d, yyyy" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }
}
