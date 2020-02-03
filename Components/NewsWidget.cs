using contosoUniversity2020.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Components
{
    //1. Create the NewsWidget View Component
    public class NewsWidget:ViewComponent // Must inherit ViewComponent
    {
        private INewsService _newsService;
        //2. Create the Constrcutor to instantiate our news aritcle
        public NewsWidget(INewsService newsService)
        {
            _newsService = newsService;
        }
        //3. Need to create the InvokeAsync method
        public async Task<IViewComponentResult> InvokeAsync(int threshold = 3)
        {
            var news = await _newsService.GetNews(threshold); // by default return a random news items at a time
            return View(news);//must return view (Default.cshtml)
            
            
            //View follows a specific search path
            //Views / Shared / Components / NewsWidget/Default.cshtml
        }

    }
}
