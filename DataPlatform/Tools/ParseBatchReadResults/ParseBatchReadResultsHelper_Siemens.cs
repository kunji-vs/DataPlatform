using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPlatform.Models.DataBase;
using System.Threading.Tasks;

namespace DataPlatform.Tools.ParseBatchReadResults
{
    public static class ParseBatchReadResultsHelper_Siemens
    {

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
                case "布尔值": return ParseBool(bytes, point._sonIndex, point._bitIndex);
                case "十六位无符号":
                    var valUshort = ParseUInt16(bytes, point._sonIndex, point._bitIndex);
                    if (point.scale_factor == 1) return valUshort.ToString();
                    else return $"{(valUshort * point.scale_factor):F2}";
                case "十六位有符号":
                    var valShort = ParseInt16(bytes, point._sonIndex, point._bitIndex);
                    if (point.scale_factor == 1) return valShort.ToString();
                    else return $"{(valShort * point.scale_factor):F2}";
                case "浮点数":
                    var valFloat = ParseFloat(bytes, point._sonIndex, point._bitIndex);
                    return $"{(valFloat * point.scale_factor):F2}";
                case "字符串":
                    var valString = ParseString(bytes, point._sonIndex, point._bitIndex);
                    return valString;
                default: return "";
            }
        }

        // 解析 Bool 类型
        static string ParseBool(byte[] data, int index, int bitIndex)
        {
            try
            {
                if (index == -1) return "错误的地址";
                byte d = data[index];
                return (((data[index] >> bitIndex) & 1) == 1).ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        // 解析 String 类型
        static string ParseString(byte[] data, int index, int bitIndex)
        {
            if (index < 0 || bitIndex < 0) return "错误的地址";
            int length = data[index + 1];
            if (length <= 0) return "";
            if (index + 2 + length > data.Length)
            {
                return "Invalid string length";
            }

            return Encoding.ASCII.GetString(data, index + 2, length);
        }

        // 解析 Int 类型
        static short ParseInt16(byte[] data, int index, int bitIndex)
        {
            if (index < 0 || bitIndex < 0) return 0;
            // 假设 Int 是 2 字节
            if (index + 1 >= data.Length)
            {
                return 0;
            }

            short value = (short)((data[index] << 8) | data[index + 1]);
            return value;
        }


        static ushort ParseUInt16(byte[] data, int index, int bitIndex)
        {
            if (index < 0 || bitIndex < 0) return 0;
            // 假设 Int 是 2 字节
            if (index + 1 >= data.Length)
            {
                return 0;
            }

            ushort value = (ushort)((data[index] << 8) | data[index + 1]);
            return value;
        }


        static float ParseFloat(byte[] data, int index, int bitIndex)
        {
            if (index < 0 || index + 3 >= data.Length)
                return 0;
            byte[] floatBytes = new byte[4];
            Array.Copy(data, index, floatBytes, 0, 4);
            Array.Reverse(floatBytes);
            float value = BitConverter.ToSingle(floatBytes, 0);
            return value;
        }
    }
}
