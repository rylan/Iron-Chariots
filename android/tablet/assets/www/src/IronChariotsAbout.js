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
	name: "com.iCottrell.IronChariotsAbout",
	kind: enyo.ModalDialog, 
	layoutKind: "VFlexLayout",
	width: "65%", 
	components: [
		{kind: "HFlexBox", components:[
			{kind: "Image", src:"img/ironchariots.png",  className:"aboutimg"},
			{kind:"VFlexBox", flex:1, components:[
				{content: "Iron Chariots", className:"abouth1", onclick: "openwiki"},
				{content: "The Counter Apologetics Wiki", className:"abouttxt"},
				{content: "Version 0.5.0", className: "aboutver"},
				{content: "Developed by iCottrell.com", className:"abouttxt"},
				{content: "Direct all inquries to dev@icottrell.com", className:"abouttxt"},
				{content: "This is an open source project http://http://rylan.github.com/Iron-Chariots--EnyoJS/", onclick: "openProject", className:"abouttxt"},
				{kind:"Spacer"},
				{content: "Content is available under Attribution-ShareAlike 2.5", className:"aboutver", onclick: "openlic"},
				{kind: "Image", src:"img/cc.png"},
				{content:"", height:"15px"},
				{kind: "HFlexBox", components:[
					{content:"Powered by "},
					{kind: "Image", src:"img/enyo_logo.png"},	
				]},
			]}
		]},
		
		
		{kind: "Button", caption: $L("Close"), onclick: "closePopup", className:"enyo-button-black", style: "margin-top:10px"}
	],
	create: function(){
		this.inherited(arguments);
	},
	openwiki: function(inSender, inEvent){
			this.launchSite("http://www.ironchariots.org");
	},
	openlic: function(inSender, inEvent){
		this.launchSite("http://creativecommons.org/licenses/by-sa/2.5/");
	},
	launchSite: function(url){
		window.location = url;
	},
	closePopup: function(){
		this.close();
	},
	openProject: function(){
		this.launchSite("http://creativecommons.org/licenses/by-sa/2.5/");
	}
});