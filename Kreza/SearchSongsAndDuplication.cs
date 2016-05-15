using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;


using TagLib;

namespace Kreza
{
    class SearchSongsAndDuplication
    {
        List<string> NewSongs = new List<string>(); //New Songs to be added to All Songs file.
        HashSet<string> Set = new HashSet<string>(); //Helper HashSet.
        
        //Dictionary is a collection that uses Hash Table.
        //Song name (key) and song path (value) to be used in GetPath Method.
        Dictionary<string, string> SongNameAndPath = new Dictionary<string, string>();

       
        //A method that takes the Song Name as a parameter and returns its Path.
        public string GetPath(string SongName)
        {
            if (SongNameAndPath.Count == 0) //Filling the Dictionary if it's Still Empty (When the user launches the application).
            {
                
                FileStream fs = new FileStream("All Songs.txt", FileMode.Open, FileAccess.Read);
                StreamReader SongDataReader = new StreamReader(fs);
                string Line;
                while ((Line = SongDataReader.ReadLine()) != null)
                {
                    string[] splitter = Line.Split('|');
                    SongNameAndPath.Add(splitter[0], splitter[3]);
                }
                fs.Close();
                SongDataReader.Close();
            }

            if (!SongNameAndPath.ContainsKey(SongName))
            {
                return "-1";
            }

            string SongPath = SongNameAndPath[SongName];
            return SongPath;
        }


        //Recursive function for searching all the windows.
        public  void SearchDirectory(string path)
        {
             
            try
            {
                System.Windows.Forms.Application.DoEvents();
            
                //Getting all the files with the ".mp3" extension in the drive.
             foreach(string SongPath in Directory.EnumerateFiles(path, "*.mp3"))
                {

           
      
                    string SongName =  Path.GetFileName(SongPath);
                     string SongData =  GettingSongData(SongPath);
                  
                

                    //Putting the SongData to the list if it's not already put.
                    if (!Set.Contains(SongName))
                    {
                        NewSongs.Add(SongData);
                        Set.Add(SongName);
                        //((MainWindow)System.Windows.Application.Current.MainWindow).SongsList.Items.Add(SongName); // accessing main window to add the songs in the song list
                    }
                }

                //Searching the sub directories.
                foreach (string sDir in Directory.EnumerateDirectories(path))
                {
                    SearchDirectory(sDir);
                }
            }

            catch (Exception)
            {

            }
            AddNewSongsToFile(NewSongs);
        }

        //A method that takes Song Path and returns it's data.
        public  string  GettingSongData(string SongPath)
        {
            string SongName = Path.GetFileName(SongPath);

            TagLib.File tagFile = TagLib.File.Create(SongPath);

            string artist = tagFile.Tag.FirstAlbumArtist;
           
            if (String.IsNullOrWhiteSpace(artist))
            {
                artist = "Unknown";
            }

            string duration = tagFile.Properties.Duration.ToString();
            duration = duration.Substring(0, 8);

            string SongData = SongName + "|" + artist + "|" + duration + "|" + SongPath;

            return SongData;
        }


        //A method to add the new songs to All Songs file and handling Duplication
        public void AddNewSongsToFile(List<string> NewSongs)
        {
            FileStream fs = new FileStream("All Songs.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter SongDataWriter = new StreamWriter(fs);
            StreamReader SongDataReader = new StreamReader(fs);

            HashSet<string> SongsOnFile = new HashSet<string>();

            fs.Seek(0, SeekOrigin.Begin);

            string Line;
            while ((Line = SongDataReader.ReadLine()) != null) //Adding old file data to SongsList.
            {
                SongsOnFile.Add(Line);
            }

            foreach (string value in NewSongs)
            {
                //Checking if the new song to be added is already on the file , if no then we will add it.
                if (!SongsOnFile.Contains(value))
                {
                    SongDataWriter.WriteLine(value);
                }
            }

            SongDataWriter.Close();
            SongDataReader.Close();
            fs.Close();
        }
    }
}

