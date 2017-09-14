namespace System.Data.Sql.Table
{
    /// <summary>
    /// 数据表字段标签枚举
    /// </summary>
    [Flags]
    public enum TableFieldLable
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// IDENTITY
        /// 标识列
        /// </summary>
        Identity = 1,

        /// <summary>
        /// PRIMARY_KEY
        /// 主键
        /// </summary>
        PrimaryKey = 2,

        /// <summary>
        /// FOREIGN_KEY
        /// 外键
        /// </summary>
        ForeignKey = 4,

        /// <summary>
        /// UNIQUE
        /// 唯一
        /// </summary>
        Unique = 8,
    }
}
