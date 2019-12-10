﻿using CefSharp;
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
    public class CustomMenuHandler : CefSharp.IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
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
                username = "gokmen",
                password = "gokmen"
            };

            try
            {
                string token = ScripterService.Login(requestLogin);

                Cef.Initialize(new CefSettings());
                browser = new ChromiumWebBrowser("http://services.viases.cloud/login?token=" + token);
                browser.MenuHandler = new CustomMenuHandler();
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
