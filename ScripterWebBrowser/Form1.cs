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

            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            tabControl1 = new TabControl();
            this.Controls.Add(this.tabControl1);

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitBrowser();
        }

        public ChromiumWebBrowser browser;
        public TabControl tabControl1;
        public void InitBrowser()
        {
            var requestLogin = new RequestLogin
            {
                username = "gokmen",
                password = "gokmen"
            };

            try
            {
                string token = ScripterService.Login(requestLogin);

                Cef.Initialize(new CefSettings());
                browser = new ChromiumWebBrowser("http://services.viases.cloud/login?token=" + token);
                browser.MenuHandler = new CustomMenuHandler();                
                browser.Dock = DockStyle.Fill;
                browser.ConsoleMessage += Browser_ConsoleMessage;

                string title = "Webhelp Karşılama";
                TabPage myTabPage = new TabPage(title);
                myTabPage.Controls.Add(browser);
                tabControl1.TabPages.Add(myTabPage);
                tabControl1.Dock = DockStyle.Fill;                
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
                        secret = data.GetValue("secret").ToString();                        

                        if (string.IsNullOrEmpty(secret) || secret != "SECRET_KEY")
                        {
                            return;
                        }

                        string type = data.GetValue("type").ToString();

                        if (type == "RESULT_CODE")
                        {
                            var resultCode = data.GetValue("ResultCode").ToString();

                            if (!string.IsNullOrEmpty(resultCode))
                            {
                                // Sonuç kodu girildi.
                            }
                        }
                        else if(type == "CRM_PAGE")
                        {
                            string title = data.GetValue("title").ToString();
                            string url = data.GetValue("url").ToString();
                            string id = data.GetValue("id").ToString();

                            foreach (TabPage tabPage in this.tabControl1.TabPages)
                            {
                                if (tabPage.Name == id)
                                {
                                    return;
                                }
                            }

                            ChromiumWebBrowser crmPage = new ChromiumWebBrowser(url);
                            crmPage.MenuHandler = new CustomMenuHandler();
                            crmPage.Dock = DockStyle.Fill;

                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    TabPage myTabPage = new TabPage(title);
                                    myTabPage.Name = id;
                                    myTabPage.Controls.Add(crmPage);
                                    this.tabControl1.TabPages.Add(myTabPage);
                                    this.tabControl1.Dock = DockStyle.Fill;
                                });
                            }
                        }
                    }
                    catch (Exception ex)
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
