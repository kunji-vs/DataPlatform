using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace DataPlatform.Models.DataBase
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("point")]
    public partial class point
    {
           public point(){


           }
           /// <summary>
           /// Desc:自增ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int id {get;set;}

           /// <summary>
           /// Desc:设备ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int device_id {get;set;}

           /// <summary>
           /// Desc:测点名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string point_name {get;set;}

           /// <summary>
           /// Desc:测点地址
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string point_address {get;set;}

           /// <summary>
           /// Desc:测点值
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string point_value {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime update_time {get;set;}
        /// <summary>
        /// Desc:测点数据类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string point_type { get; set; }


        /// <summary>
        /// Desc:批量读取配置
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string parent_config { get; set; }

        /// <summary>
        /// Desc:放大缩小比例
        /// Default:
        /// Nullable:True
        /// </summary>           
        public float scale_factor { get; set; }

        /// <summary>
        /// 测点在批量读取结果中的索引
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int _sonIndex { get; set; }

        /// <summary>
        /// 测点在批量读取结果中的索引
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int _bitIndex { get; set; }
        /// <summary>
        /// 上一次的值
        /// </summary>

        [SugarColumn(IsIgnore = true)]
        public string last_value { get; set; } = string.Empty;
    }
}
