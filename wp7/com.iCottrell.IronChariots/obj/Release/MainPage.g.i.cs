﻿#pragma checksum "C:\Users\cottrell\dev\windows phone\Iron-Chariots--Windows-Phone-7-\com.iCottrell.IronChariots\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A39CE1321CC0CC11BE321B4F7F6739F4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace com.iCottrell.IronChariots {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Primitives.Popup LoadingScreen;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal Microsoft.Phone.Controls.Pivot PivotControl;
        
        internal System.Windows.Controls.TextBox search;
        
        internal System.Windows.Controls.Button searchButton;
        
        internal System.Windows.Controls.Canvas spoint;
        
        internal System.Windows.Controls.Image image1;
        
        internal System.Windows.Controls.Image image2;
        
        internal System.Windows.Controls.Image image3;
        
        internal System.Windows.Controls.Image image4;
        
        internal System.Windows.Controls.Canvas listcanvas;
        
        internal System.Windows.Controls.ListBox resultSet;
        
        internal System.Windows.Controls.TextBox inputBoxTO;
        
        internal System.Windows.Controls.Button SearchTO;
        
        internal System.Windows.Controls.Canvas TOCanvasMain;
        
        internal System.Windows.Controls.ListBox TopicSet;
        
        internal System.Windows.Controls.Canvas TOListCanvas;
        
        internal System.Windows.Controls.ListBox TOResultSet;
        
        internal System.Windows.Controls.ListBox ImportantList;
        
        internal System.Windows.Controls.Border RCAd;
        
        internal System.Windows.Controls.TextBlock tex;
        
        internal System.Windows.Controls.TextBlock des;
        
        internal System.Windows.Controls.TextBlock link;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/com.iCottrell.IronChariots;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.LoadingScreen = ((System.Windows.Controls.Primitives.Popup)(this.FindName("LoadingScreen")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PivotControl = ((Microsoft.Phone.Controls.Pivot)(this.FindName("PivotControl")));
            this.search = ((System.Windows.Controls.TextBox)(this.FindName("search")));
            this.searchButton = ((System.Windows.Controls.Button)(this.FindName("searchButton")));
            this.spoint = ((System.Windows.Controls.Canvas)(this.FindName("spoint")));
            this.image1 = ((System.Windows.Controls.Image)(this.FindName("image1")));
            this.image2 = ((System.Windows.Controls.Image)(this.FindName("image2")));
            this.image3 = ((System.Windows.Controls.Image)(this.FindName("image3")));
            this.image4 = ((System.Windows.Controls.Image)(this.FindName("image4")));
            this.listcanvas = ((System.Windows.Controls.Canvas)(this.FindName("listcanvas")));
            this.resultSet = ((System.Windows.Controls.ListBox)(this.FindName("resultSet")));
            this.inputBoxTO = ((System.Windows.Controls.TextBox)(this.FindName("inputBoxTO")));
            this.SearchTO = ((System.Windows.Controls.Button)(this.FindName("SearchTO")));
            this.TOCanvasMain = ((System.Windows.Controls.Canvas)(this.FindName("TOCanvasMain")));
            this.TopicSet = ((System.Windows.Controls.ListBox)(this.FindName("TopicSet")));
            this.TOListCanvas = ((System.Windows.Controls.Canvas)(this.FindName("TOListCanvas")));
            this.TOResultSet = ((System.Windows.Controls.ListBox)(this.FindName("TOResultSet")));
            this.ImportantList = ((System.Windows.Controls.ListBox)(this.FindName("ImportantList")));
            this.RCAd = ((System.Windows.Controls.Border)(this.FindName("RCAd")));
            this.tex = ((System.Windows.Controls.TextBlock)(this.FindName("tex")));
            this.des = ((System.Windows.Controls.TextBlock)(this.FindName("des")));
            this.link = ((System.Windows.Controls.TextBlock)(this.FindName("link")));
        }
    }
}

