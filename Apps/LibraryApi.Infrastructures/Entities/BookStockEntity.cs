using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Infrastructures.Entities;

[Table("book_stock")]
public class BookStockEntity: ITimestampEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(36)]
    [Column("stock_uuid")]
    public string StockUuid { get; set; } = string.Empty;

    [Required]
    [Column("stock")]
    public int Stock { get; set; }

    [Required]
    [Column("book_id")]
    public int BookId { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookId")]
    public BookEntity? Book { get; set; }
}
