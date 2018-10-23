using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Threading;
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
                OnPropertyChanged(Text);
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
            ReplaceWordCommand = new DelegateCommand<object>(ReplaceWord);

            _isModified = false;
        }


        public DelegateCommand EnableCheckingCommand { get; set; }

        public DelegateCommand<object> ReplaceWordCommand { get; set; }


        private void SwitchTimer()
        {
            _timer.IsEnabled = !_timer.IsEnabled;
        }


        //Todo add tag to wrong word and set parametr as WrongWord in xaml
        private void ReplaceWord(object param)
        {
            var i = 0;
            //var str = param as string;
            //if (str != null)
            //{
            //    _model.Replace(str, word);
            //}
        }
    }
}
