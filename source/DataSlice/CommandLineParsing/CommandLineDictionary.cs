// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace DataSlice.CommandLineParsing
{
    [Serializable]
    public class CommandLineDictionary : Dictionary<string, string>
    {

        public CommandLineDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            KeyCharacter = '/';
            ValueCharacter = '=';
        }

        protected CommandLineDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static CommandLineDictionary FromArguments(IEnumerable<string> arguments)
        {
            return FromArguments(arguments, '/', '=');
        }

        public static CommandLineDictionary FromArguments(IEnumerable<string> arguments, char keyCharacter, char valueCharacter)
        {
            CommandLineDictionary cld = new CommandLineDictionary();
            cld.KeyCharacter = keyCharacter;
            cld.ValueCharacter = valueCharacter;
            foreach (string argument in arguments)
            {
                cld.AddArgument(argument);
            }

            return cld;
        }


        public override string ToString()
        {
            string commandline = String.Empty;
            foreach (KeyValuePair<String, String> pair in this)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    commandline += String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3} ", KeyCharacter, pair.Key, ValueCharacter, pair.Value);
                }
                else // There is no value, so we just serialize the key
                {
                    commandline += String.Format(CultureInfo.InvariantCulture, "{0}{1} ", KeyCharacter, pair.Key);
                }
            }
            return commandline.TrimEnd();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


        private char KeyCharacter { get; set; }

        private char ValueCharacter { get; set; }

        private void AddArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            string key;
            string value;

            if (argument.StartsWith(KeyCharacter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                string[] splitArg = argument.Substring(1).Split(ValueCharacter);

                //Key is extracted from first element
                key = splitArg[0];

                //Reconstruct the value. We could also do this using substrings.
                if (splitArg.Length > 1)
                {
                    value = string.Join("=", splitArg, 1, splitArg.Length - 1);
                }
                else
                {
                    value = string.Empty;
                }
            }
            else
            {
                throw new ArgumentException("Unsupported value line argument format.", argument);
            }

            Add(key, value);
        }
    }

}
