using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using Microsoft.Extensions.Configuration;
using LibraryApi.Presentations.Configs;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Models;


namespace LibraryApi.Applications.Tests.Usecases.Books.Interactors;

[TestClass]
[TestCategory("Usecase/Books/Interactor")]
public class DeleteBookUsecaseTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private static IDeleteUsecase? _usecase;
    // 商品リポジトリ
    private static IBookRepository? _bookRepository;


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
            .AddJsonFile("appsettings.json", optional: false).Build();
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
        _usecase =
        _scope.ServiceProvider.GetRequiredService<IDeleteUsecase>();
        // 商品リポジトリを取得する
        _bookRepository =
        _scope.ServiceProvider.GetRequiredService<IBookRepository>();
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


    [TestMethod("存在する商品を削除できる")]
    public async Task DeleteAsync_ExistingBook_ShouldDeleteSuccessfully()
    {
        var uuid = "726eaff4-4d5d-4de7-8758-50850a0c8989";
        var category = new BookCategory("18836923-5194-47f1-bf4c-e09eb5fa8fef", "技術書");
        var stock = new BookStock("e7bd6533-9946-4b5a-b378-5c480d07390d", 5);
        var book = new Book(uuid, "test2", "testAuthor2", category, stock);

        await _bookRepository!.CreateAsync(book);
        await _usecase!.DeleteBookAsync(book);

        var exists = await _bookRepository!.ExistsByNameAsync(book.BookUuid);
        Assert.IsFalse(exists, "削除されたはずの図書がまだ存在しています。");
    }

    [TestMethod("存在しない商品を削除できず、InternalExceptionがスローされる")]
    public async Task DeleteAsync_NonExistingBook_ShouldThrowInternalException()
    {
        var uuid = "726eaff4-4d5d-4de7-8758-51850a0c8989";
        var category = new BookCategory("18836923-5194-47f1-bf4c-e09eb5fa8fef", "技術書");
        var stock = new BookStock("e7bd6533-9946-4b5a-b378-5c480d07390d", 5);
        var book = new Book(uuid, "test2", "testAuthor2", category, stock);

        var ex = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            await _usecase!.DeleteBookAsync(book);
        });

        Assert.AreEqual($"Id:{uuid}の図書削除中に予期しないエラーが発生しました。", ex.Message);
    }
}
