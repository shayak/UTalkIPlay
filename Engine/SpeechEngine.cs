using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

namespace SpeechRecognition
{
    public class SpeechEngine
    {
        private SpeechRecognitionEngine recognizer;
        private SpeechSynthesizer speaker;
        private GrammarManager grammarManager;

        // for UI updates
        private EventHandler<SpeechRecognizedUpdateArgs> recognizedSongEvent;
        private EventHandler<SpeechRecognizedUpdateArgs> recognizedMenuEvent;

        public bool Listening { get; set; }
        public bool Started { get; set; }

        public SpeechEngine(EventHandler<SpeechRecognizedUpdateArgs> recognizedSongEvent, EventHandler<SpeechRecognizedUpdateArgs> recognizedMenuEvent)
        {
            var sw = new StreamWriter("seError.log");
            try
            {
                speaker = new SpeechSynthesizer();

                this.recognizedSongEvent = recognizedSongEvent;
                this.recognizedMenuEvent = recognizedMenuEvent;

                grammarManager = new GrammarManager();

                recognizer = new SpeechRecognitionEngine();

                recognizer.LoadGrammar(grammarManager.GetMenuGrammar());
                LoadGrammar();

                sw.WriteLine("loading mic...");

                recognizer.SetInputToDefaultAudioDevice();

                sw.WriteLine("loaded mic.");

                recognizer.SpeechRecognized += sr_SpeechRecognizedHandler;

                sw.WriteLine("subscribe to SR handler success");
            }
            catch (Exception ex)
            {
                sw.WriteLine("Exception Details: " + ex.Message + ex.StackTrace);
            }
            finally
            {
                sw.Close();
            }
            

            //recognizer.RecognizeCompleted += sr_RecognizeCompletedHandler;
            //recognizer.SpeechDetected += sr_SpeechDetectedHandler;
            //recognizer.SpeechHypothesized += sr_SpeechHypothesizedHandler;
            //recognizer.SpeechRecognitionRejected += sr_SpeechRecognitionRejectedHandler;
        }

        public void Start()
        {
            LoadGrammar();
            if (!Started)
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            Started = true;            
        }

        public void Stop()
        {
            recognizer.RecognizeAsyncCancel();
            
            Started = false;
        }

        public void LoadGrammar()
        {
            var gram = grammarManager.GetSongGrammar();
            if (gram != null)
                recognizer.LoadGrammar(gram);
        }

        public void Speak(string text)
        {
            //speaker.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
            speaker.SpeakAsync(text);
        }

        private void sr_SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            var args = new SpeechRecognizedUpdateArgs(e.Result.Text, e.Result.Confidence);

            if (GrammarManager.IsMenuOption(e.Result.Text.ToString()) && e.Result.Confidence >= 0.80)
            {
                if (e.Result.Text == "Are you there?")
                {
                    if (Listening) Speak("Yes bitch tell me what to play!");
                    else Speak("No bitch start me up!");                    
                }

                if (e.Result.Text.ToString() == "Yo Stop" && Listening || e.Result.Text.ToString() == "Yo Listen" && !Listening)
                {
                    Listening = !Listening;
                    recognizedMenuEvent(this, args); 
                }
                else if (e.Result.Text.ToString() != "Yo Stop" && e.Result.Text.ToString() != "Yo Listen")
                    recognizedMenuEvent(this, args); 
            }
            else if (Listening) 
                recognizedSongEvent(this, args);
        }


        private void sr_RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e)
        {
            
        }

        private void sr_SpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)
        {
            
        }

        private void sr_SpeechHypothesizedHandler(object sender, SpeechHypothesizedEventArgs e)
        {

        }

        private void sr_SpeechRecognitionRejectedHandler(object sender, SpeechRecognitionRejectedEventArgs e)
        {

        }
    }

    public class SpeechRecognizedUpdateArgs : EventArgs
    {
        public string title;
        public double confidence;

        public SpeechRecognizedUpdateArgs(string title, double confidence)
        {
            this.title = title;
            this.confidence = confidence;
        }
    }
}
