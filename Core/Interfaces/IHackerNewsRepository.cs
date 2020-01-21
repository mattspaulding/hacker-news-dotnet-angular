
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace hacker_news_dotnet_angular.Core.Interfaces
{
    public interface IHackerNewsRepository
    {
        Task<HttpResponseMessage> BestStoriesAsync();
        Task<HttpResponseMessage> GetStoryByIdAsync(int id);

    }
}