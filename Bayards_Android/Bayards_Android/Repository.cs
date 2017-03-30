using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bayards_Android
{
    class Repository
    {
        public bool sendPassword(string password)
        {
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