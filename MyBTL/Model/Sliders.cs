using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBTL.Model
{
    [Table("Sliders")]
    public class Sliders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string URL { get; set; }

        public string Img { get; set; }

        public int? Order { get; set; }

        public string Position { get; set; }

        public int CreateBy { get; set; }

        public DateTime CreateAt { get; set; }

        public int UpdateBy { get; set; }

        public DateTime UpdateAt { get; set; }

        public int Status { get; set; }
    }
}
