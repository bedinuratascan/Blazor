using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Static
{
    public static class Endpoints
    {
        public static string BaseUrl = "https://localhost:44313/";
        public static string AuthorsEndpoint = $"{BaseUrl}api/author/";
        public static string BooksEndpoint = $"{BaseUrl}api/book/";
        public static string RegisterEndpoint = $"{BaseUrl}api/user/register";
    }
}
