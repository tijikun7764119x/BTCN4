using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTCN4_1212451.Models
{
    public class UrlRepository : IUrlRepository
    {
        private List<News> showposts = new List<News>();

        public UrlRepository()
        {

        }

        public IQueryable<News> GetAll()
        {
            return showposts.AsQueryable();
        }


        public void Add(News showpost)
        {
            this.showposts.Add(showpost);

        }


    }
}