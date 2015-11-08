using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class MachineTypeDao : IDisposable
    {

        public MachineTypeDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-MachineTypeDao", e);
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
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="MachineType">Class instance of MachineType infomation</param>
        /// <param name="scMachineTypeId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertMachineType(MachineType MachineType, ref int scMachineTypeId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO MACHINETYPE (NAME,BGCOLOR,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@BGCOLOR,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = MachineType.Name;
                    sqlcomm.Parameters.Add("@BGCOLOR", DbType.String);
                    sqlcomm.Parameters["@BGCOLOR"].Value = MachineType.BgColor;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = MachineType.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = MachineType.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SqlCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as MACHINETYPE;";
                    scMachineTypeId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-InsertMachineType", e);
                return false;
            }
            return true;
        }


        public bool UpdateMachineType(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update MACHINETYPE set ";
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
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-UpdateMachineType", e);
                return false;
            }
            return true;
        }


        public bool DeleteMachineType(Int64 scMachineTypeId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM MACHINETYPE WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scMachineTypeId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-DeleteMachineType", e);
                return false;
            }
            return true;
        }

        public List<MachineType> SelectMachineType(Dictionary<string, object> condition)
        {
            var list = new List<MachineType>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from MACHINETYPE order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<MachineType>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from MACHINETYPE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<MachineType>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MachineTypeDao.cs-SelectMachineType", e);
                return list;
            }
        }
    }
}
