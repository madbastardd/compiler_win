﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Concrete.TableSpace;
using System.IO;

namespace Concrete.IdentifierTableSpace {
    public class IdentifierTables : Table {
		public enum States {
			/// <summary>
			/// states of ID
			/// </summary>
			UNDEFINED, VAR, CONST, PROCEDURE, FUNCTION, BUILT_IN
		}

		Dictionary<ushort, States> IDType = new Dictionary<ushort, States>();	//type of ID

        public IdentifierTables() : base(1001) {
            //creates empty table
			this.ReadFromFile ("startid.dat");
        }
        public override void Insert(string _value) {
			//just call overloaded method
			this.Insert (_value, States.UNDEFINED);
        }

		public void Insert(string _value, States state) {
			base.Insert(_value);

			this.IDType.Add ((ushort)(this.CurrentIndex - 1), state);
		}

		public void SetType(ushort key, States state) {
			if (key >= CurrentIndex)
				throw new IndexOutOfRangeException ("Key is too big");

			IDType [key] = state;
		}

		public States GetType(ushort key) {
			return IDType [key];
		}

		public States GetType(string _value) {
			return GetType(this.GetKey(_value));
		}

		public override void SaveToFile (string fileName, bool WithKeys)
    	{
			try {
				using (StreamWriter sw = new StreamWriter(fileName)) {
					foreach (var item in IDType.Keys) {
						if (WithKeys)
							sw.WriteLine(String.Format("{0}:{1} - {2}", item, this.data[item], this.IDType[item]));
						else
							sw.WriteLine(String.Format("{0} - {2}", this.data[item], this.IDType[item]));
					}
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
    	}
    }
}
