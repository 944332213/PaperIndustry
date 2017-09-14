using System;
using System.Data.Sql.Table;
using System.Data.Sql.Table.Attribute;

namespace Lushi.PaperProducts.Model.Table
{
    [Serializable]
    public class ModelGuestbook
    {
        [TableFieldInfo(Lable = TableFieldLable.Identity | TableFieldLable.PrimaryKey)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public EnumStatus.Value Status { get; set; }

        public DateTime AddTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
