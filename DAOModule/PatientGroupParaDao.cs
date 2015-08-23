﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Utils;

namespace WpfApplication1.DAOModule
{
    class PatientGroupParaDao : IDisposable
    {

        public PatientGroupParaDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-PatientGroupParaDao", e);
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
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="PatientGroupPara">Class instance of PatientGroupPara infomation</param>
        /// <param name="scPatientGroupParaId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertPatientGroupPara(PatientGroupPara PatientGroupPara, ref int scPatientGroupParaId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENTGROUPPARA (GROUPID,KEY,SYMBOL,VALUE,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@KEY,@SYMBOL,@VALUE,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@GROUPID", DbType.Int64);
                    sqlcomm.Parameters["@GROUPID"].Value = PatientGroupPara.GroupId;
                    sqlcomm.Parameters.Add("@KEY", DbType.String);
                    sqlcomm.Parameters["@KEY"].Value = PatientGroupPara.Key;
                    sqlcomm.Parameters.Add("@SYMBOL", DbType.String);
                    sqlcomm.Parameters["@SYMBOL"].Value = PatientGroupPara.Symbol;
                    sqlcomm.Parameters.Add("@VALUE", DbType.String);
                    sqlcomm.Parameters["@VALUE"].Value = PatientGroupPara.Value;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = PatientGroupPara.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = PatientGroupPara.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as PATIENTGROUPPARA;";
                    scPatientGroupParaId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-InsertPatientGroupPara", e);
                return false;
            }
            return true;
        }


        public bool UpdatePatientGroupPara(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update PATIENTGROUPPARA set ";
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
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-UpdatePatientGroupPara", e);
                return false;
            }
            return true;
        }


        public bool DeletePatientGroupPara(Int64 scPatientGroupParaId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM PATIENTGROUPPARA WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scPatientGroupParaId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-DeletePatientGroupPara", e);
                return false;
            }
            return true;
        }

        public List<PatientGroupPara> SelectPatientGroupPara(Dictionary<string, object> condition)
        {
            var list = new List<PatientGroupPara>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENTGROUPPARA order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<PatientGroupPara>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENTGROUPPARA where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<PatientGroupPara>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupParaDao.cs-SelectPatientGroupPara", e);
                return list;
            }
        }
    }
}