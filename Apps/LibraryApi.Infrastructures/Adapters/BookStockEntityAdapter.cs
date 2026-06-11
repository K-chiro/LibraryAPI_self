using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Infrastructures.Entities;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Adapters;
using LibraryApi.Applications.Exceptions;

namespace LibraryApi.Infrastructures.Adapters;

public class BookStockEntityAdapter:IConverter<BookStock, BookStockEntity>, IRestorer<BookStock, BookStockEntity>
{
    /// <summary>
    /// ドメインオブジェクト:BookStockをBookStockEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:BookStock</param>
    /// <returns>EFCore:BookStockEntity</returns>
    public Task<BookStockEntity> ConvertAsync(BookStock domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:BookStockをBookStockEntityに変換する
        var entity = new BookStockEntity();
        entity.StockUuid = domain.StockUuid;
        entity.Stock = domain.Stock;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// BookStockEntityからドメインオブジェクト:BookStockを復元する
    /// </summary>
    /// <param name="target">>EFCore:BookStockEntity</param>
    /// <returns>ドメインオブジェクト:BookStock</returns>
    public Task<BookStock> RestoreAsync(BookStockEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // BookStockEntityからドメインオブジェクト:BookStockを復元する
        var domain = new BookStock(target.StockUuid, target.Stock);
        return Task.FromResult(domain);
    }
}
