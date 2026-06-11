using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Infrastructures.Entities;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Adapters;
using LibraryApi.Applications.Exceptions;


namespace LibraryApi.Infrastructures.Adapters;

public class BookCategoryEntityAdapter : IConverter<BookCategory, BookCategoryEntity>, IRestorer<BookCategory, BookCategoryEntity>
{

    public Task<BookCategoryEntity> ConvertAsync(BookCategory domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:BookCategoryをBookCategoryEntityに変換する
        var entity = new BookCategoryEntity();
        entity.CategoryUuid = domain.CategoryUuid;
        entity.Name = domain.Name;
        return Task.FromResult(entity);
    }


    public Task<BookCategory> RestoreAsync(BookCategoryEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // BookCategoryEntityからドメインオブジェクト:BookCategoryを復元する
        var domain = new BookCategory(target.CategoryUuid, target.Name);
        return Task.FromResult(domain);
    }
}
