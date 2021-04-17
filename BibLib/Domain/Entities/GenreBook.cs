namespace BibLib.Domain.Entities
{
    public class GenreBook
    {
        public int GenreId { get; set; }
        public GenreDTO Genre { get; set; }
        public int BookId { get; set; }
        public BookDTO Book { get; set; }
    }
}