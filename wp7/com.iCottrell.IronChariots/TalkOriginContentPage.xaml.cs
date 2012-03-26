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
using Microsoft.Phone.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;


namespace com.iCottrell.IronChariots
{
    public partial class TalkOriginContentPage : PhoneApplicationPage
    {
        public String index_url = "http://www.talkorigins.org/indexcc/";
        public String CurrentPage { get; private set; }

        public TalkOriginContentPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String href, index;
            if (NavigationContext.QueryString.TryGetValue("href", out href))
            {
                if (href.Contains("..") || href.Contains("talkorigins.org") || !href.Contains("http://") )
                {
                    String title; 
                    if (NavigationContext.QueryString.TryGetValue("topic", out title))
                    {
                        if (!title.Contains("http://"))
                        {
                            PageTitle.Text = title.Substring(0, 1).ToUpper() + title.Substring(1, title.Count() - 1);
                        }
                        else
                        {
                            PageTitle.Text = "";
                        }
                    }

                    if (NavigationContext.QueryString.TryGetValue("index", out index))
                    {
                        CurrentPage = index_url + href;         
                        loadContent(index_url + href);
                    }
                    else if (href.Contains("../"))
                    {
                        href = href.Replace("../", "");
                        CurrentPage = index_url + href;  
                        loadContent(index_url + href);
                    }
                    else if (!href.Contains("http://") && href[0] != '/' )
                    {
                        CurrentPage = index_url + href;  
                        loadContent(index_url + href);
                    }
                    else if (href[0] == '/')
                    {
                        CurrentPage = "http://www.talkorigins.org" + href;  
                        loadContent("http://www.talkorigins.org" + href);
                    }
                    else
                    {
                        CurrentPage = href;
                        loadContent(href);
                    }
                }
                else
                {
                    WebBrowserTask task = new WebBrowserTask();
                    task.Uri = new Uri(href);
                    task.Show();
                }
            }
        }

        private void loadContent(String href) 
        {
            progress.Visibility = Visibility.Visible;

            if (App.ViewModel.isStarred(href))
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.rest.png", UriKind.Relative);
            }
            else
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn.IconUri = new Uri("/images/appbar.favs.addto.rest.png", UriKind.Relative);
            }

            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
               if(href.Contains("faqs")){
                   HtmlWeb webGet = new HtmlWeb();
                   webGet.LoadCompleted += parse_TalkOriginsContentFAQCompleted;
                   webGet.LoadAsync(href);
               }else if(href.Contains("indexcc")){
                   HtmlWeb webGet = new HtmlWeb();
                   webGet.LoadCompleted += parse_TalkOriginsContentIndexCompleted;
                   webGet.LoadAsync(href);
               }
            }
        }
        public void parse_TalkOriginsContentFAQCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document != null)
            {
                e.Document.OptionCheckSyntax = false;
                IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();
                int count = 0;
                IList<Block> pageBody = new List<Block>();
                Paragraph paragraph = new Paragraph();
                Boolean flag_enteredP = false;
                foreach (HtmlNode htmlNode in hnc)
                {
                    if (htmlNode.Name.ToLower() == "p")
                    {
                       
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        if (htmlNode.ParentNode != null && htmlNode.ParentNode.Name.ToLower() == "noscript")
                        {
                            continue;
                        }
                        foreach (HtmlNode p in htmlNode.DescendantNodes().ToList())
                        {
                            flag_enteredP = true;
                            if (p.Name.ToLower() == "i" || p.Name.ToLower() == "em")
                            {
                                String str = ConvertWhitespacesToSingleSpaces(p.InnerText);
                                if (str != " ")
                                {
                                    Italic i = new Italic();
                                    i.Inlines.Add(str);
                                    paragraph.Inlines.Add(i);
                                }
                            }
                            else if (p.Name.ToLower() == "a")
                            {
                                Hyperlink hl = new Hyperlink();
                                hl.Inlines.Add(p.InnerText);
                                foreach (HtmlAttribute att1 in p.Attributes)
                                {
                                    if (att1.Name.ToLower() == "href")
                                    {
                                        if (att1.Value.ToCharArray()[0] != '#' && !att1.Value.Contains("ackbib.html"))
                                        {
                                            String uri = att1.Value;
                                            if (!att1.Value.Contains("http://"))
                                            {
                                                uri = uri + "&topic=" + p.InnerText;
                                            }
                                            hl.NavigateUri = new Uri("/TalkOriginContentPage.xaml?href=" + uri, UriKind.Relative);
                                            paragraph.Inlines.Add(hl);
                                        }
                                        else
                                        {
                                            String str = ConvertWhitespacesToSingleSpaces(p.InnerText);
                                            if (str != " ")
                                            {
                                                Run run = new Run();
                                                run.Text = str;
                                                paragraph.Inlines.Add(run);
                                            }
                                        }
                                    }
                                }

                            }
                            else if (p.Name.ToLower() == "b")
                            {
                                if (p.ParentNode.Name.ToLower() != "i" && p.ParentNode.Name.ToLower() != "em")
                                {
                                    String str = ConvertWhitespacesToSingleSpaces(p.InnerText);
                                    if (str != " ")
                                    {
                                        Bold i = new Bold();
                                        i.Inlines.Add(str);
                                        paragraph.Inlines.Add(i);
                                    }
                                }
                            }
                            else if (p.Name.ToLower() == "#text")
                            {
                                if (p.ParentNode.Name.ToLower() != "b"
                                    && p.ParentNode.Name.ToLower() != "a"
                                    && p.ParentNode.Name.ToLower() != "em"
                                    && p.ParentNode.Name.ToLower() != "i")
                                {
                                    String str = ConvertWhitespacesToSingleSpaces(p.InnerText);
                                    if (str != " ")
                                    {
                                        Run run = new Run();
                                        run.Text = str;
                                        paragraph.Inlines.Add(run);
                                    }
                                }
                            }
                            else if (p.Name.ToLower() == "img")
                            {
                                foreach (HtmlAttribute a in p.Attributes)
                                {
                                    if (a.Name.ToLower() == "alt")
                                    {
                                        Run run = new Run();
                                        run.Text = a.Value;
                                        paragraph.Inlines.Add(run);
                                    }
                                }
                            }

                        }
                        paragraph.Inlines.Add(new LineBreak());
                    }
                    else if (htmlNode.Name.ToLower() == "h1")
                    {
                        PageTitle.Text = htmlNode.InnerText;
                    }
                    /* Problem with other info being pulled along with it 
                      else if (htmlNode.Name.ToLower() == "div" && htmlNode.Attributes.Count() > 0)
                     {
                         if (htmlNode.Attributes[0].Name.ToLower() == "class" && htmlNode.Attributes[0].Value == "minus1")
                         {
                             Run run = new Run();
                             String str = htmlNode.InnerText.Replace("&copy;", "©");
                             run.Text = "  " + htmlNode.InnerText;
                             paragraph.Inlines.Add(run);
                         }
                     }//*/
                    else if (htmlNode.Name.ToLower() == "address" && htmlNode.Attributes.Count() == 0)
                    {
                        Run run = new Run();
                        run.Text = "by " + htmlNode.InnerText;
                        paragraph.Inlines.Add(run);
                    }
                    else if (htmlNode.Name.ToLower() == "h2")
                    {
                        TextBlock t = new TextBlock();
                        t.Text = htmlNode.InnerText;
                        t.TextWrapping = TextWrapping.Wrap;
                        t.Style = (Style)Application.Current.Resources["PhoneTextExtraLargeStyle"];

                        if (pageBody.Count > 0 || paragraph.Inlines.Count > 0)
                        {
                            if (paragraph.Inlines.Count > 0)
                            {
                                paragraph.Inlines.Add(new LineBreak());
                                pageBody.Add(paragraph);
                                paragraph = new Paragraph();
                            }
                            foreach (Block b in pageBody)
                            {
                                RichTextBox rtb = new RichTextBox();
                                rtb.IsReadOnly = true;
                                rtb.VerticalAlignment = VerticalAlignment.Top;
                                rtb.Blocks.Add(b);
                                PageBody.Children.Add(rtb);
                            }
                        }
                        pageBody.Clear();
                        PageBody.Children.Add(t);
                    }
                }

                if (!flag_enteredP)
                {
                    pageBody.Clear();
                    PageBody.Children.Clear();
                    paragraph = new Paragraph();
                    Boolean flag_begin = false;
                    foreach (HtmlNode htmlNode in hnc)
                    {
                        if (htmlNode.Name.ToLower() == "body")
                        {
                          //  flag_begin = true;
                        }else if(htmlNode.Name.ToLower()=="#comment"){
                            if (htmlNode.InnerText.Trim() == "<!-- begin trailer -->")
                            {
                                break;
                            }
                            else if (htmlNode.InnerText.Trim() == "<!-- end header -->")
                            {
                                flag_begin = true;
                            }
                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "p")
                        {
                            if (paragraph.Inlines.Count > 0 )
                            {
                                paragraph.Inlines.Add(new LineBreak());
                                pageBody.Add(paragraph);
                                paragraph = new Paragraph();
                            }

                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "#text")
                        {
                            if (htmlNode.ParentNode != null && htmlNode.ParentNode.Name.ToLower() != "b"
                                && htmlNode.ParentNode.Name.ToLower() != "h1" && htmlNode.ParentNode.Name.ToLower() != "h2"
                                && htmlNode.ParentNode.Name.ToLower() != "i" && htmlNode.ParentNode.Name.ToLower() != "em"
                                && htmlNode.ParentNode.Name.ToLower() != "a" && htmlNode.ParentNode.Name.ToLower() != "img"
                                && htmlNode.ParentNode.Name.ToLower() != "area" && htmlNode.ParentNode.Name.ToLower() != "map"
                                && htmlNode.ParentNode.Name.ToLower() != "font"
                                && htmlNode.ParentNode.Name.ToLower() != "#comment")
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
                        else if (flag_begin && htmlNode.ParentNode != null && htmlNode.ParentNode.Name.ToLower() == "noscript")
                        {
                            continue;
                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "i" || htmlNode.Name.ToLower() == "em")
                        {
                            String str = ConvertWhitespacesToSingleSpaces(htmlNode.InnerText);
                            if (str != " ")
                            {
                                Italic i = new Italic();
                                i.Inlines.Add(str);
                                paragraph.Inlines.Add(i);
                            }
                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "a")
                        {
                            if (!(htmlNode.ParentNode != null && htmlNode.ParentNode.Name.ToLower() == "font"))
                            {
                                Hyperlink hl = new Hyperlink();
                                hl.Inlines.Add(htmlNode.InnerText);
                                foreach (HtmlAttribute att1 in htmlNode.Attributes)
                                {
                                    if (att1.Name.ToLower() == "href")
                                    {
                                        if (att1.Value.ToCharArray()[0] != '#' && !att1.Value.Contains("ackbib.html"))
                                        {
                                            String uri = att1.Value;
                                            if (!att1.Value.Contains("http://"))
                                            {
                                                uri = uri + "&topic=" + htmlNode.InnerText;
                                            }
                                            hl.NavigateUri = new Uri("/TalkOriginContentPage.xaml?href=" + uri, UriKind.Relative);
                                            paragraph.Inlines.Add(hl);
                                        }
                                        else
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
                                }
                            }
                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "b" || htmlNode.Name.ToLower() == "strong")
                        {
                            if (htmlNode.ParentNode.Name.ToLower() != "i" && htmlNode.ParentNode.Name.ToLower() != "em")
                            {
                                if (paragraph.Inlines.Count > 0 && htmlNode.ParentNode != null && htmlNode.ParentNode.Name.ToLower() == "dt")
                                {
                                    paragraph.Inlines.Add(new LineBreak());
                                    pageBody.Add(paragraph);
                                    paragraph = new Paragraph();
                                }

                                String str = ConvertWhitespacesToSingleSpaces(htmlNode.InnerText);
                                if (str != " ")
                                {
                                    Bold i = new Bold();
                                    i.Inlines.Add(str);
                                    paragraph.Inlines.Add(i);
                                }
                                if(htmlNode.ParentNode!=null && htmlNode.ParentNode.Name.ToLower()=="dt" && htmlNode.NextSibling!=null && htmlNode.NextSibling.Name.ToLower() != "p"){
                                    paragraph.Inlines.Add(new LineBreak());
                                }
                            }
                        }
                        else if (flag_begin && htmlNode.Name.ToLower() == "img")
                        {
                            foreach (HtmlAttribute a in htmlNode.Attributes)
                            {
                                if (a.Name.ToLower() == "alt")
                                {
                                    Run run = new Run();
                                    run.Text = a.Value;
                                    paragraph.Inlines.Add(run);
                                }
                            }
                        }
                        else if (htmlNode.Name.ToLower() == "h1")
                        {
                            PageTitle.Text = htmlNode.InnerText;
                        }
                        else if (htmlNode.Name.ToLower() == "font")
                        {
                            foreach (HtmlAttribute att in htmlNode.Attributes)
                            {
                                if (att.Name.ToLower() == "size" && att.Value == "+2")
                                {
                                    TextBlock t = new TextBlock();
                                    t.Text = htmlNode.InnerText.Trim();
                                    t.TextWrapping = TextWrapping.Wrap;
                                    t.Style = (Style)Application.Current.Resources["PhoneTextExtraLargeStyle"];
                                    PageBody.Children.Add(t);
                                }
                                else if (att.Name.ToLower() == "size" && att.Value == "-1")
                                {
                                    Run run = new Run();
                                    String str = htmlNode.InnerText.Replace("&copy;", "©");
                                    str = str.Replace("&#169;", "©");
                                    run.Text = str.Trim();
                                    paragraph.Inlines.Add(run);
                                }
                            }
                        }
                        else if (htmlNode.Name.ToLower() == "address" && htmlNode.Attributes.Count() == 0)
                        {
                            Run run = new Run();
                            run.Text = "by " + htmlNode.InnerText;
                            paragraph.Inlines.Add(run);
                        }
                        else if (htmlNode.Name.ToLower() == "h2")
                        {
                            TextBlock t = new TextBlock();
                            t.Text = htmlNode.InnerText;
                            t.TextWrapping = TextWrapping.Wrap;
                            t.Style = (Style)Application.Current.Resources["PhoneTextExtraLargeStyle"];

                            if (pageBody.Count > 0 || paragraph.Inlines.Count > 0)
                            {
                                if (paragraph.Inlines.Count > 0)
                                {
                                    paragraph.Inlines.Add(new LineBreak());
                                    pageBody.Add(paragraph);
                                    paragraph = new Paragraph();
                                }
                                foreach (Block b in pageBody)
                                {
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.IsReadOnly = true;
                                    rtb.VerticalAlignment = VerticalAlignment.Top;
                                    rtb.Blocks.Add(b);
                                    PageBody.Children.Add(rtb);
                                }
                            }
                            pageBody.Clear();
                            PageBody.Children.Add(t);
                        }
                    }

                }

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
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/ContentLoadError.xaml?href="+CurrentPage, UriKind.Relative));
            }
            progress.Visibility = Visibility.Collapsed;
        }

        public void parse_TalkOriginsContentIndexCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Document != null)
            {
                IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();
                Boolean flag_content = false;
                int count = 0;
                IList<Block> pageBody = new List<Block>();
                Paragraph paragraph = new Paragraph();

                foreach (HtmlNode htmlNode in hnc)
                {
                    if (htmlNode.Name.ToLower() == "h2" && htmlNode.Attributes.Count > 0
                        && htmlNode.Attributes[0].Name.ToLower() == "class" && htmlNode.Attributes[0].Value == "c")
                    {
                        flag_content = true;
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
                    else if (!flag_content && htmlNode.Name.ToLower() == "small" && htmlNode.InnerText.Contains("edited"))
                    {
                        String str = htmlNode.InnerText;
                        str = str.Substring(0, 1).ToUpper() + str.Substring(1, str.Count() - 1);
                        str = str.Replace("&nbsp;", " ");
                        str = str.Replace("&copy;", "©");
                        Run run = new Run();
                        run.Text = str;
                        paragraph.Inlines.Add(run);
                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "#text")
                    {
                        if (htmlNode.ParentNode.Name.ToLower() != "h2"
                            && htmlNode.ParentNode.Name.ToLower() != "h3"
                            && htmlNode.ParentNode.Name.ToLower() != "a"
                            && htmlNode.ParentNode.Name.ToLower() != "i")
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
                    else if (flag_content && htmlNode.Name.ToLower() == "i")
                    {
                        String str = ConvertWhitespacesToSingleSpaces(htmlNode.InnerText);
                        if (str != " ")
                        {
                            Italic i = new Italic();
                            i.Inlines.Add(str);
                            paragraph.Inlines.Add(i);
                        }

                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "h3")
                    {

                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        Bold h2 = new Bold();
                        h2.Inlines.Add(htmlNode.InnerText);
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new LineBreak());
                        para.Inlines.Add(h2);
                        pageBody.Add(para);
                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "h2")
                    {

                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        Bold h2 = new Bold();
                        h2.Inlines.Add(htmlNode.InnerText);
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new LineBreak());
                        para.Inlines.Add(h2);
                        pageBody.Add(para);
                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "ol")
                    {
                        count = 0;
                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "a")
                    {
                        Hyperlink hl = new Hyperlink();
                        hl.Inlines.Add(htmlNode.InnerText);
                        foreach (HtmlAttribute att1 in htmlNode.Attributes)
                        {
                            if (att1.Name.ToLower() == "href")
                            {
                                if (att1.Value.ToCharArray()[0] != '#')
                                {
                                    String p = att1.Value;
                                    if (!att1.Value.Contains("http://"))
                                    {
                                        p = p + "&topic=" + htmlNode.InnerText;
                                    }
                                    hl.NavigateUri = new Uri("/TalkOriginContentPage.xaml?href=" + p, UriKind.Relative);
                                    paragraph.Inlines.Add(hl);
                                }
                                else
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
                        }

                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "div" && htmlNode.InnerText.Contains("Previous Claim"))
                    {
                        flag_content = false;
                    }
                    else if (flag_content && htmlNode.Name.ToLower() == "li")
                    {
                        if (paragraph.Inlines.Count > 0)
                        {
                            pageBody.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        Run run = new Run();


                        if (htmlNode.ParentNode.Name.ToLower() == "ol")
                        {
                            count++;
                            run.Text = count + ". ";
                        }
                        else
                        {
                            run.Text = "- ";
                        }
                        paragraph.Inlines.Add(run);
                    }
                }
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
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/ContentLoadError.xaml?href=" + CurrentPage, UriKind.Relative));
            }
            progress.Visibility = Visibility.Collapsed;
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

        private void OpenAbout(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void BRApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "This Talk Origin page "+CurrentPage+" was found to have rendering issues.<br /><br /> Additional comments<br />-----------------------------";
            emailComposeTask.Subject = "Windows Phone 7 - Iron Chariots Bug Report (Talk Origins)";
            emailComposeTask.Show();
        }

        private void ShareEvent(object sender, EventArgs e)
        {
            ShareLinkTask slt = new ShareLinkTask();
            slt.LinkUri = new Uri(CurrentPage);
            slt.Title = PageTitle.Text;
            slt.Message = "Checkout this page from TalkOrigins Archive.";
            slt.Show();
        }

        private void EmailEvent(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "";
            emailComposeTask.Body = "I think you will find this page <a href=\""+CurrentPage+">"+PageTitle.Text+"</a> from the <a href=\"http://www.talkorigins.org\">TalkOrigins Archive</a> interesting.";
            emailComposeTask.Subject = "TalkOrigins Archive - Exploring the Creation/Evolution Controversy";
            emailComposeTask.Show();
        }

        private void WebEvent(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(CurrentPage);
            task.Show();
        }

        private void AddStarred(object sender, EventArgs e)
        {
            if (!App.ViewModel.isStarred(CurrentPage))
            {
                App.ViewModel.addStarred(CurrentPage, PageTitle.Text, "/TalkOriginContentPage.xaml");
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