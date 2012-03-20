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
	name: "com.iCottrell.IronChariotsBookmarks",
	kind: "VFlexBox",
	style: "background-color: white",
	events: {
		onBookmarkSelected: "",
		onBookmarkDeleted: "",
	},
	components: [
		{kind:"PageHeader", className:"enyo-header-dark", components:[
			{content:"Bookmarks"}
		]},
		{flex: 1, name: "list", kind: "VirtualList", className: "list", onSetupRow: "listSetupRow", components: [
			{kind: "Divider"},
			{kind: "SwipeableItem", className: "item", onclick:"itemClick", onConfirm: "deleteItem", components: [
				{name: "itemName", flex: 1},
			]}
		]}
	],
	create: function() {
		this.inherited(arguments);
		this.bookmarks = [];
		this.selectedBookmark = null;
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
	loadData: function() {
		if(this.icDB){
			var queryBookmarks = 'SELECT title FROM bookmarks;'
			try {
				this.icDB.transaction(
					enyo.bind(this, (function (transaction) {
						transaction.executeSql(queryBookmarks, [], enyo.bind(this, this.queryDataHandler), enyo.bind(this,this.errorHandler));
					}))
				);
			}
			catch(e){
				this.error(e);
			}
		}
	},
	queryDataHandler: function(transaction, results) {
		this.bookmarks = [];
		try{
			for(var i=0; i < results.rows.length; i++){
				this.bookmarks.push( results.rows.item(i).title );
			}
		}
		catch(e) {
			this.error(e);
		}
		this.bookmarks.sort(function(inA, inB) {
			var an = inA;
			var bn = inB;
			if (an < bn) return -1;
			if (an > bn) return 1;
			return 0;
		});
		this.$.list.refresh();
	},
	errorHandler: function(transaction, error) {
		this.log(error);
	},
	getGroupName: function(inIndex) {
		// get previous record
		var r0 = this.bookmarks[inIndex -1];
		// get (and memoized) first letter of last name
		if (r0 && !r0.letter) {
			r0.letter = r0[0];
		}
		var a = r0 && r0.letter;
		// get record
		var r1 = this.bookmarks[inIndex];
		if (!r1.letter) {
			r1.letter = r1[0];
		}
		var b = r1.letter;
		// new group if first letter of last name has changed
		return a != b ? b : null;
	},
	setupDivider: function(inIndex) {
		// use group divider at group transition, otherwise use item border for divider
		var group = this.getGroupName(inIndex);
		this.$.divider.setCaption(group);
		this.$.divider.canGenerate = Boolean(group);
		this.$.swipeableItem.applyStyle("border-top", Boolean(group) ? "none" : "1px solid silver;");
	},
	listSetupRow: function(inSender, inIndex) {
		var record = this.bookmarks[inIndex];
		if (record) {
			// bind data to item controls
			this.setupDivider(inIndex);
			this.$.swipeableItem.applyStyle("background-color", inSender.isSelected(inIndex) ? "lightgrey" : null);
			this.$.itemName.setContent(record);
			
			//if(inSender.isSelected(inIndex)){
			//	this.selectedBookmark = this.bookmarks[inIndex];
			//	this.doBookmarkSelected();
			//}
			return true;
		}
	},
	itemClick: function(inSender, inEvent, inRowIndex){
		this.$.list.select(inRowIndex);
		this.selectedBookmark = this.bookmarks[inRowIndex];
		this.doBookmarkSelected();
		return true;
	},
	deleteItem: function (inSender, inIndex){
		var sqlDelete = 'DELETE FROM bookmarks WHERE title="'+this.bookmarks[inIndex]+'";'
		try {
			this.icDB.transaction(
				enyo.bind(this, (function (transaction) {
					transaction.executeSql(sqlDelete, [], enyo.bind(this, this.deleteDataHandler), enyo.bind(this,this.errorHandler));
				}))
			);
		}
		catch(e){
			this.error(e);
		}
	},
	deleteDataHandler: function(){
		this.loadData();
		this.$.list.refresh();
		this.doBookmarkDeleted();
	},
});