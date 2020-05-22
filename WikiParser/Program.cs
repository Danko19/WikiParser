using System.Linq;
using System.Net;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Html.Dom;
using WikiParser.MySql;

namespace WikiParser
{
    internal class Program
    {
        private const string domain = "https://ru.wiktionary.org/";

        public static void Main(string[] args)
        {
            DoShit2();
        }

        private static void DoShit1()
        {
            var i = 1;
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
            {
                using (var client = new HttpClient(handler))
                {
                    using (var mySql = new MySqlStorage())
                    {
                        var nextPage =
                            "https://ru.wiktionary.org/w/index.php?title=%D0%9A%D0%B0%D1%82%D0%B5%D0%B3%D0%BE%D1%80%D0%B8%D1%8F:%D0%90%D0%B1%D0%B1%D1%80%D0%B5%D0%B2%D0%B8%D0%B0%D1%82%D1%83%D1%80%D1%8B/ru&pageuntil=%D0%B1%D0%B8%D0%BE%D1%81%D1%82%D0%B0%D1%82%D0%B8%D1%81%D1%82%D0%B8%D0%BA%D0%B0%0A%D0%B1%D0%B8%D0%BE%D1%81%D1%82%D0%B0%D1%82%D0%B8%D1%81%D1%82%D0%B8%D0%BA%D0%B0#mw-pages";
                        while (nextPage != null)
                        {
                            var response = client.GetAsync(
                                    nextPage)
                                .GetAwaiter()
                                .GetResult();
                            var html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            var config = Configuration.Default;
                            var context = BrowsingContext.New(config);
                            var document = context.OpenAsync(req => req.Content(html)).Result;
                            var querySelectorAll = document.QuerySelectorAll(".mw-category-group");

                            foreach (var div in querySelectorAll)
                            {
                                foreach (var element in div.GetElementsByTagName("a"))
                                {
                                    var a = (IHtmlAnchorElement) element;
                                    mySql.InsertOrUpdate(new AbbreviationEntity
                                    {
                                        Id = i++,
                                        Title = a.InnerHtml,
                                        Link = a.Href.Replace(a.BaseUri, domain)
                                    });
                                }
                            }

                            var nextA = document
                                .GetElementById("mw-pages")
                                .GetElementsByTagName("a")
                                .Select(x => (IHtmlAnchorElement) x)
                                .FirstOrDefault(x => x.InnerHtml == "Следующая страница");

                            nextPage = nextA?.Href?.Replace(nextA.BaseUri, domain);
                        }
                    }
                }
            }
        }

        private static void DoShit2()
        {
            var i = 1;
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
            {
                using (var client = new HttpClient(handler))
                {
                    using (var mySql = new MySqlStorage())
                    {
                        foreach (var abbreviation in mySql.GetAll<AbbreviationEntity>().OrderBy(x => x.Decryption?.Length ?? 0))
                        {
                            if (string.IsNullOrEmpty(abbreviation.Decryption))
                                continue;
                            var response = client.GetAsync(
                                    abbreviation.Link)
                                .GetAwaiter()
                                .GetResult();
                            var html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            var config = Configuration.Default;
                            var context = BrowsingContext.New(config);
                            var document = context.OpenAsync(req => req.Content(html)).Result;
                            var div = document.GetElementsByClassName("mw-parser-output").Single();

                            var ol = div.Children
                                .SkipWhile(x =>
                                    x.TagName != "H4" ||
                                    !x.Children.Any(z => z.TagName == "SPAN" && z.Id == "Значение"))
                                .Skip(1)
                                .FirstOrDefault(x => x.TagName == "OL");

                            abbreviation.Decryption = ol?.TextContent;
                            mySql.InsertOrUpdate(abbreviation);
                        }
                    }
                }
            }
        }
    }
}