using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Applications.Exceptions;
namespace LibraryApi.Applications.Tests.Applications.Domains.Models;
/// <summary>
/// Bookクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class BookTests
{
    // ヘルパー：有効なカテゴリ
    private BookCategory CreateCategory(string title = "雑誌") =>
    new BookCategory(title);
    // ヘルパー：有効な在庫
    private BookStock CreateStock(int stock = 10) => new BookStock(stock);

    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var uuid = Guid.NewGuid().ToString();
        var title = "testタイトル";
        var author = "テスト著者";
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Book(uuid, title, author, category, stock);
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 図書Idを検証する
        Assert.AreEqual(uuid, product.BookUuid);
        // 図書名を検証する
        Assert.AreEqual(title, product.Title);
        // 単価を検証する
        Assert.AreEqual(author, product.Author);
        // 図書カテゴリを検証する
        Assert.AreEqual(category, product.Category);
        // 図書在庫を検証する
        Assert.AreEqual(stock, product.Stock);
    }

    [TestMethod("新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var title = "testタイトル";
        var author = "テスト著者";
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Book(title, author, category, stock);
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 図書IdがUUID形式かどうかを検証する
        Assert.IsTrue(Guid.TryParse(product.BookUuid, out _));
        // 図書名を検証する
        Assert.AreEqual(title, product.Title);
        // 単価を検証する
        Assert.AreEqual(author, product.Author);
        // 図書カテゴリを検証する
        Assert.AreEqual(category, product.Category);
        // 図書在庫を検証する
        Assert.AreEqual(stock, product.Stock);
    }

    [TestMethod("不正なUUIDの場合、DomainExceptionがスローされる")]
    public void InvalidUuid_ShouldThrowDomainException()
    {
        // 不正なUUIDを用意する
        var invalidUuid = "abcde";
        var title = "testタイトル";
        var author = "テスト著者";
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(invalidUuid, title, author, category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("UUIDの形式が正しくありません。", ex.Message);
    }

    [TestMethod("図書が空白の場合、DomainExceptionがスローされる")]
    public void EmptyBookTitle_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(Guid.NewGuid().ToString(), "", "テスト著者", category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("図書名は必須です。", ex.Message);
    }

    [TestMethod("図書名が51文字以上の場合、DomainExceptionがスローされる")]
    public void CategoryTitleLongerThan30Chars_ShouldThrowDomainException()
    {
        var title = new string('あ', 51); // 31文字
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(Guid.NewGuid().ToString(), title, "テスト著者", category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("図書名は1~50文字で入力してください", ex.Message);
    }


    [TestMethod("有効な図書名に変更できる")]
    public void BookTitle_WithValidValue_ShouldSucceed()
    {
                var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Book("旧タイトル", "テスト著者", category, stock);
        // 図書名を変更する
        product.ChangeTitle("新図書");
        // 変更結果を検証する
        Assert.AreEqual("新図書", product.Title);
    }

    [TestMethod("有効な著者に変更できる")]
    public void BookAuthor_WithValidValue_ShouldSucceed()
    {
                var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Book("図書", "テスト著者", category, stock);
        // 単価を変更する
        product.ChangeAuthor("テスト著者2");
        // 変更結果を検証する
        Assert.AreEqual("テスト著者2", product.Author);
    }

    [TestMethod("有効な図書カテゴリに変更できる")]
    public void BookCategory_WithValidValue_ShouldSucceed()
    {
                var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var newCategory = CreateCategory("新カテゴリ");
        var product = new Book("図書",  "テスト著者", category, stock);
        // 図書カテゴリを変更する
        product.ChangeCategory(newCategory);
        // 図書カテゴリを検証する
        Assert.AreEqual("新カテゴリ", product.Category!.Name);
    }


    [TestMethod("有効な図書在庫に変更できる")]
    public void BookStock_WithValidValue_ShouldSucceed()
    {
                var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var newStock = CreateStock(30);
        var product = new Book("図書",  "テスト著者", category, stock);
        // 図書在庫を変更する
        product.ChangeStock(newStock);
        // 図書在庫を検証する
        Assert.AreEqual(30, product.Stock!.Stock);
    }

    [TestMethod("UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
                var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var uuid = Guid.NewGuid().ToString();
        var p1 = new Book(uuid, "A",  "テスト著者", category, stock);
        var p2 = new Book(uuid, "B",  "テスト著者", category, stock);
        // 等価性を検証する
        var result = p1.Equals(p2);
        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod("異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {     
           var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var p1 = new Book("A",  "テスト著者", category, stock);
        var p2 = new Book("B",  "テスト著者", category, stock);
        // 等価性を検証する
        var result = p1.Equals(p2);
        // 非等価であることを評価する
        Assert.IsFalse(result);
    }
}