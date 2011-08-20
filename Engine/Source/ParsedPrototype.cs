﻿/* 
 * Class: GregValure.NaturalDocs.Engine.ParsedPrototype
 * ____________________________________________________________________________
 * 
 * A class that wraps a <Tokenizer> for a prototype that's been marked with <PrototypeParsingTypes>, providing easier 
 * access to things like parameter lines.
 * 
 * Usage:
 * 
 *		The functions and properties obviously rely on the relevant tokens being set.  You cannot expect a proper result from
 *		<GetParameter()> or <NumberOfParameters> unless the tokens are marked with <PrototypeParsingTypes.StartOfParams>
 *		and <PrototypeParsingTypes.ParamSeparator>.  Likewise, you can't get anything from <GetParameterName()> unless
 *		you also have tokens marked with <PrototypeParsingTypes.Name>.  However, you can set the parameter divider tokens,
 *		call <GetParameter()>, and then use those bounds to further parse the parameter and set tokens like
 *		<PrototypeParsingTypes.Name>.
 * 
 *		An important thing to remember though is that the parameter divisions are calculated once and saved.  Only call
 *		functions like <GetParameter()> after *ALL* the separator tokens (<PrototypeParsingTypes.StartOfParams>,
 *		<PrototypeParsingTypes.ParamSeparator>, and <PrototypeParsingTypes.EndOfParams>) are set and will not change
 *		going forward.
 */

// This file is part of Natural Docs, which is Copyright © 2003-2011 Greg Valure.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;
using System.Collections.Generic;
using GregValure.NaturalDocs.Engine.Tokenization;


namespace GregValure.NaturalDocs.Engine
	{
	public class ParsedPrototype
		{

		// Group: Functions
		// __________________________________________________________________________
		
		
		/* Constructor: ParsedPrototype
		 * Creates a new parsed prototype.
		 */
		public ParsedPrototype (Tokenizer prototype)
			{
			tokenizer = prototype;
			sectionBounds = null;
			}

			
		/* Function: GetBeforeParameters
		 * Returns the bounds of the section of the prototype prior to the parameters.  If it has parameters, it will include the 
		 * starting symbol of the parameter list such as the opening parenthesis.  If there are no parameters, this will return the 
		 * bounds of the entire prototype.
		 */
		public void GetBeforeParameters (out TokenIterator start, out TokenIterator end)
			{
			if (sectionBounds == null)
				{  CalculateSectionBounds();  }

			start = tokenizer.FirstToken;
			start.Next(sectionBounds[0]);

			end = tokenizer.FirstToken;
			end.Next(sectionBounds[1]);
			}


		/* Function: GetParameter
		 * Returns the bounds of the numbered parameter and whether or not it exists.  Numbers start at zero.
		 */
		public bool GetParameter (int index, out TokenIterator start, out TokenIterator end)
			{
			if (sectionBounds == null)
				{  CalculateSectionBounds();  }

			if (index >= NumberOfParameters)
				{
				start = tokenizer.LastToken;
				end = tokenizer.LastToken;
				return false;
				}
			else
				{			
				start = tokenizer.FirstToken;
				start.Next(sectionBounds[2 + (index * 2)]);

				end = tokenizer.FirstToken;
				end.Next(sectionBounds[3 + (index * 2)]);

				return true;
				}
			}


		/* Function: GetParameterName
		 * Returns the bounds of the name of the passed parameter, or false if it couldn't find it.
		 */
		public bool GetParameterName (int index, out TokenIterator start, out TokenIterator end)
			{
			TokenIterator paramStart, paramEnd;

			if (!GetParameter(index, out paramStart, out paramEnd))
				{  
				start = paramEnd;
				end = paramEnd;
				return false;  
				}

			start = paramStart;

			while (start < paramEnd && start.PrototypeParsingType != PrototypeParsingType.Name)
				{  start.Next();  }

			if (start < paramEnd)
				{
				end = start;

				do
					{  end.Next();  }
				while (end.PrototypeParsingType == PrototypeParsingType.Name && end < paramEnd);

				return true;
				}
			else
				{  
				start = paramEnd;
				end = paramEnd;
				return false;  
				}
			}


		/* Function: GetFullParameterType
		 * Returns the bounds of the type of the passed parameter, or false if it couldn't find it.  This includes modifiers and type
		 * suffixes.  If there's any additional type information that appears after the name (int x[12]) that will be returned in the
		 * extension.
		 */
		public bool GetFullParameterType (int index, out TokenIterator start, out TokenIterator end, 
																			out TokenIterator extensionStart, out TokenIterator extensionEnd)
			{
			TokenIterator paramStart, paramEnd;

			if (!GetParameter(index, out paramStart, out paramEnd))
				{  
				start = paramEnd;
				end = paramEnd;
				extensionStart = paramEnd;
				extensionEnd = paramEnd;
				return false;  
				}

			start = paramStart;
			PrototypeParsingType type = start.PrototypeParsingType;

			while (start < paramEnd && 
						type != PrototypeParsingType.Type &&
						type != PrototypeParsingType.TypeModifier &&
						type != PrototypeParsingType.TypeQualifier &&
						type != PrototypeParsingType.NamePrefix_PartOfType)
				{  start.Next();  }

			if (start < paramEnd)
				{
				end = start;
				int nestLevel = 0;

				do
					{  
					if (end.PrototypeParsingType == PrototypeParsingType.OpeningTypeSuffix)
						{  nestLevel++;  }
					else if (end.PrototypeParsingType == PrototypeParsingType.ClosingTypeSuffix)
						{  nestLevel--;  }

					end.Next();
					type = end.PrototypeParsingType;
					}
				while ( (nestLevel > 0 ||
							  type == PrototypeParsingType.Type ||
							  type == PrototypeParsingType.TypeModifier ||
							  type == PrototypeParsingType.TypeQualifier ||
							  type == PrototypeParsingType.OpeningTypeSuffix ||
							  type == PrototypeParsingType.ClosingTypeSuffix ||
							  type == PrototypeParsingType.TypeSuffix ||
							  type == PrototypeParsingType.NamePrefix_PartOfType ||
							  end.FundamentalType == FundamentalType.Whitespace) &&
							end < paramEnd);

				// Find an extension if there is one.
				extensionStart = end;

				while (extensionStart < paramEnd &&
							 extensionStart.PrototypeParsingType != PrototypeParsingType.NameSuffix_PartOfType)
					{  extensionStart.Next();  }

				extensionEnd = extensionStart;

				while (extensionEnd < paramEnd &&
							 extensionEnd.PrototypeParsingType == PrototypeParsingType.NameSuffix_PartOfType)
					{  extensionEnd.Next();  }

				// Trim trailing whitespace from the regular type
				TokenIterator temp = end;
				temp.Previous();

				while (temp.FundamentalType == FundamentalType.Whitespace)
					{
					end = temp;
					temp.Previous();
					}

				return true;
				}
			else
				{  
				start = paramEnd;
				end = paramEnd;
				extensionStart = paramEnd;
				extensionEnd = paramEnd;
				return false;  
				}
			}


		/* Function: GetBaseParameterType
		 * Returns the bounds of the type of the passed parameter, or false if it couldn't find it.  This excludes modifiers and type
		 * suffixes.
		 */
		public bool GetBaseParameterType (int index, out TokenIterator start, out TokenIterator end)
			{
			TokenIterator paramStart, paramEnd;

			if (!GetParameter(index, out paramStart, out paramEnd))
				{  
				start = paramEnd;
				end = paramEnd;
				return false;  
				}

			// First search for an explicit type, like "int".
			start = paramStart;

			while (start < paramEnd && 
						start.PrototypeParsingType != PrototypeParsingType.Type &&
						start.PrototypeParsingType != PrototypeParsingType.TypeQualifier)
				{  start.Next();  }

			if (start < paramEnd)
				{
				end = start;

				do
					{  end.Next();  }
				while ((end.PrototypeParsingType == PrototypeParsingType.Type ||
							 end.PrototypeParsingType == PrototypeParsingType.TypeQualifier) &&
							end < paramEnd);

				return true;
				}
			else
				{
				// If there's no explicit type, seach for a name prefix like "$", "@", or "%".
				start = paramStart;

				while (start < paramEnd && 
							start.PrototypeParsingType != PrototypeParsingType.NamePrefix_PartOfType)
					{  start.Next();  }

				if (start < paramEnd)
					{
					end = start;

					do
						{  end.Next();  }
					while (end.PrototypeParsingType == PrototypeParsingType.NamePrefix_PartOfType &&
								end < paramEnd);

					return true;
					}
				else
					{  
					start = paramEnd;
					end = paramEnd;
					return false;  
					}
				}
			}


		/* Function: GetAfterParameters
		 * Returns the bounds of the section of the prototype after the parameters and whether it exists.  If it does
		 * exist, the bounds will include the closing symbol of the parameter list such as the closing parenthesis.
		 */
		public bool GetAfterParameters (out TokenIterator start, out TokenIterator end)
			{
			if (sectionBounds == null)
				{  CalculateSectionBounds();  }

			if (NumberOfParameters == 0)
				{
				start = tokenizer.LastToken;
				end = tokenizer.LastToken;
				return false;
				}
			else
				{
				start = tokenizer.FirstToken;
				start.Next( sectionBounds[ sectionBounds.Length - 2 ] );

				end = tokenizer.FirstToken;
				end.Next( sectionBounds[ sectionBounds.Length - 1 ] );

				return true;
				}
			}
			

		/* Function: CalculateSectionBounds
		 */
		protected void CalculateSectionBounds ()
			{
			// Count the parameters

			int numberOfParameters = 0;
			TokenIterator iterator = tokenizer.FirstToken;

			while (iterator.IsInBounds && iterator.PrototypeParsingType != PrototypeParsingType.StartOfParams)
				{  iterator.Next();  }

			if (iterator.IsInBounds)
				{
				bool hasNonWhitespace = false;

				iterator.Next();
				numberOfParameters++;

				while (iterator.IsInBounds && iterator.PrototypeParsingType != PrototypeParsingType.EndOfParams)
					{
					if (iterator.PrototypeParsingType == PrototypeParsingType.ParamSeparator)
						{  numberOfParameters++;  }
					if (iterator.FundamentalType != FundamentalType.Whitespace)
						{  hasNonWhitespace = true;  }

					iterator.Next();
					}

				// Ignore empty parameter groups like Function() and Function( ).
				if (numberOfParameters == 1 && hasNonWhitespace == false)
					{  numberOfParameters = 0;  }
				}


			// Create the bounds array

			if (numberOfParameters == 0)
				{
				sectionBounds = new int[2];

				TokenIterator start = tokenizer.FirstToken;
				TokenIterator end = tokenizer.LastToken;

				TrimWhitespace(ref start, ref end);

				sectionBounds[0] = start.TokenIndex;
				sectionBounds[1] = end.TokenIndex;
				}

			else
				{
				sectionBounds = new int[ (numberOfParameters * 2) + 4 ];

				TokenIterator start, end;

				iterator = tokenizer.FirstToken;
				start = iterator;

				while (iterator.PrototypeParsingType != PrototypeParsingType.StartOfParams)
					{  iterator.Next();  }

				// Include StartOfParams in the first segment
				iterator.Next();
				end = iterator;

				TrimWhitespace(ref start, ref end);
				sectionBounds[0] = start.TokenIndex;
				sectionBounds[1] = end.TokenIndex;

				int boundsIndex = 2;

				do
					{
					start = iterator;

					while (iterator.IsInBounds && 
								iterator.PrototypeParsingType != PrototypeParsingType.EndOfParams &&
								iterator.PrototypeParsingType != PrototypeParsingType.ParamSeparator)
						{  iterator.Next();  }

					// Include ParamSeparator in the segment
					if (iterator.PrototypeParsingType == PrototypeParsingType.ParamSeparator)
						{  iterator.Next();  }

					end = iterator;

					TrimWhitespace(ref start, ref end);

					sectionBounds[boundsIndex] = start.TokenIndex;
					sectionBounds[boundsIndex+1] = end.TokenIndex;
					boundsIndex += 2;
					}
				while (iterator.IsInBounds && 
							iterator.PrototypeParsingType != PrototypeParsingType.EndOfParams);

				// Section after the parameters.  If there is none, these will both be on the end of the tokenizer and we add
				// them anyway.

				start = iterator;
				end = tokenizer.LastToken;

				TrimWhitespace(ref start, ref end);

				sectionBounds[boundsIndex] = start.TokenIndex;
				sectionBounds[boundsIndex+1] = end.TokenIndex;
				}
			}


		/* Function: TrimWhitespace
		 * Shrinks the passed bounds to exclude whitespace on the edges.
		 */
		protected void TrimWhitespace (ref TokenIterator start, ref TokenIterator end)
			{
			TokenIterator temp = end;
			temp.Previous();

			while (temp.FundamentalType == FundamentalType.Whitespace && temp >= start)
				{  
				end = temp;
				temp.Previous();
				}

			while (start.FundamentalType == FundamentalType.Whitespace && start < end)
				{  start.Next();  }
			}
			


		// Group: Properties
		// __________________________________________________________________________
		
		
		/* Property: Tokenizer
		 * The tokenized prototype.
		 */
		public Tokenizer Tokenizer
			{
			get
				{  return tokenizer;  }
			}


		/* Property: NumberOfParameters
		 */
		public int NumberOfParameters
			{
			get
				{  
				if (sectionBounds == null)
					{  CalculateSectionBounds();  }

				if (sectionBounds.Length == 2)
					{  return 0;  }
				else
					{  return (sectionBounds.Length / 2) - 2;  }
				}
			}
			

		
		// Group: Variables
		// __________________________________________________________________________
		
		/* var: tokenizer
		 * The <Tokenizer> containing the full prototype.
		 */
		protected Tokenizer tokenizer;

		/* var: sectionBounds
		 * An array of token indexes representing the start and stop of each section of the prototype, or null if it hasn't been
		 * calculated yet.  The first pair will be the start and stop of the section before the parameters.  If parameters exist,
		 * a subsequent pair will exist for each parameter and then one final one for the section after the parameters.
		 */
		protected int[] sectionBounds;

		}
	}