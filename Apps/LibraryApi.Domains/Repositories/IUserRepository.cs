using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;

namespace LibraryApi.Domains.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username, string email);
    Task<User?> SelectByUsernameAsync(string username);
    Task<User?> SelectByIdAsync(string id);
}
