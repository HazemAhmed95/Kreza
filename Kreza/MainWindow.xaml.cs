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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;


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
        SearchSongsAndDuplication sr = new SearchSongsAndDuplication();
        HashSet<string> Set = new HashSet<string>(); //Helper Set 

        // play and pause buttons
        private void PauseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            ME1.LoadedBehavior = MediaState.Pause; // to Pause the MediaElement 
            PauseBtn.Visibility = Visibility.Collapsed; // this to change the visibitly of the Pause button
            PlayBtn.Visibility = Visibility.Visible;  // this to change the visibitly of the play button
        }

        private void PlayBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            ME1.LoadedBehavior = MediaState.Play;
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Method : Forward song
        /// Plays the next song.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForBtn_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (SongsList.Items.Count!=0)
            {
                PlayNextSong();
            }
        }
        /// <summary>
        /// Method : Play the next song
        /// Selecting the next index in list
        /// </summary>
        private void PlayNextSong()//Move to the next song
        {
            if ((SongsList.SelectedIndex + 1) < SongsList.Items.Count)
            {
                SongsList.SelectedItem = SongsList.Items[(SongsList.SelectedIndex + 1)];
            }
            else
            {
                SongsList.SelectedItem = SongsList.Items[0];
            }
            PlaySelectedSong();
        }
        /// <summary>
        /// Method : backward song
        /// Plays the previous song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SongsList.Items.Count != 0)
            {
                PlayNextSong();
            }
        }
        /// <summary>
        /// Method  : previous song
        /// Selecting the previous index in the list if its not equal 0
        /// </summary>
        private void PreviousSong()//Previous song!
        {
            if (SongsList.SelectedIndex != 0)
            {
                SongsList.SelectedItem = SongsList.Items[SongsList.SelectedIndex - 1];
            }

            PlaySelectedSong();
        }



        private void ViewAllSongs(object sender, RoutedEventArgs e)//Viewing all songs.
        {
            if (System.IO.File.Exists("All Songs.txt"))
            {
                FileReader = new StreamReader("All Songs.txt");//Accessing AllSongs file.
            }
            else
            {

                System.Windows.MessageBox.Show("There is no Music Added.");
                return;
            }
            string line;
            while ((line = FileReader.ReadLine()) != null) //Looking for the songs on the file.
            {
                string[] splitter = Cutter(line);

                if (!Set.Contains(splitter[0]))
                {
                    Set.Add(splitter[0]);
                    SongsList.Items.Add(splitter[0]);//Adding the songs to the list.

                }

            }

            FileReader.Close();//Closing the file.
        }
        /// <summary>
        /// Method : Open the songs
        /// Open dialog to get the songs and add them to the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method : Plays the song
        /// Play the song when the selected is doubled clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)//Play the item
        {
            PlaySelectedSong();
        }


        /// <summary>
        /// Method : Play the song
        /// checks the selected song, then searching for the path and play it
        /// Uses the Timethiker for the buffering
        /// Uses the albumArt
        /// </summary>
        private void PlaySelectedSong()//Play the song!
        {
            if (SongsList.SelectedItem.ToString() != null)
            {
                string Selected = SongsList.SelectedItem.ToString();//getting the item and change it to string.
                // textSong.Text = Selected;
                Selected = Selected + ".mp3";
                string SelectedSongPath = sr.GetPath(Selected);
                Uri path = new Uri(SelectedSongPath);//Giving it its path
                ME1.Source = path;
                ME1.LoadedBehavior = MediaState.Play; //play the song.
            }
            TimeThiker();
            SetAlbumArt();

        }





        /// <summary>
        ///  Event: Search
        ///  Triger : any Key 
        /// This event search using the song name as an input it searches in all songs if the input matches some parts of the whole song(s) name
        /// the matched song(s) will be added to the current played song list
        /// </summary>

        private void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            SongsList.Items.Clear(); //Clear the list to remove the dismatched songs 

            if (System.IO.File.Exists("All Songs.txt"))
            {
                FileReader = new StreamReader("All Songs.txt");//Accessing AllSongs file.
            }
            else
            {

                System.Windows.MessageBox.Show("There is no Music Added.");
                return;
            }

            string line;
            while ((line = FileReader.ReadLine()) != null) //Looking for the songs on the file.
            {
                string[] splitter = Cutter(line);


                if (splitter[0].ToLower().Contains(SearchBar.Text.ToLower()))
                {
                    Set.Add(splitter[0]);
                    SongsList.Items.Add(splitter[0]);//Adding the songs to the list.

                }

            }



        }

        /// <summary>
        ///  Method: Cutter
        ///  Split the line into (song info ) into an array of strings
        ///  and Cut the .mp3 part from the song name
        /// </summary>
        ///   Single parameter.
        /// <param name="Line">A String contain the song data and a dilmeter .</param>
        /// <returns>Array of strings contain the song data.</returns>
        private string[] Cutter(string line)
        {

            string[] splitter = line.Split('|'); //Split the line

            int cutter = 0;
            for (int i = 0; i < splitter[0].Length; i++) //Cut the .mp3 part
            {
                if (splitter[0][i] == '.' && splitter[0][i + 3] == '3')
                {
                    cutter = i;
                    break;
                }
            }
            splitter[0] = splitter[0].Substring(0, cutter);

            return splitter;
        }


        /// <summary>
        /// Method : Album art
        /// Gets the songs path then it gets its source
        /// then it give its album art
        /// if there is no albumart, gives its default.
        /// </summary>

        private void SetAlbumArt()
        {
            if (SongsList.SelectedItem.ToString() != null)
            {

                try
                {
                    string getSongsPath = sr.GetPath((SongsList.SelectedItem.ToString() + ".mp3"));
                    TagLib.File tagFile = TagLib.File.Create(getSongsPath);
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
                catch
                {

                    BitmapImage image = new BitmapImage(new Uri(@"\Assets\no_album_art_by_gouki113.png", UriKind.Relative));
                    AlbumArt.Source = image;

                }
            }
        }
        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((ME1.Source != null) && (ME1.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = ME1.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = ME1.Position.TotalSeconds;
            }
        }
        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            ME1.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }  private bool userIsDraggingSlider = false;
        void TimeThiker()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        /// <summary>
        /// Method: Media Ended
        /// When the song ends, it plays the next song.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ME1_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayNextSong();
        }

    }
}
