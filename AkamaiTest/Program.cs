using System;
using System.Collections.Generic;
using NDesk.Options;
using RestSharp;

namespace InvalidateAkamaiCache
{
    class Program
    {
        public static void Main(string[] args)
        {
            var showHelp = false;
            var username = "";
            var password = "";

            var p = new OptionSet
                {
                    {"u|username", "Akamai Username", v => username = v},
                    {"p|password", "Akamai Password", v => password = v},
                    {"h|help", "show this message and exit", v => showHelp = v != null},
                };

            List<string> urls;
            try
            {
                urls = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `AkamaiTest --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }

            if (urls.Count > 0)
            {
                InvalidateCache(username, password, urls);

            }
            else
            {
                Console.WriteLine(
                    "You need to specify at least one url to invalidate. `AkamaiTest --help' for more information.");
            }

        }

        static void InvalidateCache(string user, string password, List<string> url)
        {
            var restClient = new RestClient("https://api.ccu.akamai.com/ccu/v2/queues/default") { Authenticator = new HttpBasicAuthenticator(user, password) };
            var request = new RestRequest(Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
            request.AddBody(new
                {
                    objects = url.ToArray()
                });
            

            var response = restClient.Execute(request);

            Console.WriteLine(response.Content);
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: AkamaiTest [OPTIONS]+ url [url]");
            Console.WriteLine("Invalidate one or more resource from akamai cache.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
