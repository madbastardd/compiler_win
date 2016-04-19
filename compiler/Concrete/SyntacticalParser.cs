using System;
using Concrete.MultySymbolSeparatorsTableSpace;
using Concrete.KeyWordTableSpace;
using Concrete.ConstantsTableSpace;
using Concrete.IdentifierTableSpace;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.Syntactycal {
	internal class TreeNode {
        /// <summary>
        /// just a node of tree, can be a root
        /// </summary>
		public List<int> node;  //data of node
		public List<TreeNode> childs;   //childs

        public TreeNode(int _rule = 0, List<int> _node = null) {
            node = _node;
            childs = null;
        }
	}
	static class SyntacticalParser {
        /// <summary>
        /// syntactical parser
        /// includes methods to make syntactical parse
        /// </summary>
		static MultySymbolSeparatorsTable MSTable;  //MultySymbol Separators Table
        static KeyWordsTable KWTable;   //Keywords Table
		static ConstantsTable CTable;   //Constant Table
		static IdentifierTable IDTable;    //ID Table

		static List<int> list;  //parsed after lexical analyzer

		static Int32 index; //current pointer

        static public Int32 Index { get { return index; } } //read-only property

		static TextBox textTree;   //result tree in string

        static public String TextTree { get { return textTree.Text; } } //read-only property to get tree

        static TreeNode tree;  //tree

        static StringBuilder indents;  //tab

        static bool makeIndents = true; //true - with indents

        static SortedList<int, string> nonterminals;

        //nonterminals
        static readonly int _PROGRAM_ = -1,
            _BLOCK_ = -2,
            _PID_ = -3,
            _ID_ = -4,
            _COND_EX_ = -5,
            _LOGICAL_ = -6,
            _LOG_SUMMAND_ = -7,
            _LOG_MULTIPLIER_ = -8,
            _LOG_MUL_LIST_ = -9,
            _EXPRESSION_ = -10,
            _COMP_OP_ = -11,
            _SUM_LIST_ = -12,
            _SUMMAND_ = -13,
            _ADD_INSTR_ = -14,
            _MULTIPLIER_ = -15,
            _MUL_LIST_ = -16,
            _MUL_INSTR_ = -17,
            _CONSTANT_ = -18,
            _COMPLEX_CONSTANT_ = -19,
            _COMPLEX_NUMBER_ = -20,
            _VARIABLE_ = -21,
            _VARIABLE_ID_ = -22,
            _DIMENSION_ = -23,
            _COMPLEX_VARIABLE_ = -24,
            _UNSIGNED_CONST_ = -25,
            _UNSIGNED_NUM_ = -26,
            _RIGHT_PART_ = -27,
            _LEFT_PART_ = -28,
            _EXP_LIST_ = -29,
            _INTEGER_ = -30,
            _REAL_ = -31,
            _SIGN_ = -32,
            _ACTUAL_ARGUMENTS_ = -33,
            _ARGUMENT_LIST_ = -34, 
            _BUILT_IN_FUNC_ID_ = -35;

        public static void SetTables(Concrete.TableSpace.Table[] tables) {
            //set all tables
			if(tables.Length != 4)
				throw new ArrayTypeMismatchException("Wrong parse tables");

			MSTable = tables[0] as MultySymbolSeparatorsTable;
			KWTable = tables[1] as KeyWordsTable;
			CTable = tables[2] as ConstantsTable;
			IDTable = tables[3] as IdentifierTable;
		}

		static string GetString(int item) {
            //return uncoded nonterminals
            if (item >= 0 && item <= 255)
                return ((char)item).ToString();
            else if (item >= 301 && item <= 400)
                return MSTable[(ushort)item];
            else if (item >= 401 && item <= 500)
                return KWTable[(ushort)item];
            else if (item >= 501 && item <= 1000)
                return CTable[(ushort)item];
            else if (item >= 1001)
                return IDTable[(ushort)item];
            if (nonterminals == null) {
                nonterminals = new SortedList<int, string> {
                    [_PROGRAM_] = "<program>",
                    [_BLOCK_] = "<block>",
                    [_PID_] = "<procedure ID>",
                    [_ID_] = "<identifier>",
                    [_COND_EX_] = "<conditional expression>",
                    [_LOGICAL_] = "<logical>",
                    [_LOG_SUMMAND_] = "<logical summand>",
                    [_LOG_MULTIPLIER_] = "<logical multiplier>",
                    [_LOG_MUL_LIST_] = "<logical multiplier list>",
                    [_EXPRESSION_] = "<expression>",
                    [_COMP_OP_] = "<comparison operator>",
                    [_SUM_LIST_] = "<summand list>",
                    [_SUMMAND_] = "<summand>",
                    [_ADD_INSTR_] = "<add instruction>",
                    [_MULTIPLIER_] = "<multiplier>",
                    [_MUL_LIST_] = "<multiplier list>",
                    [_MUL_INSTR_] = "<multiplier instruction>",
                    [_CONSTANT_] = "<constant>",
                    [_COMPLEX_CONSTANT_] = "<complex constant>",
                    [_COMPLEX_NUMBER_] = "<complex number>",
                    [_VARIABLE_] = "<variable>",
                    [_VARIABLE_ID_] = "<variable ID>",
                    [_DIMENSION_] = "<dimension>",
                    [_COMPLEX_VARIABLE_] = "<complex variable>",
                    [_UNSIGNED_CONST_] = "<unsigned const>",
                    [_UNSIGNED_NUM_] = "<unsigned number>",
                    [_RIGHT_PART_] = "<right part>",
                    [_LEFT_PART_] = "<left part>",
                    [_EXP_LIST_] = "<expression list>",
                    [_INTEGER_] = "<integer>",
                    [_REAL_] = "<real>",
                    [_SIGN_] = "<sign>",
                    [_ACTUAL_ARGUMENTS_] = "<actual arguments>",
                    [_ARGUMENT_LIST_] = "<arguments list>",
                    [_BUILT_IN_FUNC_ID_] = "<builtin-function-identifier>"
                };
            }

            return nonterminals[item];
		}

        public static bool Start(List<int> _list, TextBox textBox = null) {
            // start point
            if (_list.Contains(-1))
                throw new ArrayTypeMismatchException("Wrong parsed list");

            list = new List<int>(_list);

            tree = new TreeNode();

            index = 0;

            textTree = textBox;

            if (SignalProgram()) {
                indents = new StringBuilder("");
                PrintTree(tree);
                return true;
            }
            return false;
        }

        static void PrintTree(TreeNode node = null) {
            if(textTree != null && node != null) {
                if (!makeIndents) {
                    textTree.Text += " { ";
                } else {
                    textTree.Text += indents + "{\n";
                }
                if (node.node != null) {
                    for(int i = 0; i < node.node.Count; i++) {
                        if (makeIndents) {
                            textTree.Text += indents + GetString(node.node[i]) + '\n';
                            indents.Append("\t");
                            PrintTree(node.childs[i]);
                            indents.Remove(0, 1);
                        }
                        else {
                            textTree.Text += GetString(node.node[i]) + ' ';
                            PrintTree(node.childs[i]);
                        }
                    }
                }
                if (!makeIndents) {
                    textTree.Text += " } ";
                } else {
                    textTree.Text += indents + "}\n";
                }
            }
		}

		static void SetTree(TreeNode node, List<int> items) {
            if (items != null) {
                node.node = new List<int>(items);
                node.childs = new List<TreeNode>(new TreeNode[items.Count]);
            } else {
                node.node = null;
                node.childs = null;
            }
		}

		static bool SignalProgram() {
//			 1.	<signal-program> --> <program>
			SetTree(tree, new List<int> { _PROGRAM_ });

			TreeNode c0 = new TreeNode();
			if(Program(c0)) {
				tree.childs[0] = c0;
				return true;
			}
			return false;
		}

		static bool Program(TreeNode node) {
//			2.	<program> --> PROGRAM <procedure-identifier> ;
//			<block>.
			SetTree(node, new List<int> { KWTable.GetKey("PROGRAM"), _PID_, ';', _BLOCK_, '.' });

			TreeNode c1 = new TreeNode(), c3 = new TreeNode();
            if (list[index++] == KWTable.GetKey("PROGRAM") &&
                ProcedureID(c1) &&
                list[index++] == ';' &&
			    Block(c3) &&
			    list[index++] == '.') {
				node.childs[1] = c1;
				node.childs[3] = c3;
				return true;
			}
			return false;
		}

		static bool Block(TreeNode node) {
//			3.	<block> --> BEGIN <conditional-expression> END
			SetTree(node, new List<int> { KWTable.GetKey("BEGIN"), _COND_EX_, KWTable.GetKey("END") });

			TreeNode c1 = new TreeNode();

			if(list[index++] == KWTable.GetKey("BEGIN") &&
			    ConditionalExp(c1) &&
			    list[index++] == KWTable.GetKey("END")) {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool ConditionalExp(TreeNode node) {
//			4.	<conditional-expression> --> <logical-summand>
//			<logical>
			SetTree(node, new List<int> { _LOG_SUMMAND_, _LOGICAL_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();
			if(LogicalSummand(c0) &&
			    Logical(c1)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool Logical(TreeNode node) {
//			5.	<logical> --> OR <logical-summand> <logical> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { KWTable.GetKey("OR"), _LOG_SUMMAND_, _LOGICAL_ } );

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();
			if(list[index++] != KWTable.GetKey("OR") || !LogicalSummand(c1) || !Logical(c2)) {
				index = CopyIndex;
                SetTree(node, null);
			} else {
				node.childs[1] = c1;
				node.childs[2] = c2;
			}

			return true;
		}

		static bool LogicalSummand(TreeNode node) {
//			6.	<logical-summand> --> <logical-multiplier>
//			<logical-multipliers-list>
			SetTree(node, new List<int> { _LOG_MULTIPLIER_, _LOG_MUL_LIST_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();
			if(LogicalMultiplier(c0) &&
			    LogicalMulList(c1)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool LogicalMulList(TreeNode node) {
//			7.	<logical-multipliers-list> --> AND <logical-mul-
//			tiplier> <logical-multipliers-list> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { KWTable.GetKey("AND"), _LOG_MULTIPLIER_, _LOG_MUL_LIST_ });

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();
			if(list[index++] != KWTable.GetKey("AND") || !LogicalMultiplier(c1) || !LogicalMulList(c2)) {
				index = CopyIndex;
                SetTree(node, null);
			} else {
				node.childs[1] = c1;
				node.childs[2] = c2;
			}
			return true;
		}

		static bool LogicalMultiplier(TreeNode node) {
//			8.	<logical-multiplier> --> <expression>
//			<comparison-operator><expression> |
//			[ <conditional-expression> ]
//			|
//			NOT <logical-multiplier>
			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();

            int CopyIndex = index;

            if (list[index] == '[') {
                ++index;

                SetTree(node, new List<int> { '[', _COND_EX_, ']' });

                if (ConditionalExp(c1) && list[index++] == ']') {
                    node.childs[1] = c1;
                    return true;
                }
            }
            else if (list[index] == KWTable.GetKey("NOT")) {
                ++index;

                SetTree(node, new List<int> { KWTable.GetKey("NOT"), _LOG_MULTIPLIER_ });

                if (LogicalMultiplier(c1)) {
                    node.childs[1] = c1;
                    return true;
                }
            }
            index = CopyIndex;

            SetTree(node, new List<int> { _EXPRESSION_, _COMP_OP_, _EXPRESSION_ });

			if(Expression(c0) && ComparisonOperator(c1) && Expression(c2)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				node.childs[2] = c2;
				return true;
			}
			return false;
		}

		static bool ComparisonOperator(TreeNode node) {
//			9.	<comparison-operator> --> <
//			|
//			<= |
//			=
//				|
//				<> |
//				>= |
//				>
			SetTree(node, new List<int>{ list[index] });

			if(list[index] == '<' ||
			    list[index] == '=' ||
			    list[index] == '>' ||
			    list[index] == MSTable.GetKey("<=") ||
			    list[index] == MSTable.GetKey("<>") ||
			    list[index] == MSTable.GetKey(">=")) {
				++index;
				return true;
			}
			return false;
		}

		static bool Expression(TreeNode node) {
//			10.	<expression> --> <summand> <summands-list>
//			|
//			- <summand> <summands-list>
            int CopyIndex = index;

            TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();

            if (list[index] == '-') {
                ++index;

                SetTree(node, new List<int> { '-', _SUMMAND_, _SUM_LIST_ });

                if (Summand(c1) && SummandsList(c2)) {
                    node.childs[1] = c1;
                    node.childs[2] = c2;
                    return true;
                }
            }
            index = CopyIndex;

            SetTree(node, new List<int> { _SUMMAND_, _SUM_LIST_ });

            if (Summand(c0) && SummandsList(c1)) {
                node.childs[0] = c0;
                node.childs[1] = c1;
                return true;
            }
            return false;
		}

		static bool SummandsList(TreeNode node) {
//			11.	<summands-list> --> <add-instruction> <summand>
//			<summands-list> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { _ADD_INSTR_, _SUMMAND_, _SUM_LIST_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();
			if(!AddInstruction(c0) || !Summand(c1) || !SummandsList(c2)) {
				index = CopyIndex;
                SetTree(node, null);
			} else {
				node.childs[0] = c0;
				node.childs[1] = c1;
				node.childs[2] = c2;
			}

			return true;
		}

		static bool AddInstruction(TreeNode node) {
//			12.	<add-instruction> -->
//			+
//			|
//			-
//			|
//			!
//			|
            SetTree(node, new List<int>{ list[index] });

			if(list[index] == '+' ||
			    list[index] == '-' ||
			    list[index] == '!') {
				++index;
				return true;
			}
			return false;
		}

		static bool Summand(TreeNode node) {
//			13.	<summand> --> <multiplier><multipliers-list>
			SetTree(node, new List<int> { _MULTIPLIER_, _MUL_LIST_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();
			if(Multiplier(c0) &&
			    MultiplierList(c1)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool MultiplierList(TreeNode node) {
//			14.	<multipliers-list> --> <multiplication-instruc-
//			tion> <multiplier><multipliers-list> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { _MUL_INSTR_, _MULTIPLIER_, _MUL_LIST_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();
			if(!MultiplicationInstr(c0) || !Multiplier(c1) || !MultiplierList(c2)) {
				index = CopyIndex;
                SetTree(node, null);
			} else {
				node.childs[0] = c0;
				node.childs[1] = c1;
				node.childs[2] = c2;
			}

			return true;
		}

		static bool MultiplicationInstr(TreeNode node) {
//			15.	<multiplication-instruction> -->
//			* |
//			/
//			|
//			&
//			|
//			MOD
            SetTree(node, new List<int>{ list[index] });

			if(list[index] == '*' ||
			   list[index] == '/' ||
			   list[index] == '&' ||
			   list[index] == KWTable.GetKey("MOD")) {
				++index;
				return true;
			}
			return false;
		}

		static bool Multiplier(TreeNode node) {
//			16.	<multiplier> --> <unsigned-constant> |
//			<complex-constant> |
//			<variable> |
//			<builtin-function-identifier><actual-
//			arguments> |
//			( <expression> ) |
//			- <multiplier>
//			|
//			^ <multiplier>
			int CopyIndex = index;

			SetTree(node, new List<int> { _UNSIGNED_CONST_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();

			if(UnsignedConstant(c0)) {
				node.childs[0] = c0;
				return true;
			}

			index = CopyIndex;

			SetTree(node, new List<int> { _COMPLEX_CONSTANT_ });

			if(ComplexConstant(c0)) {
				node.childs[0] = c0;
				return true;
			}

            index = CopyIndex;

            SetTree(node, new List<int> { _BUILT_IN_FUNC_ID_, _ACTUAL_ARGUMENTS_ });

            if (BuiltInFuncID(c0) && ActualArguments(c1)) {
                node.childs[0] = c0;
                node.childs[1] = c1;
                return true;
            }

			index = CopyIndex;

			SetTree(node, new List<int> { '(', _EXPRESSION_, ')' });

			if(list[index++] == '(' && Expression(c1) && list[index++] == ')') {
				node.childs[1] = c1;
				return true;
			}
			index = CopyIndex;

			SetTree(node, new List<int> { '-', _MULTIPLIER_ });

			if(list[index++] == '-' && Multiplier(c1)) {
				node.childs[1] = c1;
				return true;
			}
			index = CopyIndex;

			SetTree(node, new List<int> { '^', _MULTIPLIER_ });

			if(list[index++] == '^' && Multiplier(c1)) {
				node.childs[1] = c1;
				return true;
			}
            index = CopyIndex;

            SetTree(node, new List<int> { _VARIABLE_ });

            if(Variable(c0)) {
                node.childs[0] = c0;
                return true;
            }

            return false;
		}

		static bool Variable(TreeNode node) {
//			17.	<variable> --> <variable-identifier>
//			<dimension>|
//			<complex-variable>
			int CopyIndex = index;

			SetTree(node, new List<int> { _VARIABLE_ID_ });

			TreeNode c0 = new TreeNode();

			if(VariableID(c0)) {
				node.childs[0] = c0;
				return true;
			}

			index = CopyIndex;
			SetTree(node, new List<int> { _COMPLEX_VARIABLE_ });

			if(ComplexVariable(c0)) {
				node.childs[0] = c0;
				return true;
			}

            index = CopyIndex;

            SetTree(node, new List<int> { _DIMENSION_ });

            if(Dimension(c0)) {
                node.childs[0] = c0;
                return true;
            }
            return false;
		}

		static bool ComplexVariable(TreeNode node) {
//			18.	<complex-variable> --> "<complex-number>"
			SetTree(node, new List<int> { '"', _COMPLEX_NUMBER_, '"' });

			TreeNode c1 = new TreeNode();

			if(list[index++] == '"' &&
			    ComplexNumber(c1) &&
			    list[index++] == '"') {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool Dimension(TreeNode node) {
//			19.	<dimension> -->[<expression><expressions-
//				list>]|
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { '[', _EXPRESSION_, _EXP_LIST_, ']' } );

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();

            if (list[index++] == '[' && Expression(c1) &&
                ExpressionList(c2) && list[index++] == ']') {
                node.childs[1] = c1;
                node.childs[2] = c2;
            }
            else {
                index = CopyIndex;
                SetTree(node, null);
            }

            return true;
		}

		static bool ExpressionList(TreeNode node) {
//			20.	<expressions-list> -->
//			,<expression><expressions-list> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { ',', _EXPRESSION_, _EXP_LIST_ });

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();

            if (list[index++] == ',' && Expression(c1) && ExpressionList(c2)) {
                node.childs[1] = c1;
                node.childs[2] = c2;
            }
            else {
                index = CopyIndex;
                SetTree(node, null);
            }

            return true;
		}

		static bool ComplexConstant(TreeNode node) {
//			21.	<complex-constant> --> '<complex-number>'
			SetTree(node, new List<int> { '\'', _COMPLEX_NUMBER_, '\'' });

			TreeNode c1 = new TreeNode();

			if(list[index++] == '\'' &&
			    ComplexNumber(c1) &&
			    list[index++] == '\'') {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool UnsignedConstant(TreeNode node) {
//			22.	<unsigned-constant> --> <unsigned-number>
			SetTree(node, new List<int> { _UNSIGNED_NUM_ });

			TreeNode c0 = new TreeNode();

			if(UnsignedNumber(c0)) {
				node.childs[0] = c0;
				return true;
			}
			return false;
		}

		static bool ComplexNumber(TreeNode node) {
//			23.	<complex-number> --> <left-part> <right-part>
			SetTree(node, new List<int> { _LEFT_PART_, _RIGHT_PART_ });

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();

			if(LeftPart(c0) && RightPart(c1)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool LeftPart(TreeNode node) {
//			24.	<left-part> --> <expression> |
//			<empty>
			int CopyIndex = index;

			SetTree(node, new List<int> { _EXPRESSION_ });

			TreeNode c0 = new TreeNode();

            if (Expression(c0)) {
                node.childs[0] = c0;
            }
            else {
                index = CopyIndex;
                SetTree(node, null);
            }

            return true;
		}

		static bool RightPart(TreeNode node) {
//			25.	<right-part> --> ,<expression> |
//			$EXP( <expression> )
//			|
//			<empty>
			TreeNode c1 = new TreeNode();

			int CopyIndex = index;

            if (list[index] == ',') {
                ++index;

                SetTree(node, new List<int> { ',', _EXPRESSION_ });

                if (Expression(c1)) {
                    node.childs[1] = c1;
                    return true;
                }
            }
            else if (list[index] == KWTable.GetKey("$EXP")) {
                ++index;

                SetTree(node, new List<int> { KWTable.GetKey("$EXP"), '(', _EXPRESSION_, ')' });

                if (list[index++] == '(' &&
                Expression(c1) &&
                list[index++] == ')') {
                    node.childs[2] = c1;
                    return true;
                }
            }
            index = CopyIndex;

            SetTree(node, null);

            return true;
        }

        static bool ActualArguments(TreeNode node) {
            //26. <actual arguments> --> ( <argument-list> )
            SetTree(node, new List<int> { '(', _ARGUMENT_LIST_, ')' });

            TreeNode c1 = new TreeNode();

            if (list[index++] == '(' &&
                ArgumentList(c1) &&
                list[index++] == ')') {
                node.childs[1] = c1;
                return true;
            }
            return false;
        }

		static bool VariableID(TreeNode node) {
//			27.	<variable-identifier> --> <identifier>
			SetTree(node, new List<int> { _ID_ });

			TreeNode c0 = new TreeNode();

			if(!ID(c0) ||
				IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTable.States.UNDEFINED &&
				IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTable.States.VAR)
				return false;
			else
				node.childs[0] = c0;

			if(IDTable.GetType((ushort)list[index]) == Concrete.IdentifierTableSpace.IdentifierTable.States.UNDEFINED)
				IDTable.SetType((ushort)list[index], IdentifierTable.States.VAR);

            ++index;

			return true;
		}

		static bool ProcedureID(TreeNode node) {
//			28.	<procedure-identifier> --> <identifier>
			SetTree(node, new List<int> { _ID_ });

			TreeNode c0 = new TreeNode();
			if(!ID(c0) ||
			    IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTable.States.UNDEFINED)
				return false;
			else
				node.childs[0] = c0;

			IDTable.SetType((ushort)list[index++], IdentifierTable.States.PROCEDURE);

			return true;
		}

        static bool ArgumentList(TreeNode node) {
            //29. <argument-list> --> <unsigned-number> <argument-list> |
            //<empty>
            int CopyIndex = index;

            SetTree(node, new List<int> { _UNSIGNED_NUM_, _ARGUMENT_LIST_ });

            TreeNode c0 = new TreeNode(), c1 = new TreeNode();

            if (UnsignedNumber(c0) &&
                ArgumentList(c1)) {
                node.childs[0] = c0;
                node.childs[1] = c1;
            } else {
                index = CopyIndex;
                SetTree(node, null);
            }
            return true;
        }

        static bool BuiltInFuncID(TreeNode node) {
            // 30.  <builtin-function-identifier> --> <identifier>
            SetTree(node, new List<int> { _ID_ });

            TreeNode c0 = new TreeNode();

            if (!ID(c0) ||
                IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTable.States.BUILT_IN)
                return false;
            else
                node.childs[0] = c0;

            ++index;

            return true;
        }

		static bool UnsignedNumber(TreeNode node) {
            //33.	<unsigned-number> --> <integer> | <real>
            int CopyIndex = index;

            TreeNode c0 = new TreeNode();

            if (Integer(c0)) {
                SetTree(node, new List<int> { _INTEGER_ });
            }
            else if (Real(c0)) {
                SetTree(node, new List<int> { _REAL_ });
            }
            else
                return false;

            ++index;
            node.childs[0] = c0;

            return true;
        }

        static bool Integer(TreeNode node) {
            // check for integer number
            SetTree(node, new List<int>{ list[index] });

            return list[index] >= 501 && list[index] <= 1000 && CTable.GetType((ushort)list[index]) == ConstantsTable.States.INTEGER;
        }

        static bool Real(TreeNode node) {
            // check for real number
            SetTree(node, new List<int>{ list[index] });

            return list[index] >= 501 && list[index] <= 1000 && CTable.GetType((ushort)list[index]) == ConstantsTable.States.REAL;
        }

        static bool Sign(TreeNode node) {
            // 38.  <sign> --> + 
            // - |
            // <empty>
			if(list[index] == '+' || list[index] == '-') {
                SetTree(node, new List<int>{ list[index] });
				++index;
			}
			return true;
		}

		static bool ID(TreeNode node) {
//			check for terminal ID
            SetTree(node, new List<int>{ list[index] });

			return list[index] >= 1001;
		}
	}
}