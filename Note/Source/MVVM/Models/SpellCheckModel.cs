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


        public string DictionaryPatch { get; private set; }
        public ObservableCollection<WrongWord> WrongWords { get; set; }
        public string Text { get; set; }


        public SpellCheckModel()
        {
            DictionaryPatch = Directory.GetCurrentDirectory() + @"\Resources\pldb-win.txt";
            _dictinary = LoadDictionary(DictionaryPatch);

            WrongWords = new ObservableCollection<WrongWord>();
        }

        public void StartSpellCheck()
        {

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
    }
}