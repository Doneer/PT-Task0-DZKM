using Library.Data.Interfaces;
using Library.Data.Models;
using Library.Data.Repositories;
using System;
using System.Collections.Generic;

namespace Library.Tests.DataGenerators
{
    public class RandomDataGenerator 
    {
        private readonly Random _random = new Random();

        public IDataRepository GenerateData()
        {
            var users = GenerateUsers(10);
            var books = GenerateBooks(20);
            var bookCopies = GenerateBookCopies(books, 40);
            var events = GenerateEvents(users, books, bookCopies, 30);

            return new DataRepository(
                new UserRepository(users),
                new CatalogRepository(books),
                new StateRepository(bookCopies),
                new EventRepository(events)
            );
        }

        private List<User> GenerateUsers(int count)
        {
            var users = new List<User>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(new User
                {
                    Id = i,
                    Name = $"User {i}",
                    Email = $"user{i}@example.com",
                    PhoneNumber = $"555-{i:D4}",
                    Type = (UserType)(_random.Next(3)),
                    RegistrationDate = DateTime.Now.AddDays(-_random.Next(365))
                });
            }
            return users;
        }

        private List<Book> GenerateBooks(int count)
        {
            var books = new List<Book>();
            string[] genres = { "Fiction", "Mystery", "Science Fiction", "Fantasy", "Biography", "History" };
            string[] authors = { "John Smith", "Jane Doe", "Michael Johnson", "Emily Brown", "Robert Wilson" };
            string[] publishers = { "Penguin", "Harper Collins", "Simon & Schuster", "Random House" };

            for (int i = 1; i <= count; i++)
            {
                books.Add(new Book
                {
                    ISBN = $"ISBN-{i:D5}",
                    Title = $"Book Title {i}",
                    Author = authors[_random.Next(authors.Length)],
                    Publisher = publishers[_random.Next(publishers.Length)],
                    PublicationYear = 2000 + _random.Next(23),
                    Genre = genres[_random.Next(genres.Length)],
                    Description = $"Description for book {i}"
                });
            }
            return books;
        }

        private List<BookCopy> GenerateBookCopies(List<Book> books, int count)
        {
            var bookCopies = new List<BookCopy>();
            for (int i = 1; i <= count; i++)
            {
                var status = (BookStatus)(_random.Next(4));
                bookCopies.Add(new BookCopy
                {
                    Id = i,
                    ISBN = books[_random.Next(books.Count)].ISBN,
                    Status = status,
                    AcquisitionDate = DateTime.Now.AddDays(-_random.Next(500)),
                    Location = $"Shelf {_random.Next(1, 20)}",
                    CurrentBorrowerId = status == BookStatus.CheckedOut ? _random.Next(1, 11) : null,
                    DueDate = status == BookStatus.CheckedOut ? DateTime.Now.AddDays(_random.Next(14)) : null
                });
            }
            return bookCopies;
        }

        private List<LibraryEvent> GenerateEvents(List<User> users, List<Book> books, List<BookCopy> bookCopies, int count)
        {
            var events = new List<LibraryEvent>();
            for (int i = 1; i <= count; i++)
            {
                var eventType = (EventType)(_random.Next(8));
                var bookCopy = bookCopies[_random.Next(bookCopies.Count)];
                var book = books.Find(b => b.ISBN == bookCopy.ISBN);
                int? userId = eventType == EventType.BookBorrowed || eventType == EventType.BookReturned ?
    _random.Next(1, users.Count + 1) : null;

                events.Add(new LibraryEvent
                {
                    Id = i,
                    Type = eventType,
                    UserId = userId,
                    ISBN = book.ISBN,
                    BookCopyId = bookCopy.Id,
                    Timestamp = DateTime.Now.AddDays(-_random.Next(30)),
                    Description = $"{eventType} event for book {book.Title}"
                });
            }
            return events;
        }
    }
}