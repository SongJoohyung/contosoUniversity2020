using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Services
{
    public class NewsService : INewsService
    {
        static List<NewsItem> news;

        public NewsService()
        {
            news = new List<NewsItem>()
            {
                new NewsItem(){Id=1,Title="contoso launches free Academic Upgrading"},
                new NewsItem(){Id=2,Title="contoso celebrates 52nd Anniversary"},
                new NewsItem(){Id=3,Title="contoso launches new website"},
                new NewsItem(){Id=4,Title="Web Dev students building NDP party website"},
                new NewsItem(){Id=5,Title="Business students appearing on Dragon's Den"},

            };
        }

        public Task <IEnumerable<NewsItem>> GetNews(int threshold)
        {
            
            //guid part is for random selection, and the take is the number of records to retrieve
            return Task.FromResult<IEnumerable<NewsItem>>(news.OrderBy(x => Guid.NewGuid()).Take(threshold).ToList());
        }
    }
}
