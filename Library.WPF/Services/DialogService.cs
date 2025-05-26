using System.Windows;
using Library.WPF.Models;
using Library.WPF.Views;

namespace Library.WPF.Services
{
    public class DialogService : IDialogService
    {
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformation(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public bool ShowUserDialog(UserViewModel user, string title)
        {
            var dialog = new UserEditDialog
            {
                Title = title,
                DataContext = user,
                Owner = Application.Current.MainWindow
            };

            return dialog.ShowDialog() == true;
        }

        public bool ShowBookDialog(BookViewModel book, string title)
        {
            var dialog = new BookEditDialog
            {
                Title = title,
                DataContext = book,
                Owner = Application.Current.MainWindow
            };

            return dialog.ShowDialog() == true;
        }
    }
}