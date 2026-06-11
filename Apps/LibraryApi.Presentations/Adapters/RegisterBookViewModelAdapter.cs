using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.ViewModels;
namespace LibraryApi.Presentations.Adapters;
/// <summary>
/// RegisterBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ
/// </summary>
public class RegisterBookViewModelAdapter : IRestorer<Book, RegisterBookViewModel>
{
    /// <summary>
    /// RegisterBookViewModelからドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">ユースケース:[新商品を登録する]を実現するViewModel</param>
    /// <param name="categoryName">カテゴリー名
    /// <returns></returns>
    public Task<Book> RestoreAsync(RegisterBookViewModel target)
    {
        // 商品カテゴリを生成する
        var category = new BookCategory(target.CategoryId, target.CategoryName!);
        // 商品在庫を生成する
        var bookStock = new BookStock(target.Stock);
        // 商品を生成する
        var book = new Book(Guid.NewGuid().ToString(), target.Title, target.Author);
        // 商品カテゴリと商品在庫を設定する
        book.ChangeCategory(category);
        book.ChangeStock(bookStock);
        return Task.FromResult(book);
    }

    public Task<RegisterBookViewModel> TransAsync(RegisterBookRequestViewModel target)
    {

        var model = new RegisterBookViewModel
        {
            Title = target.Title,
            Author = target.Author,
            Stock = target.Stock,
            CategoryId = target.CategoryId,
        };
        return Task.FromResult(model);
    }
}