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
            IsPlaying(false);
          
        }

        // isPlaying for Play and pause
        private void IsPlaying(bool value)
        {
            PauseBtn.IsEnabled = true;
            StopBtn.IsEnabled = value;
            ForBtn.IsEnabled = value;
            BackBtn.IsEnabled = value;
            MuteBtn.IsEnabled = value;

        }

        // play and pause btn
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IsPlaying(true);
            if (PauseBtn.Content.ToString() == "Pause")
            {
                ME1.Pause();
                PauseBtn.Content = "Play";
            }
            else
            {
                ME1.Play();
                PauseBtn.Content = "Pause";
            }
            VolSlider.Value = ME1.Volume;
        }
        // stop btn 
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ME1.Stop();
            PauseBtn.Content = "Play";
        }

        // FW and BW btns
        private void ForBtn_Click(object sender, RoutedEventArgs e)
        {
            ME1.Position = ME1.Position + TimeSpan.FromSeconds(10);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            ME1.Position = ME1.Position - TimeSpan.FromSeconds(10);
            //ME1.IsMuted = true;

        }
        //volSlider
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ME1.Volume = e.NewValue;
        }
        //mute btn
        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MuteBtn.Content.ToString() == "Mute")
            {
                ME1.IsMuted = true;
                MuteBtn.Content = "Unmute";
            }
            else
            {
                ME1.IsMuted = false;
                MuteBtn.Content = "Mute";
            }
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
                //string Spaces ="";
                string[] splitter = line.Split('|');
                //padding = 80 - splitter[0].Length;
               
                //for (int i = 0; i < padding; i++)//Making spaces for the margins.
                //{
                //    Spaces += " ";
                //}
                int cutter=0;
              for(int i = 0; i < splitter[0].Length; i++)
                  {
               if(splitter[0][i] == '.'){
                  cutter = i + 1;
                  break;
                    }	
                   }

            splitter[0] = splitter[0].Substring(0,cutter);
                SongsList.Items.Add(splitter[0]);//Adding the songs to the list.
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
            Selected = Selected + "mp3";
            FileReader = new StreamReader("All Songs.txt");//Accessing the file

            while ((line = FileReader.ReadLine()) != null)
            {
                string[] splitter = line.Split('|');//Splitting the line to 3 fields

                if (Selected.Contains(splitter[0]))//checking if the the name of the songs simialer to selected item or not
                {
                    Uri path = new Uri(splitter[3]);//Giving it it's path
                    ME1.Source = path;
                    ME1.LoadedBehavior = MediaState.Play; //play the song.
                    
                    try
                    { //Getting the Album art source
                    TagLib.File tagFile = TagLib.File.Create(splitter[3]);
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
                    catch (Exception)// exception if there is no Album art.
                    { 
                        BitmapImage image = new BitmapImage(new Uri(@"\Assets\no_album_art_by_gouki113.png", UriKind.Relative));
                        AlbumArt.Source = image;
                    }
                }
            }

            FileReader.Close();//Closing the access
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

       
              
        
        
    }
}
