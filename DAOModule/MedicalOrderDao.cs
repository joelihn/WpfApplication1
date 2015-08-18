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
    class MedicalOrderDao : IDisposable
    {
        public MedicalOrderDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-MedicalOrderDao", e);
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
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="medicalOrder">Class instance of Medical Order infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertInfectType(MedicalOrder medicalOrder, ref int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO MEDICALORDER (PATIENTID,ACTIVATED,SEQ,PALN,METHODID,INTERVAL,TIMES,
                           DESCRIPTION,RESERVED1,RESERVED2) VALUES 
                        (@PATIENTID,@ACTIVATED,@SEQ,@PALN,@INTERVAL,@TIMES,
                            @DESCRIPTION ,@RESERVED1,@RESERVED2)";
                    sqlcomm.Parameters.Add("@PATIENTID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTID"].Value = medicalOrder.PatientId;

                    sqlcomm.Parameters.Add("@ACTIVATED", DbType.Boolean);
                    sqlcomm.Parameters["@ACTIVATED"].Value = medicalOrder.Activated;
                    sqlcomm.Parameters.Add("@SEQ", DbType.String);
                    sqlcomm.Parameters["@SEQ"].Value = medicalOrder.Seq;

                    sqlcomm.Parameters.Add("@PALN", DbType.String);
                    sqlcomm.Parameters["@PALN"].Value = medicalOrder.Plan;
                    sqlcomm.Parameters.Add("@METHODID", DbType.Int32);
                    sqlcomm.Parameters["@METHODID"].Value = medicalOrder.MethodId;

                    sqlcomm.Parameters.Add("@INTERVAL", DbType.Int32);
                    sqlcomm.Parameters["@INTERVAL"].Value = medicalOrder.Interval;
                    sqlcomm.Parameters.Add("@TIMES", DbType.Int32);
                    sqlcomm.Parameters["@TIMES"].Value = medicalOrder.Times;

                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = medicalOrder.Description;

                    sqlcomm.Parameters.Add("@RESERVED1", DbType.String);
                    sqlcomm.Parameters["@RESERVED1"].Value = medicalOrder.Reserved1;
                    sqlcomm.Parameters.Add("@RESERVED2", DbType.String);
                    sqlcomm.Parameters["@RESERVED2"].Value = medicalOrder.Reserved2;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as MEDICALORDER;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-InsertMedicalOrder", e);
                return false;
            }
            return true;
        }


        public bool UpdateMedicalOrder(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update MEDICALORDER set ";
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
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-UpdateMedicalOrder", e);
                return false;
            }
            return true;
        }


        public bool DeleteMedicalOrder(int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM MEDICALORDER WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-DeleteMedicalOrder", e);
                return false;
            }
            return true;
        }

        public List<MedicalOrder> SelectMedicalOrder(Dictionary<string, object> condition)
        {
            var list = new List<MedicalOrder>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from MEDICALORDER order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<MedicalOrder>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from MEDICALORDER where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<MedicalOrder>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("MedicalOrderDao.cs-SelectMedicalOrder", e);
                return list;
            }
        }
    }
}
