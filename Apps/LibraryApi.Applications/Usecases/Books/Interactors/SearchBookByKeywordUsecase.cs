using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
namespace LibraryApi.Applications.Usecases.Books.Interactors;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するインターフェイスの実装
/// </summary>
public class SearchBookByKeywordUsecase : ISearchBookByKeywordUsecase
{
    private readonly IBookRepository _repository;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">商品CRUD操作リポジトリ</param>
    public SearchBookByKeywordUsecase(IBookRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索した結果を返す
    /// </summary>
    /// <param name="keyword">商品キーワード</param>
    /// <returns>キーワード検索結果</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    public async Task<List<Book>> ExecuteAsync(string keyword)
    {
        var result = await _repository
            .SelectByNameLikeWithBookStockAndBookCategoryAsync(keyword);
        return result;
    }
}