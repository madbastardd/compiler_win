using Concrete.ConstantsTableSpace;
using Concrete.IdentifierTableSpace;
using Concrete.KeyWordTableSpace;
using Concrete.MultySymbolSeparatorsTableSpace;
using Concrete.Syntactycal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.CodeGeneratorSpace {
    static class CodeGenerator {
        static MultySymbolSeparatorsTable MSTable;  //MultySymbol Separators Table
        static KeyWordsTable KWTable;   //Keywords Table
        static ConstantsTable CTable;   //Constant Table
        static IdentifierTable IDTable;    //ID Table

        //static readonly List<string> Registers = new List<string>() {
        //    "AX",
        //    "BX",
        //    "CX",
        //    "DX",
        //    "BP",
        //    "SP",
        //    "DI",
        //    "SI"
        //};

        static int labelCounter;

        static string addInstruction = "ADD";
        static string mulInstruction = "MUL";

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

            String code = "model small, c";

            code += "\n\n.data";

            string vars = Variables();

            code += vars;

            code += "\n\n.stack 100h";

            code += "\n\n.code\n\t?main:";
            code += "\n\t\tMOV AX, @data";
            code += "\n\t\tMOV DS, AX";

            //string program = "";
            //if (tree != null && tree.node[0] == SyntacticalParser._PROGRAM_)
            //    program = Program();

            string program;
            try {
                program = Program(tree.childs[0]);
            }
            catch (Exception ex) {
                program = "";
                Console.WriteLine(ex.StackTrace);
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
            result += "\n\t?ML\tDW\t0";
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

            result += Logical(tree.childs[1], reg);
            result += LogicalSummand(tree.childs[0], "AX");
            
            result += "\n\t\tOR AX, " + reg;
            result += "\n\t\tMOV " + reg + ", AX";

            return result;
        }

        static string LogicalSummand(TreeNode tree, string reg) {
            string result = "";

            result += LogicalMultiplier(tree.childs[0], "BX");
            result += LogicalMultiplierList(tree.childs[1], "?LM");

            //result += "\n\t\tPUSH AX";
            result += "\n\t\tAND BX, ?LM";
            result += "\n\t\tMOV " + reg + ", BX";
            //result += "\n\t\tPOP AX";

            return result;
        }

        static string Logical(TreeNode tree, string reg) {
            if (tree.node == null)
                return null;

            string result = "";

            result += Logical(tree.childs[2], reg);
            result += LogicalSummand(tree.childs[1], "AX");
            
            //result += "\n\t\tPUSH AX";
            result += "\n\t\tOR AX, " + reg;
            result += "\n\t\tMOV " + reg + ", AX";
            //result += "\n\t\tPOP AX";

            return result;
        }

        static string LogicalMultiplier(TreeNode tree, string reg) {
            string result = "";

            if (tree.node[0] == KWTable.GetKey("NOT")) {
                result += LogicalMultiplier(tree.childs[1], reg);
                result += "\n\t\tMOV BX, " + reg;
                result += "\n\t\tNOT BX";
                result += "\n\t\tMOV " + reg + ", BX";

                return result;
            }

            if (tree.node[0] == '[') {
                result += ConditionalExpression(tree.childs[1], reg);

                result += "\n\t\tMOV BX, " + reg;
                result += "\n\t\tMOV " + reg + ", BX";

                return result;
            }

            result += Expression(tree.childs[0], "?EXP1");
            result += Expression(tree.childs[2], "?EXP2");

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

        static string LogicalMultiplierList(TreeNode tree, string reg) {
            if (tree.node == null)
                return null;

            string result = "";

            result += LogicalMultiplier(tree.childs[1], "BX");
            result += LogicalMultiplierList(tree.childs[2], reg);

            //result += "\n\t\tPUSH BX";
            result += "\n\t\tAND BX, " + reg;
            result += "\n\t\tMOV " + reg + ", BX";
            //result += "\n\t\tPOP BX";

            return result;
        }

        static string Expression(TreeNode tree, string reg) {
            string result = "";
            if (tree.node[0] == '-') {
                result += Summand(tree.childs[1], "CX");
                result += SummandList(tree.childs[2], "?SM"); 
            }
            else {
                result += Summand(tree.childs[0], "CX");
                result += SummandList(tree.childs[1], "?SM");
            }

            result += "\n\t\t" + addInstruction + " CX, ?SM";

            if (tree.node[0] == '-')
                result += "\n\t\tNEG CX";

            result += "\n\t\tMOV " + reg + ", CX";

            return result;
        }

        static string SummandList(TreeNode tree, string reg) {
            if (tree.node == null) {
                return "\n\t\tMOV " + reg + ", 0";
            }

            string result = "";

            result += Summand(tree.childs[1], "CX");
            result += SummandList(tree.childs[2], reg);

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

        static string Summand(TreeNode tree, string reg) {
            string result = "";

            result += Multiplier(tree.childs[0], "DX");
            result += MultiplierList(tree.childs[1], "?ML");

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

        static string MultiplierList(TreeNode tree, string reg) {
            string result = "";

            result += Multiplier(tree.childs[1], "DX");
            result += MultiplierList(tree.childs[2], reg);

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

        static string Multiplier(TreeNode tree, string reg) {
            throw new NotImplementedException();
        }
    }
}
