using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.KeyWordTableSpace {
    public class KeyWordsTable : TableSpace.Table {
        public KeyWordsTable() : base(401) {
            //some keywords
            this.ReadFromFile("kwords.dat");
        }
    }
}
