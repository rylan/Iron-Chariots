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
	name: "com.iCottrell.IronChariotsStartPage.Phone",
	kind: enyo.VFlexBox, 
	style: "background-color: white",
	events: {
		onTopicClick: "",
	},
	components:[ 
		{kind: "Scroller", flex:1, components:[
		{kind: "VFlexBox", style:"background-color:#343434;-webkit-border-radius:5px; boarder:1px solid #000; padding:10px; margin:10px;", components:[
			{content: "Starting Points", style:"margin-left:5px; margin-bottom:5px; color:white; font-weight:bold;"},
			{kind: "Image", src:"img/Button_argumentsfor.png", style:"width:200px;",  onclick:"afeg"},
			{kind: "Image", src:"img/Button_argumentsagainst.png", style:"width:200px;", onclick:"aaeg"},
			{kind: "Image", src:"img/Button_commonobjections.png", style:"width:200px;", onclick:"coaca"},
			{kind: "Image", src:"img/Button_atheism.png", style:"width:200px;", onclick:"atheism"}
		]},
		{kind: "VFlexBox", style:"background-color:#343434;-webkit-border-radius:5px; boarder:1px solid #000; padding:10px; margin:10px;", components:[
			{content: "Feature Articles", style:"margin-left:5px; margin-bottom:5px; color:white; font-weight:bold;"},
			{kind: "HtmlContent", srcId: "featurearticles", className:"startpage", onLinkClick: "featureclick"}
			
		]}
		]}
	],
	create: function() { 
		this.inherited( arguments );
		this.urlClick = null;
	},
	initComponents: function() {
		this.inherited( arguments );
	},
	featureclick: function(inSender, inURL){
		if(inURL.search("file://localhost") > -1){//chrome
			this.urlClick = inURL.replace( "file://localhost", "" );
		} else if(inURL.search("file:///C:") > -1){//emulator
			this.urlClick = inURL.replace( "file:///C:", "" );
		} else if(inURL.search("file://") > -1){//emulator
			this.urlClick = inURL.replace( "file://", "" );
		} else if(inURL.search("C:/") > -1){//emulator
			this.urlClick = inURL.replace( "C:/", "" );
		} 
		this.doTopicClick();	
	},
	afeg: function(){
		this.urlClick = "/index.php?title=Arguments_for_the_existence_of_god";
		this.doTopicClick();
	},
	aaeg: function() {
		this.urlClick = "/index.php?title=Arguments_against_the_existence_of_god";
		this.doTopicClick();
	},
	coaca: function() {
		this.urlClick = "/index.php?title=Common_objections_to_atheism_and_counter-apologetics";
		this.doTopicClick();
	},
	atheism: function() {
		this.urlClick = "/index.php?title=Atheism";
		this.doTopicClick();
	}
});