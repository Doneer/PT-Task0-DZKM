using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        UserType Type { get; set; }
        DateTime RegistrationDate { get; set; }
    }
}
