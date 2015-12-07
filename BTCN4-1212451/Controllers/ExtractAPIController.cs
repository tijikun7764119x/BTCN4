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
            //var address = "http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97";
            var address = "http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=36";
            var document = await BrowsingContext.New(config).OpenAsync(address);
            // Asynchronously get the document in a new context using the configuration
            //var document = await BrowsingContext.New(config).OpenAsync(address);
            // This CSS selector gets the desired content
            //var tagaShowPostLinks = "a.ShowPostLinks";
            //var tagspanShowPostDate = "span.ShowPostDate";
            var tagtd_title = "td.post_title";
            var tagtd_day_month = "td.day_month";
            var tagtd_post_year = "td.post_year";
            // Perform the query to get all cells with the content
            //var cell_tagaShowPostLinks = document.QuerySelectorAll(tagaShowPostLinks);
            //var cell_tagspanShowPostDate = document.QuerySelectorAll(tagspanShowPostDate);
            var cell_tagtd_title = document.QuerySelectorAll(tagtd_title);
            var cell_tagtd_day_month = document.QuerySelectorAll(tagtd_day_month);
            var cell_tagtd_post_year = document.QuerySelectorAll(tagtd_post_year);
            // We are only interested in the text - select it with LINQ
            //List<String> list_cells_tagaShowPostLinks = cell_tagaShowPostLinks.Select(m => m.TextContent).ToList();
            //List<String> list_cells_tagspanShowPostDate = cell_tagspanShowPostDate.Select(m => m.TextContent).ToList();
            List<String> list_cells_tagtd_title = cell_tagtd_title.Select(m => m.TextContent).ToList();
            List<String> list_cells_tagtd_title_href = cell_tagtd_title.Select(m => m.InnerHtml).ToList();
            List<String> list_cells_tagtd_day_month = cell_tagtd_day_month.Select(m => m.TextContent).ToList();
            List<String> list_cells_tagtd_post_year = cell_tagtd_post_year.Select(m => m.TextContent).ToList();

            //int amout = list_cells_tagaShowPostLinks.Count();
            int amout = list_cells_tagtd_title.Count();
            Regex number = new Regex(@"\d+");
            for (int i = 0; i < amout; i++)
            {
                string str_title = list_cells_tagtd_title[i].Replace('\n', '\t');
                str_title = System.Text.RegularExpressions.Regex.Replace(str_title, "\t", "");
                string str_href = list_cells_tagtd_title_href[i].Substring(list_cells_tagtd_title_href[i].IndexOf("href=\"") + 6, list_cells_tagtd_title_href[i].IndexOf("\" title") - list_cells_tagtd_title_href[i].IndexOf("href=\"") - 6);
                string str_day = number.Match(list_cells_tagtd_day_month[i * 2]).Value;
                string str_month = number.Match(list_cells_tagtd_day_month[i * 2 + 1]).Value;
                string str_year = number.Match(list_cells_tagtd_post_year[i]).Value;
                News temp = new News()
                {
                    //title = list_cells_tagaShowPostLinks[i],
                    title = str_title,
                    day = str_day,
                    month = str_month,
                    year = str_year,
                    href = "http://www.fit.hcmus.edu.vn/vn/" + str_href
                };
                url.Add(temp);
            }

            return url.GetAll();
        }
    }
}
