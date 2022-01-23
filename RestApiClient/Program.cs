using System;
using RestSharp;
namespace WebAppAspNetMvcIdentity
{
    class Program
    {

        static void Main(string[] args)
        {
            string url = "https://anapioficeandfire.com/api/characters/583";
            var client = new RestClient(url);
            var request = new RestRequest();
            var body = new post { name = "This is test body", gender = "test post request", born = "this test" };
            request.AddBody(body);
            var response = client.Post(request);
            Console.WriteLine(response.StatusCode.ToString() + "    " + response.Content.ToString());
            Console.Read();
        }
    }
}
