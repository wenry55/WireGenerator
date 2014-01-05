using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGenerator.classes
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

    }
}
