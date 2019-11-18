using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScripterWebBrowser.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScripterWebBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitBrowser();
        }

        public ChromiumWebBrowser browser;
        public void InitBrowser()
        {
            var requestLogin = new RequestLogin
            {
                username = "sys.test",
                password = "sys.test"
            };

            try
            {
                string token = ScripterService.Login(requestLogin);

                Cef.Initialize(new CefSettings());
                browser = new ChromiumWebBrowser("http://10.10.55.51/login?token=" + token);
                this.Controls.Add(browser);
                browser.Dock = DockStyle.Fill;

                browser.ConsoleMessage += Browser_ConsoleMessage;
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            JObject data;

            if (e.Level == LogSeverity.Info && TryParseJson(e.Message, out data))
            {
                if (data.HasValues)
                {
                    string secret;

                    try
                    {
                        secret = data.GetValue("Secret").ToString();                        

                        if (string.IsNullOrEmpty(secret) || secret != "SECRET_KEY")
                        {
                            return;
                        }

                        var resultCode = data.GetValue("ResultCode").ToString();

                        if (!string.IsNullOrEmpty(resultCode))
                        {
                            // Sonuç kodu girildi.
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private static bool TryParseJson(string strInput, out JObject msg)
        {
            msg = new JObject();

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")))
            {
                try
                {
                    msg = JObject.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
