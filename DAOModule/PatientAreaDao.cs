using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class PatientAreaDao : IDisposable
    {
        public PatientAreaDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-PatientAreaDao", e);
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
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="patientArea">Class instance of PatientArea infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertPatientArea(PatientArea patientArea, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENTAREA (NAME,TYPE,DESCRIPTION,INFECTTYPEID,SEQ,POSITION,RESERVED) VALUES 
                        (@NAME,@TYPE,@DESCRIPTION,@INFECTTYPEID,@SEQ,@POSITION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = patientArea.Name;
                    sqlcomm.Parameters.Add("@TYPE", DbType.String);
                    sqlcomm.Parameters["@TYPE"].Value = patientArea.Type;
                    sqlcomm.Parameters.Add("@INFECTTYPEID", DbType.Int32);
                    sqlcomm.Parameters["@INFECTTYPEID"].Value = patientArea.InfectTypeId;
                    sqlcomm.Parameters.Add("@SEQ", DbType.Int32);
                    sqlcomm.Parameters["@SEQ"].Value = patientArea.Seq;
                    sqlcomm.Parameters.Add("@POSITION", DbType.String);
                    sqlcomm.Parameters["@POSITION"].Value = patientArea.Position;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = patientArea.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = patientArea.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SqlCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as PATIENTAREA;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-InsertPatientArea", e);
                return false;
            }
            return true;
        }


        public bool UpdatePatientArea(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update PATIENTAREA set ";
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
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-UpdatePatientArea", e);
                return false;
            }
            return true;
        }


        public bool DeletePatientArea(long scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM PATIENTAREA WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-DeletePatientArea", e);
                return false;
            }
            return true;
        }

        public List<PatientArea> SelectPatientArea(Dictionary<string, object> condition)
        {
            var list = new List<PatientArea>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENTAREA order by Seq asc;";
                        list = DatabaseOp.ExecuteQuery<PatientArea>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENTAREA where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by Seq asc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<PatientArea>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-SelectPatientArea", e);
                return list;
            }
        }
    }
}
