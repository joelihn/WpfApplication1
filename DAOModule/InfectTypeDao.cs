using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class InfectTypeDao : IDisposable
    {

        public InfectTypeDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-InfectTypeDao", e);
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
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="infectType">Class instance of infectType infomation</param>
        /// <param name="scinfectTypeId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertInfectType(InfectType infectType, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO INFECTTYPE ([NAME],[DESCRIPTION],[RESERVED]) VALUES 
                        (@NAME,@DESCRIPTION,@RESERVED) SET @ID = SCOPE_IDENTITY() ";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    if (infectType.Name != null) sqlcomm.Parameters["@NAME"].Value = infectType.Name;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    if (infectType.Description != null)
                        sqlcomm.Parameters["@DESCRIPTION"].Value = infectType.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    if (infectType.Reserved != null) sqlcomm.Parameters["@RESERVED"].Value = infectType.Reserved;
                    sqlcomm.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    scId = (int)sqlcomm.Parameters["@ID"].Value;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-InsertInfectType", e);
                return false;
            }
            return true;
        }


        public bool UpdateInfectType(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update INFECTTYPE set ";
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
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-UpdateInfectType", e);
                return false;
            }
            return true;
        }


        public bool DeleteInfectType(Int64 scInfectTypeId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM INFECTTYPE WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scInfectTypeId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-DeleteInfectType", e);
                return false;
            }
            return true;
        }

        public List<InfectType> SelectInfectType(Dictionary<string, object> condition)
        {
            var list = new List<InfectType>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from INFECTTYPE order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<InfectType>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from INFECTTYPE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<InfectType>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("InfectTypeDao.cs-SelectInfectType", e);
                return list;
            }
        }
    }
}
