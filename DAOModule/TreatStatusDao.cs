﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class TreatStatusDao : IDisposable
    {
        public TreatStatusDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-TreatStatusDao", e);
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
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="treatStatus">Class instance of infectType infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertTreatStatus(TreatStatus treatStatus, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO TREATSTATUS (NAME,ACTIVATED,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@ACTIVATED,@DESCRIPTION,@RESERVED) SET @ID = SCOPE_IDENTITY() ";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    if (treatStatus.Name != null) sqlcomm.Parameters["@NAME"].Value = treatStatus.Name;
                    sqlcomm.Parameters.Add("@ACTIVATED", DbType.Boolean);
                    sqlcomm.Parameters["@ACTIVATED"].Value = treatStatus.Activated;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    if (treatStatus.Description != null)
                        sqlcomm.Parameters["@DESCRIPTION"].Value = treatStatus.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    if (treatStatus.Reserved != null) sqlcomm.Parameters["@RESERVED"].Value = treatStatus.Reserved;
                    sqlcomm.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    scId = (int)sqlcomm.Parameters["@ID"].Value;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-InsertTreatStatus", e);
                return false;
            }
            return true;
        }


        public bool UpdateTreatStatus(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update TREATSTATUS set ";
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
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-UpdateTreatStatus", e);
                return false;
            }
            return true;
        }


        public bool DeleteTreatStatus(Int64 scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM TREATSTATUS WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-DeleteTreatStatus", e);
                return false;
            }
            return true;
        }

        public List<TreatStatus> SelectTreatStatus(Dictionary<string, object> condition)
        {
            var list = new List<TreatStatus>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from TREATSTATUS order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<TreatStatus>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from TREATSTATUS where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<TreatStatus>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatStatusDao.cs-SelectTreatStatus", e);
                return list;
            }
        }
    }
}
