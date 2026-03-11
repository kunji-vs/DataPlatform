using DataPlatform.Models;
using Opc.Ua;
using Opc.Ua.Client;
using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DataPlatform.Drive
{
    public class DriveOPCUA : IDrive
    {
        private OpcUaClient clinet;

        public int _DeviceId;

        string _DeviceName = "";

        bool _State = false;

        public event Action<int, bool> StateChanged;

        DateTime _reconnectTime;

        IReadWriteNet _readWriteNet;

        ConcurrentDictionary<string, OPCTagClass> _dataCollection
            = new ConcurrentDictionary<string, OPCTagClass>();

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

        public IReadWriteNet readWriteNet
        {
            get => null;
            set => _readWriteNet = null;
        }

        public string DeviceName => _DeviceName;
        public int DeviceId => _DeviceId;
        private string _linkAddress;
        private string _userName;
        private string _passWord;
        private string _cerPath;

        public DriveOPCUA(int deviceId, string deviceName, string linkAddress, string userName, string passWord, string cerPath)
        {
            this._DeviceName = deviceName;
            this._DeviceId = deviceId;
            this._linkAddress = linkAddress;
            this._userName = userName;
            this._passWord = passWord;
            this._cerPath = cerPath;
            clinet = new OpcUaClient();
        }

        public void SetSucSubscription(List<OPCTagClass> tags)
        {
            clinet.RemoveAllSubscription();
            _dataCollection.Clear();

            foreach (var item in tags)
            {
                _dataCollection.AddOrUpdate(
                    item.tagAddress,
                    va => returnNewTag(item.id, item.tagAddress),
                    (k, v) =>
                    {
                        return returnNewTag(item.id, item.tagAddress,item.value,item.serverTimestamp);
                    });
            }
        }

        public OPCTagClass returnNewTag(int id, string address,object value = null,DateTime timestamp=new DateTime())
        {
            return new OPCTagClass()
            {
                id = id,
                tagAddress = address,
                serverTimestamp = DateTime.Now,
                value = value,
                Status = false,
            };
        }

        public bool TryGetData(string key, out OPCTagClass data) => _dataCollection.TryGetValue(key, out data);


        public bool Connect()
        {
            clinet.StateChanged += Clinet_StateChanged;
            clinet.ConnectServer(_linkAddress).Wait();
            if (clinet.ConnectedState)
            {
                clinet?.AddSubscription(DeviceName, _dataCollection.Select(x => x.Key).ToArray(), CallBack);
            }
            return State;
        }

        private void Clinet_StateChanged(bool obj)
        {
            this.State = obj;
        }

        public void Disconnect()
        {
            clinet?.RemoveAllSubscription();
            _dataCollection.Clear();
            clinet?.Disconnect();
            clinet.StateChanged -= Clinet_StateChanged;
        }

        private void CallBack(string arg1, MonitoredItem arg2, MonitoredItemNotificationEventArgs arg3)
        {
            var item = arg3.NotificationValue as MonitoredItemNotification;
            if (item == null) return;
            //Console.WriteLine($"标记名称：{arg2.DisplayName}值：{item.Value.Value}***时间戳：{item.Value.ServerTimestamp}***质量：{item.Value.StatusCode}");
            _dataCollection.AddOrUpdate(
                            arg2.DisplayName,
                            // 键不存在时的添加操作
                            k => new OPCTagClass { tagAddress = k, value = item.Value.Value, serverTimestamp = item.Value.ServerTimestamp, Status = StatusCode.IsGood(item.Value.StatusCode) },
                            // 键已存在时的更新操作
                            (k, existingItem) =>
                            {
                                // 只有当新数据的时间戳更新时才更新值
                                existingItem.value = item.Value.Value;
                                existingItem.serverTimestamp = item.Value.ServerTimestamp;
                                existingItem.Status = StatusCode.IsGood(item.Value.StatusCode);
                                return existingItem;
                            });

        }
    }
}
