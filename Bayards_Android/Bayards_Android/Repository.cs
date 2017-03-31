using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayards_Android
{
    class Repository
    {
        public async Task<bool> sendPassword(string password)
        {
            await Task.Delay(5000);
            return password == "hsepassword";
        }

        
        //public  string getLorem()
        //{

        //    var url = "http://lorem.vn/api/?type=meat-and-filler";

        //    using (var client = new HttpClient())
        //    {
        //        var response = client.GetStringAsync(url).Result;
        //        return response;
        //    }
            
        //}



    }
}