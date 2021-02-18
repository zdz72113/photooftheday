using AutoMapper;
using DayPhoto.API.Entities;
using DayPhoto.API.Infrastructure.Data;
using DayPhotos.API.Entities;
using DayPhotos.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<CTMHistroy, QdasSensor>(MemberList.Destination)
            //    .ForMember(d => d.QdasTimeTicks, o => o.MapFrom(s => this.ConvertDateTimeToTicks(s.QdasTimeStamp)));

            CreateMap<SystemParameter, SystemParameterDto>(MemberList.Destination);
            CreateMap<Photo, PhotoDto>(MemberList.Destination);
            CreateMap<Page<Photo>, Page<PhotoDto>>(MemberList.Destination);
        }

        private long ConvertDateTimeToTicks(DateTime? time)
        {
            if (time.HasValue)
            {
                DateTime baseTime = Convert.ToDateTime("1970-1-1 00:00:00");
                DateTime utc = TimeZoneInfo.ConvertTimeToUtc(time.Value, TimeZoneInfo.Local);
                return Convert.ToInt64((utc - baseTime).TotalMilliseconds);
            }
            return 0;
        }
    }
}
