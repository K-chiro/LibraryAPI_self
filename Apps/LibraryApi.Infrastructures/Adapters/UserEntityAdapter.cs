using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Infrastructures.Entities;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Adapters;
using LibraryApi.Applications.Exceptions;

namespace LibraryApi.Infrastructures.Adapters;

public class UserEntityAdapter : IConverter<User, UserEntity>, IRestorer<User, UserEntity>
{
    /// <summary>
    /// ドメインオブジェクト:UserをUserEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:User</param>
    /// <returns>EFCore:UserEntity</returns>
    public Task<UserEntity> ConvertAsync(User domain)
    {
        if (domain == null)
        {
            throw new InternalException("引数domainがnullです。");
        }
        var entity = new UserEntity();
        entity.UserUuid = domain.UserUuid;
        entity.Username = domain.Username;
        entity.Password = domain.Password;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// UserEntityからドメインオブジェクト:Userを復元する
    /// </summary>
    /// <param name="target">>EFCore:UserEntity</param>
    /// <returns>ドメインオブジェクト:User</returns>
    public Task<User> RestoreAsync(UserEntity target)
    {
        if (target == null)
        {
            throw new InternalException("引数targetがnullです。");
        }
        var domain = new User(
            target.UserUuid.ToString(),
            target.Username,
            target.Password);
        return Task.FromResult(domain);
    }
}
