using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWire.Entity
{
    /*
    public enum ColumnType
    {
        SingleLine,
        MultipleLine,
        Choice,
        Number,
        Currency,
        DateAndTime,
        YesOrNo
    }
     */

    public class DbColumn
    {
        public string columnType { get; set; }
        public string columnName { get; set; }
        public List<string> choices { get; set; }
    }
}
