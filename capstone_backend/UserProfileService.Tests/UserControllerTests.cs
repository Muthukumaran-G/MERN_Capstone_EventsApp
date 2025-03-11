using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using UserProfileService.Models;
using RestSharp;

public class UserControllerTests
{
    [Fact]
    public void Test1()
    {
        var restclient = new RestClient("https://jsonplaceholder.typicode.com/");
        var restrequest = new RestRequest("users", Method.Get);

        var response = restclient.Execute(restrequest);
        Assert.Equal(200, (int)response.StatusCode);
    }

    [Fact]
    public void GetUsersTest()
    {
        var restclient = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://localhost:7097")
        });
        var restreq = new RestRequest("/api/User", Method.Get);
        var response = restclient.Execute(restreq);
        Assert.Equal(200, (int)response.StatusCode);
    }

    [Fact]
    public void RegisterUserTest()
    {
        var restclient = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://localhost:7097")
        });
        var restreq = new RestRequest("/api/User/register", Method.Post);
        var random = new Random().Next(10,10000);
        restreq.AddJsonBody(new User
        {
            FirstName = "User",
            LastName = "1",
            Email = $"user{random}@mail.com",
            Password = "123"
        });

        var response = restclient.Execute(restreq);
        Assert.Equal(200, (int)response.StatusCode);

    }

}