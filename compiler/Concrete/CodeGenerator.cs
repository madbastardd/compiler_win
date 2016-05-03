using Concrete.ConstantsTableSpace;
using Concrete.IdentifierTableSpace;
using Concrete.KeyWordTableSpace;
using Concrete.MultySymbolSeparatorsTableSpace;
using Concrete.Syntactycal;
using System;
using System.Windows.Controls;

namespace Concrete.CodeGeneratorSpace {
    static class CodeGenerator {
        static MultySymbolSeparatorsTable MSTable;  //MultySymbol Separators Table
        static KeyWordsTable KWTable;   //Keywords Table
        static ConstantsTable CTable;   //Constant Table
        static IdentifierTable IDTable;    //ID Table

        enum Type {
            INTEGER, REAL, COMPLEX, ANOTHER
        };

        class IncorrectTypeException : Exception {
            //empty class
        }

        static int labelCounter;

        static string addInstruction = "ADD";
        static string mulInstruction = "MUL";
        static string functionToCall = "SIN";

        static String code;

        static public String Code {
            get {
                return code;
            }
        }

        public static void ShowInTextView(TextBox textBox) {
            textBox.Text = code;
        }

        public static void SetTables(Concrete.TableSpace.Table[] tables) {
            //set all tables
            if (tables.Length != 4)
                throw new ArrayTypeMismatchException("Wrong parse tables");

            MSTable = tables[0] as MultySymbolSeparatorsTable;
            KWTable = tables[1] as KeyWordsTable;
            CTable = tables[2] as ConstantsTable;
            IDTable = tables[3] as IdentifierTable;
        }

        static public bool CreateCode(TreeNode tree) {
            labelCounter = 0;

            code = "model small, c";

            code += "\n\n.data";

            string vars = Variables();

            code += vars;

            code += "\n\n.stack 100h";

            code += "\n\n.code\n\t?main:";
            code += "\n\t\tMOV AX, @data";
            code += "\n\t\tMOV DS, AX";

            string program;
            try {
                program = Program(tree.childs[0]);
            }
            catch (IncorrectTypeException ex) {
                code = "Incorrect type";
                return false;
            }
            catch (Exception ex) {
                code = "Error";
                return false;
            }

            code += program;

            code += "\n\t\tMOV AX 4C00h";
            code += "\n\t\tINT 21h";
            code += "\n\tend ?main";
            code += "\nend";

            Console.WriteLine(code);

            return true;
        }

        static public string Variables() {
            String result = "";
            foreach (var item in IDTable) {
                if (IDTable.GetType(item.Key) == IdentifierTable.States.VAR)
                    result += "\n\t" + item.Value + "\tDW\t0";
            }
            result += "\n\t?LS\tDW\t0";
            result += "\n\t?LM\tDW\t0FFFFh";
            result += "\n\t?SM\tDW\t0";
            result += "\n\t?ML\tDW\t1";
            result += "\n\t?EXP1\tDW\t0";
            result += "\n\t?EXP2\tDW\t0";

            return result.ToString();
        }

        static string Program(TreeNode tree) {
            string result = Block(tree.childs[3]);     //_BLOCK_
            return result;
        }

        static string Block(TreeNode tree) {
            string result = ConditionalExpression(tree.childs[1], "?LS");
            return result;
        }

        static string ConditionalExpression(TreeNode tree, string reg) {
            string result = "";

            Type type1, type2;
            result += Logical(tree.childs[1], reg, out type1);
            result += LogicalSummand(tree.childs[0], "AX", out type2);

            //if (type1 != type2)
            //    throw new IncorrectTypeException();
            
            result += "\n\t\tOR AX, " + reg;
            result += "\n\t\tAND AX, 1";
            result += "\n\t\tMOV " + reg + ", AX";

            return result;
        }

