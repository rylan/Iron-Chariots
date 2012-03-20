/*******************************************************************************
 * Copyright (c) 2011 Rylan Cottrell. iCottrell.com.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Rylan Cottrell - initial API and implementation and/or initial documentation
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace com.iCottrell.IronChariots
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static String searchInputText_prefixIC = "http://wiki.ironchariots.org/index.php?fulltext=Search&search=";
        public ObservableCollection<SearchResults> ICSearchResults { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            LoadingScreen.Child = new PopupSplashScreen();  
            DataContext = App.ViewModel;
            App.ViewModel.PropertyChanged += new PropertyChangedEventHandler(NotifyPropertyChanged);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

       
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingScreen.IsOpen = true;
            App.ViewModel.LoadTopicData();
        }

        private void NotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TopicsRetrived")
            {
                LoadingScreen.IsOpen = false;
                PivotControl.Visibility = Visibility.Visible;
                TitlePanel.Visibility = Visibility.Visible;
                RCAd.Visibility = Visibility.Visible;
            }
            else if (e.PropertyName == "ICSearchRetrived")
            {
                spoint.Visibility = Visibility.Collapsed;
                listcanvas.Visibility = Visibility.Visible;
            }
            else if (e.PropertyName == "TOSearchRetrived")
            {
                TOCanvasMain.Visibility = Visibility.Collapsed;
                TOListCanvas.Visibility = Visibility.Visible;
            }
        }

        private void search_KeyDownTO(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchTO.Focus();
                submitTO();
            }
        }

        private void search_TapTO(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.inputBoxTO.Text == "Input...")
            {
                this.inputBoxTO.Text = "";
            }
        }

        public void submitTO()
        {
            if (this.inputBoxTO.Text != "" && DeviceNetworkInformation.IsNetworkAvailable)
            {
                String str = this.inputBoxTO.Text.Trim();
                str = str.Replace(" ", "%20");
                App.ViewModel.callTalkOriginsSearch(str);
                
            }
            else if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }
        private void search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.search.Text == "Input...")
            {
                this.search.Text = "";
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            submit();
        }

        private void searchButtonTO_Click(object sender, RoutedEventArgs e)
        {
            submitTO();
        }
        public void displaySearchResults()
        {
            spoint.Visibility = System.Windows.Visibility.Collapsed;
            DataContext = SearchResultsContainer.SearchResultItems;
            listcanvas.Visibility = System.Windows.Visibility.Visible;
            resultSet.InvalidateArrange();
            resultSet.InvalidateMeasure();
        }

        private void ICSearchResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            String href = ((TextBlock)e.OriginalSource).Tag.ToString();
            if (DeviceNetworkInformation.IsNetworkAvailable && href != "Error")
            {
                this.NavigationService.Navigate(new Uri("/WikiContentPage.xaml?href=" + href, UriKind.Relative));
            }

        }
        private void TOSearchResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            String href = ((TextBlock)e.OriginalSource).Tag.ToString();
            if (DeviceNetworkInformation.IsNetworkAvailable && href != "Error")
            {
                this.NavigationService.Navigate(new Uri("/TalkOriginContentPage.xaml?href=" + href, UriKind.Relative));
            }

        }

        private void search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchButton.Focus();
                submit();
            }
        }

        public void submit()
        {
            if (this.search.Text != "" && DeviceNetworkInformation.IsNetworkAvailable)
            {
                String str = this.search.Text.Trim();
                str = str.Replace(" ", "%20");
                App.ViewModel.callIronChariotsSearch(str);
            }
            else if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/WikiContentPage.xaml?href=/index.php?title=Arguments_for_the_existence_of_god", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void image2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/WikiContentPage.xaml?href=/index.php?title=Arguments_against_the_existence_of_god", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void image3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/WikiContentPage.xaml?href=/index.php?title=Common_objections_to_atheism_and_counter-apologetics", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void image4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/WikiContentPage.xaml?href=/index.php?title=Atheism", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void WikiApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://wiki.ironchariots.org");
            task.Show();
        }

        private void BRApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "This main page was found to have rendering issues.<br /><br /> Additional comments<br />-----------------------------";
            emailComposeTask.Subject = "Windows Phone 7 - Iron Chariots Bug Report";
            emailComposeTask.Show();
        }

        private void EmailApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "";
            emailComposeTask.Body = "Here's a website I think you will find interesting <a href=\"http://wiki.ironchariots.org\">Iron Chariots</a>";
            emailComposeTask.Subject = "Iron Chariots - The counter apologetics wiki";
            emailComposeTask.Show();
        }

        private void ForumApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://forum.ironchariots.org/");
            task.Show();
        }
        private void OpenAbout(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/IronChariotsPivot.xaml", UriKind.Relative));
        }

        private void TopicTapEvent(object sender, System.Windows.Input.GestureEventArgs e)
        {
            String topic = ((TextBlock)sender).Text;
            if (topic == "Data connection error, tap here to reload Topics")
            {
                App.ViewModel.LoadTopicData();
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/TopicPage.xaml?topic=" + topic, UriKind.Relative));
            }
        }

        private void openAbout(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void openStarredItem(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string url = (string)((TextBlock)sender).Tag;
            StarredItems si = App.ViewModel.getStarredItem(url);
            if (si != null)
            {
                Uri uri = new Uri(si.NaviPage+"?href="+Uri.EscapeDataString(si.URL), UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
        }

        private void removeStarred(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string url = (string)((TextBlock)sender).Tag;
            App.ViewModel.removeStarred(url);
        }

        private void OpenRC(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.responsiblecharity.org/donate");
            task.Show();
        }
    }
}