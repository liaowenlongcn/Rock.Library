using Rock.Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        #region 事务执行SQL
        /// <summary>
        /// 事务执行SQL
        /// </summary>
        public int ExecuteSql(string connectionString, params SqlEntity[] sqlItems)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
                using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandTimeout = 0;
                    int executeCount = 0;
                    try
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.Transaction = sqlTransaction;
                        foreach (var item in sqlItems)
                        {
                            if (item == null) continue;
                            if (item.IsBatch)
                            {
                                executeCount += ExecuteSqlBulkCopy(sqlConnection, item.SourseData, item.TableName, sqlTransaction);
                            }
                            else
                            {
                                sqlCommand.CommandText = item.SqlValue;
                                AddParams(sqlCommand, item.CustomType, item.Params);
                                int cnt = sqlCommand.ExecuteNonQuery();
                                executeCount += cnt;
                                sqlCommand.Parameters.Clear();
                            }
                        }
                        sqlTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        sqlTransaction.Rollback();
                        executeCount = -1;
                    }
                    sqlCommand.Dispose();
                    sqlTransaction.Dispose();
                    sqlConnection.Dispose();
                    return executeCount;
                }
            }
        }
        /// <summary>
        /// SQL执行事务 
        /// </summary>
        private int ExecuteSqlBulkCopy(SqlConnection conn, DataTable sourceData, String tableName, SqlTransaction sqlTransaction)
        {
            if (sourceData != null && sourceData.Rows.Count > 0)
            {
                int count = sourceData.Rows.Count;
                using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, sqlTransaction))
                {
                    sbc.DestinationTableName = tableName;
                    sbc.BatchSize = 500000;
                    sbc.BulkCopyTimeout = 500000;
                    for (int i = 0; i < sourceData.Columns.Count; i++)
                    {
                        sbc.ColumnMappings.Add(sourceData.Columns[i].ColumnName, sourceData.Columns[i].ColumnName);
                    }
                    sbc.WriteToServer(sourceData);
                    sourceData.Dispose();
                    return count;
                }
            }
            return 0;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        private void AddParams(SqlCommand cmd, String customType, params IDataParameter[] sqlParams)
        {
            if (sqlParams != null)
            {
                foreach (SqlParameter parameter in sqlParams)
                {
                    if (!string.IsNullOrEmpty(customType))
                    {
                        parameter.SqlDbType = SqlDbType.Structured;
                        parameter.TypeName = customType;
                    }
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 获取SQL语句

        #region 获取插入语句（批量）
        /// <summary>
        /// 获取插入语句（批量）
        /// </summary>
        public List<SqlEntity> InsertSqlItemsList<T>(List<T> source, string tableName = "") where T : class, new()
        {
            List<SqlEntity> list = new List<SqlEntity>();
            foreach (var s in source)
            {
                list.Add(InsertSqlItem(s, tableName));
            }
            return list;
        }
        #endregion

        #region 获取插入语句

        /// <summary>
        /// 获取插入语句
        /// </summary>
        public SqlEntity InsertSqlItem<T>(T t, string tableName = "") where T : class, new()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var properties = typeof(T).GetProperties().Where(p => p.GetMethod.IsVirtual == false && p.PropertyType != typeof(byte[]));
                stringBuilder.AppendFormat("INSERT INTO [{0}]", string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName);
                stringBuilder.AppendFormat("({0})", string.Join(",", properties.Select(p => p.Name)));
                stringBuilder.AppendFormat("VALUES({0})", string.Join(",", properties.Select(q => "@" + q.Name)));
                return new SqlEntity { SqlValue = stringBuilder.ToString(), Params = properties.Select(p => new SqlParameter("@" + p.Name, p.GetValue(t))).ToArray() };
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 获取更新语句（批量）
        /// <summary>
        /// 获取更新语句（批量）
        /// </summary>
        public List<SqlEntity> UpdateSqlItemsList<T>(List<T> source, string tableName = "") where T : class, new()
        {
            List<SqlEntity> list = new List<SqlEntity>();
            foreach (var s in source)
            {
                list.Add(UpdateSqlItem(s, tableName));
            }
            return list;
        }
        #endregion

        #region 获取更新语句
        /// <summary>
        /// 获取更新语句
        /// </summary>
        public SqlEntity UpdateSqlItem<T>(T t, string tableName = "") where T : class, new()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var properties = typeof(T).GetProperties().Where(p => p.GetMethod.IsVirtual == false && p.PropertyType != typeof(byte[]));
                stringBuilder.AppendFormat("UPDATE [{0}] ", string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName);

                stringBuilder.AppendFormat("SET {0} ", string.Join(",", properties.Select(p => p.Name + "=@" + p.Name)));
                stringBuilder.AppendFormat("WHERE Id=@Id ");
                return new SqlEntity { SqlValue = stringBuilder.ToString(), Params = properties.Select(p => new SqlParameter("@" + p.Name, p.GetValue(t))).ToArray() };
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 获取删除语句（批量）
        /// <summary>
        /// 获取删除语句（批量）
        /// </summary>
        public List<SqlEntity> DeleteSqlItemList<T>(List<T> source, string tableName = "") where T : class, new()
        {
            List<SqlEntity> list = new List<SqlEntity>();
            foreach (var s in source)
            {
                list.Add(DeleteSqlItem(s, tableName));
            }
            return list;
        }
        #endregion

        #region 获取删除语句
        /// <summary>
        /// 获取删除语句
        /// </summary>
        public SqlEntity DeleteSqlItem<T>(T t, string tableName = "") where T : class, new()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var properties = typeof(T).GetProperties().Where(p => p.GetMethod.IsVirtual == false && p.PropertyType != typeof(byte[]));
                if (properties.Where(p => p.Name.ToLower() == "id") == null) return null;
                stringBuilder.AppendFormat("DELETE FROM [{0}] ", string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName);
                stringBuilder.AppendFormat(" WHERE Id= @Id");
                return new SqlEntity { SqlValue = stringBuilder.ToString(), Params = properties.Where(p => p.Name.ToLower() == "id").Select(p => new SqlParameter("@Id", p.GetValue(t))).ToArray() };
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #endregion
    }
}
