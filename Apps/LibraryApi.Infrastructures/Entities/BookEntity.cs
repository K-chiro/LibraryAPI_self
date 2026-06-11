using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace LibraryApi.Infrastructures.Entities;

[Table("book")]
public class BookEntity: ITimestampEntity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(36)]
    [Column("book_uuid")]
    public string BookUuid { get; set; } = string.Empty;


    [Required]
    [StringLength(50)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    [Column("author")]
    public string Author { get; set; } = string.Empty;


    [Required]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    public BookCategoryEntity? BookCategory { get; set; }

    public BookStockEntity? BookStock { get; set; }

}