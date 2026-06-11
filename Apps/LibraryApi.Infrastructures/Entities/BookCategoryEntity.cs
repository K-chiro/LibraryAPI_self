using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LibraryApi.Infrastructures.Entities;

[Table("category")]
public class BookCategoryEntity : ITimestampEntity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(36)]
    [Column("category_uuid")]
    public string CategoryUuid { get; set; } = string.Empty;


    [Required]
    [StringLength(20)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("created_at")]

    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]

    public DateTime UpdatedAt { get; set; }

    public List<BookEntity> Books { get; set; } = new();



}
