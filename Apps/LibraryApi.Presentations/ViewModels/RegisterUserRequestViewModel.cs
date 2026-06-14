using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Presentations.ViewModels;

public class RegisterUserRequestViewModel
{
    //　ユーザ名
    [Required]
    [StringLength(30, ErrorMessage ="ユーザ名は1~30文字で入力してください。") ]
    public string username { get; set; } = string.Empty;

    //password
    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上で入力してください。")]
    public string password { get; set; } = string.Empty;
}
