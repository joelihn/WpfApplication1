#region File Header Text

//一些涉及事务等的操作

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
    public class FmriComplexDao : IDisposable
    {
        public FmriComplexDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-FMRIComplexDao", e);
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
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-Dispose", e);
            }
        }

        #endregion

        public int GetMaxRowIdOfPatientTable()
        {
            int max = 0;
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "SELECT IFNULL(max(_ROWID_) , 1) AS Id FROM  FMRI_PATIENT;";
                    sqlcomm.CommandText = sqlcommand;
                    sqlcomm.CommandType = CommandType.Text;
                    SQLiteDataReader sqReader = sqlcomm.ExecuteReader();
                    while (sqReader.Read())
                    {
                        max = sqReader.GetInt32(0);
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-GetMaxRowIdOfPatientTable", e);
            }

            return max;
        }

        public List<FmriPatient> SelectFMriPatient(Dictionary<string, object> condition, DateTime begin,
                                                   DateTime endtemp)
        {
            var list = new List<FmriPatient>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    //DateTime end = endtemp.AddDays(1);
                    DateTime end = endtemp;
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from FMRI_PATIENT where " + "PATIENT_REGESITER_DATE between '" +
                            begin.ToString("yyyy-MM-dd") +
                            "' and  '" + end.ToString("yyyy-MM-dd") + "'" + "order by PATIENT_ID desc;";
                        list = DatabaseOp.ExecuteQuery<FmriPatient>(sqlcomm);
                        return list;
                    }

                    string sqlcommand = "select * from FMRI_PATIENT where ";
                    TransferLikeParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));

                    sqlcommand += " and PATIENT_REGESITER_DATE between '" + begin.ToString("yyyy-MM-dd") +
                                  "' and  '" + end.ToString("yyyy-MM-dd") + "'";
                    sqlcommand += " order by PATIENT_ID desc; ";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<FmriPatient>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-SelectFMriPatient", e);
                return list;
            }
        }

        public static void TransferLikeParameteres(ref string sql, string mark1, string mark2,
                                                   Dictionary<string, object> conditions,
                                                   SQLiteParameterCollection parameters)
        {
            try
            {
                foreach (var condition in conditions)
                {
                    sql += condition.Key + " like " + mark1 + condition.Key + " " + mark2 + " ";
                    if (condition.Key.Equals("PATIENT_ID"))
                    {
                        int patientId = int.Parse(condition.Value.ToString());
                        parameters.AddWithValue(condition.Key, "%" + patientId + "%");
                    }
                    else
                        parameters.AddWithValue(condition.Key, "%" + condition.Value + "%");
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-TransferLikeParameteres", e);
            }
        }
    }
}