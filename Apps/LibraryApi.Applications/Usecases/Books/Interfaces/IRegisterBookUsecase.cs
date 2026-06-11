using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;
namespace LibraryApi.Applications.Usecases.Books.Interfaces;

public interface IRegisterBookUsecase
{
    Task<List<BookCategory>> GetCategoriesAsync();
    Task<BookCategory> GetCategoryByIdAsync(string id);
    Task<bool> ExistsByBookNameAsync(string name);
    Task RegisterBookAsync(Book book);
}