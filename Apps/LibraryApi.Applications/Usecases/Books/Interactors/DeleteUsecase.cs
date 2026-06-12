using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;

namespace LibraryApi.Applications.Usecases.Books.Interactors;

public class DeleteBookUsecase : IDeleteUsecase
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="bookRepository">図書CRUD操作リポジトリ</param>
    /// <param name="unitOfWork">トランザクション制御機能</param>
    public DeleteBookUsecase(
        IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Book> GetBookByIdAsync(string id)
    {
        var result = await _bookRepository
            .SelectByIdWithBookStockAndBookCategoryAsync(id);
        if (result is null)
        {
            throw new NotFoundException($"商品Id:{id}の商品は存在しません。");
        }
        return result;
    }


    /// <summary>
    /// 図書を変更するする
    /// </summary>
    /// <param name="book">変更対象対象図書</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">図書が存在しない場合にスローされる</exception>
    public async Task DeleteBookAsync(Book book)
    {
        // トランザクションを開始する
        await _unitOfWork.BeginAsync();
        try
        {
            var result = await _bookRepository.DeleteByIdAsync(book.BookUuid);
            if (result is false)
            {
                throw new NotFoundException($"図書Id:{book.Title}の図書は存在しないため変更できません。");
            }
            // トランザクションをコミットする
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            // トランザクションをロールバッする
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
