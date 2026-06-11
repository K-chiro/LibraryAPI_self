using System.Threading.Tasks;
using LibraryApi.Domains.Models;

namespace LibraryApi.Applications.Usecases.Users.Interfaces;

public interface IRegisterUserUsecase
{
    Task ExistsByUsernameOrEmailAsync(string username, string email);
    Task RegisterUserAsync(User user);
}