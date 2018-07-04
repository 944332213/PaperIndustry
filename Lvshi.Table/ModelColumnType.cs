using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lvshi.PaperProducts.Model.Table
{
    [Table("ColumnType")]
    [Serializable]
    public class ModelColumnType
    {
        [Key]
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; }

        public string EnName { get; set; }

        public EnumColumnTypeLevel.Value Level { get; set; }

        public int ParentId { get; set; }

        public string Url { get; set; }
        
        public EnumCharacter.Value Character { get; set; }

        public EnumColumnTypeDisplayModel.Value DisplayModel { get; set; }
        
        public int Icon { get; set; }

        public int Rank { get; set; }

        public EnumStatus.Value Status { get; set; }

        public DateTime AddTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
