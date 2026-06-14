using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Usecases.Users.Interfaces;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.Configs;
using LibraryApi.Presentations.Controllers;
using LibraryApi.Presentations.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace LibraryApi.Presentations.Tests.Controllers;

[TestClass]
[TestCategory("Controllers")]
public class RegisterUserControllerTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // ユースケース:[ユーザーを登録する]を実現するインターフェイス
    private IRegisterUserUsecase? _usecase;
    // RegisterUserViewModelからドメインオブジェクト:Userへ変換するアダプタ
    private RegisterUserViewModelAdapter? _adapter;
    // テストターゲット
    private RegisterUserController? _controller;
    // UserRepository
    private IUserRepository? _repository;

    private static readonly Random _random = new Random();

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
        // [ユーザーを登録する]を実現インターフェイスを取得する
        _usecase = _scope.ServiceProvider.GetRequiredService<IRegisterUserUsecase>();
        // RegisterUserViewModelからドメインオブジェクト:Userへ変換するアダプタを取得する
        _adapter = _scope.ServiceProvider.GetRequiredService<RegisterUserViewModelAdapter>();
        // テストターゲットを生成する
        _controller = new RegisterUserController(_usecase, _adapter);
        // UserRepositoryを取得する
        _repository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
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

    //ユーザ名が30文字以上でValidationErrorを返す400BadRequest
    [TestMethod]
    public async Task MyMethodAsync1()
    {
        var request = new RegisterUserRequestViewModel();
        request.username = new string('A', 31);
        request.password = new string("testtest");

        //実行
        var response = await _controller!.Register(request);

        //検証
        var bad = response as BadRequestObjectResult;
        Assert.IsNotNull(bad);
        Assert.AreEqual(400, bad!.StatusCode);

        var msg = GetResponseMessage(bad.Value);
        Assert.AreEqual("ユーザー名は30文字以内で指定してください。", msg);
    }

    //ユーザ名がnullでValidationErrorを返す400BadRequest
    [TestMethod("ユーザ名がnullでValidationErrorを返す400BadRequest")]
    public async Task Register_ShouldReturnBadRequest_WhenUsernameIsNull()
    {
        var request = new RegisterUserRequestViewModel();
        request.username = null!;
        request.password = "validpass123";

        //実行
        var response = await _controller!.Register(request);

        //検証
        var bad = response as BadRequestObjectResult;
        Assert.IsNotNull(bad);
        Assert.AreEqual(400, bad!.StatusCode);

        var msg = GetResponseMessage(bad.Value);
        Assert.AreEqual("ユーザー名は必須です。", msg);
    }

    //パスワードが8文字未満でValidattionErrorを返す400BadRequest
    [TestMethod("パスワードが8文字未満でValidattionErrorを返す400BadRequest")]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordIsTooShort()
    {
        var request = new RegisterUserRequestViewModel();
        request.username = CreateRandomUsername();
        request.password = "short";  // 8文字未満

        //実行
        var response = await _controller!.Register(request);

        //検証
        var bad = response as BadRequestObjectResult;
        Assert.IsNotNull(bad);
        Assert.AreEqual(400, bad!.StatusCode);

        var msg = GetResponseMessage(bad.Value);
        Assert.AreEqual("パスワードは8文字以上で指定してください。", msg);
    }

    //パスワードがnullでValidattionErrorを返す400BadRequest
    [TestMethod("パスワードがnullでValidattionErrorを返す400BadRequest")]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordIsNull()
    {
        var request = new RegisterUserRequestViewModel();
        request.username = CreateRandomUsername();
        request.password = null!;

        var response = await _controller!.Register(request);

        var bad = response as BadRequestObjectResult;
        Assert.IsNotNull(bad);
        Assert.AreEqual(400, bad!.StatusCode);

        var msg = GetResponseMessage(bad.Value);
        Assert.AreEqual("パスワードは必須です。", msg);
    }

    //存在するユーザ名でExist Exceptionを返す409Conflict
    [TestMethod("存在するユーザ名でExist Exceptionを返す409Conflict")]
    public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        var uniqueUsername = $"test";
        var firstRequest = new RegisterUserRequestViewModel();
        firstRequest.username = uniqueUsername;
        firstRequest.password = "validpass123";

        // 最初のユーザーを登録する
        await _controller!.Register(firstRequest);

        // 同じユーザー名で再度登録を試みる
        var secondRequest = new RegisterUserRequestViewModel();
        secondRequest.username = uniqueUsername;
        secondRequest.password = "validpass456";

        //実行
        var response = await _controller!.Register(secondRequest);

        //検証
        var conflict = response as ConflictObjectResult;
        Assert.IsNotNull(conflict);
        Assert.AreEqual(409, conflict!.StatusCode);

        var msg = GetResponseMessage(conflict.Value);
        Assert.AreEqual($"ユーザー名:{uniqueUsername}のユーザーは既に存在します。", msg);
    }

    //有効なユーザで登録完了201Created
    [TestMethod("有効なユーザで登録完了201Created")]
    public async Task Register_ShouldReturnOk_WhenValidUser()
    {
        var uniqueUsername = CreateRandomUsername();
        var request = new RegisterUserRequestViewModel();
        request.username = uniqueUsername;
        request.password = "validpassff";

        //実行
        var response = await _controller!.Register(request);

        //検証
        var created = response as CreatedResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(StatusCodes.Status201Created, created!.StatusCode);

        // レスポンスの内容を検証
        var result = created.Value as RegisterUserResponseViewModel;
        Assert.IsNotNull(result);
        Assert.AreEqual(uniqueUsername, result!.username);
    }

    private static string? GetResponseMessage(object? responseValue)
    {
        return responseValue?.GetType().GetProperty("message")?.GetValue(responseValue) as string;
    }

    private static string CreateRandomUsername(int length = 8)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[_random.Next(chars.Length)])
            .ToArray());
    }
}