using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class ScheduleTemplateDao : IDisposable
    {
        public ScheduleTemplateDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-ScheduleTemplateDao", e);
            }
        }

        public SqlConnection SqlConn { get; set; }

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
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO SCHEDULETEMPLATE (PATIENTID,DATE,AMPME,METHOD,DESCRIPTION,RESERVED,ISTEMP) VALUES 
                        (@PATIENTID,@DATE,@AMPME,@METHOD,@DESCRIPTION,@RESERVED,@ISTEMP,@ISAUTO)";
                    sqlcomm.Parameters.Add("@PATIENTID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTID"].Value = scheduleTemplate.PatientId;
                    sqlcomm.Parameters.Add("@DATE", DbType.String);
                    sqlcomm.Parameters["@DATE"].Value = scheduleTemplate.Date;
                    sqlcomm.Parameters.Add("@AMPME", DbType.String);
                    sqlcomm.Parameters["@AMPME"].Value = scheduleTemplate.AmPmE;
                    sqlcomm.Parameters.Add("@METHOD", DbType.String);
                    sqlcomm.Parameters["@METHOD"].Value = scheduleTemplate.Method;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = scheduleTemplate.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = scheduleTemplate.Reserved;
                    sqlcomm.Parameters.Add("@BEDID", DbType.Int32);
                    sqlcomm.Parameters["@BEDID"].Value = scheduleTemplate.BedId;
                    sqlcomm.Parameters.Add("@ISTEMP", DbType.Boolean);
                    sqlcomm.Parameters["@ISTEMP"].Value = scheduleTemplate.IsTemp;
                    sqlcomm.Parameters.Add("@ISAUTO", DbType.Boolean);
                    sqlcomm.Parameters["@ISAUTO"].Value = scheduleTemplate.IsAuto;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SqlCommand comm = SqlConn.CreateCommand();
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
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
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

        public bool UpdateScheduleTemplate1(string fields, string condition, DateTime begin)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update SCHEDULETEMPLATE set BedId =";
                    sqlcommand += fields;
                    sqlcommand += " where BedId =";
                    sqlcommand += condition;
                    string str = " and Date >= '" + begin.Date.ToString("yyyy-MM-dd") + "' ";
                    sqlcommand += str;
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
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
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

        public bool DeleteScheduleTemplate2(int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM SCHEDULETEMPLATE WHERE PatientId = @PatientId";
                    sqlcomm.Parameters.Add("@PatientId", DbType.Int32);
                    sqlcomm.Parameters["@PatientId"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTemplateDao.cs-DeleteScheduleTemplate2", e);
                return false;
            }
            return true;
        }

        public List<ScheduleTemplate> SelectScheduleTemplate(Dictionary<string, object> condition)
        {
            var list = new List<ScheduleTemplate>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from SCHEDULETEMPLATE order by Date desc;";
                        list = DatabaseOp.ExecuteQuery<ScheduleTemplate>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from SCHEDULETEMPLATE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by Date desc";
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