        static string LogicalSummand(TreeNode tree, string reg, out Type type) {
            string result = "";

            Type type1, type2;
            result += LogicalMultiplier(tree.childs[0], "BX", out type1);
            result += LogicalMultiplierList(tree.childs[1], "?LM", out type2);

            if (type1 != Type.INTEGER && type1 != type2)
                throw new IncorrectTypeException();

            //result += "\n\t\tPUSH AX";
            result += "\n\t\tAND BX, ?LM";
            result += "\n\t\tAND BX, 1";
            result += "\n\t\tMOV " + reg + ", BX";
            //result += "\n\t\tPOP AX";

            type = Type.INTEGER;
            return result;
        }

        static string Logical(TreeNode tree, string reg, out Type type) {
            type = Type.INTEGER;
            if (tree.node == null)
                return null;

            string result = "";

            Type type1, type2;
            result += Logical(tree.childs[2], reg, out type1);
            result += LogicalSummand(tree.childs[1], "AX", out type2);

            if (type1 != Type.INTEGER && type1 != type2)
                throw new IncorrectTypeException();

            //result += "\n\t\tPUSH AX";
            result += "\n\t\tOR AX, " + reg;
            result += "\n\t\tAND AX, 1";
            result += "\n\t\tMOV " + reg + ", AX";
            //result += "\n\t\tPOP AX";

            return result;
        }

        static string LogicalMultiplier(TreeNode tree, string reg, out Type type) {
            string result = "";

            if (tree.node[0] == KWTable.GetKey("NOT")) {
                result += LogicalMultiplier(tree.childs[1], reg, out type);
                if (type != Type.INTEGER)
                    throw new IncorrectTypeException();

                result += "\n\t\tMOV BX, " + reg;
                result += "\n\t\tNOT BX";
                result += "\n\t\tAND BX, 1";
                result += "\n\t\tMOV " + reg + ", BX";

                return result;
            }

            if (tree.node[0] == '[') {
                type = Type.INTEGER;

                result += ConditionalExpression(tree.childs[1], reg);

                result += "\n\t\tMOV BX, " + reg;
                result += "\n\t\tAND BX, 1";
                result += "\n\t\tMOV " + reg + ", BX";

                return result;
            }

            Type type1, type2;
            result += Expression(tree.childs[0], "?EXP1", out type1);
            result += Expression(tree.childs[2], "?EXP2", out type2);

            if (type1 == Type.ANOTHER || type2 == Type.ANOTHER || type1 == type2)
                type = Type.INTEGER;
            else
                throw new IncorrectTypeException();

            string cmpOperator = ChooseComparisonOperator(tree.childs[1]);

            //result += "\n\t\tPUSH AX";
            result += "\n\t\tMOV CX, ?EXP1";
            result += "\n\t\tCMP CX, ?EXP2";
            result += "\n\t\t" + cmpOperator + " ?L" + (labelCounter++);
            result += "\n\t\tMOV CX, 0";
            result += "\n\t\tJMP ?L" + (labelCounter++);
            result += "\n\t\t?L" + (labelCounter - 2) + ":";
            result += "\n\t\t\tMOV CX, 1";
            result += "\n\t\t?L" + (labelCounter - 1) + ":";
            result += "\n\t\t\tMOV BX, " + reg;
            result += "\n\t\t\tAND BX, CX";
            result += "\n\t\t\tMOV " + reg + ", BX";

            return result;
        }

        static string ChooseComparisonOperator(TreeNode tree) {
            if (tree.node[0] == '<')
                return "JL";
            if (tree.node[0] == '>')
                return "JG";
            if (tree.node[0] == '=')
                return "JE";
            if (tree.node[0] == MSTable.GetKey("<="))
                return "JLE";
            if (tree.node[0] == MSTable.GetKey(">="))
                return "JGE";
            return "JNE";
        }

