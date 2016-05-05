using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kreza
{
    class SearchSongsAndDuplication
    {
        List<string> NewSongs = new List<string>();
         //recursive function for searching all the windows
        private void SearchDirectory(string path)
        {
            try
            {
                // getting all the files with the ".mp3" extension in the drive
                foreach (string file in Directory.EnumerateFiles(path, "*.mp3"))
                {
                    // getting the file name
                    string SongName = Path.GetFileName(file); 
                    string SongPath = file;
                    // getting the file data
                    string SongData = SongName + "|" + SongPath;
                    // putting the file data to the list
                    NewSongs.Add(SongData);
        
                }
                // searching the sub directories
                foreach (string sDir in Directory.EnumerateDirectories(path))
                {
                    SearchDirectory(sDir);
                }

            }

            catch (Exception)
            {

            }
        }
        // remove duplication data
        private void RemoveDuplication(List<string> NewSongs)
        {
            FileStream fs = new FileStream("Songs Paths.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter SongDataWriter = new StreamWriter(fs);
            StreamReader SongDataReader = new StreamReader(fs);

            List<string> SongsList = new List<string>();
            // pointing to the beginning of the file
            fs.Seek(0, SeekOrigin.Begin);

            string Line;
            // putting the data of the file into the list to remove the duplicated data
            while ((Line = SongDataReader.ReadLine()) != null)
            {
                SongsList.Add(Line);
            }

            // sorting the list
            SongsList.Sort();
              // binary search to find the duplicated
            foreach(string value in NewSongs)
            {
                if(SongsList.BinarySearch(value) < 0)
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
    
