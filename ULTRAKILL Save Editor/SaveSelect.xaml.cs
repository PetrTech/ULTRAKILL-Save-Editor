using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace ULTRAKILL_Save_Editor
{
    /// <summary>
    /// Interaction logic for SaveSelect.xaml
    /// </summary>
    public partial class SaveSelect
    {
        MainWindow window;

        public SaveSelect()
        {
            InitializeComponent();
        }

        public void CreateWindow(MainWindow win)
        {
            window = win;
            this.Show();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string p = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\";

            if (File.Exists("lastopened.txt"))
            {
                p = File.ReadAllText("lastopened.txt") + "\\";
            }

            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = p;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;

                DirectoryInfo info = new DirectoryInfo(path);
                
                if(info.Name.StartsWith("Slot") && info.Parent.Name == "Saves" && info.Parent.Parent.Name == "ULTRAKILL")
                {
                    window.SelectSave(path);

                    File.WriteAllText("lastopened.txt", info.Parent.FullName);

                    this.Hide();
                }
                else if(info.Name == "Saves")
                {
                    MessageBox.Show("Select a save slot, not the 'Saves' folder.", "Invalid", MessageBoxButton.OK);
                }
                else if(info.Name == "ULTRAKILL")
                {
                    MessageBox.Show("Select a save slot, not the game folder. The save is in 'Saves\\SlotX'.", "Invalid", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Invalid folder.", "Invalid", MessageBoxButton.OK);
                }
            }
        }
    }
}
