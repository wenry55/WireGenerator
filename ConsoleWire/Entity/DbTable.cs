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

        public string paramName
        {
            get
            {
                return char.ToLower(name[0]) + name.Substring(1);
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
    }
}
