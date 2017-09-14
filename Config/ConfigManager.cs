using System;
using System.Configuration;

namespace Nsfttz.DataAccessLayer.Client.Config
{
    /// <summary>
    /// 配置管理
    /// </summary>
    public class ConfigManager
    {
        public ConfigManager() { }

        public ConfigManager(string key)
            : this()
        {
            Key = key;
        }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 获取应用程序配置
        /// </summary>
        /// <returns></returns>
        public string GetAppSetting()
        {
            if (string.IsNullOrEmpty(Key))
            {
                return string.Empty;
            }
            try
            {
                return ConfigurationManager.AppSettings[Key] ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置应用程序配置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetAppSetting(string value)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return false;
            }
            try
            {
                if (value == null)
                {
                    ConfigurationManager.AppSettings.Remove(Key);
                }
                else
                {
                    ConfigurationManager.AppSettings.Set(Key, value);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置应用程序配置
        /// 文件更改
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFileAppSetting(string value)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return false;
            }
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[Key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取应用程序配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return new ConfigManager(key).GetAppSetting();
        }

        /// <summary>
        /// 设置应用程序配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetAppSetting(string key, string value)
        {
            return new ConfigManager(key).SetAppSetting(value);
        }
    }
}
