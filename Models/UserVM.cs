using ProiectMaster.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar_1.Models
{
    public class UserVM
    {
        public UserVM()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(2000)]
        public string Password { get; set; }
        public DateTime? LastLogin { get; set; }

        public static User VMUserToUser(UserVM vm)
        {
            var user = new User();

            user.UserName = vm.UserName;
            user.Password = vm.Password;
            user.LastLogin = vm.LastLogin;

            return user;
        }

        public UserVM ProdToProdVM(User? user)
        {
            if (user == null)
                return new UserVM();

            var vm = new UserVM();

            vm.UserName = user.UserName;
            vm.Password = user.Password;
            vm.LastLogin = user.LastLogin;

            return vm;
        }

    }
}
