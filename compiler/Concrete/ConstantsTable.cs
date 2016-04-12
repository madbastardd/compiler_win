using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.ConstantsTableSpace {
    public class ConstantsTable : TableSpace.Table {
        public ConstantsTable() : base(501) {
            //constant create
			this.ReadFromFile("startconst.dat");
        }
        public override void Insert(string _value) {
            base.Insert(_value);
        }
    }
}
