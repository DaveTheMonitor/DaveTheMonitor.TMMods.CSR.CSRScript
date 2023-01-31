using DaveTheMonitor.TMMods.CSR.CSRScript;
using DaveTheMonitor.TMMods.CSR.CSRScript.Generators;
using DaveTheMonitor.TMMods.CSR.CSRScript.Generators.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace DaveTheMonitor.CSRSandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _prevLines;
        private StringBuilder _lineBuilder;
        private ScriptCompiler _compiler;
        private List<string> _output;
        private Stopwatch _compileTimer;
        private SandboxScriptRuntime _runtime;

        public MainWindow()
        {
            InitializeComponent();
            _prevLines = 0;
            _lineBuilder = new StringBuilder();
            _lineBox.Text = "1";
            _compiler = new ScriptCompiler();
            _compiler.AddErrorHandler(LogError);
            _output = new List<string>();
            _compileTimer = new Stopwatch();
            _runtime = new SandboxScriptRuntime(128, 128, this);
        }

        private void _codeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lines = _codeInput.LineCount;
            if (_prevLines != lines)
            {
                _prevLines = lines;
                _lineBox.Clear();
                _lineBuilder.Clear();
                for (int i = 0; i < lines; i++)
                {
                    _lineBuilder.AppendLine((i + 1).ToString());
                }
                _lineBox.Text = _lineBuilder.ToString();
            }
        }

        private void _codeInputScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _lineBox.ScrollToVerticalOffset(_codeInputScroll.VerticalOffset);
        }

        private void CompileScript()
        {
            try
            {
                ClearOutput();
                Log("Compiling...");
                _opOutput.Clear();
                _compileTimer.Restart();
                Script script = _compiler.Compile("sandbox", _codeInput.Text);
                _compileTimer.Stop();
                if (script != null)
                {
                    Log("Script compiled successfully in " + _compileTimer.ElapsedMilliseconds + " ms");
                    _opOutput.Text = ScriptGenerator.ConvertOps(script, true);

                    SandboxWorld world = new SandboxWorld(this);
                    _runtime.Run(script, (r) => r.InitVar("world", new ScriptVar(world)));
                }
            }
            catch(Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal void Log(string message)
        {
            _output.Add(message);
            _outputBox.Text += message + "\n";
        }

        private void ClearOutput()
        {
            _output.Clear();
            _outputBox.Clear();
        }

        internal void LogError(string header, string message)
        {
            Log(header + ":\n    " + message);
        }

        private void _compileButton_Click(object sender, RoutedEventArgs e)
        {
            CompileScript();
        }

        private void _toggleOpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_opOutput.Width > 0)
            {
                HideOps();
            }
            else
            {
                ShowOps();
            }
        }

        private void HideOps()
        {
            _opOutput.Width = 0;
            _opSeparator.Width = 0;
            _opOutput.Visibility = Visibility.Hidden;
            _opSeparator.Visibility = Visibility.Hidden;
            _toggleOpButton.Content = "Show Ops";
        }

        private void ShowOps()
        {
            _opOutput.Width = 300;
            _opSeparator.Width = 2;
            _opOutput.Visibility = Visibility.Visible;
            _opSeparator.Visibility = Visibility.Visible;
            _toggleOpButton.Content = "Hide Ops";
        }
    }
}
