namespace BibLib.Domain.Entities
{
    public class AuthorBook
    {
        public int AuthorId { get; set; }
        public AuthorDTO Author { get; set; }
        public int BookId { get; set; }
        public BookDTO Book { get; set; }
    }
}