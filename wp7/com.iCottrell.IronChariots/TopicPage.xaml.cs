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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;


namespace com.iCottrell.IronChariots
{
    public partial class TopicPage : PhoneApplicationPage
    {
        public ObservableCollection<TalkOriginsTopicViewModel> TopicItems { get; private set; }

        
        public TopicPage()
        {
            InitializeComponent();
            this.TopicItems = new ObservableCollection<TalkOriginsTopicViewModel>();
            this.DataContext = this.TopicItems;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String topic;
            if (NavigationContext.QueryString.TryGetValue("topic", out topic))
            {
                loadIndexTopics(topic);
            }
        }

        private void loadIndexTopics (String topic) {
            PageTitle.Text = topic;
            foreach (TalkOriginsTopicCategoryViewModel t in App.ViewModel.TOCategoryTopics)
            {
                if (t.Topic == topic)
                {
                    TopicItems.Clear();
                    foreach (TalkOriginsTopicViewModel i in t.TOTopics)
                    {
                        TopicItems.Add(i);
                    }
                }
            }
            TopicSet.InvalidateArrange();
            TopicSet.InvalidateMeasure();
            NotifyPropertyChanged("Items");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GotoTopicPage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            TextBlock t = (TextBlock)sp.Children[1];
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/TalkOriginContentPage.xaml?href="+sp.Tag+"&topic="+t.Text+"&index=true", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/DCErrorPage.xaml", UriKind.Relative));
            }
        }

        private void OpenAbout(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void OpenRC(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.responsiblecharity.org/donate");
            task.Show();
        }

    }
}