using LibraryApi.Domains.Models;
namespace LibraryApi.Applications.Usecases.Books.Interfaces;
/// <summary>
/// ユースケース:[図書を変更する]を実現するインターフェイス
/// </summary>
public interface IUpdateBookUsecase
{


    /// <summary>
    /// 指定ざれた図書の存在有無を調べる
    /// </summary>
    /// <param name="bookName">図書目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一図書名が存在する場合にスローされる</exception>
    Task ExistsByBookNameAsync(string bookName);

    /// <summary>
    /// 図書を変更するする
    /// </summary>
    /// <param name="book">変更対象対象図書</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">図書が存在しない場合にスローされる</exception>
    Task<Book?> UpdateBookAsync(Book book );
}