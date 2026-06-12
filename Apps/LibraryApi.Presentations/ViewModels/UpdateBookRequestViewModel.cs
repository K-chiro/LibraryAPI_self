using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace LibraryApi.Presentations.ViewModels;

public class UpdateBookRequestViewModel
{
    // 商品Id(UUID)
    [Required(ErrorMessage = "図書名は必須です")]
    [StringLength(50, ErrorMessage = "図書名は{1}文字以内で入力してください。")]
    public string Title { get; set; } = string.Empty;
    // 商品名
    [Required(ErrorMessage = "著者は必須です")]
    [StringLength(30, ErrorMessage = "著者名は{1}文字以内で入力してください。")]
    public string Author { get; set; } = string.Empty;
    // 在庫数
    [Required(ErrorMessage = "在庫数は必須です。")]
    [Range(0, int.MaxValue, ErrorMessage = "在庫数は0以上の整数を指定してください。")]
    public int Stock { get; set; }
}
