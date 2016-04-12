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
        public override void Insert(string _value) {
            base.Insert(_value);
        }
    }
}
