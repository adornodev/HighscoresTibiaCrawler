using System;
using System.Collections.Generic;
using WebUtilsLib;

namespace HighscoresTibiaForm.utils
{
    public class WebUtilsDriver : IDisposable
    {
        ///////////////////////////////////////////////////////////////////////
        //                           Fields                                  //
        ///////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Web request
        /// </summary>
        public WebRequests webrequest;

        /// <summary>
        /// Web request constants
        /// </summary>
        private const string ACCEPT             = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private const string ACCEPT_ENCODING    = "Accept-Encoding: gzip,deflate,sdch";
        private const string ACCEPT_LANGUAGE    = "Accept-Language: pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4";
        private const string ACCEPT_CHARSET     = "Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3";
        private const string COVERT_USERAGENT   = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; EIE11;ENUSMSN; rv:11.0) like Gecko";

        private const int MaxPageSize = 5 * (1024 * 1024);


        ///////////////////////////////////////////////////////////////////////
        //                    Methods & Functions                            //
        ///////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Class constructor
        /// </summary>
        public WebUtilsDriver()
        {
            InitializeWebRequest(out webrequest);
        }

        /// <summary>
        /// Class disposer
        /// </summary>
        public void Dispose()
        {
            webrequest = null;
        }

        /// <summary>
        /// Initializes the "webrequest" object
        /// </summary>
        private void InitializeWebRequest(out WebRequests webreq)
        {
            // Create new
            webreq = new WebRequests();

            // Request options
            webreq.KeepAlive = false;
            webreq.Accept = ACCEPT;
            webreq.ContentType = "";
            webreq.AllowAutoRedirect = true;

            // Request encoding
            webreq.Encoding = "ISO-8859-1";
            webreq.EncodingDetection = WebRequests.CharsetDetection.MozillaCharsetDetection;

            // Request max response size
            webreq.MaxResponseSize = MaxPageSize;

            // Request headers
            webreq.UserAgent = COVERT_USERAGENT;
            webreq.Headers.Add(ACCEPT_ENCODING);
            webreq.Headers.Add(ACCEPT_LANGUAGE);
            webreq.Headers.Add(ACCEPT_CHARSET);
        }

        /// <summary>
        /// Navigates to the url
        /// </summary>
        public bool Navigate(string url, int timeoutseconds, out string finalurl, out string pagesource, Dictionary<string, string> docproperties, out string errormessage)
        {
            // Initialize results
            pagesource = string.Empty;
            finalurl = string.Empty;
            errormessage = string.Empty;

            // Capture the page
            try
            {
                // Set request timeout
                webrequest.ReadWriteTimeout = timeoutseconds * 1000;
                webrequest.Timeout = webrequest.ReadWriteTimeout + (webrequest.ReadWriteTimeout / 2);

                // Goto the page
                pagesource = webrequest.Get(url);
                finalurl = url;
                errormessage = webrequest.Error;
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
            }

            return !string.IsNullOrWhiteSpace(errormessage);
        }
    }
}
