using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayards_Android
{
    class CreditnailsProvider
    {
        public bool IsAuthorized { get; set; }
        public async Task<bool> sendPassword(string password)
        {
            await Task.Delay(5000);
            return password == "hsepassword";
        }




    }
}