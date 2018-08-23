using Rock.Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Rock.Library.Helper
{
    public class SQLHelper
    {
        #region 获取连接字符串
        public static string GetDbConnect(DataBaseConnect model)
        {
            string sql = "data source={0};initial catalog={1};persist security info=True;user id={2};password={3};pooling=true;min pool size=1;max pool size=1000";
            return string.Format(sql, model.DataSource, model.DataBase, model.UserName, model.Password);
        }
        #endregion

        #region 获取数据库列表
        /// <summary>
        /// 获取数据库列表
        /// </summary> 
        public static List<string> GetDataBaseList(DataBaseConnect model)
        {
            List<string> list = new List<string>();
            var sql = "SELECT name FROM SYSDATABASES ";
            DataSet ds = ExecuteSQLDataSet(GetDbConnect(model), sql);
            if (ds == null || ds.Tables.Count == 0) return null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(dr["name"].ToString());
            }
            return list;
        }
        #endregion

        #region 获取数据库所有表名称
        /// <summary>
        /// 获取数据库所有表名称
        /// </summary> 
        public static List<string> GetTableList(DataBaseConnect model)
        {
            List<string> list = new List<string>();
            var sql = @"SELECT name FROM sysobjects WHERE xtype = 'U' ORDER BY crdate DESC";
            DataSet ds = ExecuteSQLDataSet(GetDbConnect(model), sql);
            if (ds == null || ds.Tables.Count == 0) return null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(dr["name"].ToString());
            }
            return list;
        }
        #endregion

        #region 查询表结构
        public static List<DataBaseStruct> GetDataBaseStruct(string dbConnect, List<string> tableNames = null, List<string> exceptTableNames = null)
        {
            if (exceptTableNames == null) exceptTableNames = new List<string>();
            if (tableNames == null) tableNames = new List<string>();
            var exceptColumnsNames = new List<string> { "Id", "IsSystem", "IsDelete", "CreateTime", "CreateUserId", "UpdateTime", "UpdateUserId" };

            var sql = @"SELECT a.TABLE_NAME TableName,a.ORDINAL_POSITION SortIndex,a.COLUMN_NAME ColumnsName,b.value [Description],a.DATA_TYPE ColumnType,a.CHARACTER_MAXIMUM_LENGTH [Length]
                        ,CONVERT(BIT,CASE WHEN a.IS_NULLABLE='YES' THEN 1 ELSE 0 END) [IsNull],e.definition [DefaultValue],CONVERT(BIT,(
                        SELECT COUNT(1) AS Is_PK 
                        FROM  syscolumns  
                        JOIN  sysindexkeys ON syscolumns.id=sysindexkeys.id  AND  syscolumns.colid=sysindexkeys.colid   
                        JOIN  sysindexes ON syscolumns.id=sysindexes.id  AND  sysindexkeys.indid=sysindexes.indid  
                        JOIN  sysobjects ON sysindexes.name=sysobjects.name  AND  sysobjects.xtype='PK'
                        WHERE syscolumns.name=a.COLUMN_NAME AND syscolumns.id=object_id(a.TABLE_NAME)
                        )) IsPrimaryKey
                        from information_schema.COLUMNS a  
                        LEFT JOIN sys.extended_properties b on a.TABLE_NAME=OBJECT_NAME(b.major_id) and a.ORDINAL_POSITION=b.minor_id 
                        LEFT JOIN sys.columns c ON b.major_id = c.object_id AND b.minor_id = c.column_id AND b.name = 'MS_Description'
                        LEFT JOIN sysobjects d ON c.object_id = d.id
                        LEFT JOIN sys.default_constraints e ON  d.id = e.parent_object_id AND c.column_id = e.parent_column_id
                        WHERE a.COLUMN_NAME  NOT IN ('{2}') "
                        + ((tableNames != null && tableNames.Count > 0) ? " AND a.TABLE_NAME IN ('{0}')" : "")
                        + ((exceptTableNames != null && exceptTableNames.Count > 0) ? " AND  a.TABLE_NAME NOT IN ('{1}')" : "")
                        + @"ORDER BY a.TABLE_NAME ASC,a.ORDINAL_POSITION ASC";

            DataSet ds = ExecuteSQLDataSet(dbConnect, string.Format(sql, tableNames == null ? "" : string.Join("','", tableNames), exceptTableNames == null ? "" : string.Join("','", exceptTableNames), string.Join("','", exceptColumnsNames)));
            if (ds == null || ds.Tables.Count == 0) return null;

            return ObjectHelper.DataTableToEntities<DataBaseStruct>(ds.Tables[0]);
        }

        #endregion

        #region 执行SQL
        public static bool ExecuteSQL(string dbConnect, string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnect))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = conn.CreateCommand();
                        var strSqls = Regex.Split(sql, "GO", RegexOptions.IgnoreCase);

                        foreach (var strSql in strSqls)
                        {
                            command.CommandText = strSql;
                            command.ExecuteNonQuery();
                        }
                        conn.Close(); //关闭连接                  
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                        if (conn.State != ConnectionState.Closed) conn.Close(); //关闭连接      
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }

        }
        #endregion

        #region 执行SQL查询
        public static DataSet ExecuteSQLDataSet(string dbConnect, string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnect))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = conn.CreateCommand();
                        command.CommandText = sql; ;
                        SqlDataAdapter adp = new SqlDataAdapter(command);
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        conn.Close();
                        return ds;
                    }
                    catch (Exception)
                    {
                        if (conn.State != ConnectionState.Closed) conn.Close();
                        throw;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }
        #endregion


    }
}
