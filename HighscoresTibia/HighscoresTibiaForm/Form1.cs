using HighscoresTibiaForm.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebUtilsLib;

namespace HighscoresTibiaForm
{
    public partial class MainForm : Form
    {

        #region ** Private Attributes **
        private static string _outputfile;
        private static int _numberPlayersTopExpExport;
        private static string _vocationPlayers;
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

        public MainForm()
        {
            InitializeComponent();

            Configure();

            // Setup HTTP headers
            SetupHeaders(_request);

        }

        private void SendDataToolStrip_Click(object sender, EventArgs e)
        {
            //crawlerTimer.Interval = 1000;
            //crawlerTimer.Enabled = true;
            consoleView.Columns.Add(new ColumnHeader().Text = "Teste");
            consoleView.Columns[0].Width = this.consoleView.Width - 4;
            consoleView.HeaderStyle = ColumnHeaderStyle.None;



            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            StringBuilder parameters = new StringBuilder();
            HtmlNodeCollection nodes;
            List<Player> listPlayerFilter = new List<Player>();

            // Used to build the string JSON.
            StringBuilder sbJSON = new StringBuilder();

            string html = null;
            string mainURL = null;
            int pageNumber = 1;
            int retries = 0;
            bool successSaved = false;
            bool needsFilter = true;
            string[] vocations;
            int count;

            // Create new stopwatch.
            Stopwatch watch = new Stopwatch();

            List<String> worlds = new List<string>();

            Dictionary<string, string> dic = new Dictionary<string, string>();

            _outputfile = Path.Combine(_path, "CRAWLER_RESULTS");

            CheckedListBox.CheckedItemCollection vocationsSelecteds = mainCheckBoxList.CheckedItems;

            // Begin timing.
            watch.Start();
           //crawlerTimer.Start();

            Console(consoleView, "Iniatilize the connection parameters");
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

                if (++retries > 10)
                {
                    Console(consoleView, "ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                    return;
                }
            }

            htmlDoc.LoadHtml(html);

            //Extract all Worlds
            nodes = htmlDoc.DocumentNode.SelectNodes("//table[@style='width:100%;']/tr[1]//option");

            if ((nodes == null) || nodes.Count == 0)
            {
                Console(consoleView, "ERROR TO GET NODE \"ALL WORLDS\". REPORT TO THE ADMINISTRATOR ");
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
                count = 1;
                retries = 0;
                Console(consoleView, "World > " + world + " < Processing...");

                mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=" + world + "&list=experience&profession=0&currentpage=" + count;

                html = String.Empty;
                retries = 0;

                while (String.IsNullOrEmpty(html))
                {
                    html = _request.Get(mainURL);
                    System.Threading.Thread.Sleep(500);

                    if (++retries > 10)
                    {
                        Console(consoleView, "ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                        return;
                    }
                }
                htmlDoc.LoadHtml(html);

                // Extract the number of pages
                nodes = htmlDoc.DocumentNode.SelectNodes("//small/div//a");

                if ((nodes == null) || (nodes.Count == 0))
                {
                    Console(consoleView, "ERROR TO CAPTURE NUMBER OF PAGES. REPORT TO THE ADMINISTRATOR");
                    return;
                }

                pageNumber = nodes.Count + 1;

                do
                {
                    mainURL = "https://secure.tibia.com/community/?subtopic=highscores&world=" + world + "&list=experience&profession=0&currentpage=" + count;

                    html = String.Empty;
                    retries = 0;

                    while (String.IsNullOrEmpty(html))
                    {
                        html = _request.Get(mainURL);
                        System.Threading.Thread.Sleep(500);

                        if (++retries > 10)
                        {
                            Console(consoleView, "ERROR TO GET INFORMATION THROUGH THE REQUEST OF THE MAINURL. REPORT TO THE ADMINISTRATOR ");
                            return;
                        }
                    }


                    htmlDoc.LoadHtml(html);

                    nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='TableContentContainer']//table//tr[contains(@style,'background-color')]");

                    if ((nodes == null) || (nodes.Count == 0))
                    {
                        Console(consoleView, "ERROR TO CAPTURE TABLE. REPORT TO THE ADMINISTRATOR");
                        return;
                    }

                    foreach (HtmlNode node in nodes)
                    {
                        dic = ProcessNewPlayer(consoleView, node, vocationsSelecteds, world);

                        if (dic == null)
                            return;

                        if (dic["world"].Equals("0"))
                            continue;

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

                Console(consoleView, "World > " + world + " < Processed!\n");
            }
            #endregion

            watch.Stop();


        }

        private static Dictionary<string, string> ProcessNewPlayer(ListView consoleView, HtmlNode node, CheckedListBox.CheckedItemCollection vocations, string world)
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
                Console(consoleView, "ERROR TO EXTRACT LINES OF TABLE. REPORT TO THE ADMINISTRATOR");
                return null;
            }

            if (nodes.Count == 5)
            {

                aux = nodes[2].InnerText.Trim().Replace(",", "");

                if (!String.IsNullOrWhiteSpace(aux))
                {          
                    for (int y = 0; y < vocations.Count; y++)
                        if (aux.IndexOf(vocations[y].ToString()) > -1)
                        {
                            dic[dic.ElementAt(0).Key] = world;
                            for (int i = 0; i < nodes.Count;i++)
                            {
                                dic[dic.ElementAt(i+1).Key] = nodes[i].InnerText.Trim().Replace(",", "");
                            }
                            break;
                        }
                }

                return dic;
            }
            return null;
        }

        public static void Configure()
        {
            _request = new WebRequests();
            _request.Encoding = "ISO-8859-1";
        }

        public static void SetupHeaders(WebRequests request)
        {
            request.Host = "secure.tibia.com";
            request.KeepAlive = true;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

            request.Headers.Add("Accept-Encoding: gzip, deflate, sdch, br");
            request.Headers.Add("Accept-Language: pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        }

        public static void Console(ListView console, string message)
        {
            console.Items.Add(message);
        }

        public static bool CheckEnableAllVocations(CheckedListBox.CheckedItemCollection checkboxesVocations)
        {
            return checkboxesVocations.Count == 4 ? true : false;
        }

        private void crawlerTimer_Tick(object sender, EventArgs e)
        {
            int time;
            time = Convert.ToInt32(processTimer.Text.Substring(processTimer.Text.IndexOf(":") + 1));

        }
    }
}
