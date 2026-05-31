using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class Role
    {
        [Column("İd")]
        public int Id { get; set; }
        public string RoleName { get; set; }
      
    }
}
