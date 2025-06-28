using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Catalogs.Application.DTOs
{
    public class CreateMediaDTO
    {
        [JsonIgnore]
        public string? Url { get; set; }
        
        [JsonIgnore]
        public Guid MediaTypeId { get; set; }
    }
}
