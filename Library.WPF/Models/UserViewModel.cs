using Library.Data.Interfaces.Models;
using Library.Data.Models;
using Library.Data.Factories;
using Library.WPF.ViewModels;

namespace Library.WPF.Models
{
    public class UserViewModel : BaseViewModel
    {
        private int _id;
        private string _name;
        private string _email;
        private string _phoneNumber;
        private UserType _type;
        private DateTime _registrationDate;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public UserType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public DateTime RegistrationDate
        {
            get => _registrationDate;
            set => SetProperty(ref _registrationDate, value);
        }

        public string TypeDisplay => Type.ToString();

        public UserViewModel()
        {
            _name = string.Empty;
            _email = string.Empty;
            _phoneNumber = string.Empty;
            _registrationDate = DateTime.Now;
        }

        public UserViewModel(IUser user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            Type = user.Type;
            RegistrationDate = user.RegistrationDate;
        }

        public IUser ToUser()
        {
            return UserFactory.CreateUser(
                Id,
                Name,
                Email,
                PhoneNumber,
                Type,
                RegistrationDate
            );
        }
    }
}