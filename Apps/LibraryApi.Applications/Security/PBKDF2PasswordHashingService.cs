using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LibraryApi.Domains.Models;

namespace LibraryApi.Applications.Security;

public class PBKDF2PasswordHashingService : IPasswordHashingService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PBKDF2PasswordHashingService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Hash(string rawPassword)
    {
        var dummy = new User(Guid.NewGuid().ToString(), "tmp", new string('A', 8));
        return _passwordHasher.HashPassword(dummy, rawPassword);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var dummy = new User(Guid.NewGuid().ToString(), "tmp", hashedPassword);
        var result = _passwordHasher.VerifyHashedPassword(dummy, hashedPassword, providedPassword);
        return result switch
        {
            // 一致したのtrueを返す
            PasswordVerificationResult.Success => true,
            // 不一致なのでfalseを返す
            PasswordVerificationResult.Failed => false,
            // 一致したが形式や強度が古いので、 PasswordRehashNeededExceptionをスローする
            PasswordVerificationResult.SuccessRehashNeeded =>
                throw new PasswordRehashNeededException("パスワードは認証されたが、再ハッシュが必要です。"),
            _ => false
        };
    }
}
