#region File Header Text

//一些涉及事务等的操作

#endregion

#region Using References

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WpfApplication1.Utils;

#endregion

namespace WpfApplication1.DAOModule
{
    public class BedInfoDao : IDisposable
    {
        public BedInfoDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ComplexDao.cs-ComplexDao", e);
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
                MainWindow.Log.WriteErrorLog("FMRIComplexDao.cs-Dispose", e);
            }
        }

        #endregion

        public List<BedDetails> SelectPatient(Dictionary<string, object> condition)
        {
            var list = new List<BedDetails>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    //DateTime end = endtemp.AddDays(1);
                    if (condition == null || condition.Count == 0)
                    {
                        /*sqlcomm.CommandText =
                            "select * from PATIENT where " + "REGISITDATE between '" +
                            begin.ToString("yyyy-MM-dd") +
                            "' and  '" + end.ToString("yyyy-MM-dd") + "'" + "order by ID desc;";*/
                        sqlcomm.CommandText =
                            "SELECT Bed.Id, Bed.Name, PatientArea.Type FROM (Bed INNER JOIN PatientRoom ON Bed.PatientRoomId=PatientRoom.Id) INNER JOIN PatientArea ON PatientRoom.PatientAreaId=PatientArea.Id";
                        list = DatabaseOp.ExecuteQuery<BedDetails>(sqlcomm);
                        return list;
                    }

                    string sqlcommand = "select Bed.Id, Bed.Name, PatientArea.Type FROM (Bed INNER JOIN PatientRoom ON Bed.PatientRoomId=PatientRoom.Id) INNER JOIN PatientArea ON PatientRoom.PatientAreaId=PatientArea.Id where ";
                    //DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    //sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    //sqlcommand += " order by Bed.ID desc";
                    foreach (var v in condition)
                    {
                        sqlcommand += v.Key + "=" + v.Value;
                    }
                    
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<BedDetails>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("ComplexDao.cs-SelectPatient", e);
                return list;
            }
        }


    }
}