using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Concrete.TableSpace;

namespace Concrete.MultySymbolSeparatorsTableSpace {
    public class MultySymbolSeparatorsTable : Table {
        public MultySymbolSeparatorsTable() : base(301) {
            //all multy symbol separators
            this.Insert("<=");
            this.Insert("<>");
            this.Insert(">=");
        }
    }
}
