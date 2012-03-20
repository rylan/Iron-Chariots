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
enyo.kind({
	name: "com.iCottrell.IronChariots.Tablet",
	kind: enyo.VFlexBox, 
	style: "background-color: white",
	onBack: "backHandler",
	flex:1,
	components:[
		
		{kind: "PageHeader", components:[
			{kind:"Image", src:"img/ironchariots.png", className:"imageLogo"},
			{kind: enyo.HFlexBox, components: [	
				{content: "Iron Chariots", style:"margin-left:3px;"},
				{content: "- The Counter Apologetics Wiki", style:"margin-left:12px;margin-top:10px;font-size:10pt"}
			]}
		]},
		{flex: 1, kind: "SlidingPane", name: "pane", components: [
			{name:"left",  flex:2, components:[
				{kind: "com.iCottrell.IronChariotsSearch", flex:1, name: "search", onResultItemSelected: "selectedItem", onEnablePaste:"enablePaste"},
			]},
			{kind:"SlidingView", dragAnywhere: false, name:"middle", flex:5, components:[
				{kind: "com.iCottrell.IronChariotsPage", flex:1, name: "page", onBackReady:"backShow", onErrorBack: "handleBack", onBookmarked:"bookmarked", showing:false},
				{kind: "com.iCottrell.IronChariotsStartPage", name:"startpage", flex:5, onTopicClick: "topicClick"},
				{kind: "Toolbar", components:[
					{slidingHandler:true, kind:"GrabButton"},
					{kind:"Spacer"},
					{content:"", name:"empty_space"},
					{icon:"img/menu-icon-back.png", name:"backimg", onclick:"handleBack", showing:false},
					{icon:"img/bookmark_add.png", name:"bookmark_add", onclick:"addBookmark", showing:false},
					{icon:"img/bookmark.png", name:"bookmark", onclick:"removeBookmark", showing:false},
					{kind:"Spacer"},
					{icon: "img/bugreport.png", onclick: "openBugReport"},
					{icon: "img/conversation.png", onclick:"openForumsPage"},
					{icon:"img/wiki.png", onclick:"openSitePage"},
					{icon: "img/bookmarks.png", onclick:"openBookMarks"}
				]}
			]},
			{name:"right", flex:1,  showing:false, components:[
				{kind: "com.iCottrell.IronChariotsBookmarks", flex:1, name: "bookmarkpane", onBookmarkSelected: "bookmarkSelected", onEnablePaste:""},
			]}
		]},
		{kind: "AppMenu", components: [
			{kind: "EditMenu", autoDisableItems: false},
			{caption: "Open Wiki", onclick: "openwiki"},
			{caption: "About", onclick: "openAbout"}
		]},
		{kind: "com.iCottrell.IronChariotsAbout", name:"about"},
		{kind: "com.iCottrell.IronChariotsBugReport", name:"bugReport"}
	],
	create: function() { 
		this.inherited(arguments);
		this.currentPageUrl = null;
		this.pageTitle = null;
		this.bookmarkshowing = false;
		this.icDB = openDatabase('IronChariotsDB', '1.0', 'Iron Chariots Data Store', '65536');
		
		try{
			this.nullHandleCount=0;
			var sqlTable = 'CREATE TABLE bookmarks (title TEXT NOT NULL, page TEXT NOT NULL);'
			this.icDB.transaction(
				enyo.bind(this, (function (transaction) {
					transaction.executeSql(
						sqlTable, 
						[], 
						enyo.bind(this,this.createTableDataHandler), 
						enyo.bind(this,this.errorTableHandler)
					);
				}))
			);
		}
		catch(e){
			this.error(e);
		}
	},
	bookmarked: function(inSender){
		this.currentPageUrl = inSender.currenturl;
		this.pageTitle = inSender.pageTitle;
		if(inSender.isBookmarked)
		{
			this.$.bookmark.show();
			this.$.bookmark_add.hide();
		} else {
			this.$.bookmark.hide();
			this.$.bookmark_add.show();
		}
	},
	addBookmark: function()
	{
		if(this.icDB !== null ){
			var sqlinsert = 'INSERT INTO bookmarks (title, page) VALUES ("'+ this.pageTitle + '","' + this.currentPageUrl  + '");'
			this.icDB.transaction (
				enyo.bind(this,(function (transaction){
					transaction.executeSql(
						sqlinsert, 
						[], 
						enyo.bind(this,this.bookmarksAddedHandler), 
						enyo.bind(this,this.errorHandler)
					); 
				}))
			);
		}

	},
	removeBookmark: function()
	{
		if(this.icDB !== null ){
			var sqldel = 'DELETE FROM bookmarks WHERE  page="' + this.currentPageUrl + '";'
			this.icDB.transaction (
				enyo.bind(this,(function (transaction){
					transaction.executeSql(
						sqldel, 
						[], 
						enyo.bind(this,this.bookmarksRemovedHandler), 
						enyo.bind(this,this.errorHandler)
					); 
				}))
			);
		}

	},
	bookmarksAddedHandler: function(transaction, results) {	
		this.$.bookmarkpane.loadData();
		this.$.bookmark_add.hide();
		this.$.bookmark.show();
	},
	bookmarksRemovedHandler: function(transaction, results) {	
		this.$.bookmarkpane.loadData();
		this.$.bookmark.hide();
		this.$.bookmark_add.show();
	},
	errorHandler: function(transaction, error){
		this.log("DB Error");
	},
	backShow: function(){
		this.$.empty_space.hide();
		this.$.backimg.show();
	},
	backHide: function(){
		this.$.empty_space.show();
		this.$.backimg.hide();
	},
	selectedItem: function(){
		this.$.startpage.hide();
		this.$.page.show();
		this.$.page.retrievePage(this.$.search.getSelected());
		this.backShow();
	},
	openAbout: function (inSender, inEvent){
		this.$.about.openAtCenter();
	},
	openAppMenu: function() {
	    this.$.appMenu.open();
	},
	openwiki: function() {
		window.location = "http://wiki.ironchariots.org";
	},
	openForumsPage: function() {
		window.location = "http://forum.ironchariots.org";
	},
	openSitePage: function(inSender) {
		this.$.page.launchInBrowser();
	},
	handleBack: function(inSender) {
		if(this.$.page.hasHistory()) {
			this.$.page.back(inSender, null);
		} else {
			this.$.page.hide();
			this.$.page.clearHistory();
			this.$.startpage.show();
			this.$.bookmark.hide();
			this.$.bookmark_add.hide();
			this.backHide();
		}
		return true;	
	},
	openBugReport: function() {
		if(this.$.page.getCurrentPage()) {
			this.$.bugReport.openDialog(this.$.page.getCurrentPage());
		} 
		else {
			this.$.bugReport.openDialog("Start Page");
		}
	},
	topicClick: function(inResponse) {
		this.$.startpage.hide();
		this.$.page.show();
		this.$.page.getNewPage(inResponse.urlClick, null);
	},
	openBookMarks: function()
	{
		this.bookmarkshowing = !this.bookmarkshowing;
		if(this.bookmarkshowing)
		{
			this.$.bookmarkpane.loadData();
			this.$.right.show();
		}else {
			this.$.right.hide();
		}
	},
	bookmarkSelected: function(inResponse)
	{
		if(inResponse.selectedBookmark)
		{
			if(this.icDB){
				var queryBookmarks = 'SELECT page FROM bookmarks WHERE title="' + inResponse.selectedBookmark + '";'
				try {
					this.icDB.transaction(
						enyo.bind(this, (function (transaction) {
							transaction.executeSql(
								queryBookmarks, 
								[], 
								enyo.bind(this, this.bookmarkSelectedDataHandler), 
								enyo.bind(this,this.errorHandler)
							);
						}))
					);
				}
				catch(e){
					this.error(e);
				}
			}
		}
	},
	bookmarkSelectedDataHandler: function(transaction, results) {
		if( results.rows.length > 0){
			this.$.startpage.hide();
			this.$.page.show();
			this.$.page.getNewPage(results.rows.item(0).page, null);
		}
	}
}); 
	