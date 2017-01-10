using HighscoresTibia.model;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WebUtilsLib;

namespace HighscoresTibia
{
    class Program
    {       
        #region ** Private Attributes **
        private static string      _outputfile;
        private static int         _numberPlayersTopExpExport;
        private static string      _vocationPlayers;
        private static WebRequests _request = null;
        private static string _path
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        #endregion

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

            HtmlDocument       htmlDoc         = new HtmlDocument();
            StringBuilder      parameters      = new StringBuilder();
            HtmlNodeCollection nodes;
            List<Player>       listPlayerFilter= new List<Player>();

            // Used to build the string JSON.
            StringBuilder sbJSON          = new StringBuilder();
            
            string        html            = null;
            string        mainURL         = null;
            int           pageNumber      = 1;
            int           retries         = 0;
            bool          successSaved    = false;
            bool          needsFilter     = true;
            string[]      vocations;
            int           count;
            
            // Create new stopwatch.
            Stopwatch     watch = new Stopwatch();

            List<String> worlds = new List<string>();

            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region INPUT DATA BY CONSOLE APPLICATION
            Console.Write("Enter here the number of players to be exported in the output file: ");
            _numberPlayersTopExpExport = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\n\n-- About vocations to Exported --");
            Console.WriteLine("\nIf you need to choose more than one, use \",\" to separate the fields. Example: Knight,Sorcerer,Paladin.");
            Console.WriteLine("You can use the word \"ALL\" to generate information independent of vocation, but should'nt be accompanied by other vocations.\n");
            Console.Write("\nEnter here vocations to be Exported: ");
            _vocationPlayers           = Console.ReadLine();

            _vocationPlayers = _vocationPlayers.Replace(" ", "");
            #endregion

            _outputfile = Path.Combine(_path, "CRAWLER_RESULTS");

            Configure();

            // Setup HTTP headers
            SetupHeaders(_request);

            // Begin timing.
            watch.Start();
            Console.Clear();
            Console.WriteLine(">>>>> HIGHSCORE TIBIA CRAWLER <<<<<\n\n");
            sbJSON.Clear();
            sbJSON.Append("{\"Players\":[");




            #region CRAWLER

            mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=Antica&list=experience&profession=0&currentpage=1";

            html = String.Empty;
            retries = 0;

