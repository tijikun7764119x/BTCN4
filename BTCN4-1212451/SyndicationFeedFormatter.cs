using BTCN4_1212451.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BTCN4_1212451
{
    public class SyndicationFeedFormatter : MediaTypeFormatter
    {
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";

        public SyndicationFeedFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(atom));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rss));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(News) || type == typeof(IEnumerable<News>))
                return true;
            else
                return false;
        };

        public override bool CanReadType(Type type)
        {
            return SupportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return SupportedType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (type == typeof(News) || type == typeof(IEnumerable<News>))
                    BuildSyndicationFeed(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSyndicationFeed(object models, Stream stream, string contenttype)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var feed = new SyndicationFeed()
            {
                Title = new TextSyndicationContent("Luong Thanh Trung - 1212451")
            };

            if (models is IEnumerable<News>)
            {
                var enumerator = ((IEnumerable<News>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationItem(enumerator.Current));
                }
            }
            else
            {
                items.Add(BuildSyndicationItem((News)models));
            }

            feed.Items = items;

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, atom))
                {
                    Atom10FeedFormatter atomformatter = new Atom10FeedFormatter(feed);
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }

        private SyndicationItem BuildSyndicationItem(News u)
        {
            
            //DateTime dt = DateTime.ParseExact(u.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dt = new DateTime(Int32.Parse(u.year), Int32.Parse(u.month), Int32.Parse(u.day));
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.title),
                Content = new TextSyndicationContent(u.title),
                BaseUri = new Uri(u.href),
                PublishDate = new DateTimeOffset(dt)
            };
            item.Authors.Add(new SyndicationPerson() { Name = "FITHCMUS" });
            return item;
        }
    }
}