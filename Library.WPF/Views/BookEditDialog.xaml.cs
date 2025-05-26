using System.Windows;
using Library.WPF.Models;

namespace Library.WPF.Views
{
    public partial class BookEditDialog : Window
    {
        private BookViewModel ViewModel => DataContext as BookViewModel;

        public BookEditDialog()
        {
            InitializeComponent();
            Loaded += BookEditDialog_Loaded;
        }

        private void BookEditDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel?.ISBN))
            {
                ISBNTextBox.IsReadOnly = true;
                ISBNTextBox.Background = System.Windows.Media.Brushes.LightGray;
                TitleTextBox.Focus();
            }
            else
            {
                ISBNTextBox.Focus();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
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

            if (string.IsNullOrWhiteSpace(ISBNTextBox.Text))
            {
                ValidationMessage.Text = "ISBN is required.";
                ISBNTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ValidationMessage.Text = "Title is required.";
                TitleTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(AuthorTextBox.Text))
            {
                ValidationMessage.Text = "Author is required.";
                AuthorTextBox.Focus();
                return false;
            }

            if (!int.TryParse(YearTextBox.Text, out int year) || year < 1000 || year > DateTime.Now.Year + 1)
            {
                ValidationMessage.Text = $"Publication year must be between 1000 and {DateTime.Now.Year + 1}.";
                YearTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}