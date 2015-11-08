#region File Header Text

#endregion

#region Using References

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

#endregion

namespace WpfApplication1.DAOModule
{
    public class DatabaseOp
    {
        /// <summary>
        /// 执行不返回结果的命令(如:UPDATE,INSERT,DELETE)
        /// </summary>
        /// <param name="SqlCommand"></param>
        public static void ExecuteNoneQuery(SqlCommand SqlCommand)
        {
            SqlCommand.ExecuteNonQuery();
        }


        /// <summary>
        ///   执行带返回结果的命令(如:SELECT)
        /// </summary>
        /// <param name="SqlCommand"></param>
        /// <returns>查询结果:DataTable</returns>
        public static List<T> ExecuteQuery<T>(SqlCommand SqlCommand)
        {
            var ds = new DataSet();
            var db = new SqlDataAdapter {SelectCommand = SqlCommand};
            db.Fill(ds);
            return DataSetToList<T>(ds, 0);
        }

        /// <summary>
        /// 将查到的数据源转换为泛型集合
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dataSet">数据源</param>
        /// <param name="tableIndex">需要转换表的索引</param>
        /// <returns>泛型集合</returns>
        private static List<T> DataSetToList<T>(DataSet dataSet, int tableIndex)
        {
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
                return null;
            DataTable dataTable = dataSet.Tables[tableIndex];
            var list = new List<T>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var t = Activator.CreateInstance<T>();
                PropertyInfo[] propertyInfos = t.GetType().GetProperties();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (
                            dataTable.Columns[j].ColumnName.ToUpper().Replace("_", "").Equals(
                                propertyInfo.Name.ToUpper()))
                        {
                            if (dataTable.Rows[i][j] != DBNull.Value)
                            {
                                if (propertyInfo.PropertyType.IsEnum)
                                {
                                    propertyInfo.SetValue(t,
                                                          Enum.Parse(propertyInfo.PropertyType,
                                                                     dataTable.Rows[i][j].ToString()), null);
                                    break;
                                }
                                propertyInfo.SetValue(t, dataTable.Rows[i][j], null);
                            }
                            else
                            {
                                propertyInfo.SetValue(t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        public static void TransferParameteres(ref string sql, string mark1, string mark2,
                                               Dictionary<string, object> conditions,
                                               SqlParameterCollection parameters)
        {
            foreach (var condition in conditions)
            {
                sql += condition.Key + "=" + mark1 + condition.Key + " " + mark2 + " ";
                parameters.AddWithValue(condition.Key, condition.Value);
            }
        }
    }
}