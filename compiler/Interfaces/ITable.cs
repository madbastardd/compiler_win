using System.Collections;
using System.Windows.Controls;

namespace Interfaces.TableSpace {
	/// <summary>
	/// ITable - interface for all tables in project
	/// </summary>
    interface ITable {
		string this[ushort index] { get; }	//iterator
        bool ContainsValue(string _value);	//return true if value is in table
        void Insert(string _value);	//inserts value in table
        void Clear();	//clear table
        void ReadFromFile(string fileName);	//read table data from file
        void SaveToFile(string fileName, bool WithKeys = false);	//save table data to file
		void ShowInTextView (TextBox textView);	//shows table in textview (gtk#)
		ushort GetKey (string _value);	//return key of element
    }
}