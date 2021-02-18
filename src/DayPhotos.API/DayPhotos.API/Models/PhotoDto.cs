using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhotos.API.Models
{
    public class QueryPhotoDto
    {
        public PhotoSource Source { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }

    public class PhotoDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public PhotoSource Source { get; set; }
    }

    public enum PhotoSource
    {
        Bing = 1,
        NationalGeographic = 2
    }
}
