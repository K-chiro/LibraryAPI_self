using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.ViewModels;
namespace LibraryApi.Presentations.Adapters;
/// <summary>
/// UpdateBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ
/// </summary>
public class UpdateBookViewModelAdapter : IRestorer<Book, UpdateBookRequestViewModel>, IConverter<Book, UpdateBookResponseViewModel>
{
    /// <summary>
    /// UpdateBookViewModelからドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">ユースケース:[商品を変更する]を実現するViewModel</param>
    /// <returns></returns>
    public async Task<Book> RestoreAsync(UpdateBookRequestViewModel target)
    {
        // // 商品在庫を生成する
        // var bookStock = new BookStock(target.Stock);
        // // 商品を生成する
        // var book = new Book(Id, target.Title, target.Author);
        // // 商品在庫を設定する
        // book.ChangeStock(bookStock);
        // return Task.FromResult(book);
        return  null;
    }

    public async Task<Book> TransAsync(UpdateBookRequestViewModel target, string id)
    {
        // 商品在庫を生成する
        var bookStock = new BookStock(target.Stock);
        // 商品を生成する
        var book = new Book(id, target.Title, target.Author);
        // 商品在庫を設定する
        book.ChangeStock(bookStock);
        return await Task.FromResult(book);
    }

    public async Task<UpdateBookResponseViewModel> ConvertAsync(Book book)
    {
        var result = new UpdateBookResponseViewModel
        {
            BookId = book.BookUuid,
            Title = book.Title,
            Author = book.Author,
            Stock = book.Stock?.Stock, 

            Category = new UpdateBookResponseViewModel.CategoryInfo
            {
                CategoryId = book.Category.CategoryUuid ,
                Name = book.Category.Name 
            }
        };

        return await Task.FromResult(result);
    }
}