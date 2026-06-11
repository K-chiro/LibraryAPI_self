using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;

namespace LibraryApi.Applications.Usecases.Books.Interfaces;

public interface IGetBookInfoByBookIdUsecase
{
    Task<Book> GetInfoAsync(string id);
}