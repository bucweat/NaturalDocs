﻿/* 
 * Class: GregValure.NaturalDocs.Engine.Output.FileMenuEntries.RootFolder
 * ____________________________________________________________________________
 * 
 * Represents the root folder in a <FileMenu>.  This may be either the bottom root containing the 
 * entire menu, or additional roots created to allow dynamic folders.
 */

// This file is part of Natural Docs, which is Copyright © 2003-2012 Greg Valure.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;


namespace GregValure.NaturalDocs.Engine.Output.FileMenuEntries
	{
	public class RootFolder : Container
		{

		// Group: Functions
		// __________________________________________________________________________

		public RootFolder () : base ()
			{
			id = 0;
			}


		// Group: Properties
		// __________________________________________________________________________

		/* Property: SortString
		 * Returns the string that should be used to sort this entry in a list.
		 */
		override public string SortString
			{  
			get
				{  return "Root";  }
			}

		/* Property: ID
		 */
		public int ID
			{
			get
				{  return id;  }
			set
				{  id = value;  }
			}


		// Group: Variables
		// __________________________________________________________________________

		protected int id;

		}
	}