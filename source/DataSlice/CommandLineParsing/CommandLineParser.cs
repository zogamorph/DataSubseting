// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataSlice.CommandLineParsing
{
    public static class CommandLineParser
    {
        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CommandLineParser()
        {
            TypeDescriptor.AddAttributes(typeof(DirectoryInfo), new TypeConverterAttribute(typeof(DirectoryInfoConverter)));
            TypeDescriptor.AddAttributes(typeof(FileInfo), new TypeConverterAttribute(typeof(FileInfoConverter)));
        }

        #endregion

        #region Public Members

        public static void ParseArguments(this object valueToPopulate, IEnumerable<string> args)
        {
            CommandLineDictionary commandLineDictionary = CommandLineDictionary.FromArguments(args);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(valueToPopulate);

            // Ensure required properties are specified.
            foreach (PropertyDescriptor property in properties)
            {
                // See whether any of the attributes on the property is a RequiredAttribute.
                if (property.Attributes.Cast<Attribute>().Any(attribute => attribute is RequiredAttribute))
                {
                    // If so, and the command line dictionary doesn't contain a key matching
                    // the property's name, it means that a required property isn't specified.
                    if (!commandLineDictionary.ContainsKey(property.Name))
                    {
                        throw new ArgumentException("A value for the " + property.Name + " property is required.");
                    }
                }
            }

            foreach (KeyValuePair<string, string> keyValuePair in commandLineDictionary)
            {
                // Find a property whose name matches the kvp's key, ignoring case.
                // We can't just use the indexer because that is case-sensitive.                
                PropertyDescriptor property = MatchProperty(keyValuePair.Key, properties, valueToPopulate.GetType());

                // If the value is null/empty and the property is a bool, we
                // treat it as a flag, which means its presence means true.
                if (String.IsNullOrEmpty(keyValuePair.Value) &&
                    (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?)))
                {
                    property.SetValue(valueToPopulate, true);
                    continue;
                }

                object valueToSet;

                switch (property.PropertyType.Name)
                {
                    case "IEnumerable`1":
                    case "ICollection`1":
                    case "IList`1":
                    case "List`1":
                        MethodInfo methodInfo = typeof(CommandLineParser).GetMethod("FromCommaSeparatedList", BindingFlags.Static | BindingFlags.NonPublic);
                        Type[] genericArguments = property.PropertyType.GetGenericArguments();
                        MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
                        valueToSet = genericMethodInfo.Invoke(null, new object[] { keyValuePair.Value });
                        break;
                    default:
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                        if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
                        {
                            throw new ArgumentException("Unable to convert from a string to a property of type " + property.PropertyType + ".");
                        }
                        valueToSet = typeConverter.ConvertFromInvariantString(keyValuePair.Value);
                        break;
                }

                property.SetValue(valueToPopulate, valueToSet);
            }

            return;
        }
     
        private static PropertyDescriptor MatchProperty(string keyName, PropertyDescriptorCollection properties, Type targetType)
        {
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                {
                    return prop;
                }
            }
            throw new ArgumentException("A matching public property of name " + keyName + " on type " + targetType + " could not be found.");
        }

        public static void PrintUsage(object component)
        {
            IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>();
            IEnumerable<string> propertyNames = properties.Select(property => property.Name);
            IEnumerable<string> propertyDescriptions = properties.Select(property => property.Description);
            IEnumerable<string> lines = FormatNamesAndDescriptions(propertyNames, propertyDescriptions, Console.WindowWidth);

            Console.WriteLine("Possible arguments:");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public static void PrintCommands(IEnumerable<Command> commands)
        {
            // Print out general descriptions for every command.
            IEnumerable<string> commandNames = commands.Select(command => command.Name);
            IEnumerable<string> commandDescriptions = commands.Select(command => command.GetAttribute<DescriptionAttribute>().Description);
            IEnumerable<string> lines = FormatNamesAndDescriptions(commandNames, commandDescriptions, Console.WindowWidth);

            Console.WriteLine("Possible commands:");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public static string ToString(object valueToConvert)
        {
            IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(valueToConvert).Cast<PropertyDescriptor>();
            IEnumerable<PropertyDescriptor> propertiesOnParent = TypeDescriptor.GetProperties(valueToConvert.GetType().BaseType).Cast<PropertyDescriptor>();
            properties = properties.Except(propertiesOnParent);
            CommandLineDictionary commandLineDictionary = new CommandLineDictionary();

            foreach (PropertyDescriptor property in properties)
            {
                commandLineDictionary[property.Name] = property.GetValue(valueToConvert).ToString();
            }

            return commandLineDictionary.ToString();
        }

        #endregion

        #region Private Members

        private static IEnumerable<string> FormatNamesAndDescriptions(IEnumerable<string> names, IEnumerable<string> descriptions, int maxLineLength)
        {
            if (names.Count() != descriptions.Count())
            {
                throw new ArgumentException("Collection sizes are not equal", "names");
            }

            int namesMaxLength = names.Max(commandName => commandName.Length);

            List<string> lines = new List<string>();

            for (int i = 0; i < names.Count(); i++)
            {
                string line = names.ElementAt(i);
                line = line.PadRight(namesMaxLength + 2);

                foreach (string wrappedLine in WordWrap(descriptions.ElementAt(i), maxLineLength - namesMaxLength - 3))
                {
                    line += wrappedLine;
                    lines.Add(line);
                    line = new string(' ', namesMaxLength + 2);
                }
            }

            return lines;
        }

        private static List<T> FromCommaSeparatedList<T>(this string commaSeparatedList)
        {
            List<T> collection = new List<T>();

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                StringBuilder builder = new StringBuilder();
                bool isEscaped = false;

                foreach (char character in commaSeparatedList)
                {
                    // If we are in escaped mode, add the character and exit escape mode
                    if (isEscaped)
                    {
                        builder.Append(character);
                        isEscaped = false;
                    }
                    // If we see the backslash and are not in escaped mode, go into escaped mode
                    else if (character == '\\' && !isEscaped)
                    {
                        isEscaped = true;
                    }
                    // A comma outside of escaped mode is an item separator, convert
                    // built string to T and add to collection, then zero out the builder
                    else if (character == ',' && !isEscaped)
                    {
                        collection.Add((T)typeConverter.ConvertFromInvariantString(builder.ToString()));
                        builder.Length = 0;
                    }
                    // Otherwise simply add the character
                    else
                    {
                        builder.Append(character);
                    }
                }

                if (builder.Length > 0 || collection.Count > 0)
                {
                    collection.Add((T)typeConverter.ConvertFromInvariantString(builder.ToString()));
                }
            }

            return collection;
        }

        private static T GetAttribute<T>(this object value) where T : Attribute
        {
            IEnumerable<Attribute> attributes = TypeDescriptor.GetAttributes(value).Cast<Attribute>();
            return (T)attributes.First(attribute => attribute is T);
        }

        private static IEnumerable<string> WordWrap(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();
            string currentLine = String.Empty;

            foreach (string word in text.Split(' '))
            {
                if (currentLine.Length + word.Length > maxLineLength)
                {
                    lines.Add(currentLine);
                    currentLine = String.Empty;
                }

                currentLine += word;

                // Add spaces between words except for when we are at exactly the
                // maximum width.
                if (currentLine.Length != maxLineLength)
                {
                    currentLine += " ";
                }
            }

            // Add the remainder of the current line except for when it is
            // empty, which is true in the case when we had just started a
            // new line.
            if (currentLine.Trim() != String.Empty)
            {
                lines.Add(currentLine);
            }

            return lines;
        }

        #endregion
    }

}
