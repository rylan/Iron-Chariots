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
	name: "com.iCottrell.IronChariotsBugReport",
	kind: "ModalDialog", 
	layoutKind: "VFlexLayout",
	contentClassName:"bugreport",
	width: "600px",
	components: [
		{kind: "HFlexBox", onclick: "openForumsPage", style:"background-color:#2071bb;-webkit-border-radius:5px; boarder:1px solid #000; padding:10px; margin:10px;", components:[
			{name:"op1", flex:1, content:"Concerns about the content, click here to address them on the Iron Chariots forum.", flex:1, style:"color:white;"},
			{kind:"Image", flex:2, src:"img/conversation_large.png", style:"width:72px; margin-top:10px;margin-left:5px;"}
		]},
		{content:"", style:"padding:10px"},
		{kind: "HFlexBox", onclick:"sendEmail", style:"background-color: #2071bb;-webkit-border-radius:5px; boarder:1px solid #000; padding:10px; margin:10px;", components:[
			{name:"op2", flex:1, content:"Click here to report a problem with the rendering of information being displayed on the current page or other application specific concerns.", style:"color:white;"},
			{kind:"Image", flex:2, src:"img/bugreport_large.png", style:"width:72px; margin-top:10px;margin-left:5px;"}
		]},
		{kind: "Button", caption: $L("Close"), onclick:"close", className:"enyo-button-black", style: "margin-top:10px"},
		{name: "openEmail", kind: "PalmService", service: "palm://com.palm.applicationManager", method: "open", onSuccess: "openEmailSuccess", onFailure: "openEmailFailure", subscribe: true}
	],
	create: function(){
		this.inherited(arguments);
		this.pagelink = "";
	},
	openForumsPage: function() {
		window.location = "http://forum.ironchariots.org";
		this.close();
	},
	sendEmail: function(){
		/*try {

			this.$.openEmail.call({
				"id" : "com.palm.app.email",
				params: {
					"summary" : "TP - Iron Chariots Bug Report",
					"text" : "This page "+this.pagelink +" was found to have rendering issues.<br /><br /> Additional comments<br />-----------------------------",
					"recipients":[{
						"type":"email",
						"role": 1,
						"value" : "dev@icottrell.com",
						"contactDisplay" : "Developer iCottrell.com"
					}]
				}
			});
			}
			catch(error)
			{*/
				window.location = "mailto:dev@icottrell.com?subject=Iron%20Chariots%20Bug%20Report&body=This%20page%20"+this.pagelink +"%20was%20found%20to%20have%20rendering%20issues.%20Please%20report%20the%20device%20you%20were%20using%20and%20any%20additional%20information.";
			//}
	},
	openEmailSuccess: function(inSender, inResponse) {
        this.log("Open email success, results=" + enyo.json.stringify(inResponse));
		this.close();
    },          
    openEmailFailure: function(inSender, inError, inRequest) {
        this.log(enyo.json.stringify(inError));
		this.close();
    },
	openDialog: function (url){
		this.pagelink = url;
		this.openAtCenter();
	}
});