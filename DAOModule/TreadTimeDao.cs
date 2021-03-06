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
    class TreatTimeDao : IDisposable
    {
        public TreatTimeDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-TreatTimeDao", e);
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
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="TreatTime">Class instance of infectType infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertTreatTime(TreatTime TreatTime, ref int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO TREATTIME (NAME,ACTIVATED,BEGINTIME,ENDTIME,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@ACTIVATED,@BEGINTIME,@ENDTIME,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = TreatTime.Name;
                    sqlcomm.Parameters.Add("@ACTIVATED", DbType.Boolean);
                    sqlcomm.Parameters["@ACTIVATED"].Value = TreatTime.Activated;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = TreatTime.Description;
                    sqlcomm.Parameters.Add("@BEGINTIME", DbType.String);
                    sqlcomm.Parameters["@BEGINTIME"].Value = TreatTime.BeginTime;
                    sqlcomm.Parameters.Add("@ENDTIME", DbType.String);
                    sqlcomm.Parameters["@ENDTIME"].Value = TreatTime.EndTime;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = TreatTime.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as TREATTIME;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-InsertTreatTime", e);
                return false;
            }
            return true;
        }


        public bool UpdateTreatTime(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update TREATTIME set ";
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
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-UpdateTreatTime", e);
                return false;
            }
            return true;
        }


        public bool DeleteTreatTime(Int64 scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM TREATTIME WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-DeleteTreatTime", e);
                return false;
            }
            return true;
        }

        public List<TreatTime> SelectTreatTime(Dictionary<string, object> condition)
        {
            var list = new List<TreatTime>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from TREATTIME order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<TreatTime>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from TREATTIME where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<TreatTime>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTimeDao.cs-SelectTreatTime", e);
                return list;
            }
        }
    }
}
