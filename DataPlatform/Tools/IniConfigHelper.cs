using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataPlatform.Tools
{
    public class IniConfigHelper
    {
        private readonly string _filePath;
        private readonly Dictionary<string, Dictionary<string, string>> _sections;

        /// <summary>
        /// 初始化INI配置帮助类
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        public IniConfigHelper(string filePath)
        {
            _filePath = filePath;
            _sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            Load();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public void Load()
        {
            _sections.Clear();

            if (!File.Exists(_filePath))
                return;

            string currentSection = "General";
            _sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(_filePath))
            {
                string trimmedLine = line.Trim();

                // 跳过注释和空行
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;

                // 检查是否是新的节
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();

                    if (!_sections.ContainsKey(currentSection))
                        _sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    continue;
                }

                // 解析键值对
                int equalsPos = trimmedLine.IndexOf('=');
                if (equalsPos > 0)
                {
                    string key = trimmedLine.Substring(0, equalsPos).Trim();
                    string value = trimmedLine.Substring(equalsPos + 1).Trim();

                    _sections[currentSection][key] = value;
                }
            }
        }
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfig(string parent,string key)
        {
            var success = _sections.TryGetValue(parent, out var dic);
            if (success)
            {
                if (dic.TryGetValue(key, out var value))
                {
                    return value;
                }
                else return string.Empty;
            }
            else return string.Empty;
        }
    }
}
