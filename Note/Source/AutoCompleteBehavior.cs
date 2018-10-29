using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Note.Source
{
    public static class AutoCompleteBehavior
    {
        private static TextChangedEventHandler _onTextChanged = OnTextChanged;
        private static KeyEventHandler _onKeyDown = OnPreviewKeyDown;
        private static string _matchingString = "";
        private static string _match;
        private static List<string> _hintWords = new List<string>();
        private static string _currentWord = "";
        private static int _currentWordPosition = 0;
        private static TextBox _textBox;

        public static readonly DependencyProperty AutoCompleteEnabledProperty =
            DependencyProperty.RegisterAttached("AutoCompleteEnabled",
                typeof(bool), typeof(AutoCompleteBehavior),
                new PropertyMetadata(false, OnAutoComplete));

        public static readonly DependencyProperty AutoCompleteDictionaryProperty =
            DependencyProperty.RegisterAttached("Dictionary",
                typeof(IEnumerable<string>), typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(null, OnIncorrectWordFound));

        private static void OnIncorrectWordFound(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #region Items Source

        public static bool GetAutoCompleteEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoCompleteEnabledProperty);
        }

        public static void SetAutoCompleteEnabled(DependencyObject obj,
            bool value)
        {
            obj.SetValue(AutoCompleteEnabledProperty, value);
        }

        public static IEnumerable<string> GetDictionary(DependencyObject obj)
        {
            object objRtn = obj.GetValue(AutoCompleteDictionaryProperty);
            if (objRtn is IEnumerable<string>)
                return (objRtn as IEnumerable<string>);

            return new ObservableCollection<string>();
        }



        public static void SetDictionary(DependencyObject obj, IEnumerable<string> value)
        {
            obj.SetValue(AutoCompleteDictionaryProperty, value);
        }
        private static void OnAutoComplete(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (sender == null)
                return;

            tb.TextChanged -= _onTextChanged;
            tb.PreviewKeyDown -= _onKeyDown;
            if (e.NewValue != null)
            {
                tb.TextChanged += _onTextChanged;
                tb.PreviewKeyDown += _onKeyDown;
            }
        }

        #endregion

        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = e.OriginalSource as TextBox;
            if (!GetAutoCompleteEnabled(tb))
                return;

            if (tb == null)
                return;

            if (e.Key == Key.RightAlt && GetCurrentWord(tb.Text, tb.CaretIndex).StartsWith(_matchingString))
            {
                tb.TextChanged -= _onTextChanged;
                tb.Text = ReplaceToNext(tb.Text, tb.CaretIndex);
                tb.CaretIndex = _currentWordPosition + _currentWord.Length;
                tb.SelectionStart = _currentWordPosition + _matchingString.Length;
                tb.SelectionLength = _currentWord.Length - _matchingString.Length;
                tb.TextChanged += _onTextChanged;
            }


            if (e.Key != Key.RightShift)
                return;

            if (tb.SelectionLength > 0)
            {
                tb.CaretIndex = tb.SelectionStart = tb.SelectionStart + tb.SelectionLength;
                tb.SelectionLength = 0;
            }
        }

        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!GetAutoCompleteEnabled(sender as TextBox))
                return;

            if
            (
                (from change in e.Changes where change.RemovedLength > 0 select change).Any() &&
                (from change in e.Changes where change.AddedLength > 0 select change).Any() == false
            ) // procs on bacspace 
                return;

            var tb = e.OriginalSource as TextBox;

            if (tb != null && tb.Text.EndsWith(" "))
                return;

            if (sender == null)
                return;

            if (String.IsNullOrEmpty(tb?.Text))
                return;

            _matchingString = GetCurrentWord(tb.Text, tb.CaretIndex);

            _hintWords = GetWordContinuations(tb, _matchingString);

            if (!_hintWords.Any())
            {
                return;
            }

            _currentWordPosition = GetWordStartPosition(tb.Text, tb.CaretIndex);

            if (String.IsNullOrEmpty(_matchingString))
                return;

            var wordLength = _matchingString.Length;

            _match =
            (
                from
                    value
                    in
                    (
                        from subvalue
                            in _hintWords
                        where subvalue != null && subvalue.Length >= wordLength
                        select subvalue
                    )
                where value.Substring(0, wordLength).Equals(_matchingString)
                select value.Substring(wordLength,
                    value.Length - wordLength)).FirstOrDefault();

            if (String.IsNullOrEmpty(_match))
                return;

            var matchStart = _currentWordPosition + _matchingString.Length;

            tb.TextChanged -= _onTextChanged;
            tb.Text = tb.Text.Insert(tb.CaretIndex, _match);
            _currentWord = _matchingString + _match;
            tb.CaretIndex = matchStart;
            tb.SelectionStart = matchStart;

            tb.SelectionLength = (_match.Length);
            tb.TextChanged += _onTextChanged;
        }

        #region Helper Methods
        private static List<string> GetWordContinuations(DependencyObject obj, string word)
        {
            var continuationWords = new List<string>();

            foreach (var wordFromDictionary in GetDictionary(obj))
            {
                if (continuationWords.Count >= 100) return continuationWords;
                if (wordFromDictionary.StartsWith(word))
                    continuationWords.Add(wordFromDictionary);
            }
            return continuationWords;
        }
        private static string GetCurrentWord(string text, int index)
        {
            var word = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ' || text[i] == '\r' || text[i] == '\n' || text[i] == '\t')
                {
                    if (i >= index - 1) return word.Trim();
                    word = "";
                }

                word += text[i];
            }

            return word.Trim();
        }

        private static int GetWordStartPosition(string text, int index)
        {
            if (index == 0) return 0;
            for (var i = index - 1; i > 0; i--)
            {
                if (char.IsSeparator(text[i]))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private static string ReplaceToNext(string text, int index)
        {
            if (_hintWords.Count == 0) return text;
            text = text.Remove(_currentWordPosition, GetCurrentWord(text, index).Length);
            _currentWord = GetNextWord();
            text = text.Insert(_currentWordPosition, _currentWord);
            return text;
        }

        private static int _currentWordId = 1;
        private static string GetNextWord()
        {
            if (_currentWordId >= _hintWords.Count)
                _currentWordId = 0;
            return _hintWords[_currentWordId++];
        }
        #endregion
    }
}