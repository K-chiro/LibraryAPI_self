namespace LibraryApi.Presentations.ViewModels;

public class UpdateBookResponseViewModel
{
    public string BookId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;

    public CategoryInfo? Category { get; set; } = new();

        public int? Stock { get; set; }


    public class CategoryInfo
    {
        public string CategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}