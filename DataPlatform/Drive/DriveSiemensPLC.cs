using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Siemens;
using HslCommunication.Core;

namespace DataPlatform.Drive
{
    /// <summary>
    /// PLC通讯类
    /// </summary>
    public class DriveSiemensPLC : IDrive
    {
        SiemensS7Net PLC;

        public int _DeviceId;

        string _DeviceName = "";

        bool _State = false;

        public event Action<int, bool> StateChanged;

        DateTime _reconnectTime;

        public DateTime reconnectTime
        {
            get { return _reconnectTime; }
            set
            {
                _reconnectTime = value;
            }
        }

        public bool State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    StateChanged?.Invoke(_DeviceId, value);
                }
            }
        }


        IReadWriteNet _readWriteNet;

        public string DeviceName => _DeviceName;
        public int DeviceId => _DeviceId;

        public IReadWriteNet readWriteNet
        {
            get => _readWriteNet;
            set => _readWriteNet = value;
        }

        string _IP = "127.0.0.1";

        int _Port = 102;

        byte _Rack = 0;

        byte _Slot = 1;

        string _CPUType = "S7-1200";
        /// <summary>
        /// 实例化PLC对象
        /// </summary>
        /// <param name="CPUType">CPU型号</param>
        /// <param name="IP">IP地址</param>
        /// <param name="Port">端口号</param>
        /// <param name="Rack">机架号</param>
        /// <param name="Slot">槽号</param>
        public DriveSiemensPLC(int deviceId, string deviceName, string CPUType, string IP, int Port, byte Rack = 0, byte Slot = 1)
        {
            this._DeviceId = deviceId;
            _CPUType = CPUType;
            _IP = IP;
            _Port = Port;
            _Rack = Rack;
            _Slot = Slot;
            _DeviceName = deviceName;
        }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns>连接成功返回true，失败返回false</returns>
        public bool Connect()
        {
            PLC = new SiemensS7Net(ChosePLCType(_CPUType))
            {
                IpAddress = _IP,
                Port = _Port,
                Rack = _Rack,
                Slot = _Slot,
            };
            _readWriteNet = PLC;
            PLC.ConnectClose();
            State = PLC.ConnectServer().IsSuccess;
            return State;
        }

        public void Disconnect()
        {
            State = false;
            PLC?.ConnectClose();
        }
        /// <summary>
        /// 转换西门子型号，默认返回S71200
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        SiemensPLCS ChosePLCType(string Type)
        {
            switch (Type)
            {
                case "S7-200": return SiemensPLCS.S200;
                case "S7-Smart200": return SiemensPLCS.S200Smart;
                case "S7-300/400": return SiemensPLCS.S300;
                case "S7-1200": return SiemensPLCS.S1200;
                case "S7-1500": return SiemensPLCS.S1500;
                default: return SiemensPLCS.S1200;
            }
        }
    }
}
