﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class ScheduleTemplateDao : IDisposable
    {
        public ScheduleTemplateDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-ScheduleTemplateDao", e);
            }
        }

        public SQLiteConnection SqlConn { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                SqlConn.Close();
                SqlConn.Dispose();
                SqlConn = null;
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="scheduleTemplate">Class instance of PatientArea infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertScheduleTemplate(ScheduleTemplate scheduleTemplate, ref int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO SCHEDULETEMPLATE (PATIENTID,DATE,MON,TUE,WED,THU,FRI,STA,SUN,NMON,NTUE,NWED,NTHU,NFRI,NSTA,NSUN,DESCRIPTION,RESERVED) VALUES 
                        (@PATIENTID,@DATE,@MON,@TUE,@WED,@THU,@FRI,@STA,@SUN,@NMON,@NTUE,@NWED,@NTHU,@NFRI,@NSTA,@NSUN,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@PATIENTID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTID"].Value = scheduleTemplate.PatientId;
                    sqlcomm.Parameters.Add("@DATE", DbType.String);
                    sqlcomm.Parameters["@DATE"].Value = scheduleTemplate.Date;

                    sqlcomm.Parameters.Add("@MON", DbType.Int32);
                    sqlcomm.Parameters["@MON"].Value = scheduleTemplate.Mon;
                    sqlcomm.Parameters.Add("@TUE", DbType.String);
                    sqlcomm.Parameters["@TUE"].Value = scheduleTemplate.Tue;
                    sqlcomm.Parameters.Add("@WED", DbType.String);
                    sqlcomm.Parameters["@WED"].Value = scheduleTemplate.Wed;
                    sqlcomm.Parameters.Add("@THU", DbType.String);
                    sqlcomm.Parameters["@THU"].Value = scheduleTemplate.Thu;
                    sqlcomm.Parameters.Add("@FRI", DbType.String);
                    sqlcomm.Parameters["@FRI"].Value = scheduleTemplate.Fri;
                    sqlcomm.Parameters.Add("@STA", DbType.String);
                    sqlcomm.Parameters["@STA"].Value = scheduleTemplate.Sta;
                    sqlcomm.Parameters.Add("@SUN", DbType.String);
                    sqlcomm.Parameters["@SUN"].Value = scheduleTemplate.Sun;

                    sqlcomm.Parameters.Add("@NMON", DbType.String);
                    sqlcomm.Parameters["@NMON"].Value = scheduleTemplate.NMon;
                    sqlcomm.Parameters.Add("@NTUE", DbType.String);
                    sqlcomm.Parameters["@NTUE"].Value = scheduleTemplate.NTue;
                    sqlcomm.Parameters.Add("@NWED", DbType.String);
                    sqlcomm.Parameters["@NWED"].Value = scheduleTemplate.NWed;
                    sqlcomm.Parameters.Add("@NTHU", DbType.String);
                    sqlcomm.Parameters["@NTHU"].Value = scheduleTemplate.NThu;
                    sqlcomm.Parameters.Add("@NFRI", DbType.String);
                    sqlcomm.Parameters["@NFRI"].Value = scheduleTemplate.NFri;
                    sqlcomm.Parameters.Add("@NSTA", DbType.String);
                    sqlcomm.Parameters["@NSTA"].Value = scheduleTemplate.NSta;
                    sqlcomm.Parameters.Add("@NSUN", DbType.String);
                    sqlcomm.Parameters["@NSUN"].Value = scheduleTemplate.NSun;

                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = scheduleTemplate.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = scheduleTemplate.Reserved;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as SCHEDULETEMPLATE;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-InsertScheduleTemplate", e);
                return false;
            }
            return true;
        }


        public bool UpdateScheduleTemplate(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update SCHEDULETEMPLATE set ";
                    var parameters = new Dictionary<string, object>();
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", ",", fields, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf(","));
                    sqlcommand += " where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcomm.CommandText = sqlcommand;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-UpdateScheduleTemplate", e);
                return false;
            }
            return true;
        }


        public bool DeleteScheduleTemplate(int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM SCHEDULETEMPLATE WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-DeleteScheduleTemplate", e);
                return false;
            }
            return true;
        }

        public List<ScheduleTemplate> SelectScheduleTemplate(Dictionary<string, object> condition)
        {
            var list = new List<ScheduleTemplate>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from SCHEDULETEMPLATE order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<ScheduleTemplate>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from SCHEDULETEMPLATE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<ScheduleTemplate>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-SelectScheduleTemplate", e);
                return list;
            }
        }
    }
}