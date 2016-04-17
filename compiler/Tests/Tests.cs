using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concrete.TableSpace;
using Concrete.IdentifierTableSpace;
using Concrete.ConstantsTableSpace;

namespace compiler.Tests {
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MyTestClass {
        [TestMethod]
        public void MyTestMethod() {
            IdentifierTable Table = new IdentifierTable();
            ushort StartIndex = 1004;
            //Table.Clear();
            Table.Insert("a");
            Table.Insert("b");
            try {
                Table.Insert("a");
                Assert.Fail();
            } catch (ArgumentException) {

            }
            Assert.AreEqual(StartIndex, Table.GetKey("a"));
            Assert.AreEqual(StartIndex + 1, Table.GetKey("b"));
            
        }

        [TestMethod]
        public void MyTestMethod1() {
            ConstantsTable Table = new ConstantsTable();
            ushort StartIndex = 501;
            Table.Insert("101");
            Table.Insert("23#2");

            Assert.AreEqual(StartIndex, Table.GetKey("101"));
            Assert.AreEqual(StartIndex + 1, Table.GetKey("23#2"));

            Assert.AreEqual(ConstantsTable.States.INTEGER, Table.GetType("101"));
            Assert.AreEqual(ConstantsTable.States.REAL, Table.GetType("23#2"));
        }
    }
}
