using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Model
{
    public class DataBaseStruct
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 字段排序
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnsName { get; set; }
        /// <summary>
        /// 字段描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumnType { get; set; }
        /// <summary>
        /// 字段最大长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; } 
    }
}
