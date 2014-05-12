using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.IO;
using System.Data;
using SpeechRecognition.DataAccess;

namespace SpeechRecognition
{
    public class GrammarManager
    {
        private SongRepo songRepo;        

        private static readonly string[] menuOptions = new[]  
                                                {
                                                    "Are you there?",
                                                    "Yo Listen",
                                                    "Yo Stop",
                                                    "Stop",
                                                    "Play", 
                                                    "Pause",
                                                    "Volume Up",
                                                    "Volume Down"
                                                };
        
        public GrammarManager()
        {        
            songRepo = new SongRepo();
        }

        public GrammarManager(SongRepo repo)
        {
            this.songRepo = repo;
        }

        public Grammar GetSongGrammar()
        {
            Choices songs = new Choices();
            var list = songRepo.GetAllTitles();

            if (list.Count() == 0) return null;
            
            songs.Add(list);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(songs);
            return new Grammar(gb);
        }

        public Grammar GetMenuGrammar()
        {
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(new Choices(menuOptions));
            return new Grammar(gb);
        }

        public static bool IsMenuOption(string phrase)
        {
            return menuOptions.Contains(phrase);
        }
    }
}
