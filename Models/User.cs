using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar_1.Models
{
    [Table("users")]
    public class User
    {
        public User()
        {
            UserName = string.Empty;
            Password= string.Empty;
        }

        public int Id { get; set; }
        [Column("username")]
        public string UserName { get; set; }
        [Column("passwrd")]
        public string Password { get; set; }
        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}
