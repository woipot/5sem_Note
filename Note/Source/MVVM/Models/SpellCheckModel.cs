using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Prism.Mvvm;

namespace Note.Source.MVVM.Models
{
    internal class SpellCheckModel : BindableBase
    {
        public const int NormalDeviation = 2;


        private readonly HashSet<string> _dictinary;


        #region Properties

        public string DictionaryPatch { get; private set; }
        public ObservableCollection<WrongWord> WrongWords { get; set; }
        public string Text { get; set; }

        #endregion


        #region Constructors

        public SpellCheckModel()
        {
            DictionaryPatch = Directory.GetCurrentDirectory() + @"\Resources\pldb-win.txt";
            _dictinary = LoadDictionary(DictionaryPatch);

            WrongWords = new ObservableCollection<WrongWord>();
        }

        #endregion


        #region Public func

        public void StartSpellCheck()
        {
            SpellCheck();
        }

        #endregion


        #region private func

        private void SpellCheck()
        {
            if (string.IsNullOrEmpty(Text)) return;

            var textCopy = Text;


            var punctuation = textCopy.Where(char.IsPunctuation).Distinct().ToArray();

            var words = textCopy.Split().Select(x => x.Trim(punctuation));

            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word)) continue;
                if (WrongWords.Count != 0 && Contains(word)) continue;


                var withoutMis = CheakWord(word);
                if (!withoutMis)
                {
                    var similarWords = GetSimilarWords(word);
                    var wrongWord = new WrongWord(word, new ObservableCollection<string>(similarWords));
                    WrongWords.Add(wrongWord);
                }
            }

        }

        private IEnumerable<string> GetSimilarWords(string word)
        {
            var result = new List<string>();

            foreach (var dictionaryWord in _dictinary)
            {
                var differenceConut = GetDifferencesCount(word, dictionaryWord);

                if (differenceConut <= NormalDeviation)
                    result.Add(dictionaryWord);
            }

            return result;
        }

        private bool CheakWord(string word)
        {
            return _dictinary.Contains(word);
        }

        private static int GetDifferencesCount(string first, string second)
        {
            var minLength = Math.Min(first.Length, second.Length);

            var result = Math.Abs(first.Length - second.Length);

            for (var i = 0; i < minLength; i++)
            {
                if (first[i] != second[i])
                    result++;
            }


            return result;
        }

        private static HashSet<string> LoadDictionary(string patch)
        {
            var result = new HashSet<string>();

            var sr = new StreamReader(patch);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                result.Add(line.Trim());
            }

            sr.Dispose();

            return result;
        }

        private bool Contains(string word)
        {
            foreach (var wrongWord in WrongWords)
            {
                if (word == wrongWord.Word)
                    return true;
            }

            return false;
        }

        #endregion
    }
}