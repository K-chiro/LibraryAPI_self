using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.Configs;
using LibraryApi.Presentations.ViewModels;
using LibraryApi.Domains.Exceptions;


namespace LibraryApi.Presentations.Tests.Adapters;


[TestClass]
[TestCategory("Adapters")]
public class RegisterUserViewModelAdapterTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private RegisterUserViewModelAdapter? _adapter;

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
        _adapter = _scope.ServiceProvider.GetRequiredService<RegisterUserViewModelAdapter>();
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

    //RequestViewModelからUserへ復元でき、UUIDが生成される。
    [TestMethod("RequestViewModelからUserへ復元でき、UUIDが生成される。")]
    public async Task RestoreAsync()
    {
        var vm = new RegisterUserRequestViewModel();
        vm.username = "test";
        vm.password = "testtest";

        var domain = await _adapter!.RestoreAsync(vm);
        Assert.IsNotNull(domain);
        Assert.IsTrue(Guid.TryParse(domain.UserUuid, out _));
        Assert.AreEqual(vm.password, domain.Password);
        Assert.AreEqual(vm.username, domain.Username);

    }
    //ユーザ名が31文字以上の場合、DomainExceptionがスローされる
    [TestMethod("ユーザ名が31文字以上の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_DomainException_WhenTooLongName()
    {
        var vm = new RegisterUserRequestViewModel();
        vm.username = new string('A', 31);
        vm.password = "testtest";

        //実行　検証
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(() =>
             _adapter!.RestoreAsync(vm)

        );
        Assert.AreEqual("ユーザー名は30文字以内で指定してください。",ex.Message);


    }
    //パスワードが8文字未満の場合DomainExceptionがスローされる
    [TestMethod("パスワードが8文字未満の場合DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_DomainException_WhenPasswordTooShort()
    {
        var vm = new RegisterUserRequestViewModel();
        vm.username = "test";
        vm.password = "short"; // 8文字未満

        var ex = await Assert.ThrowsExceptionAsync<DomainException>(() =>
            _adapter!.RestoreAsync(vm)
        );

        Assert.AreEqual("パスワードは8文字以上で指定してください。", ex.Message);
    }

    //パスワードが空の場合、DomainExceptionがスローされる
    [TestMethod("パスワードが空の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_DomainException_WhenPasswordIsEmpty()
    {
        var vm = new RegisterUserRequestViewModel();
        vm.username = "test";
        vm.password = null!; // 空またはnull

        var ex = await Assert.ThrowsExceptionAsync<DomainException>(() =>
            _adapter!.RestoreAsync(vm)
        );

        Assert.AreEqual("パスワードは必須です。", ex.Message);
    }

    //UserからResponseVMに変換できる。
    [TestMethod("UserからResponseVMに変換できる。")]
    public async Task ConvertAsync_ShouldConvert_UserToResponseViewModel()
    {
        var vm = new RegisterUserRequestViewModel();
        vm.username = "convtest";
        vm.password = "convpass";

        var user = await _adapter!.RestoreAsync(vm);
        var resp = await _adapter.ConvertAsync(user);

        Assert.IsNotNull(resp);
        Assert.AreEqual(user.UserUuid, resp.userId);
        Assert.AreEqual(user.Username, resp.username);
    }
}
