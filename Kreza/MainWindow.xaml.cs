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
        HashSet<string> Set = new HashSet<string>(); //Helper Hashset for handlibg duplication.

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
        /// Method : Volume Bar
        /// Controls the Volume of the Media Element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        // Volume Slider
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ME1.Volume = e.NewValue; 

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
                PreviousSong();
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

        private void ViewingAllSongs() //Viewing all songs.
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

            SongsList.Items.Clear();
            Set.Clear();

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

        private void ViewAllSongs(object sender, RoutedEventArgs e)//Viewing all songs event.
        {
            Add_To_PlayLists_ListBox.Visibility = Visibility.Hidden;
            ViewSongs_in_PlayLists_ListBox.Visibility = Visibility.Hidden;
            SongsList.Visibility = Visibility.Visible;
            ViewingAllSongs();
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
               
                Selected = Selected + ".mp3";
                string SelectedSongPath = sr.GetPath(Selected);
                if (SelectedSongPath.Equals("-1"))
                {
                    return;
                }
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

        //------------------------------------------------------------------------------------
        //                                    PlayLists
        //------------------------------------------------------------------------------------
        private bool ExistInFile(string Search, string FilePath) //Check if a string exitsts in a file.
        {
            StreamReader FileReader = new StreamReader(FilePath);

            string Line;
            while ((Line = FileReader.ReadLine()) != null)
            {
                string[] splitter = Line.Split('|');
                if (splitter.Contains(Search))
                {
                    FileReader.Close();
                    return true;
                }
            }

            FileReader.Close();
            return false;
        }
        private void CreatePlayList(object sender, RoutedEventArgs e) //Creating new playlist.
        {
            string PlayListName = Microsoft.VisualBasic.Interaction.InputBox("Please Add The PlayList Name?", "PlayList", "");

            if (ExistInFile(PlayListName, "PlayListsNames.txt") && PlayListName != "")
            {
                System.Windows.MessageBox.Show("This PlayList Name Already Exists!");
            }
            else if (PlayListName != "")
            {
                System.IO.File.Create(PlayListName + ".txt");

                FileStream fs = new FileStream("PlayListsNames.txt", FileMode.Open, FileAccess.Write);
                StreamWriter PlayListsNamesWriter = new StreamWriter(fs);

                fs.Seek(0, SeekOrigin.End);
                PlayListsNamesWriter.WriteLine(PlayListName);

                PlayListsNamesWriter.Close();
                fs.Close();

                System.Windows.MessageBox.Show("Play List Created.");
            }
        }

        string RightClickedSong;
        private void SongsList_MouseRightButtonUp(object sender, MouseButtonEventArgs e) //Event to get the content of a right clicked item.
        {
            if (SongsList.SelectedItem.ToString() != null)
            {
                RightClickedSong = SongsList.SelectedItem.ToString();
            }
        }

        string RightClickedPlayList;
        private void ViewSongs_in_PlayLists_ListBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(ViewSongs_in_PlayLists_ListBox.SelectedItem.ToString() != null)
            {
                RightClickedPlayList = ViewSongs_in_PlayLists_ListBox.SelectedItem.ToString();
            }
        }

        private void Remove_PlayList(object sender, RoutedEventArgs e)
        {
            System.IO.File.Delete(RightClickedPlayList + ".txt"); 

            ViewSongs_in_PlayLists_ListBox.Items.RemoveAt(ViewSongs_in_PlayLists_ListBox.Items.IndexOf(ViewSongs_in_PlayLists_ListBox.SelectedItem));

            string[] Lines = System.IO.File.ReadAllLines("PlayListsNames.txt"); //read all lines of PlayListsNames file.

            System.IO.File.WriteAllText("PlayListsNames.txt", String.Empty); //deleting all lines of PlayListsNames file.

            StreamWriter PlayListsNamesWriter = new StreamWriter("PlayListsNames.txt");

            foreach(string item in Lines) //rewriting all the lines except the line we want to delete.
            {
                if (item != RightClickedPlayList)
                    PlayListsNamesWriter.WriteLine(item);
            }

            PlayListsNamesWriter.Close();
        }


        //viewing created playlist when user click on add to playlist button to choose the playlist he wants to add the rightclicked song to.
        private void AddToPlaylist(object sender, RoutedEventArgs e)
        {
            Add_To_PlayLists_ListBox.Visibility = Visibility.Visible;
            ViewSongs_in_PlayLists_ListBox.Visibility = Visibility.Hidden;
            SongsList.Visibility = Visibility.Hidden;

            if(Add_To_PlayLists_ListBox.Items.Count  == 0)
            {
                Add_To_PlayLists_ListBox.Items.Add("Choose A Playlist :\n");

                StreamReader PlayListsFileReader = new StreamReader("PlayListsNames.txt");

                string Line;
                while ((Line = PlayListsFileReader.ReadLine()) != null) //Displaying Created Playlists
                {
                    Add_To_PlayLists_ListBox.Items.Add(Line);
                }

                PlayListsFileReader.Close();
            }
        }

        private void Add_To_PlayLists_ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Adding a song to the playlist the user has chosen
        {
            string Selected = Add_To_PlayLists_ListBox.SelectedItem.ToString();

            RightClickedSong += ".mp3";
            string RightClickedSongPath = sr.GetPath(RightClickedSong);

            if (!ExistInFile(RightClickedSong, Selected + ".txt"))
            {
                FileStream fs = new FileStream(Selected + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter PlayListsWriter = new StreamWriter(fs);
                fs.Seek(0, SeekOrigin.End);
                PlayListsWriter.WriteLine(RightClickedSong + '|' + RightClickedSongPath);
                PlayListsWriter.Close();
                fs.Close();
                System.Windows.MessageBox.Show("Added to Play List.");
            }
            else
            {
                System.Windows.MessageBox.Show("This Song Already Exists in the file");
            }

            Add_To_PlayLists_ListBox.Visibility = Visibility.Hidden;
            ViewSongs_in_PlayLists_ListBox.Visibility = Visibility.Hidden;
            SongsList.Visibility = Visibility.Visible;
        }

        private void ViewSongs_in_PlayLists_ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Viewing songs in the playlist that the user has clicked on.
        {
            Add_To_PlayLists_ListBox.Visibility = Visibility.Hidden;
            ViewSongs_in_PlayLists_ListBox.Visibility = Visibility.Hidden;
            SongsList.Visibility = Visibility.Visible;

            SongsList.Items.Clear();

            string SelectedPlayList = ViewSongs_in_PlayLists_ListBox.SelectedItem.ToString();

            StreamReader SongsInPlayListReader = new StreamReader(SelectedPlayList + ".txt");

            string Line;
            while ((Line = SongsInPlayListReader.ReadLine()) != null)
            {
                string[] splitter = Cutter(Line);
                SongsList.Items.Add(splitter[0]);
            }

            SongsInPlayListReader.Close();
        }

        HashSet<string> PlayListSet = new HashSet<string>(); //Helper Hashset for handlibg duplication.

        private void View_Playlists(object sender, MouseButtonEventArgs e) //Viewing created playlist so the user can open any playlist and play songs.
        {
            Add_To_PlayLists_ListBox.Visibility = Visibility.Hidden;
            ViewSongs_in_PlayLists_ListBox.Visibility = Visibility.Visible;
            SongsList.Visibility = Visibility.Hidden;

            if (ViewSongs_in_PlayLists_ListBox.Items.Count == 0)
            {
                ViewSongs_in_PlayLists_ListBox.Items.Add("Your PlayLists :\n");
            }

            StreamReader PlayListsFileReader = new StreamReader("PlayListsNames.txt");

            string Line;
            while ((Line = PlayListsFileReader.ReadLine()) != null) //Displaying Created Playlists
            {
                if (!PlayListSet.Contains(Line))
                {
                    ViewSongs_in_PlayLists_ListBox.Items.Add(Line);
                    PlayListSet.Add(Line);
                }
            }

            PlayListsFileReader.Close();
        }

        private void Shuffle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            shuffle();
        }
        private void shuffle()
        {
            int NumberOfSongs = SongsList.Items.Count;
            Random random = new Random();
            int randomNumber = random.Next(1, NumberOfSongs);// Generate numbers bet.(1,nofsongs)
            for (int i = 0; i <= NumberOfSongs; i++)
            {
                if (randomNumber == i)
                {
                    SongsList.SelectedItem = SongsList.Items[i];
                    PlaySelectedSong();
                }

            }

        }

        
    }
}
