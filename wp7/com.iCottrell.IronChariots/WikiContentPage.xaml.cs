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
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Net.NetworkInformation;


namespace com.iCottrell.IronChariots
{
    public partial class WikiContentPage : PhoneApplicationPage
    {

        private Dictionary<String, String> hyperLinkDic;
        public String Href { get; set; }
        public String CurrentPage { get; set; }
        public static String IronUrl = "http://wiki.ironchariots.org";
        public WikiContentPage()
        {
            InitializeComponent();
            CurrentPage = "";
            hyperLinkDic = new Dictionary<String, String>();
        }

        public void loadPage(String url)
        {
            if (!url.Contains("http://wiki.ironchariots.org"))
            {
                CurrentPage = IronUrl + url;
            }
            else
            {
                CurrentPage = url;
            }

            if (App.ViewModel.isStarred(CurrentPage))
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.rest.png", UriKind.Relative);
            }
            else
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.addto.rest.png", UriKind.Relative);
            }

            if (CurrentPage != "")
            {
                progress.Visibility = Visibility.Visible;

                HtmlWeb webGet = new HtmlWeb();
                webGet.LoadCompleted += parse_DownloadWikiPageCompleted;
                webGet.LoadAsync(CurrentPage, Encoding.UTF8);
            }
        }

        public void parse_DownloadWikiPageCompleted(Object sender, HtmlDocumentLoadCompleted e)
        {
            if (e != null && e.Document != null && e.Document.DocumentNode != null)
            {
                IList<Block> pageBody = new List<Block>(); 
                IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();
                
                Paragraph paragraph = new Paragraph();
                Boolean flag = false;
                Boolean flag_table = false;
                foreach (HtmlNode htmlNode in hnc)
                {
                    try
                    {
                        if (flag_table && htmlNode.PreviousSibling != null && htmlNode.PreviousSibling.Name.ToLower() == "table")
                        {
                            flag_table = false;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.Error.Write(err.StackTrace);
                    }
                    if (htmlNode.Name.ToLower() == "div")
                    {
                        foreach (HtmlAttribute att in htmlNode.Attributes)
                        {
                            if (att.Name.ToLower() == "id" && att.Value == "bodyContent")
                            {
                                flag = true;
                                break;
                            }
                            else if (att.Name.ToLower() == "class" && att.Value == "printfooter")
                            {
                                flag = false;
                                goto BreakParseLoop;
                            }
                        }
                    }
                    else if (htmlNode.Name.ToLower() == "table")
                    {
                        flag_table = true;
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "h2")
                    {
                        
                            if (!(htmlNode.ParentNode.Name.ToLower() == "div" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Value == "toctitle"))
                            {
                                if (paragraph.Inlines.Count > 0)
                                {
                                    pageBody.Add(paragraph);
                                    paragraph = new Paragraph();
                                }
                                Bold h2 = new Bold();
                                h2.Inlines.Add(htmlNode.InnerText);
                                Paragraph para = new Paragraph();
                                para.Inlines.Add(h2);
                                para.Inlines.Add(new LineBreak());
                                pageBody.Add(para);
                            }  
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "h3")
                    {
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        Bold h2 = new Bold();
                        h2.Inlines.Add(htmlNode.InnerText);
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(h2);
                        para.Inlines.Add(new LineBreak());
                        
                        pageBody.Add(para);
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "#text")
                    {
                        
                            if (htmlNode.ParentNode.Name.ToLower() != "a" && htmlNode.ParentNode.Name.ToLower() != "b" && htmlNode.ParentNode.Name.ToLower() != "p"
                                    && htmlNode.ParentNode.Name.ToLower() != "i"
                                    && htmlNode.ParentNode.Name.ToLower() != "ul" && htmlNode.ParentNode.Name.ToLower() != "li" && htmlNode.ParentNode.Name.ToLower() != "h2"
                                    && htmlNode.ParentNode.Name.ToLower() != "h3" && htmlNode.ParentNode.Name.ToLower() != "td" && htmlNode.ParentNode.Name.ToLower() != "tr"
                                    && htmlNode.ParentNode.Name.ToLower() != "table" && !(htmlNode.ParentNode.Name.ToLower() == "div" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Value == "jump-to-nav")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "td") && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "tr")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "table") && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "li")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "ul")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "i" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "p")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "div" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Value == "editsection")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Name == "class" && htmlNode.ParentNode.Attributes[0].Value == "toctext")
                                    && !(htmlNode.ParentNode.Name.ToLower() == "span" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Name == "class" && htmlNode.ParentNode.Attributes[0].Value == "tocnumber")
                                    && htmlNode.ParentNode.Name.ToLower() != "script" && htmlNode.ParentNode.Name.ToLower() != "#comment"
                                    && htmlNode.ParentNode.Name.ToLower() != "dd" && htmlNode.ParentNode.Name.ToLower() != "dl")
                            {
                                String str = ConvertWhitespacesToSingleSpaces(htmlNode.InnerText);
                                if (str != " ")
                                {
                                    Run run = new Run();
                                    run.Text = str;
                                    paragraph.Inlines.Add(run);
                                }
                            }
                        
         
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "a")
                    {
                        
                            if (htmlNode.ParentNode.Name != "p" && !(htmlNode.ParentNode.Name.ToLower() == "div" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Value == "jump-to-nav") 
                                && htmlNode.ParentNode.Name.ToLower() != "li" && htmlNode.ParentNode.Name.ToLower() != "ul"
                                && htmlNode.ParentNode.Name.ToLower() != "dd" && htmlNode.ParentNode.Name.ToLower() != "dl"
                                && !(htmlNode.ParentNode.Name.ToLower() == "div" && htmlNode.ParentNode.Attributes.Count > 0 && htmlNode.ParentNode.Attributes[0].Value == "editsection")
                                && !(htmlNode.ParentNode.Name.ToLower() == "i" && htmlNode.ParentNode.ParentNode.Name.ToLower() == "p"))
                            {
                                Boolean image_flag = false;
                                Boolean flag_local = false;
                                Hyperlink hl = new Hyperlink();
                               
                                
                                hl.Inlines.Add(htmlNode.InnerText);
                                foreach (HtmlAttribute att1 in htmlNode.Attributes)
                                {
                                    if (att1.Name.ToLower() == "href")
                                    {
                                        try
                                        {
                                            if (att1.Value.ToCharArray()[0] != '#')
                                            {
                                                hl.NavigateUri = new Uri("/WikiContentPage.xaml?href="+att1.Value, UriKind.Relative);
                                            }
                                            else
                                            {
                                                flag_local = true;
                                            }
                                        }
                                        catch (Exception err)
                                        {
                                            image_flag = true;
                                        }
                                    }
                                    else if (att1.Name.ToLower() == "class" && att1.Value == "image")
                                    {
                                        image_flag = true;
                                    }
                                }
                                if (!image_flag && !flag_local)
                                {
                                   // hl.Click += new RoutedEventHandler(hyperlink_Click); 
                                    paragraph.Inlines.Add(hl);
                                }
                                else if (flag_local)
                                {
                                    Run r = new Run();
                                    r.Text = htmlNode.InnerText;
                                    paragraph.Inlines.Add(r);
                                }
                            }
                      
                       
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "p")
                    {
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }

                        Paragraph np = new Paragraph();
                       
                        foreach (HtmlNode pc in htmlNode.DescendantNodes().ToList())
                        {
                            if (pc.Name.ToLower() == "a")
                            {
                                if (pc.ParentNode.Name.ToLower() != "i" && pc.ParentNode.Name.ToLower() != "b")
                                {
                                    Boolean image_flag = false;
                                    Boolean flag_local = false;
                                    Hyperlink hl = new Hyperlink();

                                    hl.Inlines.Add(pc.InnerText);

                                    foreach (HtmlAttribute att1 in pc.Attributes)
                                    {
                                        if (att1.Name.ToLower() == "href")
                                        {
                                            try
                                            {
                                                if (att1.Value.ToCharArray()[0] != '#')
                                                {
                                                    hl.NavigateUri = new Uri("/WikiContentPage.xaml?href=" + att1.Value, UriKind.Relative);
                                                }
                                                else
                                                {
                                                    flag_local = true;
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                image_flag = true;
                                            }
                                        }
                                        else if (att1.Name.ToLower() == "class" && att1.Value == "image")
                                        {
                                            image_flag = true;
                                        }
                                    }
                                    if (!image_flag && !flag_local)
                                    {
                                        np.Inlines.Add(hl);
                                    }
                                    else if (flag_local)
                                    {
                                        Run r = new Run();
                                        r.Text = pc.InnerText;
                                        np.Inlines.Add(r);
                                    }
                                }
                                }
                                else if (pc.Name.ToLower() == "#text")
                                {
                                    if (pc.ParentNode.Name.ToLower() != "a" && pc.ParentNode.Name.ToLower() != "b" && pc.ParentNode.Name.ToLower() != "i")
                                    {
                                        String str = ConvertWhitespacesToSingleSpaces(pc.InnerText);
                                        if (str != " ")
                                        {
                                            Run run = new Run();
                                            run.Text = pc.InnerText;
                                            np.Inlines.Add(run);
                                        }
                                    }
                                }
                                else if (pc.Name.ToLower() == "b")
                                {
                                   Bold b = new Bold();
                                   foreach(HtmlNode n in pc.DescendantNodes().ToList()){
                                        if (n.Name.ToLower() == "#text" && n.ParentNode.Name.ToLower() != "a"){
                                            b.Inlines.Add(n.InnerText);
                                        }
                                        else if (n.Name.ToLower() == "a")
                                        {
                                            Boolean image_flag = false;
                                            Boolean flag_local = false;
                                            Hyperlink hl = new Hyperlink();

                                            hl.Inlines.Add(n.InnerText);

                                            foreach (HtmlAttribute att1 in n.Attributes)
                                            {
                                                if (att1.Name.ToLower() == "href")
                                                {
                                                    try
                                                    {
                                                        if (att1.Value.ToCharArray()[0] != '#')
                                                        {
                                                            hl.NavigateUri = new Uri("/WikiContentPage.xaml?href=" + att1.Value, UriKind.Relative);
                                                        }
                                                        else
                                                        {
                                                            flag_local = true;
                                                        }
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        image_flag = true;
                                                    }
                                                }
                                                else if (att1.Name.ToLower() == "class" && att1.Value == "image")
                                                {
                                                    image_flag = true;
                                                }
                                            }
                                            if (!image_flag && !flag_local)
                                            {
                                                b.Inlines.Add(hl);
                                            }
                                            else
                                            {
                                                b.Inlines.Add(n.InnerText);
                                            }
                                        }
                                    }
                                    
                                    np.Inlines.Add(b);
                                }
                                else if (pc.Name.ToLower() == "i")
                                {
                                    Italic i = new Italic();
                                    foreach (HtmlNode n in pc.DescendantNodes().ToList())
                                    {
                                        if (n.Name.ToLower() == "#text" && n.ParentNode.Name.ToLower() != "a")
                                        {
                                            i.Inlines.Add(n.InnerText);
                                        }
                                        else if (n.Name.ToLower() == "a")
                                        {
                                            Boolean image_flag = false;
                                            Boolean flag_local = false;
                                            Hyperlink hl = new Hyperlink();

                                            hl.Inlines.Add(n.InnerText);

                                            foreach (HtmlAttribute att1 in n.Attributes)
                                            {
                                                if (att1.Name.ToLower() == "href")
                                                {
                                                    try
                                                    {
                                                        if (att1.Value.ToCharArray()[0] != '#')
                                                        {
                                                            hl.NavigateUri = new Uri("/WikiContentPage.xaml?href=" + att1.Value, UriKind.Relative);
                                                        }
                                                        else
                                                        {
                                                            flag_local = true;
                                                        }
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        image_flag = true;
                                                    }
                                                }
                                                else if (att1.Name.ToLower() == "class" && att1.Value == "image")
                                                {
                                                    image_flag = true;
                                                }
                                            }
                                            if (!image_flag && !flag_local)
                                            {
                                                i.Inlines.Add(hl);
                                            }
                                            else
                                            {
                                                i.Inlines.Add(n.InnerText);
                                            }
                                        }
                                    }

                                    np.Inlines.Add(i);
                                }
                            }
                           
                            pageBody.Add(np);
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "dl")
                    {
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                         Paragraph np = new Paragraph();
                         np.TextAlignment = TextAlignment.Justify;
                         
                            foreach (HtmlNode a in htmlNode.DescendantNodes().ToList())
                            {
                                if (a.Name.ToLower() == "a")
                                {
                                    Boolean err_flag = false;
                                    Hyperlink hl = new Hyperlink();
                                    
                                    hl.Inlines.Add(a.InnerText);
                                     
                                    foreach (HtmlAttribute att1 in a.Attributes)
                                    {
                                        if (att1.Name.ToLower() == "href")
                                        {
                                            try
                                            {
                                                if (att1.Value.ToCharArray()[0] != '#')
                                                {
                                                    hl.NavigateUri = new Uri("/WikiContentPage.xaml?href="+att1.Value, UriKind.Relative);
                                                }
                                                else
                                                {
                                                    err_flag = true;
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                err_flag = true;
                                            }
                                            break;
                                        }
                                    }
                                    if (!err_flag)
                                    {
                                        np.Inlines.Add(hl);
                                    }
                                    else
                                    {
                                        Run r = new Run();
                                        r.Text = a.InnerText;
                                        np.Inlines.Add(r);
                                    }
                                }
                                else if (a.Name.ToLower() == "dd")
                                {
                                    try
                                    {
                                        if (a.PreviousSibling != null & a.PreviousSibling.Name.ToLower() == "dd")
                                        {
                                            np.Inlines.Add(new LineBreak());
                                        }
                                    }
                                    catch (Exception err)
                                    {
                                        Console.Error.Write(err.StackTrace);
                                    }
                                }
                                else if (a.Name.ToLower() == "b")
                                {
                                    Bold b = new Bold();
                                    b.Inlines.Add(a.InnerText);
                                    np.Inlines.Add(b);
                                }
                                else if (a.Name.ToLower() == "i")
                                {
                                    Italic i = new Italic();
                                    i.Inlines.Add(a.InnerText);
                                    np.Inlines.Add(i);
                                }
                                else if (a.Name.ToLower() == "#text")
                                {
                                    if (a.ParentNode.Name.ToLower() != "a" && a.ParentNode.Name.ToLower() != "b" && a.ParentNode.Name.ToLower() != "i")
                                    {


                                        String str = ConvertWhitespacesToSingleSpaces(a.InnerText);
                                        if (str != " ")
                                        {
                                            Italic i = new Italic();
                                            i.Inlines.Add(str);
                                            np.Inlines.Add(i);
                                        }
                                    }

                                }
                            }

                            np.Inlines.Add(new LineBreak());
                            pageBody.Add(np);
                    }
                    else if (flag && !flag_table && htmlNode.Name.ToLower() == "ul")
                    {
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }

                        if (htmlNode.ParentNode.Name.ToLower() != "td" && htmlNode.ParentNode.Name.ToLower() != "tr" && htmlNode.ParentNode.Name.ToLower() != "table"
                              && !(htmlNode.ParentNode.Name.ToLower() == "ul" && htmlNode.ParentNode.ParentNode != null && htmlNode.ParentNode.ParentNode.Name.ToLower() == "td")
                              && !(htmlNode.ParentNode.Name.ToLower() == "li" && htmlNode.ParentNode.ParentNode != null && htmlNode.ParentNode.ParentNode.Name.ToLower() == "ul"
                              && htmlNode.ParentNode.ParentNode.ParentNode.Name.ToLower() == "td"))
                        {
                            Paragraph np = new Paragraph();
                            foreach (HtmlNode a in htmlNode.DescendantNodes().ToList())
                            {
                                if (a.Name.ToLower() == "a")
                                {
                                    Boolean err_flag = false;
                                    Hyperlink hl = new Hyperlink();
                                   
                                    hl.Inlines.Add(a.InnerText);
                                    foreach (HtmlAttribute att1 in a.Attributes)
                                    {
                                        if (att1.Name.ToLower() == "href")
                                        {
                                            try
                                            {
                                                if (att1.Value.ToCharArray()[0] != '#')
                                                {
                                                    hl.NavigateUri = new Uri("/WikiContentPage.xaml?href=" + att1.Value, UriKind.Relative);
                                                    
                                                }
                                                else
                                                {
                                                    err_flag = true;
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                err_flag = true;
                                            }
                                            break;
                                        }
                                    }
                                    if (!err_flag)
                                    {
                                        np.Inlines.Add(hl);
                                    }
                                    else
                                    {
                                        Run r = new Run();
                                        r.Text = a.InnerText;
                                        np.Inlines.Add(r);
                                    }
                                }
                                else if (a.Name.ToLower() == "li")
                                {
                                    try
                                    {
                                        if (a.PreviousSibling != null & a.PreviousSibling.Name.ToLower() == "li")
                                        {
                                            np.Inlines.Add(new LineBreak());
                                        }
                                    }
                                    catch (Exception err)
                                    {
                                        Console.Error.Write(err.StackTrace);
                                    }
                                    Bold b = new Bold();
                                    b.Inlines.Add("- ");
                                    np.Inlines.Add(b);
                                }
                                else if (a.Name.ToLower() == "b")
                                {
                                    Bold b = new Bold();
                                    b.Inlines.Add(a.InnerText);
                                    np.Inlines.Add(b);
                                }
                                else if (a.Name.ToLower() == "i")
                                {
                                    Italic i = new Italic();
                                    i.Inlines.Add(a.InnerText);
                                    np.Inlines.Add(i);
                                }
                                else if (a.Name.ToLower() == "#text")
                                {
                                    if (a.ParentNode.Name.ToLower() != "a" && a.ParentNode.Name.ToLower() != "b" && a.ParentNode.Name.ToLower() != "i")
                                    {


                                        String str = ConvertWhitespacesToSingleSpaces(a.InnerText);
                                        if (str != " ")
                                        {
                                            Run run = new Run();
                                            run.Text = str;
                                            np.Inlines.Add(run);
                                        }
                                    }

                                }
                            }

                            np.Inlines.Add(new LineBreak());
                            pageBody.Add(np);

                        }

                    }
                    else if (htmlNode.Name.ToLower() == "h1")
                    {
                        foreach (HtmlAttribute att2 in htmlNode.Attributes)
                        {
                            if (att2.Name.ToLower() == "class" && att2.Value == "firstHeading")
                            {
                                PageTitle.Text = htmlNode.InnerText;
                            }
                        }
                    }
                }
            BreakParseLoop:
                
                if (paragraph != null && paragraph.Inlines.Count > 0)
                {
                    pageBody.Add(paragraph);
                }
                foreach (Block b in pageBody)
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.IsReadOnly = true;
                    rtb.VerticalAlignment = VerticalAlignment.Top;
                    rtb.Blocks.Add(b);
                    PageBody.Children.Add(rtb);

                }
                PageBody.InvalidateArrange();
                PageBody.InvalidateMeasure();
                scrollViewer1.InvalidateArrange();
                scrollViewer1.InvalidateMeasure();
                scrollViewer1.InvalidateScrollInfo();
            } else {
                this.NavigationService.Navigate(new Uri("/ContentLoadError.xaml?href="+CurrentPage, UriKind.Relative));
            }
            progress.Visibility = Visibility.Collapsed;
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);
           if (DeviceNetworkInformation.IsNetworkAvailable)
           {
                string href = "";
            
                if (NavigationContext.QueryString.TryGetValue("href", out href))
                {
                    Href = href;
                    loadPage(href);
                }
            }else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        public static string ConvertWhitespacesToSingleSpaces(string value)
        {
            if (value != null)
            {
                value = Regex.Replace(value, @"\s+", " ");
                return value;
            }
            else
            {
                return " ";
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
            emailComposeTask.Body = "This page " + CurrentPage + " was found to have rendering issues.<br /><br /> Additional comments<br />-----------------------------";
            emailComposeTask.Subject = "Windows Phone 7 - Iron Chariots Bug Report";
            emailComposeTask.Show();
        }

        private void ForumApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://forum.ironchariots.org/");
            task.Show();
        }

        private void ShareApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ShareLinkTask slt = new ShareLinkTask();
            slt.LinkUri = new Uri(CurrentPage);
            slt.Title = PageTitle.Text;
            slt.Message = "Checkout this page from Iron Chariots.";
            slt.Show();
        }
        private void EmailApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "";
            emailComposeTask.Body = "I think you will find this page <a href=\"" + CurrentPage + ">" + PageTitle.Text + "</a> from the <a href=\"http://wiki.ironchariots.org\">Iron Chariots Wiki</a> interesting.";
            emailComposeTask.Subject = "Iron Chariots - The counter apologetics wiki";
            emailComposeTask.Show();
        }

        private void OpenAbout(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void AddStarred(object sender, EventArgs e)
        {
            if (!App.ViewModel.isStarred(CurrentPage) )
            {
                App.ViewModel.addStarred(CurrentPage, PageTitle.Text, "/WikiContentPage.xaml");
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.rest.png", UriKind.Relative);
            }
            else
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.addto.rest.png", UriKind.Relative);
            }
        }

        private void OpenRC(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.responsiblecharity.org/donate");
            task.Show();
        }
    }
}