using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangman
{
    class Game
    {
        public enum State
        {
            CONT0,
            CONT1,
            CONT2,
            CONT3,
            CONT4,
            CONT5,
            LOSS,
            WIN
        }
        private string Guesses;
        private string publicWord;
        private int Fails;
        private Random random;

        public string PublicWord { get => publicWord; }

        private string CurrentWord;

        public string Answer { get => CurrentWord; }

        private State CurrentState = State.CONT0;

        private List<String> GoodWords;


        // A new game, but for when we already have the word list.
        public void NewGame()
        {
            CurrentWord = GoodWords.ElementAt(random.Next(GoodWords.Count - 1));
            Log.Info("Hangman", CurrentWord);
            CurrentState = State.CONT0;
            Guesses = "";
            publicWord = "";
            Fails = 0;
            foreach (var chr in CurrentWord)
            {
                publicWord += "_";
            }

        }


        // New game, we remove some words we don't want, and any plurals, or words that are stupidly short.
        public void NewGame(List<string> words)
        {
            random = new Random();

            if (GoodWords is null)
            {
                // exclude words smaller than 3, purals and anything compounded.
                GoodWords = words.Where(a => (a.Length >= 3)).ToList().Where((a => !a.Contains("'") && !a.Contains("-"))).Where(a => a.Last() != 's').Where(a => Char.IsLower(a.First())).ToList();
            }

            NewGame();
        }
        public bool Guess(string str)
        {
            if (str.Length > 1) throw new Exception("string too long");

            if (CurrentWord.ToLower().Contains(str))
            {
                Guesses += str;
                var word = "";
                foreach (var chr in CurrentWord)
                {
                    if (Guesses.Contains(chr))
                    {
                        word += chr;
                    }
                    else
                    {
                        word += '_';
                    }

                }
                publicWord = word;
                return true;
            }
            Fails += 1;
            return false;
        }


        // This function check's the state of the game.
        public State CheckState()
        {

            if (!publicWord.Contains("_"))
            {
                CurrentState = State.WIN;
            }

            else if (Fails < 5)
            {
                CurrentState = (Game.State)Fails;
            }

            else if (Fails >= 5) CurrentState = State.LOSS;

            return CurrentState;
        }

        // Scores based on easiness of the word.
        public int Score()
        {
            int vows = "aeiou".Select<char, int>(a => CurrentWord.Where(b => b == a).Count()).Aggregate(0, (a, b) => a + b);
            int score = 25 - Math.Abs(value: 4 - CurrentWord.Distinct().Count()) - vows;
            return score;
        }
    }
}
