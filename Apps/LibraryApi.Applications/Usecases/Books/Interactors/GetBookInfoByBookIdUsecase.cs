using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;

namespace LibraryApi.Applications.Usecases.Books.Interactors;

public class GetBookInfoByBookIdUsecase : IGetBookInfoByBookIdUsecase
{
    //プロパティ
    public IBookRepository _repository { get; set; }

    //コンストラクタ
    public GetBookInfoByBookIdUsecase(IBookRepository repository)
    {
        _repository = repository;
    }

    //method
    public async Task<Book> GetInfoAsync(string id) 
    {
        var result = await _repository.SelectByIdWithBookStockAndBookCategoryAsync(id);
        if (result is null)
        {
            throw new NotFoundException("指定された図書が存在しません");
        }
        return result;
    }
}
