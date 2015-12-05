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
        IUrlRepository ShowPosts = new UrlRepository();
        [Route("api/fithcmus/news")]
        public async Task<IEnumerable<News>> GetNews()
        {
            // Setup the configuration to support document loading
            var config = new Configuration().WithDefaultLoader();
            var address = "http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97";
            var document = await BrowsingContext.New(config).OpenAsync(address);
            // Asynchronously get the document in a new context using the configuration
            //var document = await BrowsingContext.New(config).OpenAsync(address);
            // This CSS selector gets the desired content
            var aShowPostLinks = "a.ShowPostLinks";
            var spanShowPostDate = "span.ShowPostDate";
            // Perform the query to get all cells with the content
            var cells_aShowPostLinks = document.QuerySelectorAll(aShowPostLinks);
            var cells_spanShowPostDate = document.QuerySelectorAll(spanShowPostDate);
            // We are only interested in the text - select it with LINQ
            List<String> titles_cells_aShowPostLinks = cells_aShowPostLinks.Select(m => m.TextContent).ToList();
            List<String> titles_cells_cells_spanShowPostDate = cells_spanShowPostDate.Select(m => m.TextContent).ToList();

            int countshowpost = titles_cells_aShowPostLinks.Count();
            Regex regex = new Regex(@"\d+\/\d+\/\d+");
            for (int i = 0; i < countshowpost; i++)
            {
                News temp = new News()
                {
                    title = titles_cells_aShowPostLinks[i],
                    date = regex.Match(titles_cells_cells_spanShowPostDate[i]).Value
                };

                ShowPosts.Add(temp);
            }

            return ShowPosts.GetAll();
        }
    }
}
