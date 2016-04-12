using System;
using System.IO;
using System.Collections.Generic;
using Interfaces.TableSpace;
using System.Windows.Controls;

namespace Concrete.TableSpace {
    /// <summary>
    /// class includes dictionary, that has unique keys and unique values
    /// </summary>
    public abstract class Table : ITable {
		protected Dictionary<ushort, string> data;
		protected ushort CurrentIndex;

		public Table(ushort index = 0) {
			this.data = new Dictionary<ushort, string>();
			this.CurrentIndex = index;
        }
        public string this[ushort index] {
            get {
                return this.data[index];
            }
        }

        public virtual void Insert(string _value) {
			if (this.data.ContainsValue (_value)) 
				throw new ArgumentException (_value);
			this.data.Add(this.CurrentIndex++, _value);          
        }

        public bool ContainsValue(string _value) {
            return this.data.ContainsValue(_value);
        }

        public virtual void ReadFromFile(string fileName) {
            using (StreamReader sr = new StreamReader(fileName)) {
                this.data.Clear();
                string _value;
                while ((_value = sr.ReadLine()) != null) {
                    this.Insert(_value);
                }
            }
        }

        public virtual void SaveToFile(string fileName, bool WithKeys = false) {
            using (StreamWriter sw = new StreamWriter(fileName)) {
                foreach (var item in this.data) {
                    if (WithKeys)
                        sw.WriteLine(String.Format("{0}:{1}", item.Key, item.Value));
                    else
                        sw.WriteLine(String.Format("{0}", item.Value));
                }
            }
        }

        public void Clear() {
            this.data.Clear();
        }

		public ushort GetKey (string _value) {
			foreach (var item in this.data) {
				if (item.Value == _value)
					return item.Key;
			}
			throw new ArgumentException (_value);
    	}

		public void ShowInTextView (TextBox textView) {
			foreach (var item in this.data) {
				textView.Text += String.Format ("{0}:{1}\n", item.Key, item.Value);
			}
		}
    }
}
