using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Models
{
    public class ConfigStrings
    {
        public string Bootsrapservers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string ElasticsearchEndpoing { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;

        
    }
}