        static string LogicalMultiplierList(TreeNode tree, string reg, out Type type) {
            type = Type.INTEGER;
            if (tree.node == null) {
                return "\n\t\tMOV ?LM, 0FFFFh";
            }

            string result = "";

            Type type1, type2;
            result += LogicalMultiplier(tree.childs[1], "BX", out type1);
            result += LogicalMultiplierList(tree.childs[2], reg, out type2);

            if (type1 != Type.INTEGER && type1 != type2)
                throw new IncorrectTypeException();

            //result += "\n\t\tPUSH BX";
            result += "\n\t\tAND BX, " + reg;
            result += "\n\t\tMOV " + reg + ", BX";
            //result += "\n\t\tPOP BX";

            return result;
        }

        static string Expression(TreeNode tree, string reg, out Type type) {
            string result = "";
            Type type1, type2;
            if (tree.node[0] == '-') {
                result += Summand(tree.childs[1], "CX", out type1);
                result += SummandList(tree.childs[2], "?SM", out type2); 
            }
            else {
                result += Summand(tree.childs[0], "CX", out type1);
                result += SummandList(tree.childs[1], "?SM", out type2);
            }

            if (type1 != Type.ANOTHER && type2 != Type.ANOTHER && type1 != type2)
                throw new IncorrectTypeException();

            type = (type1 == Type.ANOTHER) ? type2 : type1;

            result += "\n\t\t" + addInstruction + " CX, ?SM";

            if (tree.node[0] == '-')
                result += "\n\t\tNEG CX";

            result += "\n\t\tMOV " + reg + ", CX";

            return result;
        }

        static string SummandList(TreeNode tree, string reg, out Type type) {
            if (tree.node == null) {
                type = Type.ANOTHER;
                return "\n\t\tMOV " + reg + ", 0";
            }

            string result = "";

            Type type1, type2;
            result += Summand(tree.childs[1], "CX", out type1);
            result += SummandList(tree.childs[2], reg, out type2);

            if (type1 != Type.ANOTHER && type2 != Type.ANOTHER && type1 != type2)
                throw new IncorrectTypeException();

            type = (type1 == Type.ANOTHER) ? type2 : type1;

            string tmpAdd = ChooseAddInstruction(tree.childs[0]);

            result += "\n\t\t" + addInstruction + " CX, " + reg;
            result += "\n\t\tMOV " + reg + ", CX";

            addInstruction = tmpAdd;

            return result; 
        }

        static string ChooseAddInstruction(TreeNode tree) {
            if (tree.node[0] == '+')
                return "ADD";
            if (tree.node[0] == '-')
                return "SUB";
            return "OR";
        }

        static string Summand(TreeNode tree, string reg, out Type type) {
            string result = "";

            Type type1, type2;
            result += Multiplier(tree.childs[0], "DX", out type1);
            result += MultiplierList(tree.childs[1], "?ML", out type2);

            if (type1 != Type.ANOTHER && type2 != Type.ANOTHER && type1 != type2)
                throw new IncorrectTypeException();

            type = (type1 == Type.ANOTHER) ? type2 : type1;

            if (mulInstruction == "MUL") {
                result += "\n\t\tPUSH EAX";
                result += "\n\t\tMOV AX, ?ML";
                result += "\n\t\tMUL DX";
                result += "\n\t\tMOV " + reg + ", AX";
                result += "\n\t\tPOP EAX";
            } else if (mulInstruction == "/" || mulInstruction == "MOD") {
                result += "\n\t\tPUSH AX";
                result += "\n\t\tXOR AX, AX";
                result += "\n\t\tMOV AL, DL";
                result += "\n\t\tMOV DL, DH";
                result += "\n\t\tXOR DH, DH";
                result += "\n\t\tDIV ?ML";
                if (mulInstruction == "/")
                    result += "\n\t\tMOV " + reg + ", AX";
                else
                    result += "\n\t\tMOV " + reg + ", DX";
                result += "\n\t\tPOP AX";
            } else if (mulInstruction == "AND") {
                result += "\n\t\tAND DX, ?ML";
                result += "\n\t\tMOV " + reg + ", DX";
            }

            return result;
        }

