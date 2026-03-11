using System;
using HslCommunication.Core;
using HslCommunication.ModBus;

namespace DataPlatform.Drive
{
    public class DriveModBusTCP : IDrive
    {
        ModbusTcpNet Device;

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
        int _Port = 502;
        byte _Station = 1;
        bool _AddressStartWithZero = false;
        string _DataFormat = "CDBA";

        public DriveModBusTCP(int deviceId, string deviceName, string ip, int port, byte station, bool isPLCAddress = false, string DataFormat = "CDBA")
        {
            this._DeviceId = deviceId;
            this._IP = ip;
            this._Port = port;
            this._Station = station;
            this._AddressStartWithZero = isPLCAddress;
            this._DataFormat = DataFormat;
            _DeviceName = deviceName;
        }

        public bool Connect()
        {
            Device = new ModbusTcpNet(_IP, _Port, _Station);
            Device.AddressStartWithZero = _AddressStartWithZero;
            Device.DataFormat = GetDataFormat(_DataFormat);
            readWriteNet = Device;
            Device.ConnectClose();
            State = Device.ConnectClose().IsSuccess;
            return State;
        }

        public void Disconnect()
        {
            State = false;
            Device?.ConnectClose();
        }

        internal static DataFormat GetDataFormat(string type)
        {
            switch (type)
            {
                case "ABCD": return DataFormat.ABCD;
                case "BADC": return DataFormat.BADC;
                case "CDAB": return DataFormat.CDAB;
                case "DCBA": return DataFormat.DCBA;
                default: return DataFormat.CDAB;
            }
        }
    }
}
