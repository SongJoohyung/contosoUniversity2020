using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsItem>> GetNews(int threshold);
    }
}
