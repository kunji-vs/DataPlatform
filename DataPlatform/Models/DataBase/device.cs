using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace DataPlatform.Models.DataBase
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("device")]
    public partial class device
    {
           public device(){


           }
           /// <summary>
           /// Desc:主键自增
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int id {get;set;}

           /// <summary>
           /// Desc:设备名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string device_name {get;set;}

           /// <summary>
           /// Desc:设备类型
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string model {get;set;}

           /// <summary>
           /// Desc:IP地址
           /// Default:127.0.0.1
           /// Nullable:False
           /// </summary>           
           public string ip {get;set;}

           /// <summary>
           /// Desc:端口号
           /// Default:502
           /// Nullable:False
           /// </summary>           
           public int port {get;set;}

           /// <summary>
           /// Desc:连接地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string link_address {get;set;}

           /// <summary>
           /// Desc:登录用户名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string user_name {get;set;}

           /// <summary>
           /// Desc:登录密码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string pass_word {get;set;}

           /// <summary>
           /// Desc: 证书路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string certificate_path {get;set;}

           /// <summary>
           /// Desc:设备状态，0离线；1在线
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int device_state {get;set;}

        /// <summary>
        /// Desc:通讯方式
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string communication_mode { get; set; }

        /// <summary>
        /// Desc:采集间隔
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int collection_interval { get; set; }
        /// <summary>
        /// 测点
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(point.device_id))]
        public List<point> Points { get; set; }

    }
}
