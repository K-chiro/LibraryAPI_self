using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.Configs;
using LibraryApi.Presentations.ViewModels;
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LibraryApi.Domains.Models;

namespace LibraryApi.Presentations.Tests.Adapters;
/// <summary>
/// UpdateBookViewModelAdapterのテストドライバ
/// </summary>
[TestClass]
[TestCategory("Adapters")]
public class UpdateUpdateViewModelAdapterTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private UpdateBookViewModelAdapter? _adapter;

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
            .GetRequiredService<UpdateBookViewModelAdapter>();
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


    [TestMethod("UpdateBookRequestViewModelとIDからBookオブジェクトを生成できる")]
    public async Task TransAsync_ShouldMapVmToDomain()
    {
        var Uuid = "2f4d3e51-6f6b-11f0-954a-00155d1bd11a";
        // ViewModelを用意する
        var viewModel = new UpdateBookRequestViewModel
        {
            Title = "セーラームーン",
            Author = "武内直子",
            Stock = 5
        };

        var book = await _adapter!.TransAsync(viewModel, Uuid);

        // 商品名を検証する
        Assert.AreEqual(viewModel.Title, book.Title);
        // 単価を検証する
        Assert.AreEqual(viewModel.Author, book.Author);
        // 商品Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(book.BookUuid));
        Assert.IsTrue(Guid.TryParse(book.BookUuid, out _));
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(book.Stock);
        // 商品在庫を検証する
        Assert.AreEqual(viewModel.Stock, book.Stock!.Stock);
    }

    [TestMethod("BookからResponseViewModelに変換できる")]
    public async Task ConvertAsync_ShouldDomainToVm()
    {
        var category = new BookCategory("2f4d3e51-6f6b-11f0-954a-00155d1bd29a", "漫画");
        var stock = new BookStock(5);
        var book = new Book(
            "2f4d3e51-6f6b-11f0-954a-00155d1bd29a", // bookUuid
            "セーラームーン",                         // title
            new string('A', 30),                   // author (境界値の30文字)
            category,                              // category
            stock                                  // stock
        );

        var viewModel = await _adapter.ConvertAsync(book);
                // 商品名を検証する
        Assert.AreEqual(viewModel.Title, book.Title);
        // 単価を検証する
        Assert.AreEqual(viewModel.Author, book.Author);
        // 商品Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(book.BookUuid));
        Assert.IsTrue(Guid.TryParse(book.BookUuid, out _));
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(book.Stock);
        // 商品在庫を検証する
        Assert.AreEqual(viewModel.Stock, book.Stock!.Stock);
    }

}