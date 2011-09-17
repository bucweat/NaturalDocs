﻿/*
	Include in output:

	This file is part of Natural Docs, which is Copyright © 2003-2011 Greg Valure.
	Natural Docs is licensed under version 3 of the GNU Affero General Public
	License (AGPL).  Refer to License.txt for the complete details.

	This file may be distributed with documentation files generated by Natural Docs.
	Such documentation is not covered by Natural Docs' copyright and licensing,
	and may have its own copyright and distribution terms as decided by its author.

	This file includes code derived from jQuery HashChange Event, which is 
	Copyright © 2010 "Cowboy" Ben Alman.  jQuery HashChange Event may be 
	obtained separately under the MIT license or the GNU General Public License (GPL).  
	However, this combined product is still licensed under the terms of the AGPLv3.

*/

"use strict";


/* Class: NDContentPage
	_____________________________________________________________________________

*/
var NDContentPage = new function ()
	{

	// Group: Functions
	// ________________________________________________________________________


	/* Function: Start
	*/
	this.Start = function ()
		{
		// Using onresize completely freezes IE 6, not worth figuring out why.
		// IE 7 doesn't return the proper measurements for prototype reformatting.
		if (NDCore.IsIE() && NDCore.IEVersion() < 8)
			{  return;  }

		this.CalculateShortFormPrototypeWidths();
		this.ReformatPrototypes();

		window.onresize = this.OnResize;
		};


	/* Function: OnResize
	*/
	this.OnResize = function ()
		{
		// Called by browser so you can't rely on "this".

		// Limit reformatting to avoid unnecessary CPU usage.  Some pages may have a lot of prototypes.  However, don't reset
		// the timeout on each event because otherwise we have to wait until the user completely stops dragging.
		if (NDContentPage.reformatPrototypesTimeout == undefined)
			{
			NDContentPage.reformatPrototypesTimeout = setTimeout("NDContentPage.ReformatPrototypes()", 200);
			}
		};



	// Group: Prototype Functions
	// ________________________________________________________________________


	/* Function: GetPrototypeIDNumber
		Returns the prototype ID number in numeric form (234 for "Prototype234") for the passed element, or -1 if it can't be found.
	*/
	this.GetPrototypeIDNumber = function (element)
		{
		if (element.id.indexOf("NDPrototype") == 0)
			{
			// Extract 234 from "NDPrototype234".
			var id = parseInt(element.id.substr(11), 10);

			if (id != NaN && id > 0)
				{  return id;  }
			}

		return -1;
		};

	
	/* Function: CalculateShortFormPrototypeWidths
		Goes through all the short form prototypes and records their widths into <shortFormPrototypeWidths>.
	*/
	this.CalculateShortFormPrototypeWidths = function ()
		{
		var prototypes = NDCore.GetElementsByClassName("NDPrototype", "div");

		for (var i = 0; i < prototypes.length; i++)
			{
			if (NDCore.HasClass(prototypes[i], "ShortForm"))
				{
				var id = this.GetPrototypeIDNumber(prototypes[i]);

				if (id != -1)
					{  
					// First child is the layout table.
					this.shortFormPrototypeWidths[id] = prototypes[i].firstChild.offsetWidth;  
					}
				}
			}
		};


	/* Function: ReformatPrototypes
		Switches each prototype between the short and long form depending on the amount of space it has.
	*/
	this.ReformatPrototypes = function ()
		{
		var prototypes = NDCore.GetElementsByClassName("NDPrototype", "div");

		for (var i = 0; i < prototypes.length; i++)
			{
			var id = this.GetPrototypeIDNumber(prototypes[i]);

			if (id == -1)
				{  continue;  }

			var shortFormWidth = this.shortFormPrototypeWidths[id];

			if (shortFormWidth == null || shortFormWidth <= 0)
				{  continue;  }

			var availableWidth = prototypes[i].offsetWidth;

			// availableWidth includes padding, so remove it by comparing its offset to its child's.
			// We can only get the left padding so assume the right is the same and double it.
			availableWidth -= (prototypes[i].firstChild.offsetLeft - prototypes[i].offsetLeft) * 2;

			// Remove an extra pixel since some browsers add the scrollbar when they're exactly equal.
			availableWidth--;

			if (availableWidth >= shortFormWidth && NDCore.HasClass(prototypes[i], "LongForm"))
				{  this.ChangePrototypeToShortForm(prototypes[i]);  }
			else if (availableWidth < shortFormWidth && NDCore.HasClass(prototypes[i], "ShortForm"))
				{  this.ChangePrototypeToLongForm(prototypes[i]);  }
			}

		if (this.reformatPrototypesTimeout != undefined)
			{
			clearTimeout(this.reformatPrototypesTimeout);
			this.reformatPrototypesTimeout = undefined;
			}
		};


	/* Function: ChangePrototypeToLongForm
		Changes the passed NDPrototype element to use the long form.  The prototype *must* be in the short form.
	*/
	this.ChangePrototypeToLongForm = function (prototype)
		{
		var newPrototype = document.createElement("div");
		newPrototype.id = prototype.id;
		newPrototype.className = prototype.className;

		NDCore.RemoveClass(newPrototype, "ShortForm");
		NDCore.AddClass(newPrototype, "LongForm");

		var table = prototype.firstChild;
		var newTable = document.createElement("table");
		newPrototype.appendChild(newTable);

		var newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[0].cloneNode(true));

		newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[1].cloneNode(true));

		newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[2].cloneNode(true));

		prototype.parentNode.replaceChild(newPrototype, prototype);
		};

	
	/* Function: ChangePrototypeToShortForm
		Changes the passed NDPrototype element to use the short form.  The prototype *must* be in the long form.
	*/
	this.ChangePrototypeToShortForm = function (prototype)
		{
		var newPrototype = document.createElement("div");
		newPrototype.id = prototype.id;
		newPrototype.className = prototype.className;

		NDCore.RemoveClass(newPrototype, "LongForm");
		NDCore.AddClass(newPrototype, "ShortForm");

		var table = prototype.firstChild;
		var newTable = document.createElement("table");
		newPrototype.appendChild(newTable);

		var newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[0].cloneNode(true));
		newRow.appendChild(table.rows[1].cells[0].cloneNode(true));
		newRow.appendChild(table.rows[2].cells[0].cloneNode(true));

		prototype.parentNode.replaceChild(newPrototype, prototype);
		};



	// Group: Variables
	// ________________________________________________________________________


	/* var: shortFormPrototypeWidths
		Maps prototype ID numbers to the pixel widths of their short form.
	*/
	this.shortFormPrototypeWidths = { };


	/* var: reformatPrototypesTimeout
		The ID of the prototype reflow timeout if one is running.
	*/
	// this.reformatPrototypesTimeout = undefined;


	};