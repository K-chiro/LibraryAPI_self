using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using Microsoft.Extensions.Configuration;
using LibraryApi.Presentations.Configs;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Models;

namespace LibraryApi.Application.Tests.Usecase.Books.Interactors;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Books/Interactor")]
public class RegisterBookUsecaseTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private static IRegisterBookUsecase? _uscase;
    // 商品リポジトリ
    private static IBookRepository? _productRepository;

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
        _uscase =
        _scope.ServiceProvider.GetRequiredService<IRegisterBookUsecase>();
        // 商品リポジトリを取得する
        _productRepository =
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

    [TestMethod("すべての商品カテゴリを取得できる")]
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        // すべてのカテゴリを取得する
        var categories = await _uscase!.GetCategoriesAsync();

        // nullでないことを検証する
        Assert.IsNotNull(categories);

        // 件数が6件であることを検証する（事実：右のテーブルには6レコード存在）
        Assert.AreEqual(6, categories.Count());

        // 取得内容を検証する（事実：右のテーブルのデータと完全に一致させています）
        // id: 1
        Assert.AreEqual("18836923-5194-47f1-bf4c-e09eb5fa8fef", categories[0].CategoryUuid);
        Assert.AreEqual("技術書", categories[0].Name);

        // id: 2
        Assert.AreEqual("1c7dc46b-5618-4d9b-ad4a-0a805e7032d6", categories[1].CategoryUuid);
        Assert.AreEqual("小説", categories[1].Name);

        // id: 3
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", categories[2].CategoryUuid);
        Assert.AreEqual("児童書", categories[2].Name);

        // id: 4
        Assert.AreEqual("9dd9db1f-14fe-42e5-879d-e1a2c74223d8", categories[3].CategoryUuid);
        Assert.AreEqual("ビジネス書", categories[3].Name);

        // id: 5
        Assert.AreEqual("51e7f90e-5d61-4546-aa42-e85d98fbe542", categories[4].CategoryUuid);
        Assert.AreEqual("漫画", categories[4].Name);

        // id: 6
        Assert.AreEqual("d652b797-d71a-4c4c-9539-65049819d942", categories[5].CategoryUuid);
        Assert.AreEqual("雑誌", categories[5].Name);

        foreach (var category in categories)
        {
            _testContext?.WriteLine(category.ToString());
        }
    }

    [TestMethod("存在する商品カテゴリIdで商品カテゴリを取得できる")]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenIdExists()
    {
        // 商品カテゴリ雑貨を取得する
        var category = await _uscase!.GetCategoryByIdAsync("18836923-5194-47f1-bf4c-e09eb5fa8fef");
        // nullでないことを検証する
        Assert.IsNotNull(category);
        // 商品カテゴリIdと商品カテゴリ名を検証する
        Assert.AreEqual("18836923-5194-47f1-bf4c-e09eb5fa8fef", category.CategoryUuid);
        Assert.AreEqual("技術書", category.Name);
    }

    [TestMethod("存在しない商品カテゴリIdを指定するとNotFoundExceptionがスローされる")]
    public async Task GetCategoryByIdAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
    {
        var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
        {
            // 存在しない商品カテゴリIdでカテゴリを取得する
            await _uscase!.GetCategoryByIdAsync("2f5016b6-6f6b-11f0-954a-00155d1bd30a");
        });
        Assert.AreEqual("図書カテゴリId:2f5016b6-6f6b-11f0-954a-00155d1bd30aの商品カテゴリは存在しません。", ex.Message);
    }


    [TestMethod("存在しない商品名を指定すると例外はスローされない")]
    public async Task ExistsByBookNameAsync_ShouldNotThrow_WhenNameExists()
    {
        await _uscase!.ExistsByBookNameAsync("油性ボールペン");
        Assert.IsTrue(true);
    }

    
    [TestMethod("新商品を登録できる")]
    public async Task RegisterBookAsync_ShouldCreateNewBook()
    {
        // テストデータを用意する
        var category = new BookCategory("18836923-5194-47f1-bf4c-e09eb5fa8fef", "技術書");
        var stock = new BookStock(Guid.NewGuid().ToString(), 20);
        var product = new Book(Guid.NewGuid().ToString(), "商品-A", "テスト著者");
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 新商品を登録する
        await _uscase!.RegisterBookAsync(product);
        // 登録された商品を取得する
        var result = await _productRepository!
            .SelectByIdWithBookStockAndBookCategoryAsync(product.BookUuid);
        // nullでないことを検証する
        Assert.IsNotNull(result);
        // 商品Idを検証する
        Assert.AreEqual(product.BookUuid, result.BookUuid);
        // 商品名を検証する
        Assert.AreEqual(product.Title  , result.Title);
        // 単価を検証する
        Assert.AreEqual(product.Author, result.Author);
        // 商品在庫Idを検証する
        Assert.AreEqual(product.Stock!.StockUuid, result.Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(product.Stock!.Stock, result.Stock!.Stock);
        // 追加したデータをクリーニングする
        await _productRepository!.DeleteByIdAsync(product.BookUuid);
    }
}