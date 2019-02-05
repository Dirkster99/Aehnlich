namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using System.Xml.Linq;
	using System.Xml.Schema;

	#endregion

	/// <summary>
	/// Methods and properties for XML processing.
	/// </summary>
	public static class XmlUtility
	{
		#region Public Methods

		/// <summary>
		/// Creates a new XmlSchemaSet from the XElement containing the schema.
		/// </summary>
		/// <param name="schemaElements">The XML elements containing the schemas.</param>
		/// <returns>The new XmlSchema instance.</returns>
		/// <remarks>
		/// An XmlSchemaException will be thrown if an error occurs.
		/// </remarks>
		public static XmlSchemaSet CreateSchemaSet(IEnumerable<XElement> schemaElements)
			=> CreateSchemaSet(schemaElements, (ValidationEventHandler)null);

		/// <summary>
		///  Creates a new XmlSchemaSet from the XElement containing the schema and stores
		///  any errors and warnings in the specified collection.
		/// </summary>
		/// <param name="schemaElements">The XML elements containing the schemas.</param>
		/// <param name="errors">The collection to add errors and warnings into.</param>
		/// <returns>The new XmlSchemaSet instance.</returns>
		public static XmlSchemaSet CreateSchemaSet(IEnumerable<XElement> schemaElements, ICollection<ValidationEventArgs> errors)
		{
			Conditions.RequireReference(errors, "errors");
			return CreateSchemaSet(schemaElements, (s, e) => errors.Add(e));
		}

		/// <summary>
		/// Creates a new XmlSchemaSet from the XElements containing the schemas using the specified handler.
		/// </summary>
		/// <param name="schemaElements">The XML elements containing the schemas.</param>
		/// <param name="handler">The handler used to process errors and warnings.  This can be null,
		/// which means an XmlSchemaException will be thrown if an error occurs.</param>
		/// <returns>The new XmlSchemaSet instance.</returns>
		public static XmlSchemaSet CreateSchemaSet(IEnumerable<XElement> schemaElements, ValidationEventHandler handler)
		{
			Conditions.RequireReference(schemaElements, "schemaElements");

			// Unfortunately, XmlSchema is NOT thread-safe by itself because adding it to an XmlSchemaSet
			// will modify it.  To use it thread-safely, we have to create the XmlSchemaSet too, and even then
			// the resulting set is only thread-safe if it's used from a validating reader (which our Validate does).
			// http://blogs.msdn.com/b/marcelolr/archive/2009/03/16/xmlschema-and-xmlschemaset-thread-safety.aspx
			XmlSchemaSet result = new XmlSchemaSet();

			foreach (XElement schemaElement in schemaElements)
			{
				using (XmlReader reader = schemaElement.CreateReader())
				{
					XmlSchema schema = XmlSchema.Read(reader, handler);
					result.Add(schema);
				}
			}

			// Call this up front so it doesn't have to be implicitly called later.  Any time validation is done,
			// the schema must be compiled first.  Doing it here means later threads won't have to bother.
			result.Compile();
			return result;
		}

		/// <summary>
		/// Gets the specified attribute's value.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The value of the attribute.  An <see cref="ArgumentException"/>
		/// is thrown if the specified attribute isn't found.</returns>
		/// <exception cref="ArgumentException">Throw if either of the arguments are null,
		/// or if the specified attribute isn't found.</exception>
		public static string GetAttributeValue(this XElement element, XName name)
		{
			string result = GetAttributeValue(element, name, null);
			if (result == null)
			{
				throw Exceptions.NewArgumentExceptionFormat(
					"The {0} element does not have a {1} attribute.", element.Name, name);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified attribute's value or a default value if the attribute isn't present or is empty.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="defaultValue">The value to return if the attribute isn't found or has an empty value.</param>
		/// <returns>The value of the attribute, or the default value if the attribute isn't found or has an empty value.</returns>
		public static string GetAttributeValue(this XElement element, XName name, string defaultValue)
			=> GetAttributeValue(element, name, defaultValue, true);

		/// <summary>
		/// Gets the specified attribute's value or a default value if the attribute isn't present.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="defaultValue">The value to return if the attribute isn't found.</param>
		/// <param name="useDefaultIfEmptyValue">If the attribute is present but with an empty value,
		/// then this parameter determines whether the <paramref name="defaultValue"/> should be
		/// returned (if true) or the actual, empty attribute value should be returned (if false).
		/// Normally, this should be true, but false is useful if you need to allow the user to explicitly
		/// set an empty attribute value to override a non-empty default value.
		/// </param>
		/// <returns>The value of the attribute, or the default value if the attribute isn't found.</returns>
		public static string GetAttributeValue(this XElement element, XName name, string defaultValue, bool useDefaultIfEmptyValue)
		{
			// It's ok if defaultValue is null.
			Conditions.RequireReference(element, "element");
			Conditions.RequireReference(name, "name");

			string result = defaultValue;

			XAttribute attr = element.Attribute(name);
			if (attr != null)
			{
				result = attr.Value;

				if (useDefaultIfEmptyValue && string.IsNullOrEmpty(result))
				{
					result = defaultValue;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the specified attribute's value or a default value if the attribute isn't present.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="defaultValue">The value to return if the attribute isn't found.</param>
		/// <returns>The value of the attribute, or the default value if the attribute isn't found.</returns>
		public static int GetAttributeValue(this XElement element, XName name, int defaultValue)
		{
			string textValue = element.GetAttributeValue(name, null);
			int result = ConvertUtility.ToInt32(textValue, defaultValue);
			return result;
		}

		/// <summary>
		/// Gets the specified attribute's value or a default value if the attribute isn't present.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="defaultValue">The value to return if the attribute isn't found.</param>
		/// <returns>The value of the attribute, or the default value if the attribute isn't found.</returns>
		public static bool GetAttributeValue(this XElement element, XName name, bool defaultValue)
		{
			string textValue = element.GetAttributeValue(name, null);
			bool result = ConvertUtility.ToBoolean(textValue, defaultValue);
			return result;
		}

		/// <summary>
		/// Gets the specified attribute's value or a default value if the attribute isn't present.
		/// </summary>
		/// <param name="element">The element to get the attribute from.</param>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="defaultValue">The value to return if the attribute isn't found.</param>
		/// <returns>The value of the attribute, or the default value if the attribute isn't found.</returns>
		public static T GetAttributeValue<T>(this XElement element, XName name, T defaultValue)
			where T : struct
		{
			string textValue = element.GetAttributeValue(name, null);
			T result = ConvertUtility.GetValue(textValue, defaultValue);
			return result;
		}

		/// <summary>
		/// Validates the XML using the given schemas and throws an XmlSchemaValidationException
		/// if there are errors or warnings.
		/// </summary>
		/// <param name="xml">The XML to validate.</param>
		/// <param name="schemas">The schemas to use for validation.</param>
		public static void RequireValidation(this XElement xml, XmlSchemaSet schemas)
		{
			IList<ValidationEventArgs> errors = xml.Validate(schemas);
			if (errors.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (var error in errors)
				{
					if (sb.Length > 0)
					{
						sb.AppendLine();
					}

					sb.Append(error.Severity).Append(": ").Append(error.Message);

					// Try to include position info and other details if they're available.
					XmlSchemaException ex = error.Exception;
					if (ex != null)
					{
						// According to the docs for IXmlLineInfo, the line number and position are 1-based,
						// but they'll only be set for XElements if LoadOptions.SetLineInfo was used.
						if (ex.LineNumber > 0 && ex.LinePosition > 0)
						{
							sb.AppendFormat(" (Line: {0}, Position: {1})", ex.LineNumber, ex.LinePosition);
						}
						else if (ex.LineNumber > 0)
						{
							sb.AppendFormat(" (Line: {0})", ex.LineNumber);
						}

						if (!string.IsNullOrEmpty(ex.SourceUri))
						{
							sb.AppendFormat(" (Schema URI: {0})", ex.SourceUri);
						}

						var sourceObject = ex.SourceSchemaObject;
						if (sourceObject != null)
						{
							if (sourceObject.LineNumber > 0 && sourceObject.LinePosition > 0)
							{
								sb.AppendFormat(" (Schema Line: {0}, Schema Position: {1})", sourceObject.LineNumber, sourceObject.LinePosition);
							}
							else if (sourceObject.LineNumber > 0)
							{
								sb.AppendFormat(" (Schema Line: {0})", sourceObject.LineNumber);
							}
						}
					}
				}

				XmlSchemaValidationException result = new XmlSchemaValidationException(sb.ToString());
				result.Data.Add("ValidationErrors", errors);
				throw Exceptions.Log(result);
			}
		}

		/// <summary>
		/// Validates the XML using the given schema set.
		/// </summary>
		/// <param name="xml">The XML to validate.</param>
		/// <param name="schemas">The schemas to use for validation.</param>
		/// <returns>A list of validation errors and warnings.</returns>
		public static IList<ValidationEventArgs> Validate(this XElement xml, XmlSchemaSet schemas)
		{
			Conditions.RequireReference(xml, "xml");
			Conditions.RequireReference(schemas, "schema");

			List<ValidationEventArgs> result = new List<ValidationEventArgs>();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.Schemas = schemas;
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += (s, e) => result.Add(e);

			using (XmlReader xmlReader = xml.CreateReader())
			using (XmlReader validatingReader = XmlReader.Create(xmlReader, settings))
			{
				while (validatingReader.Read())
				{
					// We have to read through every node to complete validation.
				}
			}

			return result;
		}

		#endregion
	}
}
