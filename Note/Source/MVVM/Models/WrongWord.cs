using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Note.Source.MVVM.Models
{
    internal class WrongWord
    {
        public const int MostSuitableCount = 5;

        public string Word { get; }

        public ObservableCollection<string> SimilarWords { get; }



        public WrongWord(string word, ObservableCollection<string> similarWords = null)
        {
            Word = word;
            SimilarWords = similarWords ?? new ObservableCollection<string>();
        }

        public override string ToString()
        {
            return Word;
        }

    }

}