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


        StreamReader FileReader; 
        string line;
        int padding;
        
        SearchSongsAndDuplication sr = new SearchSongsAndDuplication();
        
        private void ViewAllSongs(object sender, RoutedEventArgs e)//Viewing all songs.
        {
            FileReader = new StreamReader("All Songs.txt");//Accessing AllSongs file.

            while ((line = FileReader.ReadLine()) != null) //Looking for the songs on the file.
            {
                string Spaces ="";
                string[] splitter = line.Split('|');
                padding = 80 - splitter[0].Length;
               
                for (int i = 0; i < padding; i++)//Making spaces for the margins.
                {
                    Spaces += " ";
                }

                SongsList.Items.Add(splitter[0] + Spaces + "\t" + splitter[1] + "\t" + splitter[2]);//Adding the songs to the list.
            }
           
            FileReader.Close();//Closing the file.
        }
        
        private void OpeningFileDialog(object sender, RoutedEventArgs e)//Opening file dialog.
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All Files|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);                       
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string path = System.IO.Path.GetDirectoryName(filename);
                sr.SearchDirectory(path);      //Searching in directory and subdirectories.
                
            }
        }

     

        private void SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)//Double clicked items.
        {
            string Selected = SongsList.SelectedItem.ToString();//getting the item and change it to string.
           
            FileReader = new StreamReader("All Songs.txt");//Accessing the file
           
            while ((line = FileReader.ReadLine()) != null)
            {
                string[] splitter = line.Split('|');//Splitting the line to 3 fields
             
                if (Selected.Contains(splitter[0]))//checking if the the name of the songs simialer to selected item or not
                {
                    Uri path = new Uri(splitter[3]);//Giving it it's path
                    ME1.Source = path;
                    ME1.LoadedBehavior = MediaState.Play; //play the song.
                    
                }
            }
           
            FileReader.Close();//Closing the access
        }

       
              
        
        
    }
}
