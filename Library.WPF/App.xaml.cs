using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Interfaces;
using Library.Data.Repositories;
using Library.Logic.Interfaces;
using Library.Logic.Services;
using Library.WPF.Services;
using Library.WPF.ViewModels;

namespace Library.WPF
{
    public partial class App : Application
    {
        private IHost _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = CreateHostBuilder().Build();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true";

                    services.AddDbContext<LibraryDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddScoped<IUserRepository>(provider =>
                        new EfUserRepository(provider.GetRequiredService<DbContextOptions<LibraryDbContext>>()));

                    services.AddScoped<ICatalogRepository>(provider =>
                        new EfCatalogRepository(provider.GetRequiredService<DbContextOptions<LibraryDbContext>>()));

                    services.AddScoped<IStateRepository>(provider =>
                        new EfStateRepository(provider.GetRequiredService<DbContextOptions<LibraryDbContext>>()));

                    services.AddScoped<IEventRepository>(provider =>
                        new EfEventRepository(provider.GetRequiredService<DbContextOptions<LibraryDbContext>>()));

                    services.AddScoped<IDataRepository>(provider =>
                        new EfDataRepository(provider.GetRequiredService<DbContextOptions<LibraryDbContext>>()));

                    services.AddScoped<ILibraryService, LibraryService>();

                    services.AddSingleton<IDialogService, DialogService>();

                    services.AddTransient<MainViewModel>();

                    services.AddTransient<MainWindow>();
                });
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

                using (var scope = _host.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    await context.Database.EnsureCreatedAsync();

                    await SeedDatabaseAsync(context);
                }

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();

                if (mainViewModel.LoadDataCommand.CanExecute(null))
                {
                    mainViewModel.LoadDataCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private async Task SeedDatabaseAsync(LibraryDbContext context)
        {
            if (context.Users.Any() || context.Books.Any())
                return;

            var users = new[]
            {
                new UserEntity { Name = "John Doe", Email = "john@example.com", PhoneNumber = "555-1234", Type = 0, RegistrationDate = DateTime.Now.AddDays(-30) },
                new UserEntity { Name = "Jane Smith", Email = "jane@example.com", PhoneNumber = "555-5678", Type = 0, RegistrationDate = DateTime.Now.AddDays(-20) },
                new UserEntity { Name = "Michael Johnson", Email = "michael@example.com", PhoneNumber = "555-9012", Type = 1, RegistrationDate = DateTime.Now.AddDays(-60) },
                new UserEntity { Name = "Emily Brown", Email = "emily@example.com", PhoneNumber = "555-3456", Type = 2, RegistrationDate = DateTime.Now.AddDays(-90) }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var books = new[]
            {
                new BookEntity { ISBN = "978-0-061-12241-5", Title = "To Kill a Mockingbird", Author = "Harper Lee", Publisher = "HarperCollins", PublicationYear = 1960, Genre = "Fiction", Description = "Classic novel about racial injustice" },
                new BookEntity { ISBN = "978-0-743-27325-1", Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Publisher = "Scribner", PublicationYear = 1925, Genre = "Fiction", Description = "Classic novel about the American Dream" },
                new BookEntity { ISBN = "978-0-141-03614-4", Title = "1984", Author = "George Orwell", Publisher = "Penguin", PublicationYear = 1949, Genre = "Science Fiction", Description = "Dystopian novel about totalitarianism" },
                new BookEntity { ISBN = "978-0-316-76948-0", Title = "The Catcher in the Rye", Author = "J.D. Salinger", Publisher = "Little, Brown", PublicationYear = 1951, Genre = "Fiction", Description = "Novel about teenage alienation" },
                new BookEntity { ISBN = "978-0-060-85040-2", Title = "The Hobbit", Author = "J.R.R. Tolkien", Publisher = "HarperCollins", PublicationYear = 1937, Genre = "Fantasy", Description = "Fantasy novel about a hobbit's adventure" }
            };

            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var bookCopies = new[]
            {
                new BookCopyEntity { ISBN = "978-0-061-12241-5", Status = 0, AcquisitionDate = DateTime.Now.AddDays(-100), Location = "Shelf A1" },
                new BookCopyEntity { ISBN = "978-0-061-12241-5", Status = 1, AcquisitionDate = DateTime.Now.AddDays(-100), Location = "Shelf A1", CurrentBorrowerId = 1, DueDate = DateTime.Now.AddDays(7) },
                new BookCopyEntity { ISBN = "978-0-743-27325-1", Status = 0, AcquisitionDate = DateTime.Now.AddDays(-80), Location = "Shelf A2" },
                new BookCopyEntity { ISBN = "978-0-141-03614-4", Status = 1, AcquisitionDate = DateTime.Now.AddDays(-60), Location = "Shelf B1", CurrentBorrowerId = 2, DueDate = DateTime.Now.AddDays(3) },
                new BookCopyEntity { ISBN = "978-0-316-76948-0", Status = 2, AcquisitionDate = DateTime.Now.AddDays(-40), Location = "Shelf B2" },
                new BookCopyEntity { ISBN = "978-0-060-85040-2", Status = 0, AcquisitionDate = DateTime.Now.AddDays(-20), Location = "Shelf C1" }
            };

            context.BookCopies.AddRange(bookCopies);
            await context.SaveChangesAsync();

            var events = new[]
            {
                new LibraryEventEntity { Type = 0, ISBN = "978-0-061-12241-5", Timestamp = DateTime.Now.AddDays(-30), Description = "Added new copy of To Kill a Mockingbird" },
                new LibraryEventEntity { Type = 2, UserId = 1, ISBN = "978-0-061-12241-5", Timestamp = DateTime.Now.AddDays(-7), Description = "John Doe borrowed To Kill a Mockingbird" },
                new LibraryEventEntity { Type = 0, ISBN = "978-0-743-27325-1", Timestamp = DateTime.Now.AddDays(-25), Description = "Added new copy of The Great Gatsby" },
                new LibraryEventEntity { Type = 2, UserId = 2, ISBN = "978-0-141-03614-4", Timestamp = DateTime.Now.AddDays(-10), Description = "Jane Smith borrowed 1984" }
            };

            context.LibraryEvents.AddRange(events);
            await context.SaveChangesAsync();
        }
    }
}