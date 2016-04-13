using System;
using Concrete.MultySymbolSeparatorsTableSpace;
using Concrete.KeyWordTableSpace;
using Concrete.ConstantsTableSpace;
using Concrete.IdentifierTableSpace;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Concrete.Syntactycal {
	internal class TreeNode {
        /// <summary>
        /// just a node of tree, can be a root
        /// </summary>
		public List<int> node;  //data of node
        public int rule;
		public List<TreeNode> childs;   //childs

        public TreeNode(int _rule = 0, List<int> _node = null) {
            node = _node;
            rule = _rule;
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
		static IdentifierTables IDTable;    //ID Table

		static List<int> list;  //parsed after lexical analyzer

		static Int32 index; //current pointer

        static public Int32 Index { get { return index; } } //read-only property

        static Stack<int> stack; //work stack

		static TextBox stackTrace;   //result tree in string

        static public String StackTrace { get { return stackTrace.Text; } } //read-only property to get tree

        static TreeNode tree;  //tree

        static List<List<int>> rules;   //parsing rules

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
            _INT_PART_ = -30,
            _FRAC_PART_ = -31,
            _UNSIGNED_INT_ = -32,
            _SIGN_ = -33,
            _ACTUAL_ARGUMENTS_ = -34,
            _ARGUMENT_LIST_ = -35, 
            _BUILT_IN_FUNC_ID_ = -36;

        static List<int> GetRule(int index) {
            //return rule
            if (rules == null) {
                rules = new List<List<int>>();

                //0
                rules.Add(new List<int>(new int[] { _PROGRAM_ }));
                //1
                rules.Add(new List<int>(new int[] { KWTable.GetKey("PROGRAM"), _PID_, ';', _BLOCK_, '.' }));
                //2
                rules.Add(new List<int>(new int[] { KWTable.GetKey("BEGIN"), _COND_EX_, KWTable.GetKey("END") }));
                //3
                rules.Add(new List<int>(new int[] { _LOG_SUMMAND_, _LOGICAL_ }));
                //4
                rules.Add(new List<int>(new int[] { KWTable.GetKey("OR"), _LOG_SUMMAND_, _LOGICAL_ }));
                //5
                rules.Add(new List<int>(new int[] { _LOG_MULTIPLIER_, _LOG_MUL_LIST_ }));
                //6
                rules.Add(new List<int>(new int[] { KWTable.GetKey("AND"), _LOG_MULTIPLIER_, _LOG_MUL_LIST_ }));
                //7
                rules.Add(new List<int>(new int[] { '[', _COND_EX_, ']' }));
                //8
                rules.Add(new List<int>(new int[] { _EXPRESSION_, _COMP_OP_, _EXPRESSION_ }));
                //9
                rules.Add(new List<int>(new int[] { KWTable.GetKey("NOT"), _LOG_MULTIPLIER_ }));
                //10
                rules.Add(new List<int>(new int[] { _SUMMAND_, _SUM_LIST_ }));
                //11
                rules.Add(new List<int>(new int[] { '-', _SUMMAND_, _SUM_LIST_ }));
                //12
                rules.Add(new List<int>(new int[] { _ADD_INSTR_, _SUMMAND_, _SUM_LIST_ }));
                //13
                rules.Add(new List<int>(new int[] { _MULTIPLIER_, _MUL_LIST_ }));
                //14
                rules.Add(new List<int>(new int[] { _MUL_INSTR_, _MULTIPLIER_, _MUL_LIST_ }));
                //15
                rules.Add(new List<int>(new int[] { _UNSIGNED_CONST_ }));
                //16
                rules.Add(new List<int>(new int[] { _COMPLEX_CONSTANT_ }));
                //17
                rules.Add(new List<int>(new int[] { _BUILT_IN_FUNC_ID_, _ACTUAL_ARGUMENTS_ }));
                //18
                rules.Add(new List<int>(new int[] { '(', _EXPRESSION_, ')' }));
                //19
                rules.Add(new List<int>(new int[] { '-', _MULTIPLIER_ }));
                //20
                rules.Add(new List<int>(new int[] { '^', _MULTIPLIER_ }));
                //21
                rules.Add(new List<int>(new int[] { _VARIABLE_ }));
                //22
                rules.Add(new List<int>(new int[] { _VARIABLE_ID_ }));
                //23
                rules.Add(new List<int>(new int[] { _COMPLEX_VARIABLE_ }));
                //24
                rules.Add(new List<int>(new int[] { _DIMENSION_ }));
                //25
                rules.Add(new List<int>(new int[] { '"', _COMPLEX_NUMBER_, '"' }));
                //26
                rules.Add(new List<int>(new int[] { '[', _EXPRESSION_, _EXP_LIST_, ']' }));
                //27
                rules.Add(new List<int>(new int[] { ',', _EXPRESSION_, _EXP_LIST_ }));
                //28
                rules.Add(new List<int>(new int[] { '\'', _COMPLEX_NUMBER_, '\'' }));
                //29
                rules.Add(new List<int>(new int[] { _UNSIGNED_NUM_ }));
                //30
                rules.Add(new List<int>(new int[] { _LEFT_PART_, _RIGHT_PART_ }));
                //31
                rules.Add(new List<int>(new int[] { _EXPRESSION_ }));
                //32
                rules.Add(new List<int>(new int[] { ',', _EXPRESSION_ }));
                //33
                rules.Add(new List<int>(new int[] { KWTable.GetKey("$EXP"), '(', _EXPRESSION_, ')' }));
                //34
                rules.Add(new List<int>(new int[] { '(', _ARGUMENT_LIST_, ')' }));
                //35
                rules.Add(new List<int>(new int[] { _ID_ }));
                //36
                rules.Add(new List<int>(new int[] { _UNSIGNED_INT_, _ARGUMENT_LIST_ }));
                //37
                rules.Add(new List<int>(new int[] { _INT_PART_, _FRAC_PART_ }));
                //38
                rules.Add(new List<int>(new int[] { _UNSIGNED_INT_ }));
                //39
                rules.Add(new List<int>(new int[] { '#', _SIGN_, _UNSIGNED_INT_ }));


                //40
                //rules.Add(new List<int>(new int[] { list[index] }));
                //41
                //rules.Add(null);
            }
            return new List<int>(rules[index]);
        }

        public static void SetTables(Concrete.TableSpace.Table[] tables) {
            //set all tables
			if(tables.Length != 4)
				throw new ArrayTypeMismatchException("Wrong parse tables");

			MSTable = tables[0] as MultySymbolSeparatorsTable;
			KWTable = tables[1] as KeyWordsTable;
			CTable = tables[2] as ConstantsTable;
			IDTable = tables[3] as IdentifierTables;
		}

		static string GetString(int item) {
            //return uncoded nonterminals
            if (item == _PROGRAM_)
                return "<program>";
            else if (item == _BLOCK_)
                return "<block>";
            else if (item == _PID_)
                return "<procedure ID>";
            else if (item == _ID_)
                return "<identifier>";
            else if (item == _COND_EX_)
                return "<conditional expression>";
            else if (item == _LOGICAL_)
                return "<logical>";
            else if (item == _LOG_SUMMAND_)
                return "<logical summand>";
            else if (item == _LOG_MULTIPLIER_)
                return "<logical multiplier>";
            else if (item == _LOG_MUL_LIST_)
                return "<logical multiplier list>";
            else if (item == _EXPRESSION_)
                return "<expression>";
            else if (item == _COMP_OP_)
                return "<comparison operator>";
            else if (item == _SUM_LIST_)
                return "<summand list>";
            else if (item == _SUMMAND_)
                return "<summand>";
            else if (item == _ADD_INSTR_)
                return "<add instruction>";
            else if (item == _MULTIPLIER_)
                return "<multiplier>";
            else if (item == _MUL_LIST_)
                return "<multiplier list>";
            else if (item == _MUL_INSTR_)
                return "<multiplier instruction>";
            else if (item == _CONSTANT_)
                return "<constant>";
            else if (item == _COMPLEX_CONSTANT_)
                return "<complex constant>";
            else if (item == _COMPLEX_NUMBER_)
                return "<complex number>";
            else if (item == _VARIABLE_)
                return "<variable>";
            else if (item == _VARIABLE_ID_)
                return "<variable id>";
            else if (item == _DIMENSION_)
                return "<dimension>";
            else if (item == _COMPLEX_VARIABLE_)
                return "<complex variable>";
            else if (item == _UNSIGNED_CONST_)
                return "<unsigned const>";
            else if (item == _UNSIGNED_NUM_)
                return "<unsigned number>";
            else if (item == _RIGHT_PART_)
                return "<right part>";
            else if (item == _LEFT_PART_)
                return "<left part>";
            else if (item == _EXP_LIST_)
                return "<expression list>";
            else if (item == _INT_PART_)
                return "<integer part>";
            else if (item == _FRAC_PART_)
                return "<fractional part>";
            else if (item == _UNSIGNED_INT_)
                return "<unsigned int>";
            else if (item == _SIGN_)
                return "<sign>";
            else if (item == _ACTUAL_ARGUMENTS_)
                return "<actual-arguments>";
            else if (item == _ARGUMENT_LIST_)
                return "<argument-list>";
            else if (item == _BUILT_IN_FUNC_ID_)
                return "<builtin-function-identifier>";
            else if (item >= 0 && item <= 255)
                return ((char)item).ToString();
            else if (item >= 301 && item <= 400)
                return MSTable[(ushort)item];
            else if (item >= 401 && item <= 500)
                return KWTable[(ushort)item];
            else if (item >= 501 && item <= 1000)
                return CTable[(ushort)item];
            else if (item >= 1001)
                return IDTable[(ushort)item];
            else
                return null;
		}

		public static bool Start(List<int> _list, TextBox textBox = null) {
            // start point
			if(_list.Contains(-1))
				throw new ArrayTypeMismatchException("Wrong parsed list");

			list = new List<int>(_list);

            stack = new Stack<int>();
			stack.Push(0);

            tree = new TreeNode();

			index = 0;

            stackTrace = textBox;
            
			if(SignalProgram()) {
				stack.Pop();
				if(stack.Count == 0) {
					PrintTree(tree);
					return true;
				}
			}
			return false;
		}

		static bool CheckTop(int _TYPE_) {
			return stack.Pop() == _TYPE_;
		}

		static void PrintTree(TreeNode node = null) {
            if(stackTrace != null && node != null) {
                stackTrace.Text += " { ";
                if(node.node != null) {
                    for(int i = 0; i < node.node.Count; i++) {
                        stackTrace.Text += GetString(node.node[i]) + ' ';
                        PrintTree(node.childs[i]);
                    }
                }
                stackTrace.Text += " } ";
            }
		}

		static void SetStackAndTree(TreeNode node, List<int> items, int ruleNum) {
            if (items != null) {
                node.node = new List<int>(items);
                node.childs = new List<TreeNode>(new TreeNode[items.Count]);
                node.rule = ruleNum;

                items.Reverse();

                foreach (var item in items)
                    stack.Push(item);
            } else {
                node.node = null;
                node.childs = null;
                node.rule = rules.Count - 1;
            }
		}

		static bool SignalProgram() {
//			 1.	<signal-program> --> <program>

			SetStackAndTree(tree, GetRule(0), 0);

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
			if(!CheckTop(_PROGRAM_))
				return false;

			SetStackAndTree(node, GetRule(1), 1);

			TreeNode c1 = new TreeNode(), c3 = new TreeNode();
			if(list[index++] == stack.Pop() &&
			    ProcedureID(c1) &&
			    list[index++] == stack.Pop() &&
			    Block(c3) &&
			    list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				node.childs[3] = c3;
				return true;
			}
			return false;
		}

		static bool Block(TreeNode node) {
//			3.	<block> --> BEGIN <conditional-expression> END
			if(!CheckTop(_BLOCK_))
				return false;

			SetStackAndTree(node, GetRule(2), 2);

			TreeNode c1 = new TreeNode();

			if(list[index++] == stack.Pop() &&
			    ConditionalExp(c1) &&
			    list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool ConditionalExp(TreeNode node) {
//			4.	<conditional-expression> --> <logical-summand>
//			<logical>
			if(!CheckTop(_COND_EX_))
				return false;
				
			SetStackAndTree(node, GetRule(3), 3);

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
			if(!CheckTop(_LOGICAL_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(4), 4);

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();
			if(list[index++] != stack.Pop() || !LogicalSummand(c1) || !Logical(c2)) {
				stack = new Stack<int>(CopyStack);
				index = CopyIndex;
				node.node = null;
				node.childs = null;
			} else {
				node.childs[1] = c1;
				node.childs[2] = c2;
			}

			return true;
		}

		static bool LogicalSummand(TreeNode node) {
//			6.	<logical-summand> --> <logical-multiplier>
//			<logical-multipliers-list>
			if(!CheckTop(_LOG_SUMMAND_))
				return false;

			SetStackAndTree(node, GetRule(5), 5);

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
			if(!CheckTop(_LOG_MUL_LIST_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(6), 6);

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();
			if(list[index++] != stack.Pop() || !LogicalMultiplier(c1) || !LogicalMulList(c2)) {
				stack = new Stack<int>(CopyStack);
				index = CopyIndex;
                SetStackAndTree(node, null, rules.Count - 1);
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
			if(!CheckTop(_LOG_MULTIPLIER_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(7), 7);

			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();
			if(list[index] == stack.Pop()) {
				++index;

				if(ConditionalExp(c1) && list[index++] == stack.Pop()) {
					node.childs[1] = c1;
					return true;
				}
			}
			stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(8), 8);

			if(Expression(c0) && ComparisonOperator(c1) && Expression(c2)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				node.childs[2] = c2;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(9), 9);

			if(list[index++] == stack.Pop() &&
			    LogicalMultiplier(c1)) {
				node.childs[1] = c1;
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
			if(!CheckTop(_COMP_OP_))
				return false;

			SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
			stack.Pop();

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
			if(!CheckTop(_EXPRESSION_))
				return false;

            Stack<int> CopyStack = new Stack<int>(stack);
            int CopyIndex = index;

            TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();

            SetStackAndTree(node, GetRule(11), 11);

            if (list[index++] == stack.Pop() &&
                Summand(c1) && SummandsList(c2)) {
                node.childs[1] = c1;
                node.childs[2] = c2;
                return true;
            }

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;

            SetStackAndTree(node, GetRule(10), 10);

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
			if(!CheckTop(_SUM_LIST_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(12), 12);

			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();
			if(!AddInstruction(c0) || !Summand(c1) || !SummandsList(c2)) {
				stack = new Stack<int>(CopyStack);
				index = CopyIndex;
                SetStackAndTree(node, null, rules.Count - 1);
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
			if(!CheckTop(_ADD_INSTR_))
				return false;

            SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
            stack.Pop();

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
			if(!CheckTop(_SUMMAND_))
				return false;

			SetStackAndTree(node, GetRule(13), 13);

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
			if(!CheckTop(_MUL_LIST_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(14), 14);

			TreeNode c0 = new TreeNode(), c1 = new TreeNode(), c2 = new TreeNode();
			if(!MultiplicationInstr(c0) || !Multiplier(c1) || !MultiplierList(c2)) {
				stack = new Stack<int>(CopyStack);
				index = CopyIndex;
                SetStackAndTree(node, null, rules.Count - 1);
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
			if(!CheckTop(_MUL_INSTR_))
				return false;

            SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
            stack.Pop();

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
			if(!CheckTop(_MULTIPLIER_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(15), 15);

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();

			if(UnsignedConstant(c0)) {
				node.childs[0] = c0;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(16), 16);

			if(ComplexConstant(c0)) {
				node.childs[0] = c0;
				return true;
			}

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;

            SetStackAndTree(node, GetRule(17), 17);

            if (BuiltInFuncID(c0) && ActualArguments(c1)) {
                node.childs[0] = c0;
                node.childs[1] = c1;
                return true;
            }

            stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(18), 18);

			if(list[index++] == stack.Pop() && Expression(c1) && list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(19), 19);

			if(list[index++] == stack.Pop() && Multiplier(c1)) {
				node.childs[1] = c1;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;

			SetStackAndTree(node, GetRule(20), 20);

			if(list[index++] == stack.Pop() && Multiplier(c1)) {
				node.childs[1] = c1;
				return true;
			}

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;

            SetStackAndTree(node, GetRule(21), 21);

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
			if(!CheckTop(_VARIABLE_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(22), 22);

			TreeNode c0 = new TreeNode();

			if(VariableID(c0)) {
				node.childs[0] = c0;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;
			SetStackAndTree(node, GetRule(23), 23);

			if(ComplexVariable(c0)) {
				node.childs[0] = c0;
				return true;
			}

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;

            SetStackAndTree(node, GetRule(24), 24);

            if(Dimension(c0)) {
                node.childs[0] = c0;
                return true;
            }
            return false;
		}

		static bool ComplexVariable(TreeNode node) {
//			18.	<complex-variable> --> "<complex-number>"
			if(!CheckTop(_COMPLEX_VARIABLE_))
				return false;

			SetStackAndTree(node, GetRule(25), 25);

			TreeNode c1 = new TreeNode();

			if(list[index++] == stack.Pop() &&
			    ComplexNumber(c1) &&
			    list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool Dimension(TreeNode node) {
//			19.	<dimension> -->[<expression><expressions-
//				list>]|
//			<empty>
			if(!CheckTop(_DIMENSION_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(26), 26);

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();

			if(list[index++] == stack.Pop() && Expression(c1) &&
			    ExpressionList(c2) && list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				node.childs[2] = c2;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;
            SetStackAndTree(node, null, rules.Count - 1);

            return true;
		}

		static bool ExpressionList(TreeNode node) {
//			20.	<expressions-list> -->
//			,<expression><expressions-list> |
//			<empty>
			if(!CheckTop(_EXP_LIST_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(27), 27);

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();

			if(list[index++] == stack.Pop() && Expression(c1) && ExpressionList(c2)) {
				node.childs[1] = c1;
				node.childs[2] = c2;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;
            SetStackAndTree(node, null, rules.Count - 1);

            return true;
		}

		static bool ComplexConstant(TreeNode node) {
//			21.	<complex-constant> --> '<complex-number>'
			if(!CheckTop(_COMPLEX_CONSTANT_))
				return false;

			SetStackAndTree(node, GetRule(28), 28);

			TreeNode c1 = new TreeNode();

			if(list[index++] == stack.Pop() &&
			    ComplexNumber(c1) &&
			    list[index++] == stack.Pop()) {
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool UnsignedConstant(TreeNode node) {
//			22.	<unsigned-constant> --> <unsigned-number>
			if(!CheckTop(_UNSIGNED_CONST_))
				return false;

			SetStackAndTree(node, GetRule(29), 29);

			TreeNode c0 = new TreeNode();

			if(UnsignedNumber(c0)) {
				node.childs[0] = c0;
				return true;
			}
			return false;
		}

		static bool ComplexNumber(TreeNode node) {
//			23.	<complex-number> --> <left-part> <right-part>
			if(!CheckTop(_COMPLEX_NUMBER_))
				return false;

			SetStackAndTree(node, GetRule(30), 30);

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
			if(!CheckTop(_LEFT_PART_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(31), 31);

			TreeNode c0 = new TreeNode();

			if(Expression(c0)) {
				node.childs[0] = c0;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;
            SetStackAndTree(node, null, rules.Count - 1);

            return true;
		}

		static bool RightPart(TreeNode node) {
//			25.	<right-part> --> ,<expression> |
//			$EXP( <expression> )
//			|
//			<empty>
			if(!CheckTop(_RIGHT_PART_))
				return false;

			TreeNode c1 = new TreeNode();

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

            SetStackAndTree(node, GetRule(32), 32);

            if (list[index++] == stack.Pop() &&
                Expression(c1)) {
                node.childs[1] = c1;
                return true;
            }

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;

            SetStackAndTree(node, GetRule(33), 33);

            if (list[index++] == stack.Pop() &&
                list[index++] == stack.Pop() &&
                Expression(c1) &&
                list[index++] == stack.Pop()) {
                node.childs[2] = c1;
                return true;
            }

            stack = new Stack<int>(CopyStack);
            index = CopyIndex;
            SetStackAndTree(node, null, rules.Count - 1);

            return true;
        }

        static bool ActualArguments(TreeNode node) {
            //26. <actual arguments> --> ( <argument-list> )
            if (!CheckTop(_ACTUAL_ARGUMENTS_))
                return false;

            SetStackAndTree(node, GetRule(34), 34);

            TreeNode c1 = new TreeNode();

            if (list[index++] == stack.Pop() &&
                ArgumentList(c1) &&
                list[index++] == stack.Pop()) {
                node.childs[1] = c1;
                return true;
            }
            return false;
        }

		static bool VariableID(TreeNode node) {
//			27.	<variable-identifier> --> <identifier>
			if(!CheckTop(_VARIABLE_ID_))
				return false;

			SetStackAndTree(node, GetRule(35), 35);

			TreeNode c0 = new TreeNode();

			if(!ID(c0) ||
				IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTables.States.UNDEFINED &&
				IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTables.States.VAR)
				return false;
			else
				node.childs[0] = c0;

			if(IDTable.GetType((ushort)list[index]) == Concrete.IdentifierTableSpace.IdentifierTables.States.UNDEFINED)
				IDTable.SetType((ushort)list[index], IdentifierTables.States.VAR);

            ++index;

			return true;
		}

		static bool ProcedureID(TreeNode node) {
//			28.	<procedure-identifier> --> <identifier>
			if(!CheckTop(_PID_))
				return false;
			SetStackAndTree(node, GetRule(35), 35);

			TreeNode c0 = new TreeNode();
			if(!ID(c0) ||
			    IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTables.States.UNDEFINED)
				return false;
			else
				node.childs[0] = c0;

			IDTable.SetType((ushort)list[index++], IdentifierTables.States.PROCEDURE);

			return true;
		}

        static bool ArgumentList(TreeNode node) {
            //29. <argument-list> --> <unsigned-integer> <argument-list> |
            //<empty>
            if (!CheckTop(_ARGUMENT_LIST_))
                return false;

            Stack<int> CopyStack = new Stack<int>(stack);
            int CopyIndex = index;

            SetStackAndTree(node, GetRule(36), 36);

            TreeNode c0 = new TreeNode(), c1 = new TreeNode();

            if (UnsignedInt(c0) &&
                ArgumentList(c1)) {
                node.childs[0] = c0;
                node.childs[1] = c1;
            } else {
                stack = new Stack<int>(CopyStack);
                index = CopyIndex;
                SetStackAndTree(node, null, rules.Count - 1);
            }
            return true;
        }

        static bool BuiltInFuncID(TreeNode node) {
            // 30.  <builtin-function-identifier> --> <identifier>
            if (!CheckTop(_BUILT_IN_FUNC_ID_))
                return false;

            SetStackAndTree(node, GetRule(35), 35);

            TreeNode c0 = new TreeNode();

            if (!ID(c0) ||
                IDTable.GetType((ushort)list[index]) != Concrete.IdentifierTableSpace.IdentifierTables.States.BUILT_IN)
                return false;
            else
                node.childs[0] = c0;

            ++index;

            return true;
        }

		static bool UnsignedNumber(TreeNode node) {
//			33.	<unsigned-number> --> <integer-part>
//			<fractional-part>
			if(!CheckTop(_UNSIGNED_NUM_))
				return false;

			SetStackAndTree(node, GetRule(37), 37);

			TreeNode c0 = new TreeNode(), c1 = new TreeNode();

			if(IntPart(c0) && FracPart(c1)) {
				node.childs[0] = c0;
				node.childs[1] = c1;
				return true;
			}
			return false;
		}

		static bool IntPart(TreeNode node) {
//			34.	<integer-part> --> <unsigned-integer>
			if(!CheckTop(_INT_PART_))
				return false;

			SetStackAndTree(node, GetRule(38), 38);

			TreeNode c0 = new TreeNode();

			if(UnsignedInt(c0)) {
				node.childs[0] = c0;
				return true;
			}
			return false;
		}

		static bool FracPart(TreeNode node) {
//			35.	<fractional-part> --> #<sign><unsigned-integer>|
//			<empty>
			if(!CheckTop(_FRAC_PART_))
				return false;

			Stack<int> CopyStack = new Stack<int>(stack);
			int CopyIndex = index;

			SetStackAndTree(node, GetRule(39), 39);

			TreeNode c1 = new TreeNode(), c2 = new TreeNode();

			if(list[index++] == stack.Pop() && Sign(c1) && UnsignedInt(c2)) {
				node.childs[1] = c1;
				node.childs[2] = c2;
				return true;
			}

			stack = new Stack<int>(CopyStack);
			index = CopyIndex;
            SetStackAndTree(node, null, rules.Count - 1);

            return true;
		}

		static bool UnsignedInt(TreeNode node) {
//			36.	<unsigned-integer> --> <digit><digits-string>
			if(!CheckTop(_UNSIGNED_INT_))
				return false;

            SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
            stack.Pop();

			return list[index] >= 501 && list[index++] <= 1000;
		}

		static bool Sign(TreeNode node) {
            // 38.  <sign> --> + 
            // - |
            // <empty>
			if(!CheckTop(_SIGN_))
				return false;

			if(list[index] == '+' || list[index] == '-') {
                SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
                stack.Pop();
				++index;
			}
			return true;
		}

		static bool ID(TreeNode node) {
//			check for terminal ID
			if(!CheckTop(_ID_))
				return false;

            SetStackAndTree(node, new List<int>(new int[] { list[index] }), 40);
            stack.Pop();

			return list[index] >= 1001;
		}
	}
}