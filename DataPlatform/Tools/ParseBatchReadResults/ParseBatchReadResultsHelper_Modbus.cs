using DataPlatform.Models.DataBase;
using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatform.Tools.ParseBatchReadResults
{
    /// <summary>
    /// 批量读取结果解析
    /// </summary>
    public static class ParseBatchReadResultsHelper_Modbus
    {
        #region ModBus
        /// <summary>
        /// 对批量结果进行解析
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="point"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static string ParseBatchReadResults(byte[] bytes, point point, device device)
        {
            switch (point.point_type)
            {
                case "布尔值": return ParseBool(bytes, point);
                case "十六位无符号":
                    var valUshort = ParseUInt16(bytes, point, device);
                    if (point.scale_factor == 1) return valUshort.ToString();
                    else return $"{(valUshort * point.scale_factor):F2}";
                case "十六位有符号":
                    var valShort = ParseInt16(bytes, point, device);
                    if (point.scale_factor == 1) return valShort.ToString();
                    else return $"{(valShort * point.scale_factor):F2}";
                case "三十二位有符号":
                    var valInt = ParseInt32(bytes, point, device);
                    if (point.scale_factor == 1) return valInt.ToString();
                    else return $"{(valInt * point.scale_factor):F2}";
                case "三十二位无符号":
                    var valUint = ParseUInt32(bytes, point, device);
                    if (point.scale_factor == 1) return valUint.ToString();
                    return $"{(valUint * point.scale_factor):F2}";
                case "浮点数":
                    var valFloat = ParseFloat(bytes, point, device);
                    return $"{(valFloat * point.scale_factor):F2}";
                default: return "";
            }
        }



        static string ParseBool(byte[] bytes, point point)
        {
            if (point._sonIndex < 0) return "错误的地址";
            int index = point._sonIndex / 8;
            int boolIndex = point._sonIndex % 8;
            return ((bytes[index] & (1 << boolIndex)) != 0).ToString();
        }


        static ushort ParseUInt16(byte[] bytes, point point, device device)
        {
            int index = point._sonIndex;
            if (index < 0 || index + 1 >= bytes.Length)
                return 0;
            ushort value;
            if (device.communication_mode == "LE")
                value = (ushort)((bytes[index] << 8) | bytes[index + 1]);
            else
                value = (ushort)((bytes[index + 1] << 8) | bytes[index]);
            return value;
        }

        static short ParseInt16(byte[] bytes, point point, device device)
        {
            int index = point._sonIndex;
            if (index < 0 || index + 1 >= bytes.Length)
                return 0;
            short value;
            if (device.communication_mode == "LE")
                value = (short)((bytes[index] << 8) | bytes[index + 1]);
            else
                value = (short)((bytes[index + 1] << 8) | bytes[index]);
            return value;
        }

        static int ParseInt32(byte[] bytes, point point, device device)
        {
            int index = point._sonIndex;
            if (index < 0 || index + 3 >= bytes.Length)
                return 0;
            byte[] dataBytes = new byte[4];
            Array.Copy(bytes, index, dataBytes, 0, 4);
            if (device.communication_mode == "LE")
                Array.Reverse(dataBytes);
            int value = BitConverter.ToInt32(dataBytes, 0);
            return value;
        }

        static uint ParseUInt32(byte[] bytes, point point, device device)
        {
            int index = point._sonIndex;
            if (index < 0 || index + 3 >= bytes.Length)
                return 0;
            byte[] dataBytes = new byte[4];
            Array.Copy(bytes, index, dataBytes, 0, 4);
            if (device.communication_mode == "LE")
                Array.Reverse(dataBytes);
            uint value = BitConverter.ToUInt32(dataBytes, 0);
            return value;
        }


        static float ParseFloat(byte[] bytes, point point, device device)
        {
            int index = point._sonIndex;
            if (index < 0 || index + 3 >= bytes.Length)
                return 0;
            //byte[] dataBytes = new byte[4];
            //Array.Copy(bytes, index, dataBytes, 0, 4);
            //if (device.communication_mode == "LE")
            //    Array.Reverse(dataBytes);
            //float value = BitConverter.ToSingle(dataBytes, 0);
            IByteTransform transform = new RegularByteTransform();
            transform.DataFormat = DataFormat.CDAB;
            var value = transform.TransSingle(bytes, index);

            return value;
        }
        #endregion
    }
}
