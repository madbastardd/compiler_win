using Concrete.ConstantsTableSpace;
using Concrete.IdentifierTableSpace;
using Concrete.KeyWordTableSpace;
using Concrete.MultySymbolSeparatorsTableSpace;
using Concrete.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Concrete.Syntactycal;
using MahApps.Metro.Controls;
using System.IO;

namespace compiler {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        SyntacticalWindow syntacticalWindow = new SyntacticalWindow();

        public MainWindow() {
            InitializeComponent();
        }

        private void submit_Click(object sender, RoutedEventArgs e) {
            string textFile = this.textBlock7.Text;

            try {
                Concrete.TableSpace.Table[] tables = new Concrete.TableSpace.Table[] {
                new MultySymbolSeparatorsTable(),
                new KeyWordsTable(),
                new ConstantsTable(),
                new IdentifierTable()
                };

                textBlock1.Text = textBlock2.Text =
                    textBlock3.Text = textBlock4.Text =
                        textBlock5.Text = textBlock6.Text = "";

                Stopwatch sw = new Stopwatch();

                sw.Start();
                List<Lexem> list = Parser.ParseFile(textFile, tables, this.textBlock5);
                sw.Stop();

                foreach (var item in list) {
                    textBlock6.Text += item.code + "(" + item.line + ", " + item.pos + ") ";
                }
                tables[0].ShowInTextView(this.textBlock1);
                tables[1].ShowInTextView(this.textBlock2);
                tables[2].ShowInTextView(this.textBlock3);
                tables[3].ShowInTextView(this.textBlock4);

                this.textBlock5.Text += "\nParsed for " + sw.ElapsedMilliseconds + " milliseconds";

                syntacticalWindow = new SyntacticalWindow();

                SyntacticalParser.SetTables(tables);

                bool Parsed = SyntacticalParser.Start(new List<int>(from p in list select p.code), syntacticalWindow.textBox1);

                syntacticalWindow.Show();

                if (Parsed) {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(textFile.Split('.')[0] + ".ddt"))
                        file.WriteLine(SyntacticalParser.TextTree);
                }
                else {
                    syntacticalWindow.textBox1.Text = "Error in line " + list[SyntacticalParser.Index].line + ", position " + list[SyntacticalParser.Index].pos;
                }
            }
            catch (FileNotFoundException) {
                this.textBlock5.Text = "File not found";
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Current.Shutdown();
        }
    }
}
