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
	name: "com.iCottrell.IronChariotsBibleDialog",
	kind: "Dialog", 
	layoutKind: "VFlexLayout",
	style: "overflow: hidden",
	components: [
		{name: "biblequote", kind: "WebService", onSuccess: "gotBibleResult", handleAs: "text", onFailure: "pageFailed"},
		{kind: "RowGroup", caption:"Bible Quote", components:[ 
			{name: "bibletitle", className: "quoteheader"},
			{name: "quotetxt", className: "quotetxt"}, 
			{kind: "Button", caption: "Close", onclick:"closeDialog"}
		]}, 
		{kind: "Scrim", layoutKind: "VFlexLayout", align: "center", pack: "center",components: [
			{kind: "SpinnerLarge", showing: true}
		]}
	],
	create: function() {
		this.inherited(arguments);
	},
	loadquote: function ( inURL ) {
		this.open();
		this.$.bibletitle.setContent("");
		this.$.quotetxt.setContent("");
		this.$.biblequote.setUrl(inURL);
		this.$.scrim.show();
		this.$.biblequote.call();
	},
	gotBibleResult: function (inSender, inResponse) { 
		this.$.bibletitle.setContent($( inResponse, null ).find("div[class='heading passage-class-0']").first().find("h3").first().text());
		var quote = $( inResponse, null ).find('div[class="result-text-style-normal text-html "]').first().text();
		while(quote.length > 0){
			console.log(this.hasWhiteSpace(quote.charAt(0)));
			if(!this.hasWhiteSpace(quote.charAt(0)) & isNaN( parseInt(quote.charAt(0), 10) )) {
				break;
			}
			quote = quote.substr(1);
		}
		this.$.quotetxt.setContent(quote);
		this.$.scrim.hide();
	},
	closeDialog: function ( inSender, inEvent ){
		this.close();
		this.$.scrim.hide();
	},
	pageFailed: function ( ) {
		this.$.scrim.hide();
		this.$.quotetxt.setContent("Bible passage could not be found.");
	},
	hasWhiteSpace: function (s) {
	  return /\s/g.test(s);
	}
});