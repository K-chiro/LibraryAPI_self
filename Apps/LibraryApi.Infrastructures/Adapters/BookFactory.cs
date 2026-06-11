using LibraryApi.Domains.Models;
using LibraryApi.Infrastructures.Entities;
using LibraryApi.Infrastructures.Adapters;

namespace LibraryApi.Infrastructures.Adapters;
/// <summary>
/// 商品、商品カテゴリ、商品在庫オブジェクトの相互変換Factoryクラス
/// ドメインオブジェクト:BookとBookEntityの相互変換
/// ドメインオブジェクト:BookCategoryとBookEntityの相互変換
/// ドメインオブジェクト:BookStockとBookStockEntityの相互変換
/// </summary>
public class BookFactory
{
    private readonly BookEntityAdapter _bookEntityAdapter;
    private readonly BookCategoryEntityAdapter _bookCategoryEntityAdapter;
    private readonly BookStockEntityAdapter _bookStockEntityAdapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="bookEntityAdapter">BookとBookEntityの相互変換</param>
    /// <param name="bookCategoryEntityAdapter">BookCategoryとBookEntityの相互変換</param>
    /// <param name="bookStockEntityAdapter">BookStockとBookStockEntityの相互変換</param>
    public BookFactory(
        BookEntityAdapter bookEntityAdapter,
        BookCategoryEntityAdapter bookCategoryEntityAdapter,
        BookStockEntityAdapter bookStockEntityAdapter)
    {
        _bookEntityAdapter = bookEntityAdapter;
        _bookCategoryEntityAdapter = bookCategoryEntityAdapter;
        _bookStockEntityAdapter = bookStockEntityAdapter;
    }

    /// <summary>
    /// 商品、商品カテゴリ、商品在庫の集約関係を構築したEntityを生成して返す
    /// </summary>
    /// <param name="domain">ルートドメインオブジェクト:Book</param>
    /// <returns>集約関係を構築したBookEntity</returns>
    public async Task<BookEntity> ConvertAsync(Book domain)
    {
        // BookからBookEntityを生成する
        var entity = await _bookEntityAdapter.ConvertAsync(domain);
        // 商品カテゴリ、在庫が存在しない場合はリターンする
        if (domain.Category is null && domain.Stock is null)
        {
            return entity;
        }
        // 商品カテゴリが存在する
        if (domain.Category != null)
        {
            // CategoryをCategoryEntityに変換してプロパティに設定する
            entity.BookCategory =
                await _bookCategoryEntityAdapter.ConvertAsync(domain.Category);
        }
        // 在庫が存在する
        if (domain.Stock != null)
        {
            // BookStockをBookStockEntityに変換してプロパティに設定する
            entity.BookStock =
                await _bookStockEntityAdapter.ConvertAsync(domain.Stock);
        }
        return entity;
    }

    /// <summary>
    /// 商品、商品カテゴリ、商品在庫の集約関係を構築したEntityリストを生成して返す
    /// </summary>
    /// <param name="domains">ルートドメインオブジェクトのリスト:List<Book></param>
    /// <returns>集約関係を構築したBookEntityのリスト</returns>
    public async Task<List<BookEntity>> ConvertAsync(List<Book> domains)
    {
        // BookEntityのリストを生成する
        var entityies = new List<BookEntity>();
        foreach (var domain in domains)
        {
            // リストから取り出したBookをBookEntityに変換してリストに追加する
            entityies.Add(await ConvertAsync(domain));
        }
        return entityies;
    }

    /// <summary>
    /// BookEntityの集約関係からドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">BookEntity</param>
    /// <returns>復元したBook</returns>
    public async Task<Book> RestoreAsync(BookEntity target)
    {
        // BookEntityからBookを復元する
        var book = await _bookEntityAdapter.RestoreAsync(target);
        // 商品カテゴリ、商品在庫が存在しない場合はリターンする   
        if (target.BookCategory is null && target.BookStock is null)
        {
            return book;
        }
        // 商品カテゴリが存在する
        if (target.BookCategory != null)
        {
            // BookCategoryEntityからBookCategoryを復元してプロパティに設定する
            book.ChangeCategory(
                await _bookCategoryEntityAdapter.RestoreAsync(target.BookCategory));
        }
        // 商品在庫が存在する
        if (target.BookStock != null)
        {
            // BookStockEntityからBookStockを復元してプロパティに設定する
            book.ChangeStock(
                await _bookStockEntityAdapter.RestoreAsync(target.BookStock));
        }
        return book;
    }

    /// <summary>
    /// 商品、商品カテゴリ、商品アジ子の集約関係を構築したEntityリストからドメインオブジェクトのリストを復元する
    /// </summary>
    /// <param name="targets">List<BookEntity></param>
    /// <returns>Book<List></returns>
    public async Task<List<Book>> RestoreAsync(List<BookEntity> targets)
    {
        // Bookのリストを生成する
        var books = new List<Book>();
        foreach (var target in targets)
        {
            // BookEntityを取り出しBookを復元してリストに追加する
            books.Add(await RestoreAsync(target));
        }
        return books;
    }
}