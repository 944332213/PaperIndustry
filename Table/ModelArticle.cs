using System;
using System.Data.Sql.Table;
using System.Data.Sql.Table.Attribute;

namespace Lushi.PaperProducts.Model.Table
{
    [Serializable]
    public class ModelArticle
    {
        [TableFieldInfo(Lable = TableFieldLable.Identity | TableFieldLable.PrimaryKey)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Intro { get; set; }

        public string Content { get; set; }

        public string EffectPicture { get; set; }

        public int TypeId { get; set; }

        public EnumCharacter.Value Character { get; set; }

        public int Rank { get; set; }

        public EnumStatus.Value Status { get; set; }

        public DateTime AddTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
