using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Models
{
    public class SystemParameterDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class SystemParameterUpdateDto
    {
        public string Value { get; set; }
        public string Remark { get; set; }
    }
}
