using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EbayScraperConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }

        private static async void GetHtmlAsync()
        {
            var url = "https://www.ebay.com/sch/i.html?_nkw=xbox+one&_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=50";
            var httpclient = new HttpClient();
            var html = await httpclient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var ProductsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                    .Equals("ListViewInner")).ToList();

            var ProductListItems = ProductsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                    .Contains("item")).ToList();

            Console.WriteLine(ProductListItems.Count());
            Console.WriteLine();

            foreach (var ProductListItem in ProductListItems)
            {
                //ID
                Console.WriteLine(ProductListItem.GetAttributeValue("listingid", ""));
                //ProductName
                Console.WriteLine(ProductListItem.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t'));
                //Price               
                Console.WriteLine(
                   Regex.Match(
                    ProductListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t')
                    , @"\d+.\d+")
                    );
                //ListingType
                Console.WriteLine(ProductListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvformat")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t')
                    );
                //URL
                Console.WriteLine(
                    ProductListItem.Descendants("a").FirstOrDefault().GetAttributeValue("href", "")
                    );
                Console.WriteLine();
            }
        }
    }
}