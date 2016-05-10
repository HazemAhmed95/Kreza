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
using TagLib;


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
        HashSet<string> Set = new HashSet<string>(); //Helper Set 

        // play and pause btn
     
        private void ForBtn_Click(object sender, RoutedEventArgs e)
        {
            ME1.Position = ME1.Position + TimeSpan.FromSeconds(10);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            ME1.Position = ME1.Position - TimeSpan.FromSeconds(10);

        }
        
        StreamReader FileReader;

        SearchSongsAndDuplication sr = new SearchSongsAndDuplication();

        private void ViewAllSongs(object sender, RoutedEventArgs e)//Viewing all songs.
        {
            FileReader = new StreamReader("All Songs.txt");//Accessing AllSongs file.

            string line;
            while ((line = FileReader.ReadLine()) != null) //Looking for the songs on the file.
            {
                string[] splitter = line.Split('|');

                int cutter = 0;
                for (int i = 0; i < splitter[0].Length; i++)
                {
                    if (splitter[0][i] == '.' && splitter[0][i + 3] == '3')
                    {
                        cutter = i;
                        break;
                    }
                }
                splitter[0] = splitter[0].Substring(0, cutter);
                if (!Set.Contains(splitter[0]))
                {
                    Set.Add(splitter[0]);
                    SongsList.Items.Add(splitter[0]);//Adding the songs to the list.
                }
                
            }

            FileReader.Close();//Closing the file.
        }

        private  void OpeningFileDialog(object sender, RoutedEventArgs e)//Opening file dialog.
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
            Selected = Selected + ".mp3";

            string SelectedSongPath = sr.GetPath(Selected);

            Uri path = new Uri(SelectedSongPath);//Giving it its path
            ME1.Source = path;
            ME1.LoadedBehavior = MediaState.Play; //play the song.

            try
            { //Getting the Album art source
                TagLib.File tagFile = TagLib.File.Create(SelectedSongPath);
                TagLib.IPicture pic = tagFile.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);
                // ImageSource 
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                AlbumArt.Source = bitmap;
            }
            catch (Exception)//exception if there is no Album art.
            {
                BitmapImage image = new BitmapImage(new Uri(@"\Assets\no_album_art_by_gouki113.png", UriKind.Relative));
                AlbumArt.Source = image;
            }
        }

        private void PauseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

    }
}
