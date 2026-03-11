using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataPlatform.Tools.AddressHelper
{
    public static class SiemensParentAddressHelper
    {
        /// <summary>
        /// 将地址转换为索引
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static (int index, int bitIndex) ParseAddressDB(string address, string parent_config, string type = "浮点数")
        {
            if (type == "布尔值" || type == "Bool")
            {
                var match = Regex.Match(address, @"DB[BDX](\d+)\.(\d+)");
                var config = parent_config.Split(';');
                var match_parent = Regex.Match(config[0], @"DB[BDX](\d+)");
                if (match.Success && match_parent.Success)
                {
                    int index = int.Parse(match.Groups[1].Value);
                    int parent_index = int.Parse(match_parent.Groups[1].Value);
                    int bitIndex = int.Parse(match.Groups[2].Value);
                    return (index - parent_index, bitIndex);
                }
                else return (-1, -1);
            }
            else
            {
                var match = Regex.Match(address, @"DB[BDX](\d+)");
                var config = parent_config.Split(';');
                var match_parent = Regex.Match(config[0], @"DB[BDX](\d+)");
                if (match.Success && match_parent.Success)
                {
                    int index = int.Parse(match.Groups[1].Value);
                    int parent_index = int.Parse(match_parent.Groups[1].Value);
                    return (index - parent_index, 0);
                }
                else return (-1, -1);
            }
        }

        /// <summary>
        /// 将地址转换为索引
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static (int index, int bitIndex) ParseAddressIQ(string address, string parent_config, string type = "浮点数")
        {
            if (type == "布尔值" || type == "Bool")
            {
                var match = Regex.Match(address, @"[IQ](\d+)\.(\d+)");
                var config = parent_config.Split(';');
                var match_parent = Regex.Match(config[0], @"[IQ](\d+)\.(\d+)");
                if (match.Success && match_parent.Success)
                {
                    int index = int.Parse(match.Groups[1].Value);
                    int parent_index = int.Parse(match_parent.Groups[1].Value);
                    int bitIndex = int.Parse(match.Groups[2].Value);
                    return (index - parent_index, bitIndex);
                }
                else return (-1, -1);
            }
            else
            {
                var match = Regex.Match(address, @"IW(\d+)");
                var config = parent_config.Split(';');
                var match_parent = Regex.Match(config[0], @"IW(\d+)");
                if (match.Success && match_parent.Success)
                {
                    int index = int.Parse(match.Groups[1].Value);
                    int parent_index = int.Parse(match_parent.Groups[1].Value);
                    return (index - parent_index, 0);
                }
                else return (-1, -1);
            }
        }

        internal static (string address, ushort length) GetAddress(string input)
        {
            string address = "";
            ushort length = 0;
            string[] parts = input.Split(';');
            if (parts.Length != 2) return (address, length);
            address = parts[0];
            length = ushort.Parse(parts[1]);
            return (address, length);
        }
    }
}
