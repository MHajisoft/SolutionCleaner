using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;

namespace SolutionCleaner
{
    public partial class MainWindow
    {
        private string _path;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Solution File|*.sln", CheckFileExists = true };

            if (dialog.ShowDialog() == true)
            {
                _path = dialog.FileName;
                SolutionName.Text = dialog.SafeFileName.Replace(".sln", string.Empty);
                ClearButton.IsEnabled = true;
            }
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            var solutionContent = File.ReadAllText(_path);
            var regEx = new Regex("Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\"", RegexOptions.Compiled);
            var matches = regEx.Matches(solutionContent).Cast<Match>();
            var projectsDir = matches.Select(x => Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(_path), x.Groups[2].Value)))).ToList();

            foreach (var dir in projectsDir)
            {
                if (Directory.Exists(Path.Combine(dir, "bin")))
                    Directory.Delete(Path.Combine(dir, "bin"), true);

                if (Directory.Exists(Path.Combine(dir, "obj")))
                    Directory.Delete(Path.Combine(dir, "obj"), true);
            }

            MessageBox.Show("Solution Cleaned Successfully!");

            _path = SolutionName.Text = null;
            ClearButton.IsEnabled = false;
        }
    }
}