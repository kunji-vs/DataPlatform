using DataPlatform.Drive;
using DataPlatform.Models.DataBase;
using DataPlatform.Tools.AddressHelper;
using DataPlatform.Tools.ParseBatchReadResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatform.Read
{
    public static class DriveRead
    {
        public static void Read(IDrive drive, IGrouping<string, point> readConfig, device device)
        {
            switch (device.model)
            {
                case "ModBusTCP":
                case "ModBusRTU":
                    ModBusRead(drive, readConfig, device);
                    break;
                case "S7-1200":
                    SiemensRead(drive, readConfig, device);
                    break;
                default:
                    break;
            }
        }

        static void ModBusRead(IDrive drive, IGrouping<string, point> readConfig, device device)
        {
            var config = ModBusParentAddressHelper.GetAddress(readConfig.Key);
            var result = drive.readWriteNet.Read(config.address, config.length);
            if (result.IsSuccess)
            {
                try
                {
                    readConfig.ToList().ForEach(x => x.point_value = ParseBatchReadResultsHelper_Modbus.ParseBatchReadResults(result.Content, x, device));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{drive.DeviceName} 解析错误");
                }
            }
            else
            {
                Console.WriteLine($"{drive.DeviceName}读取失败:{result.Message} 错误代码:{result.ErrorCode}");
                drive.State = result.ErrorCode >= 0;
            }
        }

        static void SiemensRead(IDrive drive, IGrouping<string, point> readConfig, device device)
        {
            var config = SiemensParentAddressHelper.GetAddress(readConfig.Key);
            var result = drive.readWriteNet.Read(config.address, config.length);
            if (result.IsSuccess)
            {
                readConfig.ToList().ForEach(x => x.point_value = ParseBatchReadResultsHelper_Siemens.ParseBatchReadResults(result.Content, x, device));
            }
            else
            {
                Console.WriteLine($"{drive.DeviceName}读取失败:{result.Message}");
                drive.State = result.ErrorCode >= 0;
            }
        }

        public static string Read(IDrive drive, point point)
        {
            switch (point.point_type)
            {
                case "布尔值": return ReadBool(drive, point).ToString();
                case "十六位无符号": return ReadU16(drive, point).ToString();
                case "十六位有符号": return Read16(drive, point).ToString();
                case "三十二位无符号": return ReadU32(drive, point).ToString();
                case "三十二位有符号": return Read32(drive, point).ToString();
                case "浮点数": return ReadFloat(drive, point).ToString();
                case "双精度浮点数": return ReadDouble(drive, point).ToString();
                default: return "";
            }
        }



        static bool ReadBool(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadBool(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return false;
            }
        }

        static ushort ReadU16(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadUInt16(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }


        static short Read16(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadInt16(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }

        static uint ReadU32(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadUInt32(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }

        static int Read32(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadInt32(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }

        static float ReadFloat(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadFloat(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }

        private static double ReadDouble(IDrive drive, point point)
        {
            var result = drive.readWriteNet.ReadDouble(point.point_address);
            if (result.IsSuccess) return result.Content;
            else
            {
                drive.State = result.ErrorCode >= 0;
                return 0;
            }
        }
    }
}
