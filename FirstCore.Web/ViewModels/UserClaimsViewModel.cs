using FirstCore.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.ViewModels
{
    //PArt: 93.4
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }

        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
}
