namespace LibraryApi.Infrastructures.Entities;
public interface ITimestampEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}