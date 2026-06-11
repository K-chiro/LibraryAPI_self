using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace LibraryApi.Presentations.ViewModels;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するViewModel
/// </summary>
public class RegisterBookViewModel
{

    [Required(ErrorMessage = "タイトルは必須です。")]
    [StringLength(50, ErrorMessage = "タイトルは{1}文字以内で入力してください。")]
    public string Title { get; set; } = string.Empty;
    //著者
    [Required(ErrorMessage = "著者は必須です。")]
    [StringLength(30, ErrorMessage = "著者は{1}文字以内で入力してください。")]
    public string Author { get; set; } = string.Empty;

    // 在庫数
    [Required(ErrorMessage = "在庫数は必須です。")]
    [Range(0, int.MaxValue, ErrorMessage = "在庫数は0以上の整数を指定してください。")]
    public int Stock { get; set; }
    // 商品カテゴリId(UUID)
    [Required(ErrorMessage = "商品カテゴリIdは必須です。")]
    [RegularExpression(
    "^[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}$",
    ErrorMessage = "商品カテゴリIdはUUID形式で指定してください。")]
    public string CategoryId { get; set; } = string.Empty;
    
    // 商品カテゴリ名
    [StringLength(20, ErrorMessage = "商品名は{1}文字以内で入力してください。")]
    public string? CategoryName { get; set; } = string.Empty;
}