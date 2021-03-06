﻿
// This file is part of Natural Docs, which is Copyright © 2003-2017 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;


namespace CodeClear.NaturalDocs.Engine.Tokenization
	{

	/* Enum: FundamentalType
	 * 
	 * The type of token it is on the most basic level.
	 * 
	 *		Null - Returned when the token iterator is out of bounds.
	 *		LineBreak - A single line break in CR, LF, or CR/LF format.
	 *		Whitespace - A series of consecutive space and/or tab characters.
	 *		Text - A series of consecutive ASCII letters, numbers, and/or characters above ASCII 0x7F.
	 *					This does *not* include underscores.
	 *		Symbol - One character not mentioned above, which are all the symbol characters available
	 *						  on the standard US QWERTY keyboard plus ASCII control characters.
	 */
	public enum FundamentalType : byte
		{  Null = 0, LineBreak, Whitespace, Text, Symbol  }


	/* Enum: CommentParsingType
	 * 
	 * The type of token it is as is relevant to comment parsing.
	 * 
	 *		Null - Returned when the token iterator is out of bounds or if a token hasn't been assigned one of these 
	 *					values yet.
	 *		
	 *		CommentSymbol - A comment symbol or part of one.
	 *		CommentDecoration - A symbol that only provides decoration for a comment, such as part of 
	 *													a horizontal line.
	 *										 
	 *		PossibleOpeningTag - An opening symbol that's a candidate for being part of a link, bold, or underline tag.
	 *		PossibleClosingTag - A closing symbol that's a candidate for being part of a link, bold, or underline tag.
	 *		OpeningTag - An opening symbol that's a part of a link, bold, or underline tag.
	 *		ClosingTag - A closing symbol that's a part of a link, bold, or underline tag.
	 *		
	 *		URL - Part of an URL.
	 *		EMail - Part of an e-mail address.
	 */
	public enum CommentParsingType : byte
		{  
		Null = 0, 
		CommentSymbol, CommentDecoration,	
		PossibleOpeningTag, PossibleClosingTag,
		OpeningTag, ClosingTag,
		URL, EMail
		}


	/* Enum: SyntaxHighlightingType
	 * 
	 * The type of token it is as is relevant to prototype parsing.
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * Keyword - A reserved word, like "int".
	 * Number - A numeric constant, like "12", "0xFF", or "-1.5".  The format doesn't matter.
	 * String - A string.  Also covers char constants for languages that have them.
	 * Comment - A comment, both symols and content.
	 * PreprocessingDirective - A preprocessing directive such as "#define x".
	 * Metadata - Code metadata such as the "[Flags]" attribute in C#.
	 */
	public enum SyntaxHighlightingType :  byte
		{
		Null = 0,
		Keyword, Number, String, Comment,
		PreprocessingDirective, Metadata
		}


	/* Enum: PrototypeParsingType
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * 
	 * StartOfPrePrototypeLine - The first token of a new pre-prototype line.  Each token marked with this starts a new line.
	 * PrePrototypeLine - Part of a line that should be shown before the prototype.
	 * 
	 * StartOfParams - The start of a parameter list, such as an opening parenthesis.
	 * EndOfParams - The end of a parameter list, such as a closing parenthesis.
	 * ParamSeparator - A separator between parameters, such as a comma.
	 * 
	 * TypeModifier - A separate word modifying a type, such as "const" in "const int".
	 * TypeQualifier - Everything prior to the ending word in a qualified type, such as "PkgA.PkgB." in "PkgA.PkgB.Class".
	 * Type - The type excluding all modifiers and qualifiers, such as "int" in "unsigned int" or "Class" in "PkgA.PkgB.Class".
	 * OpeningTypeSuffix - An opening symbol after a type, such as "[" in "int[]" or "<" in "List<int>".
	 * ClosingTypeSuffix - A closing symbol after a type, such as "]" in "int[]" or ">" in "List<int>".
	 * TypeSuffix - A neutral symbol after a type, such as "^" in "integer^".
	 * 
	 * NameTypeSeparator - In languages that use them, the symbol separating a variable name from its type, such
	 *												 as ":" in "x: int".  In languages that simply use a space this type won't appear.
	 *											 
	 * NamePrefix_PartOfType - Any symbols appearing before a name, such as "*" in "int *x" or "$" in "$x", that are actually
	 *													  part of its type.  It doesn't matter if it's textually attached to the type ("int* x") because that
	 *													  still means the same thing ("int *x") in C++.
	 * Name - The name of the parameter or the code element being defined by the prototype.
	 * NameSuffix_PartOfType - Any symbols appearing after a name, such as "[]" in "int x[]", that are actually part of its type.  
	 *														Unlike with types, we don't have to distinguish between opening and closing symbols to search
	 *														for nested types.
	 *														
	 * DefaultValueSeparator - The symbol separating the name and type from its default value, such as "=" or ":=".
	 * DefaultValue - The default value of the parameter.
	 * 
	 * StartOfPostPrototypeLine - The first token of a new post-prototype line.  Each token marked with this starts a new line.
	 * PostPrototypeLine - Part of a line that should be shown after the prototype.
	 */
	public enum PrototypeParsingType :  byte
		{
		Null = 0,

		StartOfPrePrototypeLine, PrePrototypeLine,

		StartOfParams, EndOfParams, ParamSeparator,

		TypeModifier, TypeQualifier, Type, OpeningTypeSuffix, ClosingTypeSuffix, TypeSuffix,

		NameTypeSeparator, 
		
		NamePrefix_PartOfType, Name, NameSuffix_PartOfType,

		DefaultValueSeparator, DefaultValue,

		StartOfPostPrototypeLine, PostPrototypeLine
		}


	/* Enum: ClassPrototypeParsingType
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * 
	 * StartOfPrePrototypeLine - The first token of a new pre-prototype line.  Each token marked with this starts a new line.
	 * PrePrototypeLine - Part of a line that should be shown before the prototype.
	 * 
	 * StartOfParents - The start of a parent list.
	 * ParentSeparator - A separator between parents, such as a comma.
	 * EndOfParents - The end of a parent list for languages like Python that use parenthesis.
	 * 
	 * Modifier - A separate word modifying the class or parent, such as "public" or "static".
	 * Keyword - The keyword used to declare the class, such as "class", "struct", or "interface".
	 * Name - The name of the class or parent including qualifiers, such as "PkgA.PkgB.Class".
	 * 
	 * TemplateSuffix - Extra template information after a class or parent, such as "<T>" in "List<T>".
	 * 
	 * StartOfPostPrototypeLine - The first token of a new post-prototype line.  Each token marked with this starts a new line.
	 * PostPrototypeLine - Part of a line that should be shown after the prototype.
	 * 
	 * StartOfBody - The start of the class's body if it is present in the prototype.  Nothing beyond that is demarcated, including the end
	 *								of the body.  This token is just present to indicate that the prototype does contain a body.
	 */
	public enum ClassPrototypeParsingType :  byte
		{
		Null = 0,

		StartOfPrePrototypeLine, PrePrototypeLine,

		StartOfParents, ParentSeparator, EndOfParents,

		Modifier, Keyword, Name,

		TemplateSuffix,

		StartOfPostPrototypeLine, PostPrototypeLine,

		StartOfBody
		}

	}