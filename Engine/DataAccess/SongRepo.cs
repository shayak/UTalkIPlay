using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace SpeechRecognition.DataAccess
{
    public class SongRepo
    {
        private SQLiteDB SongDB;
        private readonly string MainTable = "SongInfo";

        public string SongDirPath { get; set; }

        public SongRepo()
        {
            SongDirPath = "";
            InitDB();
        }

        public SongRepo(string path)
        {
            SongDirPath = path;
            InitDB();
        }

        private void InitDB()
        {
            var dbPath = Directory.GetCurrentDirectory() + "\\songs.db";

            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            SongDB = new SQLiteDB(dbPath);

            CreateMappingTable();
        }

        private void CreateMappingTable()
        {
            string sql = "create table if not exists " + MainTable +
                "(" +
                   "Title varchar(50) not null, " +
                   "Artist varchar(50), " +
                   "Album varchar(50) not null, " +
                   "FilePath varchar(200)," +
                   "primary key(Title, Album)" + 
                ");";

            SongDB.ExecuteNonQuery(sql);
        }

        public void InsertSong(string name, string artist, string album, string path)
        {
            var dict = new Dictionary<string, string>() 
            {
                {"Title", name},                
                {"Artist", artist},
                {"Album", album},
                {"FilePath", path}
            };

            SongDB.Insert(MainTable, dict);            
        }

        public string[] GetAllTitles()
        {
            string[] result = SongDB.GetDataTable("select distinct Title from " + MainTable).AsEnumerable().Select(row => row.Field<string>("Title")).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
            return result;
        }

        public string GetSongPathByName(string name)
        {
            //maybe top 1 * ?
            string sql = String.Format("select FilePath from {0} where Title = '{1}';", MainTable, name.Replace("'", "''"));
            return SongDB.ExecuteScalar(sql);
        }

        public void AddSongsToDB(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return;

            var songFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".mp3")/* || x.EndsWith(".wav")*/);
            foreach (var song in songFiles)
            {
                try
                {
                    var tag = TagLib.File.Create(song).Tag;
                    if (!String.IsNullOrWhiteSpace(tag.Title))
                        InsertSong(tag.Title, tag.FirstAlbumArtist ?? tag.FirstPerformer, tag.Album, song);
                }
                catch (TagLib.CorruptFileException ex)
                {
                    Console.WriteLine(song + ex.Message);
                }
            }
        }

        /*
        public string[] getSongNames(string path)
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".mp3") || x.EndsWith(".wav"))
                .Select(x => processFileName(x)).Where(x => !String.IsNullOrEmpty(x)).ToArray();
            return files;
        }

        private string processFileName(string file)
        {
            char[] fn = Path.GetFileNameWithoutExtension(file).ToCharArray();
            var start = fn.Select((v, i) => new { Index = i, Value = v }).FirstOrDefault(c => Char.IsLetter(c.Value));
            if (start != null)
                return new String(fn, start.Index, fn.Length - start.Index);
            else
                return String.Empty;
        }
        */
    }
}
