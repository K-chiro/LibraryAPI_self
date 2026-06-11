using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;

namespace LibraryApi.Domains.Repositories;

public interface IBookCategoryRepository
{
    Task<List<BookCategory>> SelectAllAsync();
    Task<BookCategory?> SelectByIdAsync(string id);
}
