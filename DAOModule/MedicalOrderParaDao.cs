using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class MedicalOrderParaDao : IDisposable
    {
        public MedicalOrderParaDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderParaDao.cs-MedicalOrderParaDao", e);
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
                MainWindow.Log.WriteErrorLog("IntervalDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="interval">Class instance of infectType infomation</param>
        /// <param name="scintervalId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertInterval(MedicalOrderPara interval, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO MEDICALORDERPARA (NAME,TYPE,COUNT,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@TYPE,@COUNT,@DESCRIPTION,@RESERVED) SET @ID = SCOPE_IDENTITY() ";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    if (interval.Name != null) sqlcomm.Parameters["@NAME"].Value = interval.Name;
                    sqlcomm.Parameters.Add("@TYPE", DbType.String);
                    if (interval.Type != null) sqlcomm.Parameters["@TYPE"].Value = interval.Type;
                    sqlcomm.Parameters.Add("@COUNT", DbType.Int32);
                    sqlcomm.Parameters["@COUNT"].Value = interval.Count;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    if (interval.Description != null) sqlcomm.Parameters["@DESCRIPTION"].Value = interval.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    if (interval.Reserved != null) sqlcomm.Parameters["@RESERVED"].Value = interval.Reserved;
                    sqlcomm.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    scId = (int)sqlcomm.Parameters["@ID"].Value;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderParaDao.cs-Insert", e);
                return false;
            }
            return true;
        }


        public bool UpdateInterval(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update MEDICALORDERPARA set ";
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
                MainWindow.Log.WriteErrorLog("MedicalOrderParaDao.cs-Update", e);
                return false;
            }
            return true;
        }


        public bool DeleteInterval(Int64 scIntervalId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM MEDICALORDERPARA WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scIntervalId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderParaDao.cs-Delete", e);
                return false;
            }
            return true;
        }

        public List<MedicalOrderPara> SelectInterval(Dictionary<string, object> condition)
        {
            var list = new List<MedicalOrderPara>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from MEDICALORDERPARA order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<MedicalOrderPara>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from MEDICALORDERPARA where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<MedicalOrderPara>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderParaDao.cs-Select", e);
                return list;
            }
        }
    }
}
