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
	name: "com.iCottrell.IronChariotsPage",
	kind: enyo.VFlexBox, 
	style: "background-color: white",
	events:{
		onBackReady: "",
		onBackHide: "",
		onErrorBack:"",
		onBookmarked:"",
	},
	components:[
		{name: "ironchariotsPage", kind: "WebService", onSuccess: "gotPageResult", handleAs: "text", onFailure: "pageFailed"},
		{kind: "Scroller", flex:9, components:[
			{style:"padding:4px;", components:[
				{kind: "BasicRichText", name:"pageInfo", allowHTML:true, style:"-webkit-user-select: text; -webkit-user-modify: read-only;", richContent: true, hint:"", readonly: true, onclick: "catchClick", onFocus:"disableActions", onkeypress: "disableKeys"}
			]}
		]},
		{kind: "Scrim", layoutKind: "VFlexLayout", align: "center", pack: "center",components: [
			{kind: "SpinnerLarge", showing: true}
		]},
		{name: "errorDialog", kind: "Dialog", components: [
			{className: "enyo-item enyo-first", style: "padding: 12px", content: "Page could not be found. Please check data connection."},
			{kind: "Button", caption: "OK", onclick:"closeDialog"}
		]},
		{kind: "com.iCottrell.IronChariotsBibleDialog", name:"biblequote"}
	],
	create: function() { 
		this.inherited( arguments ); 
		this.currentPage = null;
		this.pageHistory = [];
		this.urlhistory = [];
		this.currenturl = null;
		this.pageTitle = null;
		this.isBookmarked = false;
		this.ironUrl = 	"http://wiki.ironchariots.org" ;
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
		}
	},
	createTableDataHandler: function(transaction, results) {	
		
	},
	initComponents: function() {
		this.inherited( arguments );
	},
	determineBookmark: function(){
		if(this.icDB){
			var queryICDB = 'SELECT title FROM bookmarks where page ="'+ this.currenturl +'";'
			try {
				this.icDB.transaction(
					enyo.bind(this, (function (transaction) {
						transaction.executeSql(queryICDB, [], enyo.bind(this, this.queryBookmarkHandler), enyo.bind(this,this.errorHandler));
					}))
				);
			}
			catch(e){
				this.error(e);
			}
		}	
	},
	queryBookmarkHandler: function(transaction, results) {
		if(results.rows.length > 0)
		{
			this.isBookmarked = true;
			this.doBookmarked();
		}else{
			this.isBookmarked = false;
			this.doBookmarked();
		}
	},
	createRecordDataHandler: function(transaction, results) {	

	},
	errorHandler: function (transaction, error){
		this.log(error);
	},
	gotPageResult: function(inSender, inResponse) {
		this.$.scrim.hide();
		if(inResponse)
		{
			this.processPageData( inResponse );
			if(this.pageHistory.length > 0){
				this.doBackReady();
			}
		} else {
			this.pageFailed();
		}
	},
	retrievePage: function( searchItem ){
		if( searchItem ){
			if( this.currentPage !== searchItem ){
				this.$.pageInfo.setContent("");
				var url = this.ironUrl + searchItem.url;
				this.$.ironchariotsPage.setUrl(url);
				this.$.ironchariotsPage.call();
				this.$.scrim.show();
				this.currentPage = searchItem;
				this.currenturl = url;
				this.determineBookmark(url);
			}
		}
	},
	pageFailed: function(inSender, inResponse) {
		this.$.scrim.hide();
		this.log("Error should have shown");
		this.$.errorDialog.toggleOpen();
	},
	closeDialog: function(){
		this.$.errorDialog.toggleOpen();
		this.doErrorBack();
	},
	processPageData: function( pageData ){
		var data = [];
		var jqr = $(pageData);
		this.pageTitle = jqr.find("h1[class=firstHeading]").first().text();

		data.push("<h1>"+ jqr.find("h1[class=firstHeading]").first().text() +"</h1>");
		var children = jqr.find("div[class=mw-content-ltr]").first().children();
		var prev="";
		for(var i=0; i < children.length; i++){
			if($(children[i]).is('p')){
				prev="p";
				data.push("<p>" + this.processHTML(children[i]) + "</p>");
			}else if($(children[i]).is('h2')){
				if(prev==="h2"){
					data.pop();
				}
				data.push( "<h2>"+ $( children[i] ).text()+"</h2>" );
				prev="h2";
			}else if($(children[i]).is('ul')){
				prev="ol";
				var qchildren = $( children[i] ).children();
				data.push("<ul>");
				for(var j=0; j<qchildren.length; j++){
					data.push("<li>" + this.processHTML(qchildren[j] ) + "</li>");
				}
				data.push("</ul>");
			}else if($(children[i]).is('h3')){
				if(prev==="h3"){
					data.pop();
				}
				data.push( "<h3>"+ $( children[i] ).text()+"</h3>" );
				prev="h3";
			}else if($(children[i]).is('dl')){
				prev="dl";
				var qchildren = $( children[i] ).children();
				data.push("<dl>");
				for(var j=0; j<qchildren.length; j++){
					data.push("<dd>" + this.processHTML(qchildren[j] ) + "</dd>");
				}
				data.push("</dl>");
			}else if($(children[i]).is('table')){
				prev="table";
				if ($(children[i]).is("table[class=toc]") ){
				
				} else if ($(children[i]).find("table").length>0) {
					var tmptab = $(children[i]).html();
					data.push(tmptab.replace( "/images/thumb/", this.ironUrl+"/images/thumb/") );
				} else {
					data.push("<table border=1>"+$(children[i]).html()+"</table>");
				}
			}
		}
		
		var str = "";
		for(var i=0; i< data.length; i++){
			str= str+data[i];
		}
		
		this.$.pageInfo.setContent(str);
		this.$.scroller.scrollTo(0,0);
		this.determineBookmark();
	},
	processHTML: function ( inData ){
		var children = $( inData ).contents();
		var str = "";
		for(var i=0; i<children.length; i++){
			if(! $(children[i]).is("a[class=image]") ){
				if( $(children[i] ).html() ) {
					str = str + children[i].outerHTML;
				}else {
					str = str +$( children[i] ).text();
				}
			}
		}
		str = str;
		return str;
	},
	openWikiWebSite: function() {
	 this.launchInBrowser( this.$.ironchariotsPage.getUrl() );
	},
	launchInBrowser: function(){
		if(this.currenturl) {
			window.location = this.currenturl;
		} else {
			window.location =  this.ironUrl;
		}
	},
	back: function( inSender, inEvent ) {
		if(this.pageHistory.length > 0){
			var his = this.pageHistory.pop();
			this.$.pageInfo.setValue( his );
			this.currenturl = this.urlhistory.pop();
			this.$.scroller.scrollTo(0,0);
		}
		if(this.pageHistory.length <= 0){
			this.doBackHide();
		}
	},
	getNewPage: function( inURL, inSender ){
		if(inURL.search("file://localhost") > -1){//chrome
			this.pageHistory.push( this.$.pageInfo.getContent() );
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			var newUrl = inURL.replace( "file://localhost", this.ironUrl );
			this.currenturl = newUrl;
			this.$.ironchariotsPage.setUrl(newUrl);
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
			
		} else if(inURL.search("file:///C:") > -1){//emulator
			this.pageHistory.push( this.$.pageInfo.getContent() );
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			var newUrl = inURL.replace( "file:///C:", this.ironUrl );
			this.currenturl = newUrl;
			this.$.ironchariotsPage.setUrl(newUrl);
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
			
		} else if(inURL.search("file://") > -1){//emulator
			this.pageHistory.push( this.$.pageInfo.getContent() );
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			var newUrl = inURL.replace( "file://", this.ironUrl );
			this.currenturl = newUrl;
			this.$.ironchariotsPage.setUrl(newUrl);
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
			
		} else if(inURL.search("C:/") > -1){//emulator
			this.pageHistory.push( this.$.pageInfo.getContent() );
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			var newUrl = inURL.replace( "C:/", this.ironUrl );
			this.currenturl = newUrl;
			this.$.ironchariotsPage.setUrl(newUrl);
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
		
		} else if(inURL === "/index.php?title=Arguments_for_the_existence_of_god" || inURL === "/index.php?title=Arguments_against_the_existence_of_god" 
				|| inURL === "/index.php?title=Common_objections_to_atheism_and_counter-apologetics" || inURL === "/index.php?title=Atheism" ){ 
			this.pageHistory.push( this.$.pageInfo.getContent() );
			
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			var newUrl =  this.ironUrl + inURL;
			this.currenturl = newUrl;
			this.$.ironchariotsPage.setUrl(newUrl);
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
			
		} else if(inURL.search("http://www.biblegateway.com") > -1 ) {
			this.$.biblequote.loadquote(inURL);
		} else if(inSender == null & inURL.search(this.ironUrl) > -1)
		{
			this.pageHistory.push( this.$.pageInfo.getContent() );
			if (this.currenturl) {
				this.urlhistory.push(this.currenturl);
			}
			this.currenturl = inURL;
			this.$.pageInfo.setContent("");
			this.$.ironchariotsPage.setUrl(inURL);
			this.$.ironchariotsPage.call();
			this.$.scrim.show();
		}else{
			window.location = inURL;
		}
		return true;
	},
	genericFailure: function(inResponse, error){
		this.log("Failed to launch browser");
		this.log(JSON.stringify(error));
	},
	findLink: function(inNode, inAncestor) {
		var n = inNode;
		while (n && n != inAncestor) {
			if (n.href) {
				return n.href;
			}
			n = n.parentNode;
		}
	},
	catchClick: function(inSender, inEvent) {
		enyo.keyboard.forceHide();
		var url = this.findLink(inEvent.target, this.hasNode());
		if (url) {
			this.getNewPage(url, inEvent);
			inEvent.preventDefault();
		} else {
			inEvent.preventDefault();
		}
		inEvent.stopPropagation();
	},
	disableActions: function(inSender, inEvent){
		enyo.keyboard.forceHide();
	},
	disableKeys: function (inSender, inEvent){
		inEvent.preventDefault();
		inEvent.stopPropagation();
		return true;
	},
	getCurrentPage: function(){
		return this.currenturl;
	},
	getCurrentTitle: function(){
		return this.pageTitle;
	},
	clearHistory: function(){
		this.currenturl = null;
		this.pageHistory = [];
		this.urlhistory = [];
	},
	hasHistory: function(){
		if(this.urlhistory){
			if(this.urlhistory.length >0 ) { 
				return true;
			}
		}
		return false;
	}
});