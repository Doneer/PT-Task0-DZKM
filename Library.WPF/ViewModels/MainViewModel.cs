using Library.Data.Interfaces.Models;
using Library.Logic.Interfaces;
using Library.WPF.Commands;
using Library.WPF.Models;
using Library.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Library.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ILibraryService _libraryService;
        private readonly IDialogService _dialogService;

        private UserViewModel _selectedUser;
        private BookViewModel _selectedBook;
        private string _statusMessage;
        private bool _isLoading;
        private string _userSearchText;
        private string _bookSearchText;

        public ObservableCollection<UserViewModel> Users { get; }
        public ObservableCollection<BookViewModel> Books { get; }

        public UserViewModel SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public BookViewModel SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string UserSearchText
        {
            get => _userSearchText;
            set => SetProperty(ref _userSearchText, value);
        }

        public string BookSearchText
        {
            get => _bookSearchText;
            set => SetProperty(ref _bookSearchText, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand BorrowBookCommand { get; }
        public ICommand ReturnBookCommand { get; }

        public MainViewModel(ILibraryService libraryService, IDialogService dialogService)
        {
            _libraryService = libraryService;
            _dialogService = dialogService;

            Users = new ObservableCollection<UserViewModel>();
            Books = new ObservableCollection<BookViewModel>();

            _statusMessage = "Ready";
            _userSearchText = string.Empty;
            _bookSearchText = string.Empty;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            AddUserCommand = new AsyncRelayCommand(AddUserAsync);
            EditUserCommand = new AsyncRelayCommand(EditUserAsync, () => SelectedUser != null);
            DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, () => SelectedUser != null);
            AddBookCommand = new AsyncRelayCommand(AddBookAsync);
            EditBookCommand = new AsyncRelayCommand(EditBookAsync, () => SelectedBook != null);
            DeleteBookCommand = new AsyncRelayCommand(DeleteBookAsync, () => SelectedBook != null);
            BorrowBookCommand = new AsyncRelayCommand(BorrowBookAsync, () => SelectedBook != null && SelectedUser != null);
            ReturnBookCommand = new AsyncRelayCommand(ReturnBookAsync);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading data...";

                await Task.Run(() =>
                {
                    var users = _libraryService.GetAllUsers();
                    var books = _libraryService.GetAllBooks();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Users.Clear();
                        Books.Clear();

                        foreach (var user in users)
                        {
                            Users.Add(new UserViewModel(user));
                        }

                        foreach (var book in books)
                        {
                            Books.Add(new BookViewModel(book));
                        }
                    });
                });

                StatusMessage = $"Loaded {Users.Count} users and {Books.Count} books";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to load data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddUserAsync()
        {
            try
            {
                var newUser = new UserViewModel();
                if (_dialogService.ShowUserDialog(newUser, "Add New User"))
                {
                    IsLoading = true;
                    StatusMessage = "Adding user...";

                    IUser addedUser = null;
                    await Task.Run(() =>
                    {
                        _libraryService.RegisterUser(newUser.ToUser());
                        addedUser = _libraryService.GetAllUsers()
                            .OrderByDescending(u => u.Id)
                            .FirstOrDefault(u => u.Email == newUser.Email);
                    });

                    if (addedUser != null)
                    {
                        var userViewModel = new UserViewModel(addedUser);
                        Users.Add(userViewModel);
                    }
                    else
                    {
                        Users.Add(newUser);
                    }

                    StatusMessage = "User added successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding user: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to add user: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EditUserAsync()
        {
            if (SelectedUser == null) return;

            try
            {
                var editUser = new UserViewModel(SelectedUser.ToUser());
                if (_dialogService.ShowUserDialog(editUser, "Edit User"))
                {
                    IsLoading = true;
                    StatusMessage = "Updating user...";

                    await Task.Run(() =>
                    {
                        _libraryService.UpdateUserInformation(editUser.ToUser());
                    });

                    var updatedUser = _libraryService.GetUserById(editUser.Id);
                    if (updatedUser != null)
                    {
                        var userViewModel = new UserViewModel(updatedUser);

                        var index = Users.ToList().FindIndex(u => u.Id == editUser.Id);
                        if (index >= 0)
                        {
                            Users[index] = userViewModel;
                            SelectedUser = userViewModel;
                        }
                    }

                    StatusMessage = "User updated successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating user: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to update user: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteUserAsync()
        {
            if (SelectedUser == null) return;

            try
            {
                if (_dialogService.ShowConfirmation("Delete User", $"Are you sure you want to delete user '{SelectedUser.Name}'?"))
                {
                    IsLoading = true;
                    StatusMessage = "Deleting user...";

                    await Task.Run(() =>
                    {
                        _libraryService.RemoveUser(SelectedUser.Id);
                    });

                    Users.Remove(SelectedUser);
                    SelectedUser = null;
                    StatusMessage = "User deleted successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting user: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to delete user: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddBookAsync()
        {
            try
            {
                var newBook = new BookViewModel();
                if (_dialogService.ShowBookDialog(newBook, "Add New Book"))
                {
                    IsLoading = true;
                    StatusMessage = "Adding book...";

                    await Task.Run(() =>
                    {
                        _libraryService.AddBook(newBook.ToBook());
                    });

                    Books.Add(newBook);
                    StatusMessage = "Book added successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding book: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to add book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EditBookAsync()
        {
            if (SelectedBook == null) return;

            try
            {
                var editBook = new BookViewModel(SelectedBook.ToBook());
                if (_dialogService.ShowBookDialog(editBook, "Edit Book"))
                {
                    IsLoading = true;
                    StatusMessage = "Updating book...";

                    await Task.Run(() =>
                    {
                        _libraryService.UpdateBookInformation(editBook.ToBook());
                    });

                    var updatedBook = _libraryService.GetBookByIsbn(editBook.ISBN);
                    if (updatedBook != null)
                    {
                        var bookViewModel = new BookViewModel(updatedBook);

                        var index = Books.ToList().FindIndex(b => b.ISBN == editBook.ISBN);
                        if (index >= 0)
                        {
                            Books[index] = bookViewModel;
                            SelectedBook = bookViewModel;
                        }
                    }

                    StatusMessage = "Book updated successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating book: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to update book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteBookAsync()
        {
            if (SelectedBook == null) return;

            try
            {
                if (_dialogService.ShowConfirmation("Delete Book", $"Are you sure you want to delete book '{SelectedBook.Title}'?"))
                {
                    IsLoading = true;
                    StatusMessage = "Deleting book...";

                    await Task.Run(() =>
                    {
                        _libraryService.RemoveBook(SelectedBook.ISBN);
                    });

                    Books.Remove(SelectedBook);
                    SelectedBook = null;
                    StatusMessage = "Book deleted successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting book: {ex.Message}";
                _dialogService.ShowError("Error", $"Failed to delete book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task BorrowBookAsync()
        {
            await Task.CompletedTask;
        }

        private async Task ReturnBookAsync()
        {
            await Task.CompletedTask;
        }
    }
}