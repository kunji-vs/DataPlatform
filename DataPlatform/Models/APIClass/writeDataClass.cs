using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatform.Models.APIClass
{
    public class writeDataClass
    {
        public int deviceId { get; set; }

        public string deviceName { get; set; }

        public string address { get; set; }

        public string dataType { get; set; }

        public string writeValue { get; set; }
    }
}
