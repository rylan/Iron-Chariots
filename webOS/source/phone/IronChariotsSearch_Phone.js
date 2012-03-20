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
	name: "com.iCottrell.IronChariotsSearch.Phone",
	kind: enyo.VFlexBox, 
	style: "background-color: white",
	events: {
		onResultItemSelected: "",
		onEnablePaste: "",
	},
	components:[
		{name: "ironchariotsSearch", kind: "WebService", onSuccess: "gotSearchResults", onFailure: "searchFailed"},
		{kind: "RowGroup", caption: "Search Wiki",  components:[
			{kind: "HFlexBox", components:[ 
				{kind: "SearchInput", flex:1, name: "searchWord", hint: enyo._$L("Tap Here To Type"), autoCapitalize: "sentence", spellcheck: true, keypressInputDelay: 500, oninput: "searchWiki", onfocus: "enableActions"},
				{kind: "Spinner", name:"actSearch", showing:false}
			]}
		]},
		{flex: 2, name: "resultlist", kind: "VirtualList", className: "resultlist", onSetupRow: "listSetupRow", showing:false, components: [
			{kind: "Divider", name: "resultDivider"},
			{kind: "Item", className: "item", onclick:"itemClick", components: [
				{name: "result_title", className: "resultTitle"},
				{name: "result_descript", className:"resultDescription"}
			]}
		]},
		{flex: 2, name:"startpage", kind:"com.iCottrell.IronChariotsStartPage.Phone", onTopicClick:"topicClick"},
		{name: "errorDialog", kind: "Dialog", components: [
			{className: "enyo-item enyo-first", style: "padding: 12px", content: "Error performing search, please check data connection."},
			{kind: "Button", caption: "OK", onclick:"closeDialog"}
		]},
		{kind: "Scrim", layoutKind: "VFlexLayout", align: "center", pack: "center",components: [
			{kind: "SpinnerLarge", showing:true}
		]}
	],
	create: function() { 
		this.inherited(arguments); 
		this.resultshtml = "";
		this.results = [];
		this.selected = null;
		this.$.searchWord.forceFocus();
		this.workingON = [];
		this.spVisible = true;
	},
	searchWiki: function(){
		if(this.$.searchWord){
			this.$.actSearch.show();
			if(! (this.$.searchWord.getValue().length <= 3 )){
				var url = "http://wiki.ironchariots.org/index.php?search="+this.$.searchWord.getValue()+"&fulltext=Search&limit=100&offset=0";
				this.$.ironchariotsSearch.setUrl(url);
				this.workingON.push(url);
				this.$.ironchariotsSearch.call();
			}else{
				this.$.actSearch.hide();
			}
		}
	},
	isStartPageShowing: function(){
		return this.spVisible;
	},
	gotSearchResults: function(inSender, inResponse){
		if(inResponse) {
			this.$.resultlist.show();
			this.$.startpage.hide();
			this.spVisible = false;
			this.workingON.pop();
			this.results = [];
			resulthtml = inResponse;
			if(this.hasResults(resulthtml)){
				this.processHTML(resulthtml);
			} 
			if(this.workingON.length <= 0){
				this.$.actSearch.hide();
			}
			this.$.resultlist.refresh();
		} else {
			this.$.errorDialog.toggleOpen();
		}
	}, 
	closeDialog: function(){
		this.$.errorDialog.toggleOpen();
	},
	searchFailed: function(inSender, inResponse){
		if(this.workingON.length <= 0){
			this.$.actSearch.hide();
		}
	},
	setupDivider: function(inIndex){
		var group = this.getGroupName(inIndex);
		this.$.resultDivider.setCaption(group);
		this.$.resultDivider.canGenerate = Boolean(group);
		this.$.item.applyStyle("border-top", Boolean(group) ? "none" : "1px solid silver;");
	},
	listSetupRow: function(inSender, inIndex){
		if(this.results[inIndex]){
			this.setupDivider(inIndex);
			this.$.item.applyStyle("background-color", inSender.isSelected(inIndex) ? "lightblue" : null);
			this.$.result_title.setContent(this.results[inIndex].title);
			this.$.result_descript.setContent(this.results[inIndex].description);
			if(inSender.isSelected(inIndex)){
				this.selected = this.results[inIndex];
			}
			return true;
		}
	},
	getGroupName: function(inIndex){
		var a = null;
		if(this.results[inIndex-1]){ 
			a = this.results[inIndex-1].category;
		}
		var b = this.results[inIndex].category;
		return a!=b ? b: null;
	},
	processHTML: function(resulthtml){
		var jq = $(resulthtml,null);
		var ol = jq.find('ol');
		var h2 = jq.find('h2');
		for(var i=0; i<ol.length; i++){
			var items = $(ol[i]).find('li');
			for(var j=0; j<items.length; j++){
				var data = new Object();
				data.description = "";
				data.category =  $( h2[i] ).contents().text();
				data.title = $( $( items[j] ).find('a')[0] ).attr('title');
				data.url = $( $( items[j] ).find('a')[0] ).attr('href');
				var tmp = $( items[j] ).find('small');
				for(var k=0; k< tmp.length; k++){
					data.description +=$( tmp[k] ).contents().text();
					data.description +=" ";
				}
				this.results.push(data);
			}
		}
	},
	hasResults: function(resulthtml){
		var h2 = $(resulthtml,null).find('h2');
		var article = $( h2[0] ).contents().text();
		var text =  $( h2[1] ).contents().text();
		return !( article == "No page title matches" &&  text == "No page text matches" );
	},
	itemClick: function(inSender, inEvent, inRowIndex){
		if(!this.$.resultlist.isSelected(inRowIndex)){
			this.$.resultlist.select(inRowIndex);
		}
		this.doResultItemSelected();
		return true;
	},
	getSelected: function(){
		return this.selected;
	},
	refresh: function(){
		this.$.resultlist.refresh();
		this.$.searchWord.forceFocus();
	},
	enableActions: function(inSender, inEvent){
		enyo.keyboard.forceShow(0);
		this.doEnablePaste();
	},
	topicClick: function(inSender) {
		var result = new Object();
		result.url = inSender.urlClick;
		this.selected = result;
		this.doResultItemSelected();
	},
	showStartPage: function(){
		this.$.startpage.show();
		this.$.resultlist.hide();
		this.spVisible = true;
	},
});