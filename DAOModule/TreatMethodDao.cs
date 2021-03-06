﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class TreatMethodDao : IDisposable
    {
        public TreatMethodDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-TreatMethodDao", e);
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
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="treatMethod">Class instance of infectType infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertTreatMethod(TreatMethod treatMethod, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO TREATMETHOD (NAME,SINGLEPUMP,DOUBLEPUMP,DESCRIPTION,RESERVED,BGCOLOR,ISAVAILABLE) VALUES 
                        (@NAME,@SINGLEPUMP,@DOUBLEPUMP,@DESCRIPTION,@RESERVED,@BGCOLOR,@ISAVAILABLE) SET @ID = SCOPE_IDENTITY() ";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    if (treatMethod.Name != null) sqlcomm.Parameters["@NAME"].Value = treatMethod.Name;
                    sqlcomm.Parameters.Add("@SINGLEPUMP", DbType.Boolean);
                    sqlcomm.Parameters["@SINGLEPUMP"].Value = treatMethod.SinglePump;
                    sqlcomm.Parameters.Add("@DOUBLEPUMP", DbType.Boolean);
                    sqlcomm.Parameters["@DOUBLEPUMP"].Value = treatMethod.DoublePump;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    if (treatMethod.Description != null)
                        sqlcomm.Parameters["@DESCRIPTION"].Value = treatMethod.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    if (treatMethod.Reserved != null) sqlcomm.Parameters["@RESERVED"].Value = treatMethod.Reserved;
                    sqlcomm.Parameters.Add("@BGCOLOR", DbType.String);
                    sqlcomm.Parameters["@BGCOLOR"].Value = treatMethod.BgColor;
                    sqlcomm.Parameters.Add("@ISAVAILABLE", DbType.Boolean);
                    sqlcomm.Parameters["@ISAVAILABLE"].Value = treatMethod.IsAvailable;
                    sqlcomm.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    scId = (int)sqlcomm.Parameters["@ID"].Value;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-InsertTreatMethod", e);
                return false;
            }
            return true;
        }


        public bool UpdateTreatMethod(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update TREATMETHOD set ";
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
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-UpdateTreatMethod", e);
                return false;
            }
            return true;
        }


        public bool DeleteTreatMethod(Int64 scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM TREATMETHOD WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-DeleteTreatMethod", e);
                return false;
            }
            return true;
        }

        public List<TreatMethod> SelectTreatMethod(Dictionary<string, object> condition)
        {
            var list = new List<TreatMethod>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from TREATMETHOD order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<TreatMethod>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from TREATMETHOD where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<TreatMethod>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatMethodDao.cs-SelectTreatMethod", e);
                return list;
            }
        }
    }
}
