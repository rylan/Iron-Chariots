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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;

namespace com.iCottrell.IronChariots
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private String Url_TOTopics = "http://www.talkorigins.org/indexcc/list.html";
        public const string STARREDITEMS = "com.iCottrell.IronChariots.StarredItems";
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public ObservableCollection<StarredItems> ICTOStarred { get; private set; }
        public ObservableCollection<IronChariotsSearchViewModel> ICSearchResults { get; private set; }
        public ObservableCollection<TalkOriginsSearchViewModel> TOSearchResults { get; private set; }
        
        public ObservableCollection<TalkOriginsTopicCategoryViewModel> TOCategoryTopics { get; private set; }

        public MainViewModel()
        {
            this.ICTOStarred = new ObservableCollection<StarredItems>();
            this.ICSearchResults = new ObservableCollection<IronChariotsSearchViewModel>();
            this.TOSearchResults = new ObservableCollection<TalkOriginsSearchViewModel>();
            this.TOCategoryTopics = new ObservableCollection<TalkOriginsTopicCategoryViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (propertyName == "StarredEvent")
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if (!settings.Contains(STARREDITEMS))
                {
                    settings.Add(STARREDITEMS, this.ICTOStarred);
                }
                else
                {
                    settings[STARREDITEMS] = this.ICTOStarred;
                }
                settings.Save();
            }
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        public void LoadTopicData()
        {
            TOCategoryTopics.Clear();
            if (this.ICTOStarred.Count == 0)
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(STARREDITEMS))
                {

                    ObservableCollection<StarredItems> list = (ObservableCollection<StarredItems>)settings[STARREDITEMS];
                    if (list != null && list.Count > 0)
                    {

                        foreach (StarredItems si in list)
                        {
                            this.ICTOStarred.Add(si);
                        }
                        NotifyPropertyChanged("StarredItemsLoaded");
                    }
                }
            }
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                HtmlWeb webGet = new HtmlWeb();
                webGet.LoadCompleted += parse_TopicIndex;
                webGet.LoadAsync(Url_TOTopics);
            }
            else
            {
                TalkOriginsTopicCategoryViewModel t = new TalkOriginsTopicCategoryViewModel();
                t.Topic = "Data connection error, tap here to reload Topics";
                TOCategoryTopics.Add(t);
                NotifyPropertyChanged("TopicsRetrived");
            }

        }

        private void parse_TopicIndex(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document != null)
            {
                IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();

                foreach (HtmlNode node in hnc)
                {
                    if (node.Name.ToLower() == "h2")
                    {
                        TalkOriginsTopicCategoryViewModel t = new TalkOriginsTopicCategoryViewModel();

                        foreach (HtmlNode i in node.DescendantNodes().ToList())
                        {
                            if (i.Name.ToLower() == "a" && i.Attributes.Count > 0)
                            {
                                if (i.Attributes[0].Name.ToLower() == "name")
                                {
                                    t.CategoryShort = i.Attributes[0].Value;
                                }
                            }
                            else if (i.Name.ToLower() == "#text" && i.InnerText != t.CategoryShort)
                            {

                                t.Topic = i.InnerText.Replace(":", "").Trim();

                            }
                        }
                        if (node.NextSibling != null && node.NextSibling.Name.ToLower() == "ul")
                        {
                            foreach (HtmlNode ul in node.NextSibling.DescendantNodes().ToList())
                            {
                                if (ul.Name.ToLower() == "a" && ul.Attributes.Count > 0 && ul.Attributes[0].Name.ToLower() == "href")
                                {
                                    TalkOriginsTopicViewModel totvm = new TalkOriginsTopicViewModel();
                                    totvm.Title = ul.InnerText;
                                    totvm.Href = ul.Attributes[0].Value;
                                    t.TOTopics.Add(totvm);
                                }
                            }
                        }
                        TOCategoryTopics.Add(t);
                    }
                }
            }
            this.IsDataLoaded = true;
            NotifyPropertyChanged("TopicsRetrived");
        }

        

        public void callIronChariots(String url)
        {
            HtmlWeb webGet = new HtmlWeb();
            webGet.LoadCompleted += parse_DownloadICSearchResultsCompleted;
            webGet.LoadAsync(url);
        }

        public void parse_DownloadICSearchResultsCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            ICSearchResults.Clear();
            IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();

            foreach (HtmlNode htmlNode in hnc)
            {
                if (htmlNode.Name.ToLower() == "div")
                {
                    for (int att = htmlNode.Attributes.Count - 1; att >= 0; att--)
                    {
                        if (htmlNode.Attributes[att].Name.ToLower() == "id" && htmlNode.Attributes[att].Value == "bodyContent")
                        {
                            IList<HtmlNode> rs = htmlNode.DescendantNodes().ToList();
                            String category = "";
                            foreach (HtmlNode j in rs)
                            {
                                if (j.Name.ToLower() == "h2")
                                {
                                    category = j.InnerText;
                                }
                                if (j.Name.ToLower() == "ol")
                                {

                                    IList<HtmlNode> li = j.DescendantNodes().ToList();
                                    foreach (HtmlNode k in li)
                                    {
                                        IronChariotsSearchViewModel sr = new IronChariotsSearchViewModel();
                                        if (k.Name.ToLower() == "li")
                                        {
                                            IList<HtmlNode> innerNode = k.DescendantNodes().ToList();
                                            foreach (HtmlNode rsk in innerNode)
                                            {
                                                if (rsk.Name.ToLower() == "a")
                                                {
                                                    foreach (HtmlAttribute at in rsk.Attributes.ToList())
                                                    {
                                                        if (at.Name.ToLower() == "href")
                                                        {
                                                            sr.Href = at.Value;
                                                        }
                                                        else if (at.Name.ToLower() == "title")
                                                        {
                                                            sr.Title = at.Value;
                                                        }
                                                    }
                                                }
                                                if (rsk.Name.ToLower() == "small")
                                                {
                                                    String str = rsk.InnerText.Trim();
                                                    str = str.Replace("&quot;", "\"");
                                                    str = str.Replace("[", "");
                                                    str = str.Replace("]", "");
                                                    sr.Description += str;
                                                    sr.Description += " ";
                                                }
                                            }
                                            sr.Category = category;
                                            ICSearchResults.Add(sr);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            NotifyPropertyChanged("ICSearchRetrived");
        }

        public void callTalkOriginsSearch(String query)
        {
            string requestString = "http://api.bing.net/xml.aspx?"
            // request fields 
            + "AppId=6D8CDFC50DC16E97FC821BF4EBA319FC069E58E4" 
            + "&Query=" + query + "%20site%3Awww.talkorigins.org"
            + "&Sources=Web"
            + "&Web.Count=25"
            + "&Version=2.0"
            + "&Market=en-us";

            WebClient wc = new WebClient();
            wc.OpenReadAsync(new Uri(requestString));
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(parseTalkOriginsSearch);
        }

        public void callIronChariotsSearch(String query)
        {
            string requestString = "http://api.bing.net/xml.aspx?"
                // request fields 
            + "AppId=6D8CDFC50DC16E97FC821BF4EBA319FC069E58E4"
            + "&Query=" + query + "%20site%3Awiki.ironchariots.org"
            + "&Sources=Web"
            + "&Web.Count=25"
            + "&Version=2.0"
            + "&Market=en-us";

            WebClient wc = new WebClient();
            wc.OpenReadAsync(new Uri(requestString));
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(parseIronChariotsSearch);
        }

        public void parseIronChariotsSearch(Object sender, OpenReadCompletedEventArgs e)
        {
            XElement resultXml;
            ICSearchResults.Clear();
            if (e.Error != null)
            {
                IronChariotsSearchViewModel ics = new IronChariotsSearchViewModel();
                ics.Title = "Error";
                ics.Description = "An error occured trying to retrieve search results, please try again.";
                ics.Href = "Error";
                ICSearchResults.Add(ics);
                NotifyPropertyChanged("ICSearchRetrived");
                return;
            }
            else
            {
                
                XNamespace web = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web";
                try
                {
                    resultXml = XElement.Load(e.Result);
                    // Search for the WebResult node and create a SearchResults object for each one.

                    foreach (XElement result in resultXml.Descendants(web + "WebResult"))
                    {
                        IronChariotsSearchViewModel ics = new IronChariotsSearchViewModel();
                        ics.Title = result.Element(web + "Title").Value.Trim();
                        if (ics.Title.Contains("Image:") || ics.Title.Contains("Talk:") || ics.Title.Contains("Template:") )
                        {
                            continue;
                        }
                        ics.Description = result.Element(web + "Description").Value.Trim();
                        ics.Href = result.Element(web + "Url").Value;
                        ICSearchResults.Add(ics);
                    }
                }
                catch (System.Xml.XmlException ex)
                {

                }
                NotifyPropertyChanged("ICSearchRetrived");
            }
        }

        public bool isStarred(String url)
        {
            foreach (StarredItems si in this.ICTOStarred)
            {
                if (url == si.URL)
                {
                    return true;
                }
            }
            return false;
        }

        public void parseTalkOriginsSearch(Object sender, OpenReadCompletedEventArgs e)
        {
            XElement resultXml;
            TOSearchResults.Clear();

            if (e.Error != null)
            {
                TalkOriginsSearchViewModel tos = new TalkOriginsSearchViewModel();
                tos.Title = "Error";
                tos.Description = "An error occured trying to retrieve search results, please try again.";
                tos.Href = "Error";
                NotifyPropertyChanged("TOSearchRetrived");
                return;
            }
            else
            {
                
                XNamespace web = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web";
                try
                {
                    resultXml = XElement.Load(e.Result);
                    
                    foreach(XElement result in resultXml.Descendants(web + "WebResult")){
                      TalkOriginsSearchViewModel tos = new TalkOriginsSearchViewModel();
                      tos.Title = result.Element(web + "Title").Value.Trim();
                      tos.Description = result.Element(web + "Description").Value.Trim();
                      tos.Href = result.Element(web + "Url").Value;
                      TOSearchResults.Add(tos);
                    }
                }
                catch (System.Xml.XmlException ex)
                {
                    
                }
                NotifyPropertyChanged("TOSearchRetrived");
            }
        }

        public void addStarred(string url, string title, string naviPage)
        {
            StarredItems si = new StarredItems();
            si.URL = url;
            si.Title = title;
            si.NaviPage = naviPage;
            this.ICTOStarred.Add(si);
            NotifyPropertyChanged("StarredEvent");
        }

        public StarredItems getStarredItem(string url)
        {
            foreach (StarredItems si in this.ICTOStarred)
            {
                if (url == si.URL)
                {
                    return si;
                }
            }
            return null;
        }
        public void removeStarred(string url)
        {
            int pos = -1;
            for (int i = 0; i < this.ICTOStarred.Count; i++)
            {
                if (url == this.ICTOStarred[i].URL)
                {
                    pos = i;
                    break;
                }
            }
            if (pos != -1)
            {
                this.ICTOStarred.RemoveAt(pos);
            }
            NotifyPropertyChanged("StarredEvent");
        }
    }
}
