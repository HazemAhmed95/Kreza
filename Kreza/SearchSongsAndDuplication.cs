using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using TagLib;


namespace Kreza
{
    class SearchSongsAndDuplication
    {
<<<<<<< HEAD
        List<string> NewSongs = new List<string>(); //New Songs to be added to All Songs file.
        HashSet<string> Set = new HashSet<string>(); //Helper HashSet.

        //Recursive function for searching all the windows.
        public void SearchDirectory(string path)
=======
        
        List<string> NewSongs = new List<string>();
         //recursive function for searching all the windows
        private void SearchDirectory(string path)
>>>>>>> a447c96fd7fb693e6830d440d4dc0dbf436cf829
        {
            try
            {
                //Getting all the files with the ".mp3" extension in the drive.
                foreach (string SongPath in Directory.EnumerateFiles(path, "*.mp3"))
                {
<<<<<<< HEAD
                    string SongName = Path.GetFileName(SongPath);
                    string SongData = GettingSongData(SongPath);

                    //Putting the SongData to the list if it's not already put.
                    if (!Set.Contains(SongName))
                    {
                        NewSongs.Add(SongData);
                        Set.Add(SongName);
                    }
=======
                    // getting the file name
                    string SongName = Path.GetFileName(file); 
                    string SongPath = file;
                    // getting the file data
                    string SongData = SongName + "|" + SongPath;
                    // putting the file data to the list
                     bool found = false;
                    // searching the soung name to clear the duplication with same name with different pathes 
                     foreach (string item in NewSongs)
                     {
                         string[] field = item.Split('|');
                         if (field[0] == SongName)
                         {
                             found = true;
                             break;
                         }
                     }
                    if ( ! found)
                    {
                        NewSongs.Add(SongData);
                    }
        
>>>>>>> a447c96fd7fb693e6830d440d4dc0dbf436cf829
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
        private string GettingSongData(string SongPath)
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

            List<string> SongsList = new List<string>();

            fs.Seek(0, SeekOrigin.Begin);

            string Line;
            while ((Line = SongDataReader.ReadLine()) != null) //Adding old file data to SongsList.
            {
                SongsList.Add(Line);
            }

            SongsList.Sort();

            foreach (string value in NewSongs)
            {
                //Checking if the new song to be added is already on the file , if no then we will add it.
                if (SongsList.BinarySearch(value) < 0)
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
    
