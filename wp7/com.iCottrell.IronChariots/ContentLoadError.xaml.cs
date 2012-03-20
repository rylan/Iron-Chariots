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
using Microsoft.Phone.Tasks;


namespace com.iCottrell.IronChariots
{
    public partial class ContentLoadError : PhoneApplicationPage
    {
        public String CurrentPage { get; private set; }

        public ContentLoadError()
        {
            InitializeComponent();
        }

        private void OpenAbout(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void ReportEvent(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "This page " + CurrentPage + " generated an unkown error.<br /><br /> Additional comments<br />-----------------------------";
            emailComposeTask.Subject = "Windows Phone 7 - Iron Chariots Bug Report";
            emailComposeTask.Show();
        }

        private void CloseEvent(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String href;
            if (NavigationContext.QueryString.TryGetValue("href", out href))
            {
                CurrentPage = href;
            }
        }
    }
}