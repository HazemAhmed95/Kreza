using System;
using System.Collections.Generic;
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
using System.IO;
using Microsoft.Win32;


namespace Kreza
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
      
           
         StreamReader Reader; 
        string reading;
        int fixing;
        
        SearchSongsAndDuplication sr = new SearchSongsAndDuplication();
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Reader = new StreamReader("All Songs.txt");
            while ((reading = Reader.ReadLine()) != null)
            {
                string add ="";
                string[] splitter = reading.Split('|');
                fixing = 80 - splitter[0].Length;
                for (int i = 0; i < fixing; i++)
                {//song
                      add += " ";
                }
                SongsList.Items.Add(splitter[0]+add+"\t"+splitter[1]+"\t\t"+splitter[2]);
            }
            Reader.Close();
        }
        
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All Files|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);                       
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string path = System.IO.Path.GetDirectoryName(filename);
                sr.SearchDirectory(path);      
                
            }
        }

     

        private void SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string IsSelected = SongsList.SelectedItem.ToString();
            Reader = new StreamReader("All Songs.txt");
            while ((reading = Reader.ReadLine()) != null)
            {
                string[] splitter = reading.Split('|');
                if (IsSelected.Contains(splitter[0]))
                {
                    Uri path = new Uri(splitter[3]);
                    ME1.Source = path;
                    ME1.LoadedBehavior = MediaState.Play;
                    
                }
            }
            Reader.Close();
        }

        private void SongsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        } 
              
        
        
    }
}
