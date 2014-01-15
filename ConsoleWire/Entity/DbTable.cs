using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWire.Entity
{
    public class DbTable
    {
        List<DbColumn> _columns = new List<DbColumn>();
        public List<DbColumn> columns
        {
            get
            {
                return _columns;
            }

            set
            {
                _columns = value;
            }
        }

        public string name { get; set; }
        public string key
        {
            get
            {
                string result = string.Empty;
                foreach (DbColumn col in _columns)
                {
                    if (col.isKey == true)
                    {
                        result = col.columnName;
                    }
                }
                return result;
            }
        }

        public string listViewName
        {
            get
            {
                return name + "ListView";
            }
        }
        public string listName
        {
            get
            {
                return name + "List";
            }
        }
        public string viewName 
        {
            get 
            {
                return name + "View";
            }
        }
        public string paramKey
        {
            get
            {
                if (string.IsNullOrEmpty(key)) { return string.Empty; }
                else
                {
                    return char.ToLower(key[0]) + key.Substring(1);
                }
            }
        }

        public string paramName
        {
            get
            {
                return char.ToLower(name[0]) + name.Substring(1);
            }
        }
        public string paramViewName
        {
            get
            {
                if (string.IsNullOrEmpty(viewName)) { return string.Empty; }
                else
                {
                    return char.ToLower(viewName[0]) + viewName.Substring(1);
                }
            }
        }
        public string paramListName
        {
            get
            {
                if (string.IsNullOrEmpty(listName)) { return string.Empty; }
                else
                {
                    return char.ToLower(listName[0]) + listName.Substring(1);
                } 
            }
        }
        public string paramListViewName
        {
            get
            {
                if (string.IsNullOrEmpty(listViewName)) { return string.Empty; }
                else
                {
                    return char.ToLower(listViewName[0]) + listViewName.Substring(1);
                }
            }
        }

        public string functionNameForRead
        {
            get
            {
                return "Get" + name;
            }
        }
        public string functionNameForList
        {
            get
            {
                return "GetList" + name;
            }
        }
        public string functionNameForSave
        {
            get
            {
                return "Save" + name;
            }
        }
        public string functionNameForRemove
        {
            get
            {
                return "Remove" + name;
            }
        }



    }
}
