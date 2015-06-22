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
    class BedDao : IDisposable
    {

        public BedDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("BedDao.cs-BedDao", e);
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
                MainWindow.Log.WriteErrorLog("BedDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="bed">Class instance of bed infomation</param>
        /// <param name="scBedId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertBed(Bed bed, ref int scBedId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO BED (PATIENTAREAID,NAME,TREATTYPEID,ISAVAILABLE, ISOCCUPY,DESCRIPTION,RESERVED) VALUES 
                        (@PATIENTAREAID,@NAME,@TREATTYPEID,@ISAVAILABLE,@ISOCCUPY,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = bed.Name;
                    sqlcomm.Parameters.Add("@TREATTYPEID", DbType.Int32);
                    sqlcomm.Parameters["@TREATTYPEID"].Value = bed.TreatTypeId;
                    sqlcomm.Parameters.Add("@ISAVAILABLE", DbType.Boolean);
                    sqlcomm.Parameters["@ISAVAILABLE"].Value = bed.IsAvailable;
                    sqlcomm.Parameters.Add("@ISOCCUPY", DbType.Boolean);
                    sqlcomm.Parameters["@ISOCCUPY"].Value = bed.IsOccupy;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = bed.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters.Add("@PATIENTAREAID", DbType.Int32);
                    sqlcomm.Parameters["@PATIENTAREAID"].Value = bed.PatientAreaId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as BED;";
                    scBedId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("BedDao.cs-InsertBed", e);
                return false;
            }
            return true;
        }


        public bool UpdateBed(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update BED set ";
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
                MainWindow.Log.WriteErrorLog("BedDao.cs-UpdateBed", e);
                return false;
            }
            return true;
        }


        public bool DeleteBed(Int64 scBedId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM BED WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scBedId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("BedDao.cs-DeleteBed", e);
                return false;
            }
            return true;
        }

        public List<Bed> SelectBed(Dictionary<string, object> condition)
        {
            var list = new List<Bed>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from BED order by NAME asc;";
                        list = DatabaseOp.ExecuteQuery<Bed>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from BED where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by NAME asc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<Bed>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("BedDao.cs-SelectBed", e);
                return list;
            }
        }
    }
}
