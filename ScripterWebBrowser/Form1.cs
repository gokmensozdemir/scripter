﻿using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("http://localhost:4200/");
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            browser.ConsoleMessage += Browser_ConsoleMessage;
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            JObject data;

            if (e.Level == LogSeverity.Info && TryParseJson(e.Message, out data))
            {
                if (data.HasValues)
                {
                    string secret;
                    string resultCode;
                    string finishCode;

                    try
                    {
                        secret = data.GetValue("Secret").ToString();
                        resultCode = data.GetValue("ResultCode").ToString();
                        finishCode = data.GetValue("FinishCode").ToString();

                        if (string.IsNullOrEmpty(secret) || secret != "SECRET_KEY")
                        {
                            return;
                        }

                        // Get result codes here
                        Console.WriteLine(resultCode);
                        Console.WriteLine(finishCode);
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
