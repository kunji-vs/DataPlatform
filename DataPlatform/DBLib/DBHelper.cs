using System;
using System.Collections.Generic;
using System.Text;
using DataPlatform.Models.DataBase;
using SqlSugar;
using DataPlatform.Log;

namespace DataPlatform.DBLib
{
    public class DBHelper
    {
        SqlSugarScope _db;

        public DBHelper(string connstr)
        {
            _db = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = connstr,
                IsAutoCloseConnection = true
            });
        }


        public bool GetConnectState()
        {
            try
            {
                var result = _db.Ado.GetInt("SELECT 1");
                return result == 1;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public void CreateClassFiles(string[] tableNames)
        {
            try
            {
                _db.DbFirst
                .Where(tableNames)
                .IsCreateAttribute()
                .CreateClassFile("D:\\project\\DataPlatform\\DataPlatform\\DataPlatform\\Models\\", "DataPlatform.Models");
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取设备配置
        /// </summary>
        /// <returns></returns>
        public List<device> GetDevices()
        {
            return _db.Queryable<device>().Includes(d => d.Points).ToList();
        }

        /// <summary>
        /// 更新测点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public int UpdatePoint(List<point> points)
        {
            try
            {
                return _db.Updateable(points)
                    .UpdateColumns(x => new { x.point_value, x.update_time })
                    .ExecuteCommand();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("更新测点失败", ex);
                return -1;
            }
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int UpdateDeviceState(int deviceId, bool state)
        {
            int runState = state ? 1 : 0;
            return _db.Updateable<device>()
                .SetColumns(device => new device() { device_state = runState })
                .Where(x => x.id == deviceId).ExecuteCommand();
        }

    }
}
