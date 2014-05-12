using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SpeechRecognition;
using SpeechRecognition.DataAccess;

namespace UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Summary:
        //     Occurs when a property value changes.
        private SongRepo songRepo;
        private SpeechEngine speechEngine;
        private SongPlayer player;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event EventHandler<SpeechRecognizedUpdateArgs> songRecognizedEvent;
        public event EventHandler<SpeechRecognizedUpdateArgs> menuRecognizedEvent;

        private MainWindow mainWin;

        private string _speech;
        private double _confidence;
        private double _confThreshold;

        private string _addButtonText;
        private string _listenButtonText;

        public MainWindowViewModel(MainWindow mainWin)
        {
            this.mainWin = mainWin;
            _speech = "";
            _confThreshold = 0.86;
            _listenButtonText = "Start Listening";
            _addButtonText = "Add Songs";

            songRecognizedEvent += HandleSongRecognized;
            menuRecognizedEvent += HandleMenuRecognized;
            
            songRepo = new SongRepo();

            speechEngine = new SpeechEngine(songRecognizedEvent, menuRecognizedEvent);

            mainWin.sw.WriteLine("SE init Success");

            speechEngine.Start();

            mainWin.sw.WriteLine("SE start Success");

            player = new SongPlayer();

        }
        
        public void AddSongsAsync(string path)
        {
            new System.Threading.Thread(() => { songRepo.AddSongsToDB(path); speechEngine.LoadGrammar(); }).Start();
        }

        public void ToggleListenMode()
        {
            if (ListenButtonText == "Start Listening")
            {
                ListenButtonText = "Listening...";
            }
            else
            {
                ListenButtonText = "Start Listening";
            }
        }

        public void HandleMenuRecognized(object sender, SpeechRecognizedUpdateArgs args)
        {
            Display = args.title;
            Confidence = args.confidence;

            if (args.title == "Yo Stop" || args.title == "Yo Listen")
                ToggleListenMode(); // for UI
            else if (args.title == "Stop")
                player.Stop();
            else if (args.title == "Play")
                player.Play();
            else if (args.title == "Pause")
                player.Pause();
            else if (args.title == "Volume Up")
                player.VolumeUp();
            else if (args.title == "Volume Down")
                player.VolumeDown();
        }

        public void HandleSongRecognized(object sender, SpeechRecognizedUpdateArgs args)
        {
            Display = args.title;
            Confidence = args.confidence;

            if (args.confidence < ConfidenceThreshold)
                return;

                player.PlaySong(songRepo, args.title);
        }

        #region Properties

        public string Display
        {
            get { return this._speech; }

            set
            {
                if (value != this._speech)
                {
                    this._speech = value;
                    OnPropertyChanged("Display");
                        
                }
            }
        }

        public double Confidence
        {
            get { return this._confidence; }

            set
            {
                if (value != this._confidence)
                {
                    this._confidence = value;
                    OnPropertyChanged("Confidence");
                        
                }
            }
        }

        public double ConfidenceThreshold
        {
            get { return this._confThreshold; }

            set 
            {
                if (value != this._confThreshold)
                {
                    this._confThreshold = value;
                    OnPropertyChanged("ConfidenceThreshold");
                }
            }
        }

        public string AddButtonText
        {
            get { return this._addButtonText; }

            set
            {
                if (value != this._addButtonText)
                {
                    this._addButtonText = value;
                    OnPropertyChanged("AddButtonText");
                }
            }
        }

        public string ListenButtonText
        {
            get { return this._listenButtonText; }

            set
            {
                if (value != this._listenButtonText)
                {
                    this._listenButtonText = value;
                    OnPropertyChanged("ListenButtonText");
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
