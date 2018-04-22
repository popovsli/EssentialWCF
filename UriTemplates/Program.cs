using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UriTemplates
{
    class Program
    {
        static void Main(string[] args)
        {
              string symbol = "MSFT";
              string[] symbol2 = { "MSFT", "GOOGLE" };
            //ByPosition
            Uri YahooStockBaseUri = new Uri("http://finance.yahoo.com");
            UriTemplate YahooStockUriTemplate = new UriTemplate("/d/quotes?s={symbol}&f=sl1t1d1");
            Uri YahooStockUri = YahooStockUriTemplate.BindByPosition(YahooStockBaseUri, symbol);
            Console.WriteLine(YahooStockUri.ToString());

            //ByName
            Uri GoogleStockBaseUri = new Uri("http://finance.google.com");
            UriTemplate GoogleStockUriTemplate = new UriTemplate("/d/quotes?s={symbol}&f=sl1t1d1");
            NameValueCollection GoogleParams = new NameValueCollection();
            GoogleParams.Add("symbol", symbol);
            Uri GoogleStockUri = GoogleStockUriTemplate.BindByName(GoogleStockBaseUri,GoogleParams);
            Console.WriteLine(GoogleStockUri.ToString());


            //Parsing URI
            Uri YahooStockBaseUriParse = new Uri("http://finance.yahoo.com");

            UriTemplate YahooStockUriTemplateParse = new UriTemplate("/d/quotes?s={symbol}");

            Uri YahooStockUriParse = new Uri("http://finance.yahoo.com/d/quotes?s=MSFT&f=spt1d");
            UriTemplateMatch match = YahooStockUriTemplateParse.Match(YahooStockBaseUriParse, YahooStockUriParse);

            foreach (string key in match.BoundVariables.Keys)
            {
                Console.WriteLine(String.Format("{0}: {1}", key, match.BoundVariables[key]));
            }

            Uri ReferenceDotComBaseUri = new Uri("http://dictionary.reference.com");
            UriTemplate ReferenceDotComTemplate = new UriTemplate("/browse/{word}");

            Uri ReferenceDotComUri = new Uri("http://dictionary.reference.com/browse/opaque");

            var match2 = ReferenceDotComTemplate.Match(ReferenceDotComBaseUri, ReferenceDotComUri);

            foreach (string key2 in match2.BoundVariables.Keys)
            {
                Console.WriteLine(String.Format("{0}: {1}", key2, match2.BoundVariables[key2]));
            }


            Console.ReadLine();
        }
    }
}
