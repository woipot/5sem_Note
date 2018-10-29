using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Note.Source.MVVM.Models
{
    internal class SpellCheckModel : BindableBase
    {
        public const int NormalDeviation = 2;


        private readonly HashSet<string> _dictionary;


        #region Properties

        public string DictionaryPatch { get; private set; }

        public ObservableCollection<WrongWord> WrongWords { get; }

        public string Text { get; set; }

        public HashSet<string> Dictionary => _dictionary;

        #endregion


        #region Constructors

        public SpellCheckModel()
        {
            DictionaryPatch = Directory.GetCurrentDirectory() + @"\Resources\pldb-win.txt";
            _dictionary = LoadDictionary(DictionaryPatch);

            WrongWords = new ObservableCollection<WrongWord>();
        }

        #endregion


        #region Public func

        public async void StartSpellCheckAsync()
        {
            var wrongWords = await Task.Run(()=>SpellCheck());

            if (WrongWords.Any()) WrongWords.Clear();
            foreach (var newWrongWord in wrongWords)
            {
                WrongWords.Add(newWrongWord);
            }
        }

         public static int GetDifferencesCount(string first, string second)
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

        #endregion


        #region private func

        private IEnumerable<WrongWord> SpellCheck()
        {
            var currentWrongWords = new List<string>();
            if (string.IsNullOrEmpty(Text)) return new List<WrongWord>();

            var textCopy = Text;


            var punctuation = textCopy.Where(char.IsPunctuation).Distinct().ToArray();

            var words = textCopy.Split().Select(x => x.Trim(punctuation));


            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word) || currentWrongWords.Contains(word)) continue;

                var withoutMis = CheakWord(word);
                if (!withoutMis)
                {
                    currentWrongWords.Add(word);
                }
            }

            return GetProcessWrongWords(currentWrongWords);
        }

        private IEnumerable<string> GetSimilarWords(string word)
        {
            var result = new List<string>();

            foreach (var dictionaryWord in _dictionary)
            {
                var differenceConut = GetDifferencesCount(word, dictionaryWord);

                if (differenceConut <= NormalDeviation)
                    result.Add(dictionaryWord);
            }

            return result;
        }

        private bool CheakWord(string word)
        {
            return _dictionary.Contains(word);
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

        private IEnumerable<WrongWord> GetProcessWrongWords(IEnumerable<string> wrongWordsList)
        {
            var processedWrongWords = new List<WrongWord>();

            foreach (var word in wrongWordsList)
            {
                var similarWords = GetSimilarWords(word);
                var wrongWord = new WrongWord(word, new ObservableCollection<string>(similarWords));
                processedWrongWords.Add(wrongWord);
            }

            return processedWrongWords;
        }
        #endregion

        public void AddToDictionary(string word)
        {
            _dictionary.Add(word);
            
            var sw = new StreamWriter(DictionaryPatch, true);

            sw.WriteLine(word);

            sw.Dispose();

        }
    }
}