using System;
using System.Collections.Generic;
using Concrete.KeyWordTableSpace;
using Concrete.IdentifierTableSpace;
using Concrete.ConstantsTableSpace;
using Concrete.AttributeClassSpace;
using Concrete.MultySymbolSeparatorsTableSpace;
using System.IO;
using System.Windows.Controls;

namespace Concrete.Parser {
	/// <summary>
	/// Parser class
	/// </summary>
	public struct Lexem {
		public int code;
		public int line;
		public int pos;

		public Lexem(int _code = 0, int _line = 0, int _pos = 0) {
			code = _code;
			line = _line;
			pos = _pos;
		}
	}
	public static class Parser {
		static StreamReader sr = null;	//file to parse

		static TextBox showErrors;	//gui element to show errors

		static Int32 curLine;
		static Int32 lineNumber;	//line counter

		static Int32 curPos;
		static Int32 position;

		static bool gotErrors;	//false if code has got no errors

		static int readedSymbol;	//readed symbol

		private static void IDParse(Concrete.TableSpace.Table[] tables, string lexem, List<Lexem> result) {
			//parse id or keyword
			while (!sr.EndOfStream &&
				(AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.WORD | AttributeClass.DIGIT)) != 0) {
				lexem += (char)readedSymbol;	//create lexem
				++curPos;
			}
            ++curPos;
				
			if (lexem [0] == '$' && lexem != "$EXP") {
				//unrecognized lexem
				result.Add (new Lexem(-1, lineNumber, position));

				gotErrors = true;
				if (showErrors != null)
					showErrors.Text += String.Format ("Unrecognized lexem " +
					"(line {0})\n", lineNumber);
			} else {
				ushort key;	//key of lexem
				//tables assign
				KeyWordsTable KWTable = tables[1] as KeyWordsTable;
				IdentifierTable IDTable = tables[3] as IdentifierTable;

				if (!KWTable.ContainsValue (lexem)) {
					//it is ID
					if (!IDTable.ContainsValue (lexem))
						IDTable.Insert (lexem);
					key = IDTable.GetKey (lexem);	//get key of ID
				} else
					key = KWTable.GetKey (lexem);	//it is keyword and get his key

				result.Add (new Lexem(key, lineNumber, position));	//add in parse result
			}
            position = curPos;
        }

