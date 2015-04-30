#region File Header Text

//单个表的操作

#endregion

#region Using References

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using WpfApplication1.Utils;

#endregion

namespace WpfApplication1.DAOModule
{
    /// <summary>
    ///   FmriPatientDao表的操作服务实现
    /// </summary>
    public class FmriPatientDao : IDisposable
    {
        public FmriPatientDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-FmriPatientDao", e);
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
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="fmriPatient">Class instance of patient infomation</param>
        /// <param name="scPatientId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertFMriPatient(FmriPatient fmriPatient, ref int scPatientId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO FMRI_PATIENT (PATIENT_NAME,PATIENT_DOB,PATIENT_GENDER,PATIENT_HAND,PATIENT_REGESITER_DATE,PATIENT_STUDY_DESCRIPTION,PATIENT_CLINIC_DESCRIPTION) VALUES (@PATIENT_NAME,@PATIENT_DOB,@PATIENT_GENDER,@PATIENT_HAND,@PATIENT_REGESITER_DATE,@PATIENT_STUDY_DESCRIPTION,@PATIENT_CLINIC_DESCRIPTION)";
                    sqlcomm.Parameters.Add("@PATIENT_NAME", DbType.String);
                    sqlcomm.Parameters["@PATIENT_NAME"].Value = fmriPatient.PatientName;
                    sqlcomm.Parameters.Add("@PATIENT_DOB", DbType.String);
                    sqlcomm.Parameters["@PATIENT_DOB"].Value = fmriPatient.PatientDob;
                    sqlcomm.Parameters.Add("@PATIENT_GENDER", DbType.String);
                    sqlcomm.Parameters["@PATIENT_GENDER"].Value = fmriPatient.PatientGender;
                    sqlcomm.Parameters.Add("@PATIENT_HAND", DbType.String);
                    sqlcomm.Parameters["@PATIENT_HAND"].Value = fmriPatient.PatientHand;
                    sqlcomm.Parameters.Add("@PATIENT_REGESITER_DATE", DbType.String);
                    sqlcomm.Parameters["@PATIENT_REGESITER_DATE"].Value = fmriPatient.PatientRegesiterDate;
                    sqlcomm.Parameters.Add("@PATIENT_STUDY_DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@PATIENT_STUDY_DESCRIPTION"].Value = fmriPatient.PatientStudyDescription;
                    sqlcomm.Parameters.Add("@PATIENT_CLINIC_DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@PATIENT_CLINIC_DESCRIPTION"].Value = fmriPatient.PatientClinicDescription;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as FMRI_PATIENT;";
                    scPatientId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-InsertFMriPatient", e);
                return false;
            }
            return true;
        }


        public bool UpdateFMriPatient(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update FMRI_PATIENT set ";
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
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-UpdateFMriPatient", e);
                return false;
            }
            return true;
        }


        public bool DeleteFMriPatient(int scPatientId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM FMRI_PATIENT WHERE PATIENT_ID = @PATIENT_ID";
                    sqlcomm.Parameters.Add("@PATIENT_ID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENT_ID"].Value = scPatientId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-DeleteFMriPatient", e);
                return false;
            }
            return true;
        }

        public List<FmriPatient> SelectFMriPatient(Dictionary<string, object> condition)
        {
            var list = new List<FmriPatient>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from FMRI_PATIENT order by PATIENT_ID desc;";
                        list = DatabaseOp.ExecuteQuery<FmriPatient>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from FMRI_PATIENT where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by PATIENT_ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<FmriPatient>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIPatientDao.cs-SelectFMriPatient", e);
                return list;
            }
        }
    }
}