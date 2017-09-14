using System;
using System.Data;
using System.Data.Sql.Table;
using System.Data.Sql.Table.Attribute;

namespace Lushi.PaperProducts.Model.Table
{
    [Serializable]
    public class ModelColumnType
    {
        [TableFieldInfo(Lable = TableFieldLable.Identity | TableFieldLable.PrimaryKey)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string EnName { get; set; }

        public EnumColumnTypeLevel.Value Level { get; set; }

        public int ParentId { get; set; }
        
        public EnumCharacter.Value Character { get; set; }

        [TableFieldInfo(SqlDbType.Int)]
        public int Icon { get; set; }

        public int Rank { get; set; }

        public EnumStatus.Value Status { get; set; }

        public DateTime AddTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
