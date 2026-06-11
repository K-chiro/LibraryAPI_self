using System;
using LibraryApi.Domains.Exceptions;

namespace LibraryApi.Domains.Models;

public class Book
{
    public string BookUuid { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
  
    public BookCategory? Category { get; private set; }
    public BookStock? Stock { get; private set; }

//復元用コンストラクタ
    public Book(string bookUuid, string title, string author, BookCategory category, BookStock stock)
    {
        ValidateBookUuid(bookUuid);
        ValidateTitle(title);
        ValidateAuthor(author);

        BookUuid = bookUuid;
        Title = title;
        Author = author;
        Category = category ?? throw new DomainException("カテゴリは必須です。");
        Stock = stock ?? throw new DomainException("在庫情報は必須です。");
    }

    //復元用コンストラクタ
    public Book(string bookUuid, string title, string author)
    {
        ValidateBookUuid(bookUuid);
        ValidateTitle(title);
        ValidateAuthor(author);

        BookUuid = bookUuid;
        Title = title;
        Author = author;
    }

    public Book(string title, string author, BookCategory category, BookStock stock)
        : this(Guid.NewGuid().ToString(), title, author, category, stock)
    {
    }

    public void ChangeTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
    }

    public void ChangeAuthor(string author)
    {
        ValidateAuthor(author);
        Author = author;
    }

    public void ChangeCategory(BookCategory category)
    {
        Category = category ?? throw new DomainException("カテゴリは必須です。");
    }

    public void ChangeStock(BookStock stock)
    {
        Stock = stock ?? throw new DomainException("在庫情報は必須です。");
    }

    public override bool Equals(object? obj)
    {
        return obj is Book other && string.Equals(BookUuid, other.BookUuid, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return BookUuid is null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(BookUuid);
    }

  public override string ToString()
        => $"{BookUuid}: {Title} , {Author} / {Category?.Name ?? "未分類"} , 在庫: {Stock?.Stock ?? 0}";



    private static void ValidateBookUuid(string bookUuid)
    {
        if (string.IsNullOrWhiteSpace(bookUuid))
        {
            throw new DomainException("UUIDは必須です。");
        }

        if (!Guid.TryParse(bookUuid, out _))
            throw new DomainException("UUIDの形式が正しくありません。");
    }

    private const int MaxTitleLength = 50;

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("タイトルは必須です。");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new DomainException($"タイトルは{MaxTitleLength}文字以内である必要があります。");
        }
    }

    private const int MaxAuthorLength = 30;

    private static void ValidateAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new DomainException("著者は必須です。");
        }

        if (author.Length > MaxAuthorLength)
        {
            throw new DomainException($"著者名は{MaxAuthorLength}文字以内である必要があります。");
        }
    }
}
