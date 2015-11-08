using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class PatientRoomDao : IDisposable
    {
        public PatientRoomDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-PatientRoomDao", e);
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
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="patientRoom">Class instance of PatientArea infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertPatientRoom(PatientRoom patientRoom, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENTROOM (PATIENTAREAID,NAME,INFECTTYPEID,DESCRIPTION,RESERVED) VALUES 
                        (@PATIENTAREAID,@NAME,@INFECTTYPEID,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@PATIENTAREAID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTAREAID"].Value = patientRoom.PatientAreaId;
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = patientRoom.Name;
                    sqlcomm.Parameters.Add("@INFECTTYPEID", DbType.Int32);
                    sqlcomm.Parameters["@INFECTTYPEID"].Value = patientRoom.InfectTypeId;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = patientRoom.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = patientRoom.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SqlCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as PATIENTROOM;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-InsertPatientRoom", e);
                return false;
            }
            return true;
        }


        public bool UpdatePatientRoom(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update PATIENTROOM set ";
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
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-UpdatePatientRoom", e);
                return false;
            }
            return true;
        }


        public bool DeletePatientRoom(Int64 scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM PATIENTROOM WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-DeletePatientRoom", e);
                return false;
            }
            return true;
        }

        public List<PatientRoom> SelectPatientRoom(Dictionary<string, object> condition)
        {
            var list = new List<PatientRoom>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENTROOM order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<PatientRoom>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENTROOM where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<PatientRoom>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientRoomDao.cs-SelectPatientRoom", e);
                return list;
            }
        }
    }
}
