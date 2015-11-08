using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class ScheduleTypeDao : IDisposable
    {
        public ScheduleTypeDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-ScheduleTypeDao", e);
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
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="scheduleType">Class instance of PatientArea infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertScheduleType(ScheduleType scheduleType, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO SCHEDULETYPE (NAME,PATIENTID,TIMERANGE,TYPE,COLOR,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@PATIENTID, @TIMERANGE,@TYPE,@COLOR,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = scheduleType.Name;
                    sqlcomm.Parameters.Add("@PATIENTID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTID"].Value = scheduleType.PatientId;
                    sqlcomm.Parameters.Add("@TIMERANGE", DbType.String);
                    sqlcomm.Parameters["@TIMERANGE"].Value = scheduleType.TimeRange;
                    sqlcomm.Parameters.Add("@TYPE", DbType.Int32);
                    sqlcomm.Parameters["@TYPE"].Value = scheduleType.Type;
                    sqlcomm.Parameters.Add("@COLOR", DbType.String);
                    sqlcomm.Parameters["@COLOR"].Value = scheduleType.Color;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = scheduleType.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = scheduleType.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SqlCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as SCHEDULETYPE;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-InsertScheduleType", e);
                return false;
            }
            return true;
        }


        public bool UpdateScheduleType(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update SCHEDULETYPE set ";
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
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-UpdateScheduleType", e);
                return false;
            }
            return true;
        }


        public bool DeleteScheduleType(int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM SCHEDULETYPE WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-DeleteScheduleType", e);
                return false;
            }
            return true;
        }

        public List<ScheduleType> SelectScheduleType(Dictionary<string, object> condition)
        {
            var list = new List<ScheduleType>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from SCHEDULETYPE order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<ScheduleType>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from SCHEDULETYPE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<ScheduleType>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ScheduleTypeDao.cs-SelectScheduleType", e);
                return list;
            }
        }
    }
}