		private static void DigitParse(Concrete.TableSpace.Table[] tables, string lexem, List<Lexem> result) {
			//parse number
			while (!sr.EndOfStream &&
				(AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.DIGIT)) != 0) {
				lexem += (char)readedSymbol;	//create lexem
				++curPos;
			}
            ++curPos;
            if ((AttributeClass.Get(readedSymbol) & (AttributeClass.HASHTAG)) != 0) {
                lexem += (char)readedSymbol;
                if (!sr.EndOfStream &&
                (AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.SIGN | AttributeClass.DIGIT)) != 0) {
                    lexem += (char)readedSymbol;
                    ++curPos;
                }
                while (!sr.EndOfStream &&
                (AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.DIGIT)) != 0) {
                    lexem += (char)readedSymbol;    //create lexem
                    ++curPos;
                }
                ++curPos;
            }

			//assign table
			ConstantsTable CTable = tables[2] as ConstantsTable;
			if (!CTable.ContainsValue(lexem))
				CTable.Insert(lexem);	//it is new number

			result.Add(new Lexem(CTable.GetKey(lexem), lineNumber, position));	//add number
			position = curPos;
		}

		private static void CommentParse(Concrete.TableSpace.Table[] tables, string lexem, List<Lexem> result) {
			//parse comment
			if (!sr.EndOfStream
				&& (AttributeClass.Get(readedSymbol = sr.Read())
					& AttributeClass.COMMENT_STARTER_ENDER) != 0) {
				// this is comment
				do {
					++curPos;
					while (!sr.EndOfStream &&
						(AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.COMMENT_STARTER_ENDER)) == 0) {
						curPos++;
						if (readedSymbol == '\n') {
							++lineNumber;
							curPos = 1;
						}
					}

					if (!sr.EndOfStream
						&& (AttributeClass.Get(readedSymbol = sr.Read()) & AttributeClass.COMMENT_END_BRACKET) != 0) {
						readedSymbol = sr.Read ();
						++curPos;
						break;
					}
					//unclosed comment
					else if (sr.EndOfStream) {
						result.Add(new Lexem(-1, curLine, position));
						gotErrors = true;
						if (showErrors != null)
							showErrors.Text += String.Format("Unclosed comment " +
								"(line {0})\n", lineNumber);
						break;
					} else if (readedSymbol == '\n') {
						curPos = 1;
						++lineNumber;
					}
				} while (true);
			} else {
				//it is separator
				result.Add(new Lexem('(', lineNumber, position));
				position = ++curPos;
			}
			lineNumber = curLine;
			position = curPos;
		}

		private static void SeparatorParse(Concrete.TableSpace.Table[] tables, string lexem, List<Lexem> result) {
			while (!sr.EndOfStream &&
				(AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.SEPARATOR)) != 0) {
				lexem += (char)readedSymbol;
				curPos++;
			}
            ++curPos;

			MultySymbolSeparatorsTable MSTable = tables[0] as MultySymbolSeparatorsTable;
			if (!MSTable.ContainsValue(lexem)) {
				foreach (var item in lexem)
					result.Add(new Lexem(item, lineNumber, position));
			}
			else {
				result.Add(new Lexem(MSTable.GetKey(lexem), lineNumber, position));
			}
			position = curPos;
		}

		private static void WhiteSpaceParse(List<Lexem> result) {
            --curPos;
			do {
                ++curPos;
				if (readedSymbol == '\n') {
					curLine++;
					curPos = 1;
				}
			} while (!sr.EndOfStream &&
				(AttributeClass.Get(readedSymbol = sr.Read()) & (AttributeClass.WHITE_SPACE)) != 0) ;
			position = curPos;
			lineNumber = curLine;
		}

		private static void ErrorParse(List<Lexem> result) {
			result.Add(new Lexem(-1, curLine, position));
			readedSymbol = sr.Read ();
			gotErrors = true;
			//Console.WriteLine ("Undefined symbol");
			if (showErrors != null)
				showErrors.Text += String.Format ("Undefined symbol (line {0})\n", lineNumber);
			position = ++curPos;
		}

        private static List<Lexem> Parse(Concrete.TableSpace.Table[] tables, TextBox _showErrors = null) {
            /*
                tables:
                    0 - Multy Symbol Table
                    1 - Keywords
                    2 - Constants
                    3 - Identifiers
            */
            List<Lexem> result = new List<Lexem>();

			if (sr == null)
				throw new IOException("File is null");
			if (tables.Length != 4)
				throw new ArrayTypeMismatchException("Wrong parse tables");

			showErrors = _showErrors;

			lineNumber = curLine = 1;
			position = 1;
            curPos = 2;

			gotErrors = false;
			ushort currentAttribute;
			readedSymbol = sr.Read ();

			while (!sr.EndOfStream) {
				currentAttribute = AttributeClass.Get (readedSymbol);
				if ((currentAttribute & (AttributeClass.WORD | AttributeClass.DOLLAR_SIGN)) != 0) {
					//idintifier ow keyword handler
					IDParse (tables, ((char)readedSymbol).ToString(), result);
                }
                else if ((currentAttribute & (AttributeClass.DIGIT)) != 0) {
                    //number handler
					DigitParse(tables, ((char)readedSymbol).ToString(), result);
                }
				else if ((currentAttribute & AttributeClass.COMMENT_START_BRACKET) != 0) {
                    //comment parse
					CommentParse(tables, ((char)readedSymbol).ToString(), result);
                }
                else if ((currentAttribute & (AttributeClass.SEPARATOR)) != 0) {
                    //separator
					SeparatorParse(tables, ((char)readedSymbol).ToString(), result);
                }
                else if ((currentAttribute & AttributeClass.WHITE_SPACE) != 0) {
                    //spaces
					WhiteSpaceParse(result);
                }
                else {
                    //error
					ErrorParse (result);
                }
            }
			if (!gotErrors && showErrors != null)
				showErrors.Text = "Success";
			return result;
        }

        public static List<Lexem> ParseFile(string filename, Concrete.TableSpace.Table[] tables, TextBox _showErrors = null) {
			using (sr = new StreamReader (filename)) {
				return Parse (tables, _showErrors);
			}
		}
    }
}
