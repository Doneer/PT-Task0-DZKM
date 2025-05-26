using Library.WPF.Models;

namespace Library.WPF.Services
{
    public interface IDialogService
    {
        void ShowError(string title, string message);
        void ShowInformation(string title, string message);
        bool ShowConfirmation(string title, string message);
        bool ShowUserDialog(UserViewModel user, string title);
        bool ShowBookDialog(BookViewModel book, string title);
    }
}