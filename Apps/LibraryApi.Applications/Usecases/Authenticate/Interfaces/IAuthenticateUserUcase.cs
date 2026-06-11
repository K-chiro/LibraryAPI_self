using System.Threading.Tasks;
using LibraryApi.Domains.Models;
namespace LibraryApi.Applications.Usecases.Authenticate.Interfaces;

public interface IAuthenticateUserUsecase
{
    Task<User> AuthenticateAsync(string username, string password);
}