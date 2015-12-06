using AngleSharp;
using BTCN4_1212451.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BTCN4_1212451.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExtractAPIController : ApiController
    {
        IUrlRepository url = new UrlRepository();
        [Route("api/fithcmus/news")]
        
        public async Task<IEnumerable<News>> GetNews()
        {
            Request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/rss+xml"));
            // Setup the configuration to support document loading
            var config = new Configuration().WithDefaultLoader();
            var address = "http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97";
            var document = await BrowsingContext.New(config).OpenAsync(address);
            // Asynchronously get the document in a new context using the configuration
            //var document = await BrowsingContext.New(config).OpenAsync(address);
            // This CSS selector gets the desired content
            var tagaShowPostLinks = "a.ShowPostLinks";
            var tagspanShowPostDate = "span.ShowPostDate";
            // Perform the query to get all cells with the content
            var cell_tagaShowPostLinks = document.QuerySelectorAll(tagaShowPostLinks);
            var cell_tagspanShowPostDate = document.QuerySelectorAll(tagspanShowPostDate);
            // We are only interested in the text - select it with LINQ
            List<String> list_cells_tagaShowPostLinks = cell_tagaShowPostLinks.Select(m => m.TextContent).ToList();
            List<String> list_cells_tagspanShowPostDate = cell_tagspanShowPostDate.Select(m => m.TextContent).ToList();

            int amout = list_cells_tagaShowPostLinks.Count();
            Regex regex = new Regex(@"\d+\/\d+\/\d+");
            for (int i = 0; i < amout; i++)
            {
                News temp = new News()
                {
                    title = list_cells_tagaShowPostLinks[i],
                    date = regex.Match(list_cells_tagspanShowPostDate[i]).Value
                };

                url.Add(temp);
            }

            return url.GetAll();
        }
    }
}
