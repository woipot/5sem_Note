using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Note.Source.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Note.Source.MVVM
{
    class NoteVM : BindableBase
    {
        public const uint DefaultSleepTime = 1;

        private readonly SpellCheckModel _model;

        private DispatcherTimer _timer;

        private uint _sleepTime;

        private bool _isModified;


        public ObservableCollection<WrongWord> WrongWords => _model.WrongWords;

        public string Text
        {
            get => _model.Text;
            set
            {
                _model.Text = value;
                _isModified = true;
                OnPropertyChanged();
            }
        }

        public bool IsTimerEnabled { get; set; } = true;

        public uint SleepTimeSec
        {
            get => _sleepTime;
            set
            {
                _sleepTime = Math.Max(1, value);
                _timer.Interval = new TimeSpan(0,0,(int)_sleepTime);
            }
        }



        public NoteVM()
        {
            _sleepTime = DefaultSleepTime;
            _model = new SpellCheckModel();
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, (int)SleepTimeSec),
            };

            _timer.Tick += (u, v) =>
            {
                if (_isModified)
                {
                    _model.StartSpellCheckAsync();
                    _isModified = false;
                }
            };

            _timer.Start();

            EnableCheckingCommand = new DelegateCommand(SwitchTimer);
            ReplaceWordCommand = new DelegateCommand<object[]>(ReplaceWord);
            AddCommand = new DelegateCommand<object>(AddToDict);

            _isModified = false;
        }


        public DelegateCommand EnableCheckingCommand { get; set; }

        public DelegateCommand<object> AddCommand { get; set; }

        public DelegateCommand<object[]> ReplaceWordCommand { get; set; }


        private void SwitchTimer()
        {
            _timer.IsEnabled = !_timer.IsEnabled;
        }


        private void ReplaceWord(object[] param)
        {
            var oldWord = param[1] as string;
            var newWord = param[0] as string;

            if(string.IsNullOrEmpty(oldWord) || string.IsNullOrEmpty(newWord)) return;
            
            Text = Text.Replace(oldWord, newWord);
        }

        private void AddToDict(object param)
        {
            if (param is string word)
            {
                try
                {
                    _model.AddToDictionary(word);
                    _isModified = true;

                }
                catch (Exception)
                {
                    MessageBox.Show("Dictionary is not found");
                }
            }
        }
    }
}
