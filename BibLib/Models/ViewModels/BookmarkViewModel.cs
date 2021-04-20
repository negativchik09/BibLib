namespace BibLib.Models.ViewModels
{
    public class BookmarkViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Page { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
    }
}