using DataPlatform.DBLib;
using DataPlatform.Drive;
using DataPlatform.Log;
using DataPlatform.Read;
using DataPlatform.Models.DataBase;
using DataPlatform.Tools.AddressHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataPlatform.Models;

namespace DataPlatform
{
    public class MainStart
    {

        /// <summary>
        /// 设备列表
        /// </summary>
        List<device> Devices;
        /// <summary>
        /// 取消线程Token;
        /// </summary>
        CancellationTokenSource cts;
        /// <summary>
        /// 读取线程
        /// </summary>
        List<Task> ReadTasks;
        /// <summary>
        /// 设备读取驱动
        /// </summary>
        List<IDrive> Drives;

        DBHelper DBHelper;

        public MainStart(DBHelper DBHelper)
        {
            this.DBHelper = DBHelper;
            Devices = DBHelper.GetDevices();
        }

        public void Start()
        {
            ReadTasks = new List<Task>();
            Drives = new List<IDrive>();
            cts = new CancellationTokenSource();
            foreach (var item in Devices)
            {
                var task = Task.Run(() => DoWork(item));
                ReadTasks.Add(task);
            }
        }

        /// <summary>
        /// 开始执行读取
        /// </summary>
        /// <param name="device"></param>
        private void DoWork(device device)
        {
            if(device.model.ToLower() == "opc ua")
            {
                OPCUADoWork(device);
            }
            else
            {
                DeviceDoWork(device);
            }
        }
        /// <summary>
        /// OPCUA设备读取
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        void OPCUADoWork(device device)
        {
            DriveOPCUA drive;
            drive = new DriveOPCUA(device.id, device.device_name, device.link_address, device.user_name, device.pass_word, device.certificate_path);
            drive.StateChanged += Drive_StateChanged;
            Drives.Add(drive);
            var tags = new List<OPCTagClass>();
            foreach (var item in device.Points)
            {
                tags.Add(new OPCTagClass()
                {
                    id = item.id,
                    value = item.point_value,
                    serverTimestamp = item.update_time,
                    Status = false,
                    tagAddress = item.point_address
                });
            }
            drive.SetSucSubscription(tags);
            while(!cts.IsCancellationRequested)
            {
                if(drive.State)
                {
                    foreach (var item in device.Points)
                    {
                        var success = drive.TryGetData(item.point_address, out OPCTagClass data);
                        if(data.Status)
                        {
                            item.point_value = data.value.ToString();
                            item.update_time = DateTime.Now;
                        }
                    }
                    if (drive.State)
                    {
                        DBHelper.UpdatePoint(device.Points);
                        Thread.Sleep(device.collection_interval);
                    }
                    else
                    {
                        drive.Disconnect();
                    }
                }
                else
                {
                    drive.Connect();
                    if (drive.State)
                    {
                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine($"{drive.DeviceName}连接成功");
                    }
                }
            }
        }

        /// <summary>
        /// HSL驱动读取
        /// </summary>
        /// <param name="device"></param>
        void DeviceDoWork(device device)
        {
            IDrive drive;
            if (new string[] { "ModBusRTU", "ModBusTCP" }.Contains(device.model))
                device.Points.ForEach(x => x._sonIndex = ModBusParentAddressHelper.ParseConfigIndex(x.point_address, x.point_type));
            if (new string[] { "S7-1200" }.Contains(device.model))
            {
                device.Points.ForEach(x =>
                {
                    if (x.point_address.StartsWith("DB"))
                    {
                        var addressInfo = SiemensParentAddressHelper.ParseAddressDB(x.point_address, x.parent_config, x.point_type);
                        x._sonIndex = addressInfo.index;
                        x._bitIndex = addressInfo.bitIndex;
                    }
                    else if (x.point_address.StartsWith("I") || x.point_address.StartsWith("Q"))
                    {
                        var addressInfo = SiemensParentAddressHelper.ParseAddressIQ(x.point_address, x.parent_config, x.point_type);
                        x._sonIndex = addressInfo.index;
                        x._bitIndex = addressInfo.bitIndex;
                    }
                    else
                    {
                        x._sonIndex = -1;
                        x._bitIndex = -1;
                    }
                });
            }
            switch (device.model)
            {
                case "ModBusRTU":
                    drive = new DriveModBusRtuOverTCP(device.id, device.device_name, device.ip, device.port, 1); break;
                case "ModBusTCP":
                    drive = new DriveModBusTCP(device.id, device.device_name, device.ip, device.port, 2, true); break;
                case "S7-1200":
                    drive = new DriveSiemensPLC(device.id, device.device_name, device.model, device.ip, device.port); break;
                default: LogHelper.WriteInfo($"{device.device_name}是不支持的设备类型：{device.model}"); return;
            }
            Drives.Add(drive);
            drive.StateChanged += Drive_StateChanged;
            Console.WriteLine($"{device.device_name}开始进行采集");
            var pointConfig = device.Points.GroupBy(x => x.parent_config);
            while (!cts.IsCancellationRequested)
            {
                if (drive.State)
                {
                    foreach (var config in pointConfig)
                    {
                        if (config.Key == null || config.Key == "")
                        {
                            foreach (var item in config.ToList())
                            {
                                item.point_value = DriveRead.Read(drive, item);
                                if (!drive.State)
                                {
                                    Console.WriteLine(DateTime.Now);
                                    Console.WriteLine($"{drive.DeviceName}读取{item.point_name}失败，导致设备掉线");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            DriveRead.Read(drive, config, device);
                            Thread.Sleep(100);
                        }
                    }
                    if (drive.State)
                    {
                        device.Points.ForEach(x => x.update_time = System.DateTime.Now);
                        DBHelper.UpdatePoint(device.Points);
                        Thread.Sleep(device.collection_interval);
                    }
                    else
                    {
                        drive.Disconnect();
                    }
                    var timespan = DateTime.Now - device.Points[0].update_time;
                    if (timespan.TotalMinutes > 5)
                    {
                        Console.WriteLine($"{device.device_name} 测点长时间未更新，强制重新连接中");
                        drive.Disconnect();
                    }
                }
                else
                {
                    drive.Connect();
                    if (drive.State)
                    {
                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine($"{drive.DeviceName}连接成功");
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void Drive_StateChanged(int deviceId, bool state)
        {
            DBHelper.UpdateDeviceState(deviceId, state);
        }


        public void Stop()
        {
            LogHelper.WriteInfo("正在采集等待结束");
            cts.Cancel();
            Task.WaitAll(ReadTasks.ToArray());
            LogHelper.WriteInfo("正在断开设备连接");
            if (Drives != null && Drives.Count > 0)
            {
                foreach (var item in Drives)
                {
                    item.Disconnect();
                }
            }
        }
    }
}
