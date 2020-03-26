using FirstCore.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.ViewModels
{
    //Part: 66.1
    public class UserRegisterViewModel
    {
        [Required]
        [EmailAddress]
        //Part: 75.2
        [Remote(action: "IsEmailIsUse", controller: "Account")]
        //Part : 76.2
        // [ValidEmailDomain(allowedDomain:"Laboursoft.com", ErrorMessage = "Email domain must be Laboursoft.com")]          for test
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="PassWord and Confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        //Part: 77.4.1
        public string City { get; set; }
    }
}
