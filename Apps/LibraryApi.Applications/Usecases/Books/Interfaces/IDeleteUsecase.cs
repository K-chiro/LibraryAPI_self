using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;

namespace LibraryApi.Applications.Usecases.Books.Interfaces;

public interface IDeleteUsecase
{
    Task<Book> GetBookByIdAsync(string id);
    Task DeleteBookAsync(Book book);
}