            while (String.IsNullOrEmpty(html))
            {
                html = _request.Get(mainURL);
                System.Threading.Thread.Sleep(500);

                if (++retries > 5)
                {
                    ConsoleError("ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                    Console.ReadKey();
                    return;
                }
            }

            htmlDoc.LoadHtml(html);

            //Extract all Worlds
            nodes = htmlDoc.DocumentNode.SelectNodes("//table[@style='width:100%;']/tr[1]//option");

            if ((nodes == null) || nodes.Count == 0)
            {
                ConsoleError("ERROR TO GET NODE \"ALL WORLDS\". REPORT TO THE ADMINISTRATOR ");
                Console.ReadKey();
                return;
            }

            foreach (HtmlNode htmlNode in nodes)
            {
                if (htmlNode.Attributes["value"] != null)
                    worlds.Add(htmlNode.Attributes["value"].Value);
            }

#if DEBUG
            worlds.Clear();
            worlds.Add("Antica");
            worlds.Add("Amera");
            worlds.Add("Menera");
#endif
            foreach (string world in worlds)
            {
                count   = 1;
                retries = 0;
                Console.WriteLine("World > " + world + " < Processing...");

                mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=" + world + "&list=experience&profession=0&currentpage=" + count;

                html    = String.Empty;
                retries = 0;

                while (String.IsNullOrEmpty(html))
                {
                    html = _request.Get(mainURL);
                    System.Threading.Thread.Sleep(500);

                    if (++retries > 10)
                    {
                        ConsoleError("ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                        Console.ReadKey();
                        return;
                    }
                } 
                htmlDoc.LoadHtml(html);
                
                // Extract the number of pages
                nodes = htmlDoc.DocumentNode.SelectNodes("//small/div//a");

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

                    html    = String.Empty;
                    retries = 0;

                    while (String.IsNullOrEmpty(html))
                    {
                        html = _request.Get(mainURL);
                        System.Threading.Thread.Sleep(500);

                        if (++retries > 10)
                        {
                            ConsoleError("ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                            Console.ReadKey();
                            return;
                        }
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

                        if (dic == null)
                            return;

                        sbJSON.Append("{");

                        foreach (string data in dic.Keys)
                        {
                            sbJSON.AppendFormat("\"{0}\":\"{1}\",", data, dic[data]);
                        }

                        //Remove last caracter
                        sbJSON.Length--;
                        sbJSON.Append("},");
                    }

                } while (++count <= pageNumber);

                Console.WriteLine("World > " + world + " < Processed!\n");
            }
            #endregion

            watch.Stop();

            // Formatting JSON
            sbJSON.Length--;
            sbJSON.Append("]}");

            Console.WriteLine(">> Capture processing time: " + watch.Elapsed.ToString(@"m\:ss"));
           
            AllPlayers listPlayers = JsonConvert.DeserializeObject<AllPlayers>(sbJSON.ToString());


            //Extract only vocations of interest
            if (_vocationPlayers.Split(',').Length == 1)
            {
                if (_vocationPlayers.IndexOf("All", StringComparison.OrdinalIgnoreCase) > -1)
                    needsFilter = false;
                else
                    needsFilter = true;

                foreach (Player p in listPlayers.Players)
                {
                    if (!needsFilter)
                        listPlayerFilter.Add(p);
                    else
                    {
                        if (p.vocation.IndexOf(_vocationPlayers, StringComparison.OrdinalIgnoreCase) > -1)
                            listPlayerFilter.Add(p);
                    }
                }
            }
            else
            {
                foreach (Player p in listPlayers.Players)
                {
                    vocations = _vocationPlayers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (vocations.Length < 1)
                    {
                        ConsoleError("ERROR TO GET VOCATIONS (APPCONFIG). REPORT TO THE ADMINISTRATOR");
                        Console.ReadKey();
                        return;
                    }

                    foreach (string v in vocations)
                    {
                        if (p.vocation.IndexOf(v, StringComparison.OrdinalIgnoreCase) > -1)
                            listPlayerFilter.Add(p);
                    }
                }
            }
 
            #region SAVE INFORMATION TO TEXT FILE   

            // Order by Experience
            List<Player> orderListPlayer = listPlayerFilter.OrderByDescending(o => Convert.ToInt64(o.experience)).ToList();

                
            StringBuilder sbJSONKnight   = new StringBuilder();
            StringBuilder sbJSONDruid    = new StringBuilder();
            StringBuilder sbJSONPaladin  = new StringBuilder();
            StringBuilder sbJSONSorcerer = new StringBuilder();
            StringBuilder sbJSONALL      = new StringBuilder();

            if (_numberPlayersTopExpExport > orderListPlayer.Count)
            {
                ConsoleError("THE NUMBER OF PLAYERS CHOSEN IN APPCONFIG FILE IS GREATER THAN ALLOWED. REPORT TO THE ADMINISTRATOR");
                Console.ReadKey();
                return;
            }

            int indexKnight   = 0;
            int indexDruid    = 0;
            int indexPaladin  = 0;
            int indexSorcerer = 0;

            for (int i = 0; i < _numberPlayersTopExpExport; i++)
            {
                sbJSON.Clear();
                //sbJSON.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                //                    i+1,
                //                    orderListPlayer[i].name, 
                //                    orderListPlayer[i].world, 
                //                    orderListPlayer[i].vocation,
                //                    orderListPlayer[i].level,
                //                    orderListPlayer[i].experience);
                //sbJSON.AppendLine();

                if (orderListPlayer[i].vocation.IndexOf("Knight", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    sbJSON.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                                    indexKnight + 1,
                                    orderListPlayer[i].name,
                                    orderListPlayer[i].world,
                                    orderListPlayer[i].vocation,
                                    orderListPlayer[i].level,
                                    orderListPlayer[i].experience);
                    sbJSON.AppendLine();
                    sbJSONKnight.Append(sbJSON.ToString());
                    indexKnight++;
                }
                else if (orderListPlayer[i].vocation.IndexOf("Druid", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    sbJSON.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                                    indexDruid + 1,
                                    orderListPlayer[i].name,
                                    orderListPlayer[i].world,
                                    orderListPlayer[i].vocation,
                                    orderListPlayer[i].level,
                                    orderListPlayer[i].experience);
                    sbJSON.AppendLine();
                    sbJSONDruid.Append(sbJSON.ToString());
                    indexDruid++;
                }
                else if (orderListPlayer[i].vocation.IndexOf("Paladin", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    sbJSON.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                                    indexPaladin + 1,
                                    orderListPlayer[i].name,
                                    orderListPlayer[i].world,
                                    orderListPlayer[i].vocation,
                                    orderListPlayer[i].level,
                                    orderListPlayer[i].experience);
                    sbJSON.AppendLine();
                    sbJSONPaladin.Append(sbJSON.ToString());
                    indexPaladin++;
                }
                else if (orderListPlayer[i].vocation.IndexOf("Sorcerer", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    sbJSON.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                                    indexSorcerer + 1,
                                    orderListPlayer[i].name,
                                    orderListPlayer[i].world,
                                    orderListPlayer[i].vocation,
                                    orderListPlayer[i].level,
                                    orderListPlayer[i].experience);
                    sbJSON.AppendLine();
                    sbJSONSorcerer.Append(sbJSON.ToString());
                    indexSorcerer++;
                }
                if (!needsFilter)
                    sbJSONALL.Append(sbJSON.ToString());

            }


            Console.WriteLine("\nSaving results ....");

            successSaved = SaveResultToTXT(sbJSONKnight.ToString(),   _outputfile,  "KNIGHT");
            successSaved = SaveResultToTXT(sbJSONDruid.ToString(),    _outputfile,  "DRUID");
            successSaved = SaveResultToTXT(sbJSONPaladin.ToString(),  _outputfile,  "PALADIN");
            successSaved = SaveResultToTXT(sbJSONSorcerer.ToString(), _outputfile,  "SORCERER");
            successSaved = SaveResultToTXT(sbJSONALL.ToString(),      _outputfile,  "ALL");

            if (successSaved)
                Console.WriteLine("Results saved!");
            else
            {
                ConsoleError("ERROR TO SAVE FILE WITH FILTER. REPORT TO THE ADMINISTRATOR");
                Console.ReadKey();
                return;
            }
            
            #endregion

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
            {
                ConsoleError("ERROR TO EXTRACT LINES OF TABLE. REPORT TO THE ADMINISTRATOR");
                Console.ReadKey();
                return null;
            }

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

        private static bool SaveResultToTXT(string text, string path,string vocation = "")
        {
            try
            {
                if (!String.IsNullOrEmpty(vocation))
                {
                    if (!String.IsNullOrEmpty(text))
                    System.IO.File.WriteAllText(path + "_" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-") + "_" + vocation + ".txt", text);
                }
                else
                    System.IO.File.WriteAllText(path + "_" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-") + ".txt", text);

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>>>> ERROR : " + ex.Message);
                return false;
            }         
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
