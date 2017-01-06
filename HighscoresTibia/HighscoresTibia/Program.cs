using HighscoresTibia.model;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WebUtilsLib;

namespace HighscoresTibia
{
    class Program
    {
        #region ** Private Attributes **
        private static string      _outputfileAllPlayers;
        private static string      _outputfileWithFilter;
        private static bool        _shouldSaveAllPlayers;
        private static int         _numberPlayersTopExpExport;
        private static string      _vocationPlayers;
        private static WebRequests _request = null;
        #endregion

        public static string LoadConfigurationSetting(string keyname, string defaultvalue)
        {
            string result = defaultvalue;
            try
            {
                result = ConfigurationManager.AppSettings[keyname];
            }
            catch
            {
                result = defaultvalue;
            }
            if (result == null)
                result = defaultvalue;
            return result;
        }

        public static void Configure()
        {
            _request = new WebRequests();
            _request.Encoding = "ISO-8859-1";
        }

        private static void SetupHeaders(WebRequests request)
        {
            request.Host = "secure.tibia.com";
            request.KeepAlive = true;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

            request.Headers.Add("Accept-Encoding: gzip, deflate, sdch, br");
            request.Headers.Add("Accept-Language: pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        }

        static void Main(string[] args)
        {

            HtmlDocument htmlDoc     = new HtmlDocument();
            StringBuilder parameters = new StringBuilder();

            string html              = null;
            string mainURL           = null;
            int pageNumber           = 1;

            List<String> worlds = new List<string>
            {
                "Amera", "Antica", "Astera", "Aurera", "Aurora", "Bellona", "Belobra", "Beneva","Calmera", "Calva", "Calvera", "Candia", "Celesta", "Chrona", "Danera", "Dolera", "Efidia", "Eldera", "Ferobra", "Fidera", "Fortera", "Garnera", "Guardia", "Harmonia", "Honera", "Hydera", "Inferna", "Iona", "Irmada", "Julera", "Justera", "Kenora", "Kronera", "Laudera", "Luminera", "Magera", "Menera", "Morta", "Mortera", "Neptera", "Nerana", "Nika", "Olympa", "Osera", "Pacera", "Premia", "Pythera", "Quilia", "Refugia", "Rowana", "Secura", "Serdebra", "Shivera", "Silvera", "Solera", "Tavara", "Thera", "Umera", "Unitera", "Veludera", "Verlana", "Xantera", "Xylana", "Yanara", "Zanera", "Zeluna"
            };

            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region Getting AppSettings fields
            _outputfileAllPlayers      = LoadConfigurationSetting("outputfileAllPlayers", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HIGHSCORES.txt"));
            _outputfileWithFilter      = LoadConfigurationSetting("outputfileWithFilter", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HIGHSCORES_TOP500.txt"));
            _numberPlayersTopExpExport = Convert.ToInt32(ConfigurationManager.AppSettings.Get("numberPlayersTopExpExport"));
            _shouldSaveAllPlayers      = (Convert.ToInt32(ConfigurationManager.AppSettings.Get("shouldSaveAllPlayers"))) == 1 ? true : false;
            _vocationPlayers           = LoadConfigurationSetting("vocationPlayers","Knight");
            Configure();
            #endregion

            // Setup HTTP headers
            SetupHeaders(_request);

            int count;
            StringBuilder sb = new StringBuilder();

            // Create new stopwatch.
            Stopwatch watch = new Stopwatch();

            // Begin timing.
            watch.Start();

            Console.WriteLine(">>>>> HIGHSCORE TIBIA CRAWLER <<<<<\n\n");
            sb.Clear();
            sb.Append("{\"Players\":[");


            foreach (string world in worlds)
            {
                count = 1;

                Console.WriteLine("World > " + world + " < Processing...");

                mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=" + world + "&list=experience&profession=0&currentpage=" + count;

                Again2:
                html = _request.Get(mainURL);
                if (String.IsNullOrEmpty(html))
                {
                    System.Threading.Thread.Sleep(2000);
                    goto Again2;
                }

                htmlDoc.LoadHtml(html);

                // Get the number of pages
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//small/div//a");

                if ((nodes == null) || (nodes.Count == 0))
                {
                    ConsoleError("ERROR TO CAPTURE NUMBER OF PAGES. REPORT TO THE ADMINISTRATOR");
                    Console.ReadKey();
                    return;
                }

                pageNumber = nodes.Count + 1;

                do
                {
                    mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=" + world + "&list=experience&profession=0&currentpage=" + count;

                    Again:
                    html = _request.Get(mainURL);
                    if (String.IsNullOrEmpty(html))
                    {
                        System.Threading.Thread.Sleep(2000);
                        goto Again;
                    }

                    htmlDoc.LoadHtml(html);

                    nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='TableContentContainer']//table//tr[contains(@style,'background-color')]");

                    if ((nodes == null) || (nodes.Count == 0))
                    {
                        ConsoleError("ERROR TO CAPTURE TABLE. REPORT TO THE ADMINISTRATOR");
                        Console.ReadKey();
                        return;
                    }

                    foreach (HtmlNode node in nodes)
                    {
                        dic = ProcessNewPlayer(node, world);
                        sb.Append("{");

                        foreach (string data in dic.Keys)
                        {
                            sb.AppendFormat("\"{0}\":\"{1}\",", data, dic[data]);
                        }

                        //Remove last caracter
                        sb.Length--;
                        sb.Append("},");
                    }

                } while (++count <= pageNumber);

                Console.WriteLine("World > " + world + " < Processed!\n");
            }
            watch.Stop();

            // Formatting JSON
            sb.Length--;
            sb.Append("]}");

            Console.WriteLine(">> Capture processing time: " + watch.Elapsed.ToString(@"m\:ss"));
           
            AllPlayers listPlayers = JsonConvert.DeserializeObject<AllPlayers>(sb.ToString());

            
            if (_shouldSaveAllPlayers)
            {
                Console.WriteLine("\nSaving results ....");

                if (SaveResultToTXT(sb.ToString(), _outputfileAllPlayers))
                    Console.WriteLine("Results saved!");
                else
                {
                    ConsoleError("ERROR TO SAVE FILE. REPORT TO THE ADMINISTRATOR");
                    Console.ReadKey();
                    return;
                }
            }

            else
            {
                watch = new Stopwatch();
                watch.Start();

                // Order by Experience
                List<Player> orderListPlayer = listPlayers.Players.OrderByDescending(o => Convert.ToInt64(o.experience)).ToList();
                watch.Stop();

                Console.WriteLine(">> Ordination processing time: " + watch.Elapsed.ToString(@"m\:ss"));
                sb.Clear();

                for (int i = 0; i < _numberPlayersTopExpExport; i++)
                {
                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                                     i+1,
                                     orderListPlayer[i].name, 
                                     orderListPlayer[i].world, 
                                     orderListPlayer[i].vocation,
                                     orderListPlayer[i].level,
                                     orderListPlayer[i].experience);
                    sb.AppendLine();
                }
                Console.WriteLine("\nSaving results ....");

                if (SaveResultToTXT(sb.ToString(), _outputfileWithFilter))
                    Console.WriteLine("Results saved!");
                else
                {
                    ConsoleError("ERROR TO SAVE FILE WITH FILTER. REPORT TO THE ADMINISTRATOR");
                    Console.ReadKey();
                    return;
                }
            }



            Console.ReadKey();
        }

        private static Dictionary<string, string> ProcessNewPlayer(HtmlNode node, string world)
        {
            string relativePosition = String.Empty;
            string name = String.Empty;
            string vocation = String.Empty;
            string level = String.Empty;
            string experience = String.Empty;
            string aux = String.Empty;


            Dictionary<string, string> dic = new Dictionary<string, string> { { "world", "0" }, { "relativePosition", "0" }, { "name", "0" }, { "vocation", "0" }, { "level", "0" }, { "experience", "0" } };
            HtmlNodeCollection nodes = null;

            nodes = node.SelectNodes(".//td");

            if (nodes == null)
                return null;

            if (nodes.Count == 5)
            {
                dic[dic.ElementAt(0).Key] = world;

                for (int i = 0; i < nodes.Count;)
                {
                    aux = nodes[i].InnerText.Trim().Replace(",", "");

                    if (!String.IsNullOrWhiteSpace(aux))
                        dic[dic.ElementAt(++i).Key] = aux;
                }

                return dic;
            }

            return null;
        }

        private static string ConvertFromDictionaryToJSON(Dictionary<string, string> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }

        private static bool SaveResultToTXT(string text, string path)
        {
            try
            {
                System.IO.File.WriteAllText(path.Substring(0, path.Length - 4) + "_" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-") + ".txt", text);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>>>> ERROR : " + ex.Message);
            }
            return false;
        }

        private static void ConsoleError(string text)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
