/**
* Copyright 2015 IBM Corp.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
 
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using System.Text;
using IBM.Worklight;
using Newtonsoft.Json.Linq;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StarterApplicationWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBM.Worklight.WLClient wlClient = null;
        public static MainPage _this;
        public JToken mainPageJSON = null;

        public MainPage()
        {
            this.InitializeComponent();
            _this = this;
            try
            {
                wlClient = WLClient.getInstance();
                wlClient.connect(new ResponseListener());

                Uri targetUri = new Uri("http://www.engadget.com");
                MainPage._this.WebViewConsole.Navigate(targetUri);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void ItemFeedClicked(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                String itemUri = ((JObject)MainPage._this.mainPageJSON[MainPage._this.FeedListBox.SelectedIndex]).GetValue("link").ToString();
                Uri targetUri = new Uri(itemUri);
                MainPage._this.WebViewConsole.Navigate(targetUri);
                MainPage._this.WebViewConsole.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public static void getFeeds()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.FeedListBox.Items.Clear();
                });
            
           WLResourceRequest adapter = new WLResourceRequest("/adapters/StarterApplicationAdapter/getEngadgetFeeds", "GET");

           Object[] parameters = { 0 };
           InvokeProcedureListener listener = new InvokeProcedureListener(_this);
           adapter.send(listener);
            
        }

        public class InvokeProcedureListener : WLResponseListener
        {
            MainPage page;

            public InvokeProcedureListener(MainPage mainPage)
            {
                page = mainPage;
            }

            public void onSuccess(WLResponse response)
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    StringBuilder result = new StringBuilder();
                    JToken feed = response.getResponseJSON().GetValue("items");
                    MainPage._this.mainPageJSON = feed;
                    for (int i = 0; i < feed.Count(); i++)
                    {
                        MainPage._this.FeedListBox.Items.Add(((JObject)feed[i]).GetValue("title").ToString());
                    }
                });
            }

            public void onFailure(WLFailResponse failResp)
            {
                Debug.WriteLine(failResp.getErrorMsg());
            }

        }

        public class ResponseListener : WLResponseListener
        {
            void WLResponseListener.onFailure(WLFailResponse response)
            {
                Debug.WriteLine("Failed connecting to server" + response.getErrorMsg());
            }

            async void WLResponseListener.onSuccess(WLResponse response)
            {
                MainPage.getFeeds();
            }
        }

        private void AboutTab_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.AboutBlock.Visibility = Visibility.Visible;
            MainPage._this.FeedListBox.Visibility = Visibility.Collapsed;
            MainPage._this.WebViewConsole.Visibility = Visibility.Collapsed;
            MainPage._this.AboutTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
        }

        private void FeedsTab_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.AboutBlock.Visibility = Visibility.Collapsed;
            MainPage._this.FeedListBox.Visibility = Visibility.Visible;
            MainPage._this.WebViewConsole.Visibility = Visibility.Visible;

        }
    }
}
