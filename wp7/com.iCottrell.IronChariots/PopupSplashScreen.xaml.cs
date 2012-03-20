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


namespace com.iCottrell.IronChariots
{
    public partial class PopupSplashScreen : UserControl
    {
       public PopupSplashScreen()
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
            this.progressBar1.IsIndeterminate = true;
        }
    }
}