        static string ChooseMultiplierInstruction(TreeNode tree) {
            if (tree.node[0] == '*')
                return "MUL";
            if (tree.node[0] == '/')
                return "/";
            if (tree.node[0] == '&')
                return "AND";
            return "MOD";
        }

        static string MultiplierList(TreeNode tree, string reg, out Type type) {
            string result = "";

            if (tree.node == null) {
                mulInstruction = "MUL";
                type = Type.ANOTHER;
                return "\n\t\tMOV " + reg + ", 1";
            }

            Type type1, type2;
            result += Multiplier(tree.childs[1], "DX", out type1);
            result += MultiplierList(tree.childs[2], reg, out type2);

            if (type1 != Type.ANOTHER && type2 != Type.ANOTHER && type1 != type2)
                throw new IncorrectTypeException();

            type = (type1 == Type.ANOTHER) ? type2 : type1;

            string tmpMul = ChooseMultiplierInstruction(tree.childs[0]);

            if (mulInstruction == "MUL") {
                result += "\n\t\tPUSH EAX";
                result += "\n\t\tMOV AX, ?ML";
                result += "\n\t\tMUL DX";
                result += "\n\t\tMOV " + reg + ", AX";
                result += "\n\t\tPOP EAX";
            }
            else if (mulInstruction == "/" || mulInstruction == "MOD") {
                result += "\n\t\tPUSH AX";
                result += "\n\t\tXOR AX, AX";
                result += "\n\t\tMOV AL, DL";
                result += "\n\t\tMOV DL, DH";
                result += "\n\t\tXOR DH, DH";
                result += "\n\t\tDIV ?ML";
                if (mulInstruction == "/")
                    result += "\n\t\tMOV " + reg + ", AX";
                else
                    result += "\n\t\tMOV " + reg + ", DX";
                result += "\n\t\tPOP AX";
            }
            else {
                result += "\n\t\tAND DX, ?ML";
                result += "\n\t\tMOV " + reg + ", DX";
            }

            mulInstruction = tmpMul;

            return result;
        }

        static string Multiplier(TreeNode tree, string reg, out Type type) {
            string result = "";
            if (tree.node[0] == SyntacticalParser._UNSIGNED_CONST_)
                return UnsignedConst(tree.childs[0], reg, out type);
            if (tree.node[0] == SyntacticalParser._COMPLEX_CONSTANT_) {
                type = Type.COMPLEX;
                return ComplexConstant(tree.childs[0], reg);
            }
            if (tree.node[0] == SyntacticalParser._VARIABLE_)
                return Variable(tree.childs[0], reg, out type);
            if (tree.node[0] == SyntacticalParser._BUILT_IN_FUNC_ID_) {
                int countParameters = 1;
                result += ActualArguments(tree.childs[1].childs[1], "AX", ref countParameters, out type);

                functionToCall = ChooseFunction(tree.childs[0]);

                result += "\n\t\t" + functionToCall + " AX";

                type = Type.REAL;
                return result;
            }
            if (tree.node[0] == '(')
                return Expression(tree.childs[1], reg, out type);
            if (tree.node[0] == '-') {
                result += Multiplier(tree.childs[1], reg, out type);
                result += "\n\t\tMOV DX, " + reg;
                result += "\n\t\tNEG DX";
                result += "\n\t\tMOV " + reg + ", DX";
                return result;
            }
            result += Multiplier(tree.childs[1], reg, out type);
            result += "\n\t\tMOV DX, " + reg;
            result += "\n\t\tNOT DX";
            result += "\n\t\tMOV " + reg + ", DX";
            return result;
        }

        private static string ChooseFunction(TreeNode tree) {
            if (tree.node[0] == IDTable.GetKey("SIN"))
                return "FSIN";
            return "FCOS";
        }

