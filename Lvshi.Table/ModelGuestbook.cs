using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lvshi.PaperProducts.Model.Table
{
    [Table("ColumnType")]
    [Serializable]
    public class ModelGuestbook
    {
        [Key]
        [Identity]
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
