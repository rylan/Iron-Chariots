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
	name: "com.iCottrell.IronChariots.Phone",
	kind: "Control",
	layoutKind: "VFlexLayout", 
	style: "background-color: white",
	onBack: "backHandler",
	flex:1,
	components:[	
		{kind: "ApplicationEvents", onBack: "handleBack", onLoad:'deviceCheck'},
		{kind: "PageHeader", name:"pheader", components:[
			{kind:"Image", src:"img/ironchariots.png", className:"imageLogo"},
			{kind: enyo.HFlexBox, components: [	
				{content: "Iron Chariots", style:"margin-left:3px;margin-top:-5px"},
				{content: "The Counter Apologetics Wiki", style:"margin-left:-113px;margin-top:15px;font-size:9pt"}
			]}
		]},
		{flex: 1, kind: "SlidingPane", name: "pane", multiView: false, components: [
			{name:"left",  flex:2, components:[
				{kind: "com.iCottrell.IronChariotsSearch.Phone", flex:1, name: "search", onResultItemSelected: "selectedItem", onEnablePaste:"enablePaste"},
				
			]},
			{kind:"SlidingView", dragAnywhere: false, name:"middle", flex:5, components:[
				{kind: "com.iCottrell.IronChariotsPage", flex:1, name: "page", onBackReady:"backShow"},
				{kind: "Toolbar", components:[
					{slidingHandler:true, kind:"GrabButton"},
					{kind:"Spacer"},
					{content:"", name:"empty_space"},
					{kind:"Spacer"},
					{icon: "img/bugreport.png", onclick: "openBugReport"},
					{icon: "img/conversation.png", onclick:"openForumsPage"},
					{icon:"img/wiki.png", onclick:"openSitePage"}
				]}
			]},
			
		]},
		{kind: "AppMenu", components: [
			{kind: "EditMenu", autoDisableItems: false},
			{caption: "Open Wiki", onclick: "openwiki"},
			{caption: "About", onclick: "openAbout"}
		]},
		{kind: "com.iCottrell.IronChariotsAbout.Phone", name:"about"},
		{kind: "com.iCottrell.IronChariotsBugReport", name:"bugReport"},
	],
	create: function() { 
		this.inherited(arguments);
		enyo.setAllowedOrientation("up");
	
		/*
		this.icDB = openDatabase('IronChariotsDB', '1.0', 'Iron Chariots Data Store', '65536');
		
		try{
			this.nullHandleCount=0;
			var sqlTable = 'CREATE TABLE bookmarks (title TEXT NOT NULL, page TEXT NOT NULL);'
			this.icDB.transaction(
				enyo.bind(this, (function (transaction) {
					transaction.executeSql(sqlTable, [], enyo.bind(this,this.createTableDataHandler), enyo.bind(this,this.errorHandler));
				}))
			);
		}
		catch(e){
			this.error(e);
		}*/
	},
	deviceCheck: function(inSender){
		
		var devInfo = enyo.fetchDeviceInfo();
		if(devInfo.screenWidth == 320 & devInfo.screenHeight == 400) {
			this.$.toolbar.hide();
			this.$.appMenu.createComponents([{"caption":"Bug Report", onclick: "openBugReport"}, {"caption":"Iron Chariots Forums", onclick:"openForumsPage"}], {owner:this});
			setTimeout(enyo.bind(this, this.hidePageHeader), 2000);
		}
	},
	hidePageHeader: function() {
		this.$.pheader.hide();
	},
	backHide: function(){
		this.$.empty_space.show();
		this.$.backimg.hide();
	},
	selectedItem: function(){
		this.$.pane.selectViewByName("middle");
		this.$.page.retrievePage(this.$.search.getSelected());
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
	handleBack: function(inSender, inEvent) {
	//	if(this.$.pane.getViewName() === "right")
	//	{
	//		this.$.pane.back();
	//		inEvent.stopPropagation();
	//		inEvent.preventDefault();
    // 		return -1;
	//	} else {
			if(this.$.page.hasHistory()) {
				this.$.page.back(inSender, null);
				inEvent.stopPropagation();
				inEvent.preventDefault();
       			return -1;
			} else {
				if(this.$.pane.getViewName() === "middle"){
					this.$.page.clearHistory();
					this.$.pane.selectViewByName("left");
					inEvent.stopPropagation();
					inEvent.preventDefault();
       				return -1;
				} else if(this.$.pane.getViewName() === "left"){
					if(!this.$.search.isStartPageShowing()){
						this.$.search.showStartPage();
						inEvent.stopPropagation();
						inEvent.preventDefault();
       					return -1;			
					}
				}
			}	
	//	}
	},
	openBugReport: function() {
		if(this.$.page.getCurrentPage()) {
			this.$.bugReport.openDialog(this.$.page.getCurrentPage());
		} 
		else {
			this.$.bugReport.openDialog("Start Page");
		}
	},
	/*
	bookmarkSelected: function(inResponse) {
		this.$.pane.selectViewByName("middle");
		this.$.page.retrievePage(inResponse.selectedBookmark, null);
	},
	openBookmarks: function ()
	{
		this.$.bookmarkpane.loadData();
		this.$.pane.selectViewByName("right");
	}, 
	addBookmark: function ()
	{
		if(this.$.page.getCurrentPage() & this.$.page.getCurrentTitle() )
		var page = this.$.page.getCurrentPage();
		var title = this.$.page.getCurrentTitle();
		this.log(page);
		if(this.icDB !== null ){
			var sqlinsert = 'INSERT INTO bookmarks (title, page) VALUES ("'+ title + '","' + page  + '");'
			this.icDB.transaction (
				enyo.bind(this,(function (transaction){
					transaction.executeSql(sqlinsert, [], enyo.bind(this,this.bookmarksAddedHandler), enyo.bind(this,this.errorHandler)); 
				}))
			);
		}
	},
	bookmarksAddedHandler: function(transaction, results) {	
		this.$.bookmarkpane.loadData();
	},*/
}); 
	