using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.ViewModels;

namespace LibraryApi.Presentations.Adapters;

public class RegisterUserViewModelAdapter : IRestorer<User, RegisterUserRequestViewModel>, IConverter<User, RegisterUserResponseViewModel>
{
    public Task<RegisterUserResponseViewModel> ConvertAsync(User domain)
    {
        var viewModel = new RegisterUserResponseViewModel();
        viewModel.userId = domain.UserUuid;
        viewModel.username = domain.Username;
        return Task.FromResult(viewModel);
    }

    public Task<User> RestoreAsync(RegisterUserRequestViewModel target)
    {
        var user = new User(
            target.username,
            target.password
        );
        return Task.FromResult(user);
    }
}
