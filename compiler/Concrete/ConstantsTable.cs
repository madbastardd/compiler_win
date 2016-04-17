using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.ConstantsTableSpace {
    public class ConstantsTable : TableSpace.Table {
        public enum States {
            INTEGER, REAL
        }
        SortedDictionary<ushort, States> CType = new SortedDictionary<ushort, States>();	//type of ID
        public ConstantsTable() : base(501) {
            //constant create
			this.ReadFromFile("startconst.dat");
        }

        public override void Insert(string _value) {
            base.Insert(_value);

            CType[(ushort)(CurrentIndex - 1)] = (_value.Contains("#")) ? States.REAL : States.INTEGER;
        }

        public States GetType(ushort key) {
            return CType[key];
        }

        public States GetType(string _value) {
            return GetType(this.GetKey(_value));
        }
    }
}
