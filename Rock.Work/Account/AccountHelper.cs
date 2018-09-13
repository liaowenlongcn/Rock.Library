using Rock.Library.Helper;
using Rock.Work.Account.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Rock.Work.Account
{
    public class AccountHelper
    {
        public static void OldUserIdToNew()
        {
            try
            {
                var list = JsonFileHelper.GetList<UserIdInfo>();
                var accountConnection = WebConfigurationManager.AppSettings["AccountConnection"].ToString();
                var fields = new List<string>() { "CreateUserId", "UpdateUserId", "UserId" };

                #region SQL模板
                var sqlTemplate = @"DECLARE @FieldName VARCHAR(50)='{0}' 
                                DECLARE @OldFieldValue VARCHAR(50)='{1}' 
                                DECLARE @NewFieldValue VARCHAR(50)='{2}' 
                                DECLARE @Rows INT,@Row INT=1
                                DECLARE @TableName VARCHAR(100)
                                DECLARE @Sql VARCHAR(MAX)=''

                                SELECT  ROW_NUMBER() OVER (ORDER BY o.name ASC) AS num,o.name INTO #Tables FROM sysobjects o 
                                INNER JOIN syscolumns c ON  o.id = c.id 
                                WHERE c.name = @FieldName ORDER BY name 

                                SELECT @Rows=@@rowcount+1

                                WHILE(@Row<@Rows)
                                BEGIN
	                                SELECT @TableName=name FROM #Tables where num=@Row
	                                --自定义需重复执行的SQL语句
	                                SELECT @Sql+='UPDATE '+@TableName+' SET '+@FieldName+'='''+@NewFieldValue+''' where '+@FieldName+'='''+@OldFieldValue+''' '+CHAR(10)
	                                SELECT @Row+=1
                                END
 
                                exec(@Sql)

                                DROP TABLE #Tables ";
                #endregion

                foreach (var item in list)
                {
                    foreach (var field in fields)
                    {
                        var sql = string.Format(sqlTemplate, field, item.OldId, item.NewId);
                        var reslut = SQLHelper.ExecuteSQL(accountConnection, sql);
                        if (reslut)
                            LogHelper.Info(new string[] { "Success", "Field:" + field + ",OldId:" + item.OldId + ",NewId:" + item.NewId });
                        else
                            LogHelper.Info(new string[] { "Error", "Field:" + field + ",OldId:" + item.OldId + ",NewId:" + item.NewId });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
    }
}
