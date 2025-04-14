using Library.Data.DataGenerators;
using Library.Logic.Services;
using System;
using System.Linq;

namespace Library.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Library Management System");
            Console.WriteLine("=========================");

            var dataRepository = new PredefinedDataGenerator().GenerateData();
            var libraryService = new LibraryService(dataRepository);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. View all books");
                Console.WriteLine("2. View all users");
                Console.WriteLine("3. View available books");
                Console.WriteLine("4. View checked out books");
                Console.WriteLine("5. View users with overdue books");
                Console.WriteLine("6. Borrow a book");
                Console.WriteLine("7. Return a book");
                Console.WriteLine("8. View events");
                Console.WriteLine("9. Exit");
                Console.Write("\nEnter your choice (1-9): ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            DisplayAllBooks(libraryService);
                            break;
                        case "2":
                            DisplayAllUsers(libraryService);
                            break;
                        case "3":
                            DisplayAvailableBooks(libraryService);
                            break;
                        case "4":
                            DisplayCheckedOutBooks(libraryService);
                            break;
                        case "5":
                            DisplayUsersWithOverdueBooks(libraryService);
                            break;
                        case "6":
                            BorrowBook(libraryService);
                            break;
                        case "7":
                            ReturnBook(libraryService);
                            break;
                        case "8":
                            DisplayEvents(libraryService);
                            break;
                        case "9":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Thank you for using the Library Management System!");
        }

        private static void DisplayAllBooks(LibraryService libraryService)
        {
            var books = libraryService.GetAllBooks().ToList();
            Console.WriteLine("All Books in Library:");
            Console.WriteLine("=====================");
            if (books.Any())
            {
                foreach (var book in books)
                {
                    Console.WriteLine($"ISBN: {book.ISBN}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"Publisher: {book.Publisher} ({book.PublicationYear})");
                    Console.WriteLine($"Genre: {book.Genre}");
                    Console.WriteLine($"Description: {book.Description}");
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No books in the library.");
            }
        }

        private static void DisplayAllUsers(LibraryService libraryService)
        {
            var users = libraryService.GetAllUsers().ToList();
            Console.WriteLine("All Library Users:");
            Console.WriteLine("=================");
            if (users.Any())
            {
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"Email: {user.Email}");
                    Console.WriteLine($"Phone: {user.PhoneNumber}");
                    Console.WriteLine($"Type: {user.Type}");
                    Console.WriteLine($"Registration Date: {user.RegistrationDate.ToShortDateString()}");
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No users registered.");
            }
        }

        private static void DisplayAvailableBooks(LibraryService libraryService)
        {
            var availableBooks = libraryService.GetAvailableBooks().ToList();
            Console.WriteLine("Available Books:");
            Console.WriteLine("================");
            if (availableBooks.Any())
            {
                foreach (var bookCopy in availableBooks)
                {
                    var book = libraryService.GetBookByIsbn(bookCopy.ISBN);
                    Console.WriteLine($"Copy ID: {bookCopy.Id}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"Location: {bookCopy.Location}");
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No available books.");
            }
        }

        private static void DisplayCheckedOutBooks(LibraryService libraryService)
        {
            var checkedOutBooks = libraryService.GetCheckedOutBooks().ToList();
            Console.WriteLine("Checked Out Books:");
            Console.WriteLine("=================");
            if (checkedOutBooks.Any())
            {
                foreach (var bookCopy in checkedOutBooks)
                {
                    var book = libraryService.GetBookByIsbn(bookCopy.ISBN);
                    var borrower = libraryService.GetUserById(bookCopy.CurrentBorrowerId.Value);
                    Console.WriteLine($"Copy ID: {bookCopy.Id}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"Borrowed by: {borrower.Name} (ID: {borrower.Id})");
                    Console.WriteLine($"Due Date: {bookCopy.DueDate.Value.ToShortDateString()}");
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No books are currently checked out.");
            }
        }

        private static void DisplayUsersWithOverdueBooks(LibraryService libraryService)
        {
            var usersWithOverdueBooks = libraryService.GetUsersWithOverdueBooks().ToList();
            Console.WriteLine("Users with Overdue Books:");
            Console.WriteLine("========================");
            if (usersWithOverdueBooks.Any())
            {
                foreach (var user in usersWithOverdueBooks)
                {
                    Console.WriteLine($"User ID: {user.Id}");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"Email: {user.Email}");
                    Console.WriteLine("Overdue Books:");

                    var borrowedBooks = libraryService.GetBorrowedBooksByUser(user.Id)
                        .Where(bc => bc.DueDate.HasValue && bc.DueDate.Value < DateTime.Now)
                        .ToList();

                    foreach (var bookCopy in borrowedBooks)
                    {
                        var book = libraryService.GetBookByIsbn(bookCopy.ISBN);
                        var daysOverdue = (DateTime.Now - bookCopy.DueDate.Value).Days;
                        Console.WriteLine($"  - {book.Title} (Copy ID: {bookCopy.Id}), {daysOverdue} days overdue");
                    }
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No users have overdue books.");
            }
        }

        private static void BorrowBook(LibraryService libraryService)
        {
            Console.WriteLine("Borrow a Book:");
            Console.WriteLine("==============");

            var availableBooks = libraryService.GetAvailableBooks().ToList();
            if (!availableBooks.Any())
            {
                Console.WriteLine("No books are available for borrowing.");
                return;
            }

            Console.WriteLine("Available Books:");
            for (int i = 0; i < availableBooks.Count; i++)
            {
                var book = libraryService.GetBookByIsbn(availableBooks[i].ISBN);
                Console.WriteLine($"{i + 1}. {book.Title} (Copy ID: {availableBooks[i].Id})");
            }

            var users = libraryService.GetAllUsers().ToList();
            Console.WriteLine("\nLibrary Users:");
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Name} (ID: {users[i].Id})");
            }

            Console.Write("\nSelect book (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int bookSelection) ||
                bookSelection < 1 || bookSelection > availableBooks.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.Write("Select user (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int userSelection) ||
                userSelection < 1 || userSelection > users.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.Write("Enter due date (days from now): ");
            if (!int.TryParse(Console.ReadLine(), out int daysToReturn) || daysToReturn < 1)
            {
                Console.WriteLine("Invalid number of days.");
                return;
            }

            var selectedBook = availableBooks[bookSelection - 1];
            var selectedUser = users[userSelection - 1];
            var dueDate = DateTime.Now.AddDays(daysToReturn);

            bool success = libraryService.BorrowBook(selectedUser.Id, selectedBook.Id, dueDate);
            if (success)
            {
                Console.WriteLine($"\nSuccess! {selectedUser.Name} has borrowed {libraryService.GetBookByIsbn(selectedBook.ISBN).Title}.");
                Console.WriteLine($"The book is due on {dueDate.ToShortDateString()}");
            }
            else
            {
                Console.WriteLine("\nBorrowing failed. The book might not be available anymore.");
            }
        }

        private static void ReturnBook(LibraryService libraryService)
        {
            Console.WriteLine("Return a Book:");
            Console.WriteLine("==============");

            var checkedOutBooks = libraryService.GetCheckedOutBooks().ToList();
            if (!checkedOutBooks.Any())
            {
                Console.WriteLine("No books are currently checked out.");
                return;
            }

            Console.WriteLine("Checked Out Books:");
            for (int i = 0; i < checkedOutBooks.Count; i++)
            {
                var book = libraryService.GetBookByIsbn(checkedOutBooks[i].ISBN);
                var borrower = libraryService.GetUserById(checkedOutBooks[i].CurrentBorrowerId.Value);
                Console.WriteLine($"{i + 1}. {book.Title} (Copy ID: {checkedOutBooks[i].Id}, Borrowed by: {borrower.Name})");
            }


            Console.Write("\nSelect book to return (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int bookSelection) ||
                bookSelection < 1 || bookSelection > checkedOutBooks.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var selectedBook = checkedOutBooks[bookSelection - 1];
            bool success = libraryService.ReturnBook(selectedBook.Id);

            if (success)
            {
                Console.WriteLine($"\nSuccess! {libraryService.GetBookByIsbn(selectedBook.ISBN).Title} has been returned.");
            }
            else
            {
                Console.WriteLine("\nReturn failed. The book might already be returned.");
            }
        }

        private static void DisplayEvents(LibraryService libraryService)
        {
            var events = libraryService.GetAllEvents().OrderByDescending(e => e.Timestamp).ToList();
            Console.WriteLine("Library Events:");
            Console.WriteLine("===============");

            if (events.Any())
            {
                foreach (var evt in events)
                {
                    Console.WriteLine($"Event ID: {evt.Id}");
                    Console.WriteLine($"Type: {evt.Type}");
                    Console.WriteLine($"Timestamp: {evt.Timestamp}");
                    Console.WriteLine($"Description: {evt.Description}");

                    if (evt.UserId.HasValue)
                    {
                        var user = libraryService.GetUserById(evt.UserId.Value);
                        Console.WriteLine($"User: {user?.Name} (ID: {evt.UserId})");
                    }

                    if (!string.IsNullOrEmpty(evt.ISBN))
                    {
                        var book = libraryService.GetBookByIsbn(evt.ISBN);
                        Console.WriteLine($"Book: {book?.Title} (ISBN: {evt.ISBN})");
                    }

                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("No events recorded yet.");
            }
        }
    }
}
