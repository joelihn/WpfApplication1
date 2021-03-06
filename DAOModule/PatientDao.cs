﻿#region File Header Text

//单个表的操作

#endregion

#region Using References

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WpfApplication1.Utils;

#endregion

namespace WpfApplication1.DAOModule
{
    /// <summary>
    ///   FmriPatientDao表的操作服务实现
    /// </summary>
    public class PatientDao : IDisposable
    {
        public PatientDao()
        {
            try
            {
                SqlConn = new SqlConnection(ConstDefinition.DbStr);
                SqlConn.Open();
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientDao.cs-PatientDao", e);
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
                MainWindow.Log.WriteErrorLog("PatientDao.cs-Dispose", e);
            }
        }

        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="patient">Class instance of patient infomation</param>
        /// <param name="scPatientId">Id of the last insert row id</param>
        /// <returns></returns>
        public bool InsertPatient(Patient patient, ref int scId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"INSERT INTO PATIENT (PATIENTID,NAME,DOB,GENDER,NATIONALITY,MARRIAGE,HEIGHT,BLOODTYPE,IDCODE, MOBILE, ORDERS, TREATSTATUSID,REGISITDATE,
                                                AREAID,ZIPCODE,WEIXINHAO,PAYMENT,INFECTTYPEID,ISFIXEDBED,BEDID,ISASSIGNED,DESCRIPTION,RESERVED1,RESERVED2) VALUES 
                                            (@PATIENTID,@NAME,@DOB,@GENDER,@NATIONALITY,@MARRIAGE,@HEIGHT,@BLOODTYPE, @IDCODE, @MOBILE,@ORDERS,@TREATSTATUSID,@REGISITDATE,
                                                @AREAID,@ZIPCODE,@WEIXINHAO,@PAYMENT,@INFECTTYPEID,@ISFIXEDBED,@BEDID,@ISASSIGNED,@DESCRIPTION,@RESERVED1,@RESERVED2) SET @ID = SCOPE_IDENTITY() ";
                    sqlcomm.Parameters.Add("@PATIENTID", DbType.String);
                    sqlcomm.Parameters["@PATIENTID"].Value = patient.PatientId;
                    sqlcomm.Parameters.Add("@NAME", DbType.String);
                    if (patient.Name != null) sqlcomm.Parameters["@NAME"].Value = patient.Name;
                    sqlcomm.Parameters.Add("@GENDER", DbType.String);
                    if (patient.Gender != null) sqlcomm.Parameters["@GENDER"].Value = patient.Gender;
                    sqlcomm.Parameters.Add("@DOB", DbType.String);
                    if (patient.Dob != null) sqlcomm.Parameters["@DOB"].Value = patient.Dob;
                    sqlcomm.Parameters.Add("@NATIONALITY", DbType.String);
                    if (patient.Nationality != null) sqlcomm.Parameters["@NATIONALITY"].Value = patient.Nationality;
                    sqlcomm.Parameters.Add("@MARRIAGE", DbType.String);
                    if (patient.Marriage != null) sqlcomm.Parameters["@MARRIAGE"].Value = patient.Marriage;
                    sqlcomm.Parameters.Add("@HEIGHT", DbType.String);
                    if (patient.Height != null) sqlcomm.Parameters["@HEIGHT"].Value = patient.Height;
                    sqlcomm.Parameters.Add("@BLOODTYPE", DbType.String);
                    if (patient.BloodType != null) sqlcomm.Parameters["@BLOODTYPE"].Value = patient.BloodType;
                    sqlcomm.Parameters.Add("@IDCODE", DbType.String);
                    if (patient.IdCode != null) sqlcomm.Parameters["@IDCODE"].Value = patient.IdCode;
                    sqlcomm.Parameters.Add("@MOBILE", DbType.String);
                    if (patient.Mobile != null) sqlcomm.Parameters["@MOBILE"].Value = patient.Mobile;
                    sqlcomm.Parameters.Add("@ORDERS", DbType.String);
                    if (patient.Orders != null) sqlcomm.Parameters["@ORDERS"].Value = patient.Orders;
                    sqlcomm.Parameters.Add("@TREATSTATUSID", DbType.Int32);
                    if (patient.TreatStatusId != null) sqlcomm.Parameters["@TREATSTATUSID"].Value = patient.TreatStatusId;
                    sqlcomm.Parameters.Add("@REGISITDATE", DbType.String);
                    if (patient.RegisitDate != null) sqlcomm.Parameters["@REGISITDATE"].Value = patient.RegisitDate;
                    sqlcomm.Parameters.Add("@AREAID", DbType.Int32);
                    if (patient.AreaId != null) sqlcomm.Parameters["@AREAID"].Value = patient.AreaId;

                    sqlcomm.Parameters.Add("@ZIPCODE", DbType.String);
                    if (patient.ZipCode != null) sqlcomm.Parameters["@ZIPCODE"].Value = patient.ZipCode;
                    sqlcomm.Parameters.Add("@WEIXINHAO", DbType.String);
                    if (patient.WeiXinHao != null) sqlcomm.Parameters["@WEIXINHAO"].Value = patient.WeiXinHao;
                    sqlcomm.Parameters.Add("@PAYMENT", DbType.String);
                    if (patient.Payment != null) sqlcomm.Parameters["@PAYMENT"].Value = patient.Payment;

                    sqlcomm.Parameters.Add("@INFECTTYPEID", DbType.Int32);
                    if (patient.InfectTypeId != null) sqlcomm.Parameters["@INFECTTYPEID"].Value = patient.InfectTypeId;
                    sqlcomm.Parameters.Add("@ISFIXEDBED", DbType.Boolean);
                    if (patient.IsFixedBed != null) sqlcomm.Parameters["@ISFIXEDBED"].Value = patient.IsFixedBed;
                    sqlcomm.Parameters.Add("@BEDID", DbType.Int32);
                    if (patient.BedId != null) sqlcomm.Parameters["@BEDID"].Value = patient.BedId;
                    sqlcomm.Parameters.Add("@ISASSIGNED", DbType.Boolean);
                    if (patient.IsAssigned != null) sqlcomm.Parameters["@ISASSIGNED"].Value = patient.IsAssigned;
                    sqlcomm.Parameters.Add("@DESCRIPTION", DbType.String);
                    if (patient.Description != null) sqlcomm.Parameters["@DESCRIPTION"].Value = patient.Description;
                    sqlcomm.Parameters.Add("@RESERVED1", DbType.String);
                    if (patient.Reserved1 != null) sqlcomm.Parameters["@RESERVED1"].Value = patient.Reserved1;
                    sqlcomm.Parameters.Add("@RESERVED2", DbType.String);
                    if (patient.Reserved2 != null) sqlcomm.Parameters["@RESERVED2"].Value = patient.Reserved2;
                    sqlcomm.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    DatabaseOp.ExecuteNoneQuery(sqlcomm);

                    scId = (int)sqlcomm.Parameters["@ID"].Value;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientDao.cs-InsertPatient", e);
                return false;
            }
            return true;
        }


        public bool UpdatePatient(Dictionary<string, object> fields, Dictionary<string, object> condition)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string sqlcommand = "update PATIENT set ";
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
                MainWindow.Log.WriteErrorLog("PatientDao.cs-UpdatePatient", e);
                return false;
            }
            return true;
        }


        public bool DeletePatient(int scPatientId)
        {
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    sqlcomm.CommandText =
                        @"DELETE FROM PATIENT WHERE ID = @ID";
                    sqlcomm.Parameters.Add("@ID", DbType.Int32);
                    sqlcomm.Parameters["@ID"].Value = scPatientId;
                    DatabaseOp.ExecuteNoneQuery(sqlcomm);
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientDao.cs-DeletePatient", e);
                return false;
            }
            return true;
        }

        public List<Patient> SelectPatient(Dictionary<string, object> condition)
        {
            var list = new List<Patient>();
            try
            {
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    if (condition == null || condition.Count == 0)
                    {
                        sqlcomm.CommandText =
                            "select * from PATIENT order by ID desc;";
                        list = DatabaseOp.ExecuteQuery<Patient>(sqlcomm);
                        return list;
                    }
                    string sqlcommand = "select * from PATIENT where ";
                    DatabaseOp.TransferParameteres(ref sqlcommand, "@", "and", condition, sqlcomm.Parameters);
                    sqlcommand = sqlcommand.Substring(0, sqlcommand.LastIndexOf("and"));
                    sqlcommand += " order by ID desc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<Patient>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientDao.cs-SelectPatient", e);
                return list;
            }
        }

        public List<Patient> SelectPatientSpecial(List<PatientGroupPara> listinParas)
        {
            var list = new List<Patient>();
            try
            {
                
                using (SqlCommand sqlcomm = SqlConn.CreateCommand())
                {
                    string condition = string.Empty;
                    foreach (var patientGroupPara in listinParas)
                    {
                        //Key["感染情况"] = "INFECTTYPEID";
                        //Key["治疗状态"] = "TREATSTATUSID";
                        //Key["固定床位"] = "ISFIXEDBED";
                        //Key["所属分区"] = "AERAID";


                        if (patientGroupPara.Value.Contains("%"))
                        {
                            if (patientGroupPara.Key.Equals("所属分区"))
                            {
                                string value = string.Empty;
                                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                                {
                                    Dictionary<string, object> con = new Dictionary<string, object>();
                                    con["NAME"] = patientGroupPara.Value;
                                    var li = patientAreaDao.SelectPatientArea(con);
                                    value = li[0].Id.ToString();
                                }

                                condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " " + value.Trim() + " " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else if (patientGroupPara.Key.Equals("治疗状态"))
                            {
                                string value = string.Empty;
                                using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                                {
                                    Dictionary<string, object> con = new Dictionary<string, object>();
                                    con["NAME"] = patientGroupPara.Value;
                                    var li = treatStatusDao.SelectTreatStatus(con);
                                    value = li[0].Id.ToString();
                                }
                                condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " " + value.Trim() + " " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else if (patientGroupPara.Key.Equals("感染情况"))
                            {
                                
                                if (patientGroupPara.Value.Equals("阴性"))
                                {
                                    condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " " + "0" + " " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                                }
                                else
                                {
                                    string value = string.Empty;
                                    using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                                    {
                                        Dictionary<string, object> con = new Dictionary<string, object>();
                                        con["NAME"] = patientGroupPara.Value;
                                        var li = infectTypeDao.SelectInfectType(con);
                                        value = li[0].Id.ToString();
                                    }
                                    condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " " + value.Trim() + " " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                                }
                                
                            }
                            else if (patientGroupPara.Key.Equals("固定床位"))
                            {
                                condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " " + patientGroupPara.Value.Trim() + " " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else
                            {
                                condition += patientGroupPara.Left.Trim() + " " + MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                               MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" + patientGroupPara.Value.Trim() + "\' " +
                               patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                        }
                        else
                        {
                            if (patientGroupPara.Key.Equals("所属分区"))
                            {
                                string value = string.Empty;
                                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                                {
                                    Dictionary<string, object> con = new Dictionary<string, object>();
                                    con["NAME"] = patientGroupPara.Value;
                                    var li = patientAreaDao.SelectPatientArea(con);
                                    value = li[0].Id.ToString();
                                }
                                condition += patientGroupPara.Left.Trim() + " " +
                                           MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                           MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                           value.Trim() + "\' " +
                                           patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else if (patientGroupPara.Key.Equals("治疗状态"))
                            {
                                
                                if (patientGroupPara.Value.Equals("在治"))
                                {
                                    condition += patientGroupPara.Left.Trim() + " " +
                                           MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                           MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                           "0" + "\' " +
                                           patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                                }
                                else
                                {
                                    string value = string.Empty;
                                    using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                                    {
                                        Dictionary<string, object> con = new Dictionary<string, object>();
                                        con["NAME"] = patientGroupPara.Value;
                                        var li = treatStatusDao.SelectTreatStatus(con);
                                        value = li[0].Id.ToString();
                                    }
                                    condition += patientGroupPara.Left.Trim() + " " +
                                           MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                           MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                           value.Trim() + "\' " +
                                           patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                                }
                                
                            }
                            else if (patientGroupPara.Key.Equals("感染情况"))
                            {
                                string value = string.Empty;
                                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                                {
                                    Dictionary<string, object> con = new Dictionary<string, object>();
                                    con["NAME"] = patientGroupPara.Value;
                                    var li = infectTypeDao.SelectInfectType(con);
                                    value = li[0].Id.ToString();
                                }
                                condition += patientGroupPara.Left.Trim() + " " +
                                           MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                           MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                           value.Trim() + "\' " +
                                           patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else if (patientGroupPara.Key.Equals("固定床位"))
                            {
                                condition += patientGroupPara.Left.Trim() + " " +
                                           MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                           MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                           patientGroupPara.Value.Trim() + "\' " +
                                           patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                            else
                            {
                                condition += patientGroupPara.Left.Trim() + " " +
                                             MainWindow.Key[patientGroupPara.Key].Trim() + " " +
                                             MainWindow.Symbol[patientGroupPara.Symbol].Trim() + " \'" +
                                             patientGroupPara.Value.Trim() + "\' " +
                                             patientGroupPara.Right.Trim() + " " + patientGroupPara.Logic.Trim();
                            }
                        }
                    }
                    string sqlcommand = "select * from PATIENT where ";
                    sqlcommand += condition;
                    sqlcommand += " order by Name asc";
                    sqlcomm.CommandText = sqlcommand;

                    list = DatabaseOp.ExecuteQuery<Patient>(sqlcomm);
                    return list;
                }
            }
            catch (Exception e)
            {
                MainWindow.Log.WriteErrorLog("PatientDao.cs-SelectPatientSpecial", e);
                return list;
            }
        }
    }
}