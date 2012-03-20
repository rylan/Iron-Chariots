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


namespace com.iCottrell.IronChariots
{
    public static class SearchResultsContainer
    {
      public static ObservableCollection<SearchResults> SearchResultItems = new ObservableCollection<SearchResults>();
    } 
 
    public class SearchResults
    {
        public String Href { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Category { get; set; } 

        public SearchResults()
        {
            Href = "";
            Title = "";
            Description = "";
            Category = "";
        }
    } 
    
}
