using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface IUser
    {
        int Id { get; }
        string Name { get; }
        string Email { get; }
        string PhoneNumber { get; }
        UserType Type { get; }
        DateTime RegistrationDate { get; }
    }
}
