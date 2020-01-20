using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace hacker_news_dotnet_angular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private IMemoryCache cache;

        private static HttpClient client = new HttpClient();

        public HackerNewsController(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        public async Task<List<HackerNewsStory>> Index(string searchTerm)
        {
            List<HackerNewsStory> stories = new List<HackerNewsStory>();

            var response = await client.GetAsync("https://hacker-news.firebaseio.com/v0/beststories.json");
            if (response.IsSuccessStatusCode)
            {
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);

                var tasks = bestIds.Select(GetStoryAsync);
                stories = (await Task.WhenAll(tasks)).ToList();

                if (!String.IsNullOrEmpty(searchTerm))
                {
                    var search = searchTerm.ToLower();
                    stories = stories.Where(s =>
                                       s.Title.ToLower().IndexOf(search) > -1 || s.By.ToLower().IndexOf(search) > -1)
                                       .ToList();
                }
            }
            return stories;
        }

        private async Task<HackerNewsStory> GetStoryAsync(int storyId)
        {
            return await cache.GetOrCreateAsync<HackerNewsStory>(storyId,
                async cacheEntry =>
                {
                    HackerNewsStory story = new HackerNewsStory();

                    var response = await client.GetAsync(string.Format("https://hacker-news.firebaseio.com/v0/item/{0}.json", storyId));
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<HackerNewsStory>(storyResponse);
                    }

                    return story;
                });
        }
    }
}
