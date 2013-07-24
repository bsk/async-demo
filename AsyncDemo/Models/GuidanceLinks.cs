using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AsyncDemo.Models
{
    public class GuidanceLinks
    {
        private readonly string[] _types = {"CG", "PHG", "TA", "IP", "MT", "DT"};

        public IEnumerable<string> GetAllLinks()
        {
            var linksByType = _types.Select(GetGuidanciesOfType);

            return linksByType.SelectMany(o => o);
        }

        public async Task<IEnumerable<string>> GetAllLinksAsync()
        {
            var linksByType = await Task.WhenAll(_types.Select(GetGuidanciesOfTypeAsync));

            return linksByType.SelectMany(o => o);
        }

        public async Task<IEnumerable<string>> GetAllLinksAsyncSafe()
        {
            var linksByType = await Task.WhenAll(_types.Select(GetGuidanciesOfTypeAsyncSafe)).ConfigureAwait(false);

            return linksByType.SelectMany(o => o);
        }

        private static IEnumerable<string> GetGuidanciesOfType(string type)
        {
            var url = string.Format(@"http://www.nice.org.uk/guidance/{0}/published/index.jsp?p=off", type);

            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            {
                var result = ExtractLinks(response.GetResponseStream());
                return result;
            }
        }

        private static async Task<IEnumerable<string>> GetGuidanciesOfTypeAsync(string type)
        {
            var url = string.Format(@"http://www.nice.org.uk/guidance/{0}/published/index.jsp?p=off", type);

            var response = await new HttpClient().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();

            var result = ExtractLinks(stream);
            return result;
        }

        private static async Task<IEnumerable<string>> GetGuidanciesOfTypeAsyncSafe(string type)
        {
            var url = string.Format(@"http://www.nice.org.uk/guidance/{0}/published/index.jsp?p=off", type);

            var response = await new HttpClient().GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var result = ExtractLinks(stream);
            return result;
        }

        private static IEnumerable<string> ExtractLinks(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);

            var links = doc.DocumentNode.SelectNodes(@"//td//a");

            var result = links.SelectMany(
                node => node.Attributes.Where(attr => attr.Name.ToLower() == "href"),
                (node, href) => href.Value);
            return result;
        }
    }
}