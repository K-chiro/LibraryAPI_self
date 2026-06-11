using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.Configs;
using LibraryApi.Presentations.ViewModels;

namespace LibraryApi.Presentations.Tests.Adapters;
/// <summary>
/// RegisterBookViewModelAdapterのテストドライバ
/// </summary>
[TestClass]
[TestCategory("Adapters")]
public class RegisterBookViewModelAdapterTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private RegisterBookViewModelAdapter? _adapter;

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
    /// テストメソッド実行の前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        // スコープドサービスを取得する
        _scope = _provider!.CreateScope();
        // テストターゲットを取得する
        _adapter = _scope.ServiceProvider
            .GetRequiredService<RegisterBookViewModelAdapter>();
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

    [TestMethod("ViewModelからBookを復元でき、商品Idと商品在庫Idが自動生成される")]
    public async Task RestoreAsync_ShouldMapVmToDomain_AndGenerateUuids()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "セーラームーン",
            Author = "武内直子",
            Stock = 5,
            CategoryId = "2f4d3e51-6f6b-11f0-954a-00155d1bd29a",
            CategoryName = "漫画"
        };
        // ViewModelからBookを復元する
        var product = await _adapter!.RestoreAsync(viewModel);
        // 商品名を検証する
        Assert.AreEqual(viewModel.Title, product.Title);
        // 単価を検証する
        Assert.AreEqual(viewModel.Author, product.Author);
        // 商品Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(product.BookUuid));
        Assert.IsTrue(Guid.TryParse(product.BookUuid, out _));
        // 商品カテゴリがnullでないことを検証する
        Assert.IsNotNull(product.Category);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(viewModel.CategoryId, product.Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual(viewModel.CategoryName, product.Category.Name);
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(product.Stock);
        // 商品在庫を検証する
        Assert.AreEqual(viewModel.Stock, product.Stock!.Stock);
        // 商品在庫Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(product.Stock.StockUuid));
        Assert.IsTrue(Guid.TryParse(product.Stock.StockUuid, out _));
    }

    [TestMethod("不正な商品Idの場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenCategoryIdIsInvalidUuid()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "セーラームーン",
            Author = "武内直子",
            Stock = 5,
            CategoryId = "2f4dAAA",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("UUIDの形式が正しくありません。", ex.Message);
    }

    [TestMethod("商品名が空白の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_WhenNameBlank_ShouldThrowDomainException()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "",
            Author = "武内直子",
            Stock = 5,
            CategoryId = "2f4d3e51-6f6b-11f0-954a-00155d1bd29a",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("タイトルは必須です。", ex.Message);
    }

    [TestMethod("商品名が50文字の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_WhenNameOver30_ShouldThrowDomainException()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = new string('A', 51),
            Author = "武内直子",
            Stock = 5,
            CategoryId = "2f4d3e51-6f6b-11f0-954a-00155d1bd29a",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("タイトルは50文字以内である必要があります。", ex.Message);
    }

    [TestMethod("カテゴリIdが空文字の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenCategoryIdIsEmpty()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "セーラームーン",
            Author = "武内直子",
            Stock = 5,
            CategoryId = "",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("UUIDの形式が正しくありません。", ex.Message);
    }

    [TestMethod("在庫数がマイナスの場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenStockIsNegative()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "セーラームーン",
            Author = "武内直子",
            Stock = -5,
            CategoryId = "2f4d3e51-6f6b-11f0-954a-00155d1bd29a",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("在庫数は0以上である必要があります。", ex.Message);
    }

     [TestMethod("著者名が31文字以上、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenPriceNegative()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "セーラームーン",
            Author = new string('A', 31),
            Stock = 5,
            CategoryId = "2f4d3e51-6f6b-11f0-954a-00155d1bd29a",
            CategoryName = "漫画"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("著者名は30文字以内である必要があります。", ex.Message);
    }
}