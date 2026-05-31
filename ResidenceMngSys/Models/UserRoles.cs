using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class UserRoles
    {
        [Column("İd")]
        public int Id { get; set; }

        
        [Column("User_İd")]
        public int User_Id {  get; set; }
        [ForeignKey("User_Id")]
        public User User { get; set; }


        [Column("Roles_İd")]
        public int Roles_Id { get; set; }

        [ForeignKey("Roles_Id")]
        public Role Role { get; set; }

    }
}
