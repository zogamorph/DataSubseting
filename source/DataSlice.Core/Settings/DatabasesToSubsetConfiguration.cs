using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Settings
{
    public class DatabasesToSubsetConfiguration : ConfigurationSection
    {

        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false, IsRequired = true)]
        public DatabaseToSubsetCollection DatabasesToSubSet
        {
            get
            {
                return base[""] as DatabaseToSubsetCollection;
            }

            set
            {
                base[""] = value;
            }
        }

        public class DatabaseToSubsetCollection : ConfigurationElementCollection
        {

            protected override ConfigurationElement CreateNewElement()
            {
                return new DatabaseToSubsetElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((DatabaseToSubsetElement)element).Name;
            }

            protected override string ElementName
            {
                get
                {
                    return "database";
                }
            }

            protected override bool IsElementName(string elementName)
            {
                return !String.IsNullOrEmpty(elementName) && elementName == "database";
            }

            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }


            public DatabaseToSubsetElement this[int index]
            {
                get
                {
                    return base.BaseGet(index) as DatabaseToSubsetElement;
                }
            }
        }

    }

    public class DatabaseToSubsetElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return base["name"] as string; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("source", IsRequired = true, IsKey = false)]
        public string Source
        {
            get { return base["source"] as string; }
            set { base["source"] = value; }
        }

        [ConfigurationProperty("destination", IsRequired = true, IsKey = false)]
        public string Destination
        {
            get { return base["destination"] as string; }
            set { base["destination"] = value; }
        }

        [ConfigurationProperty("order", IsRequired = true, IsKey = false)]
        public int Order
        {
            get
            {
                int val = 0;

                if (base["order"] != null)
                {
                    Int32.TryParse(base["order"].ToString(), out val);
                }

                return val;
            }
            set { base["order"] = value; }
        }

        [ConfigurationProperty("ignore", IsRequired = false, IsKey = false)]
        public bool Ignore
        {
            get
            {
                bool canIgnore = false;

                if (base["ignore"] != null)
                {
                    Boolean.TryParse(base["ignore"].ToString(), out canIgnore);
                }

                return canIgnore;
            }
            set { base["ignore"] = value; }
        }

        [ConfigurationProperty("ignoretables", IsRequired = false, IsKey = false)]
        public string IgnoreTable
        {
            get
            {
                return base["ignoretables"] as string;
            }
            set
            {
               base["ignoretables"] = value; 
            }
        }
    }
}
