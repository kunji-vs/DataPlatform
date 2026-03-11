using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatform.Tools.AddressHelper
{
    /// <summary>
    /// ModBus地址辅助
    /// </summary>
    public static class ModBusParentAddressHelper
    {
        /// <summary>
        /// 得到批量读取的配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static (string address, ushort length) GetAddress(string input)
        {
            string address = "";
            int start = 0;
            ushort length = 0;
            string[] parts = input.Split(';');
            if (parts.Length != 4) return (address, length);
            address = $"{parts[0]};{parts[1]}";
            foreach (var part in parts)
            {
                // 使用 Split 方法将键值对分割成键和值
                string[] keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    // 根据键名赋值
                    if (key == "p")
                    {
                        start = int.Parse(value);
                    }
                    else if (key == "l")
                    {
                        length = ushort.Parse(value);
                    }
                }
            }
            return ($"{address};{start}", length);
        }

        /// <summary>
        /// 获取测点在批量中的定位
        /// </summary>
        /// <param name="point_address"></param>
        /// <returns></returns>
        public static int ParseConfigIndex(string point_address, string point_type)
        {
            if (string.IsNullOrEmpty(point_address)) return 0;
            if (point_type == "布尔值")
            {
                string[] parts = point_address.Split('.');

                if (parts.Length != 2)
                {
                    Console.WriteLine($"{point_address}测点无法解析");
                    return -1;
                }
                int byteIndex = int.Parse(parts[0]);
                int bitIndex = int.Parse(parts[1]);
                return byteIndex * 8 + bitIndex;
            }
            else
            {
                return int.Parse(point_address);
            }
        }
    }
}
