using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using Library.Data.Repositories;
using System;
using System.Collections.Generic;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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

        private List<IUser> GenerateUsers(int count)
        {
            var users = new List<IUser>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(UserFactory.CreateUser(
                    i,
                    $"User {i}",
                    $"user{i}@example.com",
                    $"555-{i:D4}",
                    (UserType)(_random.Next(3)),
                    DateTime.Now.AddDays(-_random.Next(365))
                ));
            }
            return users;
        }

        private List<IBook> GenerateBooks(int count)
        {
            var books = new List<IBook>();
            string[] genres = { "Fiction", "Mystery", "Science Fiction", "Fantasy", "Biography", "History" };
            string[] authors = { "John Smith", "Jane Doe", "Michael Johnson", "Emily Brown", "Robert Wilson" };
            string[] publishers = { "Penguin", "Harper Collins", "Simon & Schuster", "Random House" };

            for (int i = 1; i <= count; i++)
            {
                books.Add(BookFactory.CreateBook(
                    $"ISBN-{i:D5}",
                    $"Book Title {i}",
                    authors[_random.Next(authors.Length)],
                    publishers[_random.Next(publishers.Length)],
                    2000 + _random.Next(23),
                    genres[_random.Next(genres.Length)],
                    $"Description for book {i}"
                ));
            }
            return books;
        }

        private List<IBookCopy> GenerateBookCopies(List<IBook> books, int count)
        {
            var bookCopies = new List<IBookCopy>();
            for (int i = 1; i <= count; i++)
            {
                var status = (BookStatus)(_random.Next(4));
                var bookISBN = books[_random.Next(books.Count)].ISBN;

                bookCopies.Add(BookCopyFactory.CreateBookCopy(
                    i,
                    bookISBN,
                    status,
                    DateTime.Now.AddDays(-_random.Next(500)),
                    $"Shelf {_random.Next(1, 20)}",
                    status == BookStatus.CheckedOut ? _random.Next(1, 11) : null,
                    status == BookStatus.CheckedOut ? DateTime.Now.AddDays(_random.Next(14)) : null
                ));
            }
            return bookCopies;
        }

        private List<ILibraryEvent> GenerateEvents(List<IUser> users, List<IBook> books, List<IBookCopy> bookCopies, int count)
        {
            var events = new List<ILibraryEvent>();
            for (int i = 1; i <= count; i++)
            {
                var eventType = (EventType)(_random.Next(8));
                var bookCopy = bookCopies[_random.Next(bookCopies.Count)];
                var book = books.FirstOrDefault(b => b.ISBN == bookCopy.ISBN);
                int? userId = eventType == EventType.BookBorrowed || eventType == EventType.BookReturned ?
                    _random.Next(1, users.Count + 1) : null;

                events.Add(LibraryEventFactory.CreateEvent(
                    i,
                    eventType,
                    DateTime.Now.AddDays(-_random.Next(30)),
                    $"{eventType} event for book {book.Title}",
                    userId,
                    bookCopy.ISBN,
                    bookCopy.Id
                ));
            }
            return events;
        }
    }
}