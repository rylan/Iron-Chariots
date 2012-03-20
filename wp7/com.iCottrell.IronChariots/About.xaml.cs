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
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];
            if (v == System.Windows.Visibility.Visible)
            {
                ironImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("images/ironchariots.dark.png", UriKind.Relative));
            }
            else
            {
                ironImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("images/ironchariots.light.png", UriKind.Relative));
            }

        }

        private void EmailDev_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "";
            emailComposeTask.Subject = "Inquiry - Iron Chariots (Windows Phone 7)";
            emailComposeTask.Show();
        }
        private void CC_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://creativecommons.org/licenses/by-sa/2.5/");
            task.Show();
        }
    }
}