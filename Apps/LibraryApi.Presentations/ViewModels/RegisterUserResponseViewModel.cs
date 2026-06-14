using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Presentations.ViewModels;

public class RegisterUserResponseViewModel
{

    //ユーザID
    public string userId { get; set; } = string.Empty;
    
    

    //　ユーザ名
    public string username { get; set; } = string.Empty;


}
