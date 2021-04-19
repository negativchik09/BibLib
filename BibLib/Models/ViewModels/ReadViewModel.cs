namespace BibLib.Models.ViewModels
{
    public class ReadViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int FontSize { get; set; }
        public PaginationViewModel Pages { get; set; }
    }
}