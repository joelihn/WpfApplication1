using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class PatientAreaDao : IDisposable
    {
        public PatientAreaDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientAreaDao.cs-PatientAreaDao", e);
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
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENTAREA (NAME,TYPE,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@TYPE,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = patientArea.Name;
                    sqlcomm.Parameters.Add("@TYPE", DbType.String);
                    sqlcomm.Parameters["@TYPE"].Value = patientArea.Type;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = patientArea.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = patientArea.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
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
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
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
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
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
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENTAREA order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<PatientArea>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENTAREA where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
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
