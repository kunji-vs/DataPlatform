using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Core;


namespace DataPlatform.Drive
{
    public interface IDrive
    {
        DateTime reconnectTime { get; }
        string DeviceName { get; }

        int DeviceId { get; }

        /// <summary>
        /// PLC状态
        /// </summary>
        bool State { get; set; }
        /// <summary>
        /// 设备状态发生改变
        /// </summary>
        event Action<int, bool> StateChanged;

        IReadWriteNet readWriteNet { get; }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns>连接成功返回true，失败返回false</returns>
        bool Connect();
        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
    }
}