        private static string ActualArguments(TreeNode tree, string reg, ref int countParameters, out Type type) {
            string result = "";

            if (countParameters > 0) {
                if (tree.node == null)
                    throw new IncorrectTypeException();

                result += UnsignedNumber(tree.childs[0], reg, out type);
                if (type != Type.INTEGER)
                    throw new IncorrectTypeException();
                if (countParameters == 1)
                    return result;

                result += "\n\t\tPUSH AX";
                --countParameters;
                result += ActualArguments(tree.childs[1], reg, ref countParameters, out type);

                return result;
            }
            type = Type.INTEGER;

            return null;
        }

        private static string ComplexConstant(TreeNode tree, string reg) {
            return ComplexNumber(tree.childs[1], reg);
        }

        static string Variable(TreeNode tree, string reg, out Type type) {
            if (tree.node[0] == SyntacticalParser._COMPLEX_VARIABLE_) {
                type = Type.COMPLEX;
                return ComplexVariable(tree.childs[0], reg);
            }

            string result = ""; 
            type = Type.ANOTHER;
            result += "\n\t\tMOV DX, " + VariableID(tree.childs[0]);
            result += "\n\t\tMOV " + reg + ", DX";

            return result;
        }

        private static string ComplexVariable(TreeNode tree, string reg) {
            return ComplexNumber(tree.childs[1], reg);
        }

        private static string ComplexNumber(TreeNode tree, string reg) {
            string result = "";

            result += "\n\t\tPUSH AX";
            result += LeftPart(tree.childs[0], "AX");
            result += "\n\t\tSHL AX, 8";
            result += "\n\t\tMOV " + reg + ", AX";

            result += RightPart(tree.childs[1], "AX");
            result += "\n\t\tAND AX, 0FFh";
            result += "\n\t\tOR " + reg + ", AX";

            result += "\n\t\tPOP AX";

            return result;
        }

        private static string RightPart(TreeNode tree, string reg) {
            if (tree.node == null)
                return "\n\t\tMOV " + reg + ", 0";

            if (tree.node[0] == ',') {
                Type type1;
                return Expression(tree.childs[1], reg, out type1);
            }

            string result = "";

            Type type;
            result += Expression(tree.childs[2], reg, out type);

            if (type != Type.INTEGER)
                throw new IncorrectTypeException();

            result += "\n\t\tPUSH EAX";
            result += "\n\t\tPUSH CX";
            result += "\n\t\tMOV CX, " + reg;
            result += "\n\t\tMOV AX, 10";
            result += "\n\t\t?L" + (labelCounter++) + ":";
            result += "\n\t\t\tMUL 10";
            result += "\n\t\t\tLOOP ?L" + (labelCounter - 1);
            result += "\n\t\tMOV " + reg + ", AX";
            result += "\n\t\tPOP CX";
            result += "\n\t\tPOP EAX";

            return result;
        }

        private static string LeftPart(TreeNode tree, string reg) {
            if (tree.node == null)
                return "\n\t\tMOV " + reg + ", 0";

            Type type;
            return Expression(tree.childs[0], reg, out type);
        }

        private static string VariableID(TreeNode tree) {
            return IDTable[(ushort)tree.childs[0].node[0]];
        }

        static string UnsignedConst(TreeNode tree, string reg, out Type type) {
            return UnsignedNumber(tree.childs[0], reg, out type);
        }

        private static string UnsignedNumber(TreeNode tree, string reg, out Type type) {
            string result = "";
            if (tree.node[0] == SyntacticalParser._INTEGER_) {
                type = Type.INTEGER;

                result += "\n\t\tMOV DX, " + CTable[(ushort)tree.childs[0].node[0]];
                result += "\n\t\tMOV " + reg + ", DX";
                return result;
            }
            type = Type.REAL;
            string[] parsed_num = CTable[(ushort)tree.childs[0].node[0]].Split('#');

            result += "\n\t\tMOV DL, " + parsed_num[0];
            result += "\n\t\tMOV DH, " + parsed_num[1];
            result += "\n\t\tMOV " + reg + ", DX";

            return result;
        }
    }
}
