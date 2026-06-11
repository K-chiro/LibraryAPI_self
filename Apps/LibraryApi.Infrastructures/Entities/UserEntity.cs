using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LibraryApi.Infrastructures.Entities;

[Table("users")]
public class UserEntity: ITimestampEntity
{
    /// <summary>
    /// オートインクリメントの主キー（内部用）
    /// </summary>
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// UUID（外部公開用）
    /// </summary>
    [Required]
    [Column("user_uuid")]
    [StringLength(36)]
    public string UserUuid { get; set; } = string.Empty;

    /// <summary>
    /// ユーザー名（ログイン名または表示名）
    /// </summary>
    [Required]
    [Column("username")]
    [StringLength(30)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// パスワードのハッシュ
    /// </summary>
    [Required]
    [Column("password_hash")]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 登録日時
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
