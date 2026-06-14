using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LibraryApi.Applications.Security;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Models;

namespace LibraryApi.Applications.Usecases.Users.Interactors
{
    public class RegisterUserUsecase
    {
        /// <summary>
        /// プロパティ
        /// </summary>
        public IUserRepository _repository { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }
        public IPasswordHashingService _service { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="passwordHashingService"></param>
        public RegisterUserUsecase(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordHashingService passwordHashingService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _service = passwordHashingService;
        }

        /// <summary>
        /// ユーザ名が既に存在していたら例外をスローする検索機能
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ExistsException"></exception>
        public async Task ExistsByUserNameAsync(string name)
        {
            var result = await _repository.ExistsByUsernameAsync(name);
            if (result)
            {
                throw new ExistsException($"ユーザー名:{name}のユーザーは既に存在します。");
            }
        }

        /// <summary>
        /// ユーザを登録する
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task RegisterUserAsync(User user)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var passwordHash = _service.Hash(user.Password);
                user.ChangePassword(passwordHash);
                await _repository.CreateAsync(user);
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }


    }
}
