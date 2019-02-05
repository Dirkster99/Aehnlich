namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Data.Entity.Design.PluralizationServices;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods and properties for text processing.
	/// </summary>
	public static class TextUtility
	{
		#region Public Methods

		/// <overloads>Ensures that the given text is quoted.</overloads>
		/// <summary>
		/// Ensures that the given text is quoted by '"' characters.
		/// </summary>
		/// <param name="text">The text to quote if necessary.</param>
		/// <returns>The text enclosed in quotes.</returns>
		public static string EnsureQuotes(string text) => EnsureQuotes(text, "\"", "\"");

		/// <summary>
		/// Ensures that the given text has the specified quotes at the start and end.
		/// </summary>
		/// <param name="text">The text to quote if necessary.</param>
		/// <param name="quote">The quote mark to use.</param>
		/// <returns>The text enclosed in quotes.</returns>
		public static string EnsureQuotes(string text, string quote) => EnsureQuotes(text, quote, quote);

		/// <summary>
		/// Ensures that the given text has the specified quotes at the start and end.
		/// </summary>
		/// <param name="text">The text to quote if necessary.</param>
		/// <param name="openQuote">The opening quote mark to use.</param>
		/// <param name="closeQuote">The closing quote mark to use.</param>
		/// <returns>The text enclosed in quotes.</returns>
		public static string EnsureQuotes(string text, string openQuote, string closeQuote)
		{
			Conditions.RequireReference(text, "text"); // An empty string is ok.
			Conditions.RequireString(openQuote, "openQuote");
			Conditions.RequireString(closeQuote, "closeQuote");

			bool needsOpenQuote = !text.StartsWith(openQuote);
			bool needsCloseQuote = !text.EndsWith(closeQuote);

			if (needsOpenQuote && needsCloseQuote)
			{
				text = string.Concat(openQuote, text, closeQuote);
			}
			else if (needsOpenQuote)
			{
				text = openQuote + text;
			}
			else if (needsCloseQuote)
			{
				text += closeQuote;
			}

			return text;
		}

		/// <summary>
		/// Replaces a character with its closest printable match.
		/// </summary>
		/// <remarks>
		/// This method makes the following simple substitutions: \r is left arrow,
		/// \n is down arrow, \t is right arrow, and null is "phi".  Vertical tabs and
		/// form feeds are replaced as well as a few other Unicode characters
		/// that normally render as multiple lines.
		/// </remarks>
		/// <param name="value">The character to evaluate.</param>
		/// <returns>The best matching printable character.</returns>
		public static char GetPrintableCharacter(char value)
		{
			// Unicode characters 0x2400-0x2426 are supposed to be "Control Pictures"
			// (i.e., graphical representations of the standard control codes).  However,
			// they all render as boxes with Courier New and other common fonts, so I'm
			// using different representations.
			switch (value)
			{
				case '\r':
					value = '\u2190'; // Left arrow
					break;

				case '\n':
					value = '\u2193'; // Down arrow
					break;

				case '\t':
					value = '\u2192'; // Right arrow
					break;

				// Sometimes text returned from unmanaged code will contain
				// embedded nulls.  If we don't translate them then none of the
				// output following them will be shown.
				case '\0':
					value = '\u03D5'; // Phi/nil
					break;

				// These characters cause a new line to be displayed in text boxes,
				// so we need to replace them with something else.
				case '\v':
					value = '\u240B'; // Vertical tab "picture"
					break;

				case '\f':
					value = '\u240C'; // Form feed "picture"
					break;

				case '\uF00B':
				case '\uF00C':
				case '\uF00D':
					value = '\u2426'; // Box or some other "picture"
					break;

				// All other characters get returned unchanged.
			}

			return value;
		}

		/// <summary>
		/// Replaces a substring within the given text using the given comparison type.
		/// </summary>
		/// <param name="text">The text to search.</param>
		/// <param name="oldValue">The substring to search for.</param>
		/// <param name="newValue">The string that should replace the substring.</param>
		/// <param name="comparisonType">The type of string comparison to perform.</param>
		/// <returns>The string with the substring instances replaced.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="oldValue"/>
		/// are null or empty.</exception>
		public static string Replace(string text, string oldValue, string newValue, StringComparison comparisonType)
		{
			// Note: It's ok if newValue is null or empty.
			if (string.IsNullOrEmpty(text))
			{
				throw Exceptions.NewArgumentNullException("text");
			}

			if (string.IsNullOrEmpty(oldValue))
			{
				throw Exceptions.NewArgumentNullException("oldValue");
			}

			string result = text;
			int currentIndex = text.IndexOf(oldValue, comparisonType);
			if (currentIndex >= 0)
			{
				int textLength = text.Length;
				int oldValueLength = oldValue.Length;
				int previousIndex = 0;
				StringBuilder sb = new StringBuilder(text.Length);
				while (currentIndex >= 0)
				{
					if (currentIndex > previousIndex)
					{
						sb.Append(text.Substring(previousIndex, currentIndex - previousIndex));
					}

					sb.Append(newValue);

					currentIndex += oldValueLength;
					previousIndex = currentIndex;
					if ((currentIndex + 1) < textLength)
					{
						currentIndex = text.IndexOf(oldValue, currentIndex + 1, comparisonType);
					}
					else
					{
						break;
					}
				}

				if (previousIndex < text.Length)
				{
					sb.Append(text.Substring(previousIndex));
				}

				result = sb.ToString();
			}

			return result;
		}

		/// <summary>
		/// Replaces the control characters in a string with the specified character.
		/// </summary>
		/// <param name="text">The text to update.</param>
		/// <param name="replacement">The character to substitute for control characters.</param>
		/// <returns>The text with the control characters replaced.</returns>
		public static string ReplaceControlCharacters(string text, char replacement)
		{
			string result = text;

			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder sb = new StringBuilder(text);
				int length = text.Length;
				for (int i = 0; i < length; i++)
				{
					if (char.IsControl(sb[i]))
					{
						sb[i] = replacement;
					}
				}

				result = sb.ToString();
			}

			return result;
		}

		/// <summary>
		/// Replaces the control characters in a string using <see cref="GetPrintableCharacter"/>.
		/// </summary>
		/// <param name="text">The text to update.</param>
		/// <returns>The text with the control characters replaced.</returns>
		public static string ReplaceControlCharacters(string text)
		{
			string result = text;

			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder sb = new StringBuilder(text);
				int length = text.Length;
				for (int i = 0; i < length; i++)
				{
					char oldChar = sb[i];
					char newChar = GetPrintableCharacter(oldChar);
					if (newChar != oldChar)
					{
						sb[i] = newChar;
					}
				}

				result = sb.ToString();
			}

			return result;
		}

		/// <summary>
		/// Tokenize the input string using a comma separator, a double quote delimiter, and trimming the resulting tokens.
		/// </summary>
		/// <param name="text">The text to tokenize.</param>
		/// <returns>A list of tokens.</returns>
		public static IList<string> SplitIntoTokens(string text) => SplitIntoTokens(text, ',', '"', true);

		/// <summary>
		/// Tokenize the input string using the specified separator (e.g., comma), delimiter (e.g., double quote),
		/// and trimming options.
		/// </summary>
		/// <remarks>
		/// It will tokenize a string like <c>"Test1","Test2", Test3, Test4</c> into:
		/// <code>
		/// Test1
		/// Test2
		/// Test3
		/// Test4
		/// </code>
		/// </remarks>
		/// <param name="text">The text to tokenize.</param>
		/// <param name="separator">The token separator character.  Typically, a comma.</param>
		/// <param name="delimiter">The token quote character.  Typically, a double quote.
		/// This can be used to enclose tokens that contain the separator character.
		/// To use this character inside a token, use it two consecutive times.
		/// </param>
		/// <param name="trimTokens">Whether the resulting tokens should have whitespace trimmed off.</param>
		/// <returns>A list of tokens.</returns>
		public static IList<string> SplitIntoTokens(string text, char separator, char? delimiter, bool trimTokens)
		{
			List<string> result = new List<string>();
			SplitIntoTokens(text, separator, delimiter, trimTokens, result);
			return result;
		}

		/// <summary>
		/// Tokenize the input string using the specified separator (e.g., comma), delimiter (e.g., double quote),
		/// and trimming options and add the output tokens to the specified collection.
		/// </summary>
		/// <param name="text">The text to tokenize.</param>
		/// <param name="separator">The token separator character.  Typically, a comma.</param>
		/// <param name="delimiter">The token quote character.  Typically, a double quote.
		/// This can be used to enclose tokens that contain the separator character.
		/// To use this character inside a token, use it two consecutive times.
		/// </param>
		/// <param name="trimTokens">Whether the resulting tokens should have whitespace trimmed off.</param>
		/// <param name="tokens">The collection to add the parsed tokens to.</param>
		/// <returns>True if all of the tokens were properly delimited (or were not delimited).
		/// False if the final token was open-delimited but never closed (which usually indicates a partial record).</returns>
		public static bool SplitIntoTokens(string text, char separator, char? delimiter, bool trimTokens, ICollection<string> tokens)
		{
			Conditions.RequireReference(tokens, "tokens");
			bool result = SplitIntoTokens(text, separator, delimiter, token => tokens.Add(trimTokens ? token.Trim() : token));
			return result;
		}

		/// <summary>
		/// Tokenize the input string using the specified separator (e.g., comma), delimiter (e.g., double quote),
		/// and trimming options and add the output tokens to the specified collection.
		/// </summary>
		/// <param name="text">The text to tokenize.</param>
		/// <param name="separator">The token separator character.  Typically, a comma.</param>
		/// <param name="delimiter">The token quote character.  Typically, a double quote.
		/// This can be used to enclose tokens that contain the separator character.
		/// To use this character inside a token, use it two consecutive times.
		/// </param>
		/// <param name="addToken">The action to invoke when a token has been matched and needs to be output.</param>
		/// <returns>True if all of the tokens were properly delimited (or were not delimited).
		/// False if the final token was open-delimited but never closed (which usually indicates a partial record).</returns>
		public static bool SplitIntoTokens(string text, char separator, char? delimiter, Action<string> addToken)
		{
			Conditions.RequireReference(addToken, "addToken");

			bool result;

			if (delimiter == null)
			{
				SplitIntoTokensWithoutDelimiter(text, separator, addToken);
				result = true;
			}
			else
			{
				result = SplitIntoTokensWithDelimiter(text, separator, delimiter.Value, addToken);
			}

			return result;
		}

		/// <overloads>Strips the quotes off of a string.</overloads>
		/// <summary>
		/// Strips the '"' quotes off of a string if they exist.
		/// </summary>
		/// <param name="text">The text to search.</param>
		/// <returns>The text with the quotes removed.</returns>
		public static string StripQuotes(string text) => StripQuotes(text, "\"", "\"");

		/// <summary>
		/// Strips the specified quotes off of a string if they exist.
		/// </summary>
		/// <param name="text">The text to search.</param>
		/// <param name="quote">The quote string.</param>
		/// <returns>The text with the quotes removed.</returns>
		public static string StripQuotes(string text, string quote) => StripQuotes(text, quote, quote);

		/// <summary>
		/// Strips the opening and closing quotes off of a string if they exist.
		/// </summary>
		/// <param name="text">The text to search.</param>
		/// <param name="openQuote">The opening quote string.</param>
		/// <param name="closeQuote">The closing quote string.</param>
		/// <returns>The text with the quotes removed.</returns>
		public static string StripQuotes(string text, string openQuote, string closeQuote)
		{
			Conditions.RequireReference(text, "text"); // An empty string is ok.
			Conditions.RequireString(openQuote, "openQuote");
			Conditions.RequireString(closeQuote, "closeQuote");

			int startIndex = 0;
			int length = text.Length;

			if (text.StartsWith(openQuote))
			{
				int quoteLength = openQuote.Length;
				startIndex += quoteLength;
				length -= quoteLength;
			}

			if (text.EndsWith(closeQuote))
			{
				length -= closeQuote.Length;
			}

			return text.Substring(startIndex, length);
		}

		/// <summary>
		/// Gets the plural form of the specified word.
		/// </summary>
		/// <param name="word">The word to make plural.</param>
		public static string MakePlural(string word)
		{
			PluralizationService service = CreatePluralizationService();
			string result = service.Pluralize(word);
			return result;
		}

		/// <summary>
		/// Gets the singular form of the specified word.
		/// </summary>
		/// <param name="word">The word to make singular.</param>
		public static string MakeSingular(string word)
		{
			PluralizationService service = CreatePluralizationService();
			string result = service.Singularize(word);
			return result;
		}

		#endregion

		#region Private Methods

		private static string GetDelimitedToken(string text, char delimiter, ref int currentIndex, ref bool properlyClosed)
		{
			Debug.Assert(text[currentIndex] == delimiter, "MatchIndex should be pointing to a delimiter character.");

			// Skip past the opening delimiter.
			int startIndex = ++currentIndex;
			int length = text.Length;
			bool containsEscapedDelimiters = false;

			while (currentIndex < length && (currentIndex = text.IndexOf(delimiter, currentIndex)) >= 0)
			{
				// If the next character is a delimiter, then we've found a doubled/escaped delimiter.
				int nextCharIndex = currentIndex + 1;
				if (nextCharIndex < length && text[nextCharIndex] == delimiter)
				{
					containsEscapedDelimiters = true;
					currentIndex = nextCharIndex + 1;
				}
				else
				{
					break;
				}
			}

			// Make sure match index doesn't go backward.  This simplifies logic for us and the caller.
			if (currentIndex < 0)
			{
				currentIndex = length;
			}

			string result;
			properlyClosed = currentIndex < length;
			if (properlyClosed)
			{
				result = text.Substring(startIndex, currentIndex - startIndex);
			}
			else
			{
				result = text.Substring(startIndex);
			}

			if (containsEscapedDelimiters)
			{
				result = result.Replace(new string(delimiter, 2), new string(delimiter, 1));
			}

			return result;
		}

		private static string GetSeparatedToken(string text, char separator, int tokenStartIndex, ref int currentIndex)
		{
			// The current index tells us the index where we need to currently start looking for the next
			// separator.  The token start index tells us where the caller originally began looking for a
			// token before it skipped whitespace.
			Debug.Assert(tokenStartIndex <= currentIndex, "The token start index can't be greater than current index.");

			string result = null;

			// Using IndexOf is much faster than using string's this[char] indexer.
			int separatorIndex = text.IndexOf(separator, currentIndex);
			if (separatorIndex >= 0)
			{
				result = text.Substring(tokenStartIndex, separatorIndex - tokenStartIndex);

				// Skip the separator character we just matched.
				currentIndex = separatorIndex + 1;
			}
			else
			{
				// Get everything through the end of the line.
				result = text.Substring(tokenStartIndex);

				// Make sure match index doesn't go backward.  This simplifies logic for us and the caller.
				currentIndex = text.Length;
			}

			return result;
		}

		private static bool SplitIntoTokensWithDelimiter(string text, char separator, char delimiter, Action<string> addToken)
		{
			// This is about as efficient as I can implement it.  The main trick is using IndexOf rather
			// than trying to manually examine each character.  Internally, string.IndexOf will do direct
			// memory reads, which is faster than us going through string's this[int] indexer.
			// http://stackoverflow.com/questions/73385/asp-net-convert-csv-string-to-string/74093#74093
			// http://stackoverflow.com/questions/736629/parse-delimited-csv-in-net

			// Assume the tokens will all be delimited properly (until proven otherwise).
			bool result = true;

			if (!string.IsNullOrEmpty(text))
			{
				int currentIndex = 0;
				int length = text.Length;

				while (currentIndex < length)
				{
					// We should be at the beginning of a token here.  Skip whitespace but remember where we started from.
					int tokenStartIndex = currentIndex;
					char ch;
					if (!TrySkipWhitespace(text, ref currentIndex, out ch))
					{
						// There was only whitespace or nothing left in the line.
						currentIndex = tokenStartIndex;
						break;
					}
					else if (ch == separator)
					{
						// We hit a separator, so the token is either empty or all whitespace.
						string token = currentIndex > tokenStartIndex ? text.Substring(tokenStartIndex, currentIndex - tokenStartIndex) : string.Empty;
						addToken(token);
						currentIndex++; // Skip past the separator.
					}
					else if (ch == delimiter)
					{
						// We need to get the delimited value.  However, there may be whitespace
						// before it and after it.  For malformed lines, there may be non-whitespace
						// text after it but before the next separator (e.g., A, "Test"B, C).
						string preToken = currentIndex > tokenStartIndex ? text.Substring(tokenStartIndex, currentIndex - tokenStartIndex) : string.Empty;
						string token = GetDelimitedToken(text, delimiter, ref currentIndex, ref result);
						if (result)
						{
							// See if there is stuff after the last delimiter, and append all chars until the next separator or EOL.
							currentIndex++; // Skip past the closing delimiter.
							string postToken = GetSeparatedToken(text, separator, currentIndex, ref currentIndex);
							addToken(preToken + token + postToken);
						}
						else
						{
							// The delimited token wasn't property closed, so this should add everything through the end of the line.
							addToken(preToken + token);
						}
					}
					else
					{
						// Treat the token as a non-delimited value.  For example, all of the values in: A, B"Test, C.
						// If a delimiter is not the first non-whitespace character, then we assume the token isn't
						// following the rules for a delimited value, and we'll just match to the next separator.
						string token = GetSeparatedToken(text, separator, tokenStartIndex, ref currentIndex);
						addToken(token);
					}
				}

				if (currentIndex < length)
				{
					string token = text.Substring(currentIndex);
					addToken(token);
				}
				else if (currentIndex == length && result && text[length - 1] == separator)
				{
					// If the last character was a separator and all the tokens were properly delimited,
					// then we need to add another empty token (e.g., "A,B," --> "A", "B", "").
					addToken(string.Empty);
				}
			}

			return result;
		}

		private static void SplitIntoTokensWithoutDelimiter(string text, char separator, Action<string> addToken)
		{
			// This is about as efficient as I can implement it.  See comments in SplitIntoTokensWithDelimiter.
			if (!string.IsNullOrEmpty(text))
			{
				int startIndex = 0;
				int length = text.Length;

				while (startIndex < length)
				{
					string token = GetSeparatedToken(text, separator, startIndex, ref startIndex);
					addToken(token);
				}

				// If the last character was a separator, then we need to add another empty token.
				if (startIndex == length && text[length - 1] == separator)
				{
					addToken(string.Empty);
				}
			}
		}

		private static bool TrySkipWhitespace(string text, ref int currentIndex, out char ch)
		{
			// Assume we'll find a non-whitespace character.
			bool result = true;
			int length = text.Length;

			// Skip any whitespace characters, possibly to the end of the line.
			while (char.IsWhiteSpace(ch = text[currentIndex]))
			{
				currentIndex++;
				if (currentIndex == length)
				{
					// We hit the end of the line without finding a non-whitespace character.
					result = false;
					break;
				}
			}

			return result;
		}

		private static PluralizationService CreatePluralizationService()
		{
			// Entity Framework's pluralization service only works with English, which is all we
			// care about, but we need to make sure we pass it an English culture.
			// http://weblogs.asp.net/kencox/archive/2010/04/10/ef-4-s-pluralizationservice-class-a-singularly-impossible-plurality.aspx
			PluralizationService result = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-US"));
			return result;
		}

		#endregion
	}
}
