using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using Microsoft.Extensions.Configuration;
using LibraryApi.Presentations.Configs;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.Controllers;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.Presentations.Adapters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using LibraryApi.Presentations.ViewModels;
using System.ComponentModel;
using Microsoft.OpenApi.Writers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibraryApi.Presentations.Tests.Controllers;

[TestClass]
[TestCategory("Controllers")]
public class DeleteControllerTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // ユースケース:[新商品を登録する]を実現するインターフェイス
    private IDeleteUsecase? _usecase;

    private IBookRepository? _repository;

    // テストターゲット
    private DeleteBookController? _controller;

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
        // [新商品を登録する]を実現インターフェイスを取得する
        _usecase = _scope.ServiceProvider.GetRequiredService<IDeleteUsecase>();
        _repository = _scope.ServiceProvider.GetRequiredService<IBookRepository>();
        // テストターゲットを生成する
        _controller = new DeleteBookController(_usecase);
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


    [TestMethod("存在する図書を削除できる：204NoContetnt")]
    public async Task DeleteAsync_ExistingBook_ShouldReturnNoContent()
    {
        var uuid = "726eaff4-4d5d-4de7-8758-50850a0c8989";
        var category = new BookCategory("18836923-5194-47f1-bf4c-e09eb5fa8fef", "技術書");
        var stock = new BookStock("e7bd6533-9946-4b5a-b378-5c480d07390d", 5);
        var book = new Book(uuid, "test2", "testAuthor2", category, stock);

        await _repository!.CreateAsync(book);

        var response = await _controller!.Deleted(book.BookUuid);

        var noContent = response as NoContentResult;
        Assert.IsNotNull(noContent, "レスポンスが NoContentResult ではありません。");
        Assert.AreEqual(StatusCodes.Status204NoContent, noContent!.StatusCode);

        var exists = await _repository.ExistsByNameAsync(book.BookUuid);
        Assert.IsFalse(exists, "削除されたはずの図書がまだ存在しています。");
    }

    [TestMethod("存在しない図書を削除できない：404NotFound")]
    public async Task DeleteAsync_NonExistingBook_ShouldReturn404NotFound()
    {
        var uuid = "2f5016b6-6f6b-11f0-954a-00155d1bd30a";

        var response = await _controller!.Deleted(uuid);
        var notFound = response as NotFoundObjectResult;

        Assert.IsNotNull(notFound, "レスポンスが NotFoundObjectResult ではありません。");
        Assert.AreEqual(StatusCodes.Status404NotFound, notFound!.StatusCode);

        var val = notFound.Value!;
        var code = (string)val.GetType().GetProperty("code")!.GetValue(val)!;
        var msg = (string)val.GetType().GetProperty("message")!.GetValue(val)!;

        Assert.AreEqual("PRODUCT_NOT_FOUND", code);
        Assert.AreEqual($"商品Id:{uuid}の商品は存在しません。", msg);
    }
}
