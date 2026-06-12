using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Infrastructures.Contexts;
using LibraryApi.Presentations.Configs;
namespace LibraryApi.Infrastructures.Tests.Repositories;
/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイスの実装の単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Repositories")]
public class BookRepositoryTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // アプリケーションで利用するDbContextの継承
    private static AppDbContext? _dbContext;
    // テストターゲット
    private static IBookRepository _bookRepository = null!;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;

    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="_"></param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
        _testContext = context;
        // アプリケーション管理を生成
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        // サービスプロバイダ(DIコンテナ)の生成 
        _provider = ApplicationDependencyExtensions.BuildAppProvider(config);
    }

    /// <summary>
    /// テストクラスクリーンアップ
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        // 生成したサービスプロバイダ(DIコンテナ)を破棄する
        _provider?.Dispose();
    }

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        // スコープドサービスを取得する
        _scope = _provider!.CreateScope();
        // テストターゲットを取得する
        _bookRepository =
        _scope.ServiceProvider.GetRequiredService<IBookRepository>();
        // AppDbContxetを取得する
        _dbContext =
        _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// テストメソッド実行後の後処理
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        // スコープドサービスを破棄する
        _scope!.Dispose();
    }

    [TestMethod("存在する商品Idで商品、商品在庫、商品カテゴリを取得できる")]
    public async Task SelectByIdWithBookStockAndBookCategoryAsync_WhenIdExists_ShouldReturnBookWithStockAndCategory()
    {
        var targetBookUuid = "94399b5c-7223-48c1-aab3-ea62378bdc13";

        var book = await _bookRepository
            .SelectByIdWithBookStockAndBookCategoryAsync(targetBookUuid);

        // nullでないことを検証する
        Assert.IsNotNull(book);

        // 商品Idを検証する
        Assert.AreEqual(targetBookUuid, book.BookUuid);

        Assert.AreEqual("リーダブルコード", book.Title);

        Assert.AreEqual("Dustin Boswell", book.Author);

        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(book.Stock);

        Assert.AreEqual("d1a3c77a-b148-4162-8dde-e5229f26cd48", book.Stock.StockUuid);

        Assert.AreEqual(5, book.Stock.Stock);

        Assert.AreEqual("18836923-5194-47f1-bf4c-e09eb5fa8fef", book.Category!.CategoryUuid);

        Assert.AreEqual("技術書", book.Category!.Name);
    }

    [TestMethod("存在しない商品Idの場合nullが返される")]
    public async Task SelectByIdWithBookStockAndBookCategoryAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        var book = await _bookRepository
        .SelectByIdWithBookStockAndBookCategoryAsync("8f81a72a-58ef-422b-b472-d982e8665282");
        // nullであることを検証する
        Assert.IsNull(book);
    }

    [TestMethod("商品と商品在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistBoth()
    {
        // 登録データを用意する
        var bookCategory = new BookCategory("51e7f90e-5d61-4546-aa42-e85d98fbe542", "漫画");
        var bookStock = new BookStock(Guid.NewGuid().ToString(), 20);
        var book = new Book(Guid.NewGuid().ToString(), "セーラームーン", "武内直子");
        book.ChangeStock(bookStock);
        book.ChangeCategory(bookCategory);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 商品と商品在庫を永続化する
                await _bookRepository.CreateAsync(book);
                // 登録された商品と商品在庫を取得して値を検証する
                var result = await _bookRepository
                     .SelectByIdWithBookStockAndBookCategoryAsync(book.BookUuid);
                // nullでないことを検証する
                Assert.IsNotNull(result);
                // 商品Idを検証する
                Assert.AreEqual(result.BookUuid, book.BookUuid);
                // 商品名を検証する
                Assert.AreEqual(result.Title, book.Title);
                // 単価を検証する
                Assert.AreEqual(result.Author, book.Author);
                // 商品在庫がnullでないことを検証する
                Assert.IsNotNull(result.Stock);
                // 商品在庫Idを検証する
                Assert.AreEqual(result.Stock.StockUuid, book.Stock!.StockUuid);
                // 在庫数を検証する
                Assert.AreEqual(result.Stock.Stock, book.Stock.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }

    [TestMethod("商品名が存在するとtrueが返される")]
    public async Task ExistsByName_WhenNameExists_ShouldReturnTrue()
    {
        var result = await _bookRepository.ExistsByNameAsync("リーダブルコード");
        Assert.IsTrue(result);
    }

    [TestMethod("商品名が存在しないとfalseが返される")]
    public async Task ExistsByName_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        var result = await _bookRepository.ExistsByNameAsync("蛍光ペン(黒)");
        Assert.IsFalse(result);
    }

    [TestMethod("存在する商品のキーワードを指定すると、該当する商品のリストが返される")]
    public async Task SelectByNameLikeWithBookStockAndBookCategoryAsync_WithExistingKeyword_ShouldReturnMatchingBooks()
    {
        var books = await _bookRepository
        .SelectByNameLikeWithBookStockAndBookCategoryAsync("達人");
        // nullでないことを検証する
        Assert.IsNotNull(books);
        // 件数が4件であることを検証する
        Assert.AreEqual(1, books.Count);
    }
    [TestMethod("存在しない商品のキーワードを指定すると、空の商品のリストが返される")]
    public async Task SelectByNameLikeWithBookStockAndBookCategoryAsync_WithNonExistingKeyword_ShouldReturnEmptyList()
    {
        var books = await _bookRepository
            .SelectByNameLikeWithBookStockAndBookCategoryAsync("商品-X");
        // nullでないことを検証する
        Assert.IsNotNull(books);
        // 件数が0であることを検証する
        Assert.AreEqual(0, books.Count);
    }

    [TestMethod("存在する商品を変更するとtrueが返される")]
    public async Task UpdateBook_WhenBookExists_ShouldReturnTrue()
    {
        // 変更データを準備する
        var bookStock = new BookStock("d1a3c77a-b148-4162-8dde-e5229f26cd48", 5);
        var book = new Book("94399b5c-7223-48c1-aab3-ea62378bdc13", "リーダブルコード", "Dustin Boswell");
        book.ChangeStock(bookStock);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 商品を変更する
                var result = await _bookRepository.UpdateByIdAsync(book);
                // 中身がであることを検証する
                Assert.IsNotNull(result);
                // 変更された商品を取得する
                var updateResult = await _bookRepository
                    .SelectByIdWithBookStockAndBookCategoryAsync(book.BookUuid);
                // 商品名を検証する
                Assert.AreEqual(book.Title, updateResult!.Title);
                // 単価を検証する
                Assert.AreEqual(book.Author, updateResult!.Author);
                // 商品在庫数を検証する
                Assert.AreEqual(book.Stock!.Stock, updateResult.Stock!.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }

    [TestMethod("存在しない商品を変更するとfalseが返される")]
    public async Task UpdateBook_WhenBookDoesNotExist_ShouldReturnFalse()
    {
        // 変更データを準備する
        var bookStock = new BookStock("828fb567-6f6b-11f0-954a-00155d1bd30a", 50);
        var book = new Book("ac413f22-0cf1-490a-9635-7e9ca810e555", "ボールペン(黒)", "テスト著者");
        book.ChangeStock(bookStock);
        // 商品を変更する
        var result = await _bookRepository.UpdateByIdAsync(book);
        // falseが返されることを検証する
        Assert.IsNull(result);
    }
}