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

        // public ObservableCollection<string> MostSuitableWords { get;  }


        public WrongWord(string word, ObservableCollection<string> similarWords = null)
        {
            Word = word;
            SimilarWords = similarWords ?? new ObservableCollection<string>();
            //MostSuitableWords = new ObservableCollection<string>(GetMostSuitable());
        }

        public override string ToString()
        {
            return Word;
        }


        //private IEnumerable<string> GetMostSuitable()
        //{
        //    var result = new Dictionary<int, List<string>>();
        //    foreach (var similarWord in SimilarWords)
        //    {
        //        var derivation = SpellCheckModel.GetDifferencesCount(Word, similarWord);



        //        if (result.Count == MostSuitableCount) break;
        //    }

        //    return result;
        //}


    }

}