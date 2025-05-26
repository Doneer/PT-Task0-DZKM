using Library.Data.Interfaces.Models;
using Library.Data.Factories;
using Library.WPF.ViewModels;

namespace Library.WPF.Models
{
    public class BookViewModel : BaseViewModel
    {
        private string _isbn;
        private string _title;
        private string _author;
        private string _publisher;
        private int _publicationYear;
        private string _genre;
        private string _description;

        public string ISBN
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Publisher
        {
            get => _publisher;
            set => SetProperty(ref _publisher, value);
        }

        public int PublicationYear
        {
            get => _publicationYear;
            set => SetProperty(ref _publicationYear, value);
        }

        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string DisplayText => $"{Title} by {Author}";

        public BookViewModel()
        {
            _isbn = string.Empty;
            _title = string.Empty;
            _author = string.Empty;
            _publisher = string.Empty;
            _genre = string.Empty;
            _description = string.Empty;
            _publicationYear = DateTime.Now.Year;
        }

        public BookViewModel(IBook book)
        {
            ISBN = book.ISBN;
            Title = book.Title;
            Author = book.Author;
            Publisher = book.Publisher;
            PublicationYear = book.PublicationYear;
            Genre = book.Genre;
            Description = book.Description;
        }

        public IBook ToBook()
        {
            return BookFactory.CreateBook(
                ISBN,
                Title,
                Author,
                Publisher,
                PublicationYear,
                Genre,
                Description
            );
        }
    }
}