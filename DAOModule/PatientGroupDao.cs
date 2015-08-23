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
    class PatientGroupDao : IDisposable
    {

        public PatientGroupDao()
        {
            try
            {
                SqlConn = new SQLiteConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-PatientGroupDao", e);
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
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="PatientGroup">Class instance of PatientGroup infomation</param>
        /// <param name="scPatientGroupId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertPatientGroup(PatientGroup PatientGroup, ref int scPatientGroupId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENTGROUP (NAME,DESCRIPTION,RESERVED) VALUES 
                        (@NAME,@DESCRIPTION,@RESERVED)";
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    sqlcomm.Parameters["@NAME"].Value = PatientGroup.Name;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    sqlcomm.Parameters["@DESCRIPTION"].Value = PatientGroup.Description;
                    sqlcomm.Parameters.Add("@RESERVED", DbType.String);
                    sqlcomm.Parameters["@RESERVED"].Value = PatientGroup.Reserved;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    //set last insert id of this table this connection
                    SQLiteCommand comm = SqlConn.CreateCommand();
                    comm.CommandText = "Select last_insert_rowid() as PATIENTGROUP;";
                    scPatientGroupId = Convert.ToInt32(comm.ExecuteScalar());
                    comm.Dispose();
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-InsertPatientGroup", e);
                return false;
            }
            return true;
        }


        public bool UpdatePatientGroup(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update PATIENTGROUP set ";
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
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-UpdatePatientGroup", e);
                return false;
            }
            return true;
        }


        public bool DeletePatientGroup(Int64 scPatientGroupId)
        {
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM PATIENTGROUP WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scPatientGroupId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-DeletePatientGroup", e);
                return false;
            }
            return true;
        }

        public List<PatientGroup> SelectPatientGroup(Dictionary<string, object> condition)
        {
            var list = new List<PatientGroup>();
            try
            {
                using (SQLiteCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENTGROUP order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<PatientGroup>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENTGROUP where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<PatientGroup>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientGroupDao.cs-SelectPatientGroup", e);
                return list;
            }
        }
    }
}