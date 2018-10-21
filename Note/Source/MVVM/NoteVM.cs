using System.Collections.ObjectModel;
using Note.Source.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Note.Source.MVVM
{
    class NoteVM : BindableBase
    {
        private readonly SpellCheckModel _model;

        public ObservableCollection<WrongWord> WrongWords => _model.WrongWords;

        public string Text
        {
            get => _model.Text;
            set
            {
                _model.Text = value;
                OnPropertyChanged(Text);
            }
        }

        public bool IsTimerEnabled { get; set; } = true;


        public NoteVM()
        {
            _model = new SpellCheckModel();
            EnableCheckingCommand = new DelegateCommand(SwitchTimer);
        }



        public DelegateCommand EnableCheckingCommand { get; set; }



        private void SwitchTimer()
        {
            _model.StartSpellCheck();
        }
    }
}
