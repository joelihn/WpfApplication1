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
    class TreatTypeDao : IDisposable
    {
        public TreatTypeDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-TreatTypeDao", e);
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
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="treatType">Class instance of infectType infomation</param>
        /// <param name="scId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertTreatType(TreatType treatType, ref int scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO TREATTYPE (NAME,DESCRIPTION,RESERVED,BGCOLOR) VALUES 
                        (@NAME,@DESCRIPTION,@RESERVED,@BGCOLOR)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = treatType.Name;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = treatType.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = treatType.Reserved;
                    sqlcomm.Parameters.Add("@BGCOLOR", DbType.String);
                    sqlcomm.Parameters["@BGCOLOR"].Value = treatType.BgColor;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as TREATTYPE;";
                    scId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-InsertTreatType", e);
                return false;
            }
            return true;
        }


        public bool UpdateTreatType(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update TREATTYPE set ";
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
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-UpdateTreatType", e);
                return false;
            }
            return true;
        }


        public bool DeleteTreatType(Int64 scId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM TREATTYPE WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-DeleteTreatType", e);
                return false;
            }
            return true;
        }

        public List<TreatType> SelectTreatType(Dictionary<string, object> condition)
        {
            var list = new List<TreatType>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from TREATTYPE order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<TreatType>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from TREATTYPE where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<TreatType>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("TreatTypeDao.cs-SelectTreatType", e);
                return list;
            }
        }
    }
}
