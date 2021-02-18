using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhotos.API.Models
{
    public class ExternalPhotoDto
    {
        #region Bing
        public class BingImageOutput
        {
            [JsonProperty("images")]
            public List<BingImageDetail> Images { get; set; }
        }

        public class BingImageDetail
        {
            [JsonProperty("enddate")]
            [JsonConverter(typeof(BingDateTimeConverter))]
            public DateTime Date { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("copyright")]
            public string Copyright { get; set; }
        }

        #endregion

        #region NationalGeographic

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

        #endregion
    }

    public class NGDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "MMMM d, yyyy" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }

    public class BingDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }

    public class BaiduTranslateOutput
    {
        [JsonProperty("trans_result")]
        public List<BaiduTranslateOutputDetail> TransResult { get; set; }
    }

    public class BaiduTranslateOutputDetail
    {
        [JsonProperty("dst")]
        public string Dst { get; set; }
    }
}
