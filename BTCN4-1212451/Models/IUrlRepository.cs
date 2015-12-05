using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTCN4_1212451.Models
{
    public interface IUrlRepository
    {
        IQueryable<News> GetAll();
        void Add(News url);
    }
}