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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace com.iCottrell.IronChariots
{
    public class TalkOriginsTopicCategoryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TalkOriginsTopicViewModel> TOTopics { get; private set; }
        
        public TalkOriginsTopicCategoryViewModel()
        {
            this.TOTopics = new ObservableCollection<TalkOriginsTopicViewModel>();
        }
        private String _topic;
        public String Topic
        {
            get
            {
                return _topic;
            }
            set
            {
                if (value != _topic)
                {
                    _topic = value;
                    NotifyPropertyChanged("Topic");
                }
            }
        }

        private String _categoryShort;
        public String CategoryShort
        {
            get
            {
                return _categoryShort;
            }
            set
            {
                if (value != _categoryShort)
                {
                    _categoryShort = value;
                    NotifyPropertyChanged("CategoryShort");
                }
            }
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
    }
}
