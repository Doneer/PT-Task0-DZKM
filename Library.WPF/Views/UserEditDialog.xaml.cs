using Library.Data.Models;
using Library.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace Library.WPF.Views
{
    public partial class UserEditDialog : Window
    {
        private UserViewModel ViewModel => DataContext as UserViewModel;

        public UserEditDialog()
        {
            InitializeComponent();
            Loaded += UserEditDialog_Loaded;
        }

        private void UserEditDialog_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();

            if (ViewModel?.Id == 0)
            {
                var idLabel = this.FindName("IdLabel") as Label;
                var idTextBox = this.FindName("IdTextBox") as TextBox;

                if (idLabel != null) idLabel.Visibility = Visibility.Collapsed;
                if (idTextBox != null) idTextBox.Visibility = Visibility.Collapsed;
            }

            if (ViewModel != null)
            {
                foreach (ComboBoxItem item in TypeComboBox.Items)
                {
                    if (item.Tag.ToString() == ViewModel.Type.ToString())
                    {
                        TypeComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                if (TypeComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    if (Enum.TryParse<UserType>(selectedItem.Tag.ToString(), out var userType))
                    {
                        ViewModel.Type = userType;
                    }
                }

                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            ValidationMessage.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ValidationMessage.Text = "Name is required.";
                NameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ValidationMessage.Text = "Email is required.";
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                ValidationMessage.Text = "Please enter a valid email address.";
                EmailTextBox.Focus();
                return false;
            }

            if (TypeComboBox.SelectedItem == null)
            {
                ValidationMessage.Text = "User type is required.";
                TypeComboBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}