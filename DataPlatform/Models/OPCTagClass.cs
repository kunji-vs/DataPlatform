using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatform.Models
{
    public class OPCTagClass
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 节点地址
        /// </summary>
        public string tagAddress { get; set; }
        /// <summary>
        /// 节点值
        /// </summary>
        public object value { get; set; }
        /// <summary>
        /// 服务器时间戳
        /// </summary>
        public DateTime serverTimestamp { get; set; }
        /// <summary>
        /// 质量
        /// </summary>
        public bool Status { get; set; }
    }
}
