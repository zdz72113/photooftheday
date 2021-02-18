using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Common
{
    public class Constants
    {
        public class PhotoUrls
        {
            public const string BingUrl = "https://www.bing.com";
            public const string BingPhotoListSubUrl = "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

            public const string NGPhotoListUrl = "https://www.nationalgeographic.com/photography/photo-of-the-day/_jcr_content/.gallery.{0:yyyy-MM}.json";

        }

        public class BaiduTranslate
        {
            public const string AppId = "";
            public const string SecretKey = "";

            public const string Url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";

        }
    }
}
