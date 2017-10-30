namespace System.Data.Sql.Table.Attribute
{
    /// <summary>
    /// 数据表字段信息特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableFieldInfoAttribute : System.Attribute
    {
        public TableFieldInfoAttribute() { }

        public TableFieldInfoAttribute(SqlDbType type)
        {
            Type = type;
        }

        /// <summary>
        /// 标签
        /// </summary>
        public TableFieldLable Lable { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public SqlDbType? Type { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public int? Size { get; set; }
    }
}
