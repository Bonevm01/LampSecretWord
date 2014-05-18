using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeamAladdin
{
    class LampSecretWord
    {
        static DateTime TimerTime;
        static List<char> AvailbleChars = new List<char>();
        static List<char> UsedLetters = new List<char>();
        static StringBuilder fullWord = new StringBuilder();
        static int lives;
        static int level = 1; //levels from 1 to 120
        static bool changeInLevels = true;
        static int score;
        static int currentScore;
        static string currentLetter;
        static bool changeInErrors = false;   //To re-draw the gibbed only if there is a new mistake
        static string secretWord;
        static string word;
        static string[,] FindedWordAndScore = new string[150, 2]; //use to keep the score of a player and to print it when game is over
        static Random rnd = new Random();
        static void Main()
        {
            Console.BufferHeight = Console.WindowHeight = 48;      //50 // will be needed in future 
            Console.BufferWidth = Console.WindowWidth = 142;    //150 // will be needed in future
            string[][] MatrixWithWords = ReadwordsFromTextFileAndDistributeByLength();


            while (true)
            {
                Console.Clear();
                if (changeInLevels)
                {
                    secretWord = GetRandomWord(MatrixWithWords);
                    word = HideWordLetters(secretWord);
                    changeInLevels = false;
                }
                
                lives = 10;
                int tempLives = new int();
                int timerTempCount = 0;
                UsedLetters.Clear();
                currentLetter = 'А'.ToString();
                WritePlayground();
                SetAvalibleChars();
                bool[] revealedChars = new bool[secretWord.Length];
                bool whileTrue = true;
                revealedChars[0] = true;
                revealedChars[revealedChars.Length - 1] = true;
                char[] secretWordChars = secretWord.ToCharArray();
                ResetTimer();
                while (whileTrue)
                {
                    if (Console.KeyAvailable)
                    {
                        CleanServiceMessage();
                        ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            try
                            {
                                bool LetterNotFound = true;
                                if (LetterIsNotUsed(currentLetter.ToUpper()[0]))
                                {
                                    ResetTimer();
                                    UsedLetters.Add(currentLetter.ToUpper()[0]);
                                    ColoringUsedLetter(currentLetter.ToUpper()[0]);
                                    for (int i = 1; i < secretWordChars.Length - 1; i++)
                                    {
                                        if (secretWordChars[i].ToString().ToUpper().Equals(currentLetter.ToUpper()))
                                        {
                                            PrintServiceMessage("Браво! Това е буква от търсената дума.");
                                            LetterNotFound = false;
                                            Console.SetCursorPosition((13 - secretWord.Length / 2) + i, 5);
                                            Console.Write(secretWordChars[i].ToString().ToUpper());
                                        }
                                    }
                                    if (LetterNotFound)
                                    {
                                        lives--;
                                        changeInErrors = true;
                                        PrintOnPosition(73, 2, lives.ToString() + " ");
                                        throw new ArgumentException("Съжалявам но избраната буква не е част от думата. Опитайте пак.");
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException("Тази буква вече е разкрита !");
                                }
                            }
                            catch (ArgumentException Arg)
                            {
                                PrintServiceMessage(Arg.Message);
                            }
                            Console.SetCursorPosition(19, 7);
                            if (WordIsFindByLetterInsert())
                            {
                                Console.Clear();
                                whileTrue = false;
                            }
                        }
                        else if (pressedKey.Key == ConsoleKey.DownArrow)
                        {
                            int count = 0;
                            Console.SetCursorPosition(19, 9);
                            for (int i = 0; i < word.Length; i++)
                            {
                                Console.Write('-');
                            }
                            Console.SetCursorPosition(19, 9);
                            while (true)
                            {
                                if (timerTempCount == 0)
                                {
                                    tempLives = lives - 1;
                                    ResetTimer();
                                    timerTempCount++;
                                }
                                if (lives == tempLives)
                                {
                                    timerTempCount = 0;
                                    break;
                                }
                                if (Console.KeyAvailable)
                                {
                                    CleanServiceMessage();
                                    ConsoleKeyInfo pressedKeyForFullWord = Console.ReadKey(true);
                                    if (pressedKeyForFullWord.Key == ConsoleKey.Enter)
                                    {
                                        timerTempCount = 0;
                                        if (CheckForCorrectWord())
                                        {
                                            whileTrue = false;
                                            break;

                                        }
                                        else
                                        {
                                            lives--;
                                            changeInErrors = true;
                                            PrintOnPosition(73, 2, lives.ToString() + " ");
                                            ResetTimer();
                                            break;
                                        }

                                    }
                                    else if (pressedKeyForFullWord.Key == ConsoleKey.Backspace)
                                    {

                                        if (count > 0)
                                        {
                                            fullWord.Remove(fullWord.Length - 1, 1);
                                            count--;
                                        }
                                        Console.SetCursorPosition(19 + count, 9);
                                        Console.Write('-');

                                    }
                                    else
                                    {
                                        try
                                        {
                                            bool isNotAvailbleKey = true;
                                            for (int i = 0; i < AvailbleChars.Count; i++)
                                            {
                                                if (pressedKeyForFullWord.KeyChar == AvailbleChars[i])
                                                {
                                                    isNotAvailbleKey = false;
                                                    Console.SetCursorPosition(19 + count, 9);
                                                    if (count == word.Length)
                                                    {
                                                        Console.SetCursorPosition(18 + count, 9);
                                                    }
                                                    if (count < word.Length)
                                                    {
                                                        count++;
                                                    }
                                                    if (fullWord.Length < word.Length)
                                                    {
                                                        fullWord.Append(AvailbleChars[i]);
                                                    }
                                                    else
                                                    {
                                                        fullWord.Remove(fullWord.Length - 2, 1);
                                                        fullWord.Append(AvailbleChars[i]);
                                                    }
                                                    currentLetter = (AvailbleChars[i]).ToString();
                                                    Console.Write(currentLetter.ToUpper());
                                                    break;
                                                }
                                            }
                                            if (isNotAvailbleKey)
                                            {
                                                throw new ArgumentException("Трябва да изберете буква от кирилица. Опитайте пак.");
                                            }
                                        }
                                        catch (ArgumentException argM)
                                        {
                                            PrintServiceMessage(argM.Message);
                                        }
                                    }
                                }
                                Timer();
                                Thread.Sleep(100);
                            }
                            Console.SetCursorPosition(19, 9);
                            for (int i = 0; i < word.Length; i++)
                            {
                                Console.Write(' ');
                            }
                            Console.SetCursorPosition(19, 7);

                        }
                        else
                        {
                            try
                            {
                                bool isNotAvailbleKey = true;
                                for (int i = 0; i < AvailbleChars.Count; i++)
                                {
                                    if (pressedKey.KeyChar == AvailbleChars[i])
                                    {
                                        isNotAvailbleKey = false;
                                        Console.SetCursorPosition(19, 7);
                                        currentLetter = (AvailbleChars[i]).ToString();
                                        Console.Write(currentLetter.ToUpper());
                                        break;
                                    }
                                }
                                if (isNotAvailbleKey)
                                {
                                    throw new ArgumentException("Трябва да изберете буква от кирилица. Опитайте пак");
                                }
                            }
                            catch (ArgumentException AgrE)
                            {
                                PrintServiceMessage(AgrE.Message);
                            }
                        }


                        }
                        if (changeInErrors)
                        {
                            PrintGibbet(lives);
                        }
                        Timer();
                        NoLives();
                        Thread.Sleep(100);



                    }
                }

            }

        static void PrintServiceMessage(string str)
        {
            Console.SetCursorPosition(21, 11);  // set on 21 to be on service message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(str);
            Console.ResetColor();
            //Console.Beep(); //времено го закоментирам, защото ме дразни когато тествам да пиука
        }
        private static void PrintHiddenWordLetters(string word)
        {
            char[] wordChars = word.ToCharArray();
            for (int i = 0; i < word.Length; i++)
            {
                Console.SetCursorPosition((13 - wordChars.Length / 2) + i, 5);
                Console.Write(wordChars[i]);
            }
            Console.SetCursorPosition(19, 7);
        }
        private static string GetRandomWord(string[][] matrix)
        {

            string secretWord = matrix[level - 1][rnd.Next(0, matrix[level - 1].Length)];
            return secretWord;
        }
        private static string HideWordLetters(string secretWord)
        {
            //string word = "А----Н";

            string word = secretWord[0] + new string('-', secretWord.Length - 2) + secretWord[secretWord.Length - 1];
            return word;
        }
        private static void WritePlayground()
        {
            using (StreamReader Playground = new StreamReader("../../playground.txt", Encoding.UTF8))
            {
                string playgroundLine = Playground.ReadLine();
                while (playgroundLine != null)
                {
                    Console.WriteLine(playgroundLine);
                    playgroundLine = Playground.ReadLine();
                }
            }
            Console.SetCursorPosition(8, 3);
            Console.Write("SECRET WORD", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.SetCursorPosition(3, 11);
            Console.Write("SERVICE MESSAGE : ", Console.ForegroundColor = ConsoleColor.Red);
            Console.ResetColor();
            Console.SetCursorPosition(3, 7);
            Console.Write("SELECT LETTER : ");
            Console.SetCursorPosition(3, 9);
            Console.Write("TRY FULL WORD : ");
            PrintOnPosition(73, 2, lives.ToString());
            PrintOnPosition(73, 1, score.ToString());
            PrintOnPosition(73, 3, level.ToString());
            PrintOnPosition(19, 7, currentLetter);
            PrintHiddenWordLetters(word);

        }
        static void PrintGibbet(int Lives)
        {
            int startX = 23;
            int startY = 50;
            if (Lives == 9)
            {
                for (int i = 0; i < 11; i++)
                {
                    Console.SetCursorPosition(startY, startX + 1 + i);
                    Console.Write("||");
                }
            }
            if (Lives == 8)
            {
                for (int i = 0; i < 12; i++)
                {
                    Console.SetCursorPosition(startY + i, startX);
                    Console.Write("=");
                }
            }
            if (Lives == 7)
            {
                for (int i = 0; i < 11; i++)
                {
                    Console.SetCursorPosition(startY + 10, startX + 1 + i);
                    Console.Write("||");
                }
            }
            if (Lives == 6)
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.SetCursorPosition(startY + 5, startX + 1 + i);
                    Console.Write("|");
                }
            }

            if (Lives == 5)
            {
                Console.SetCursorPosition(startY + 4, startX + 3);
                Console.Write("/");
                Console.SetCursorPosition(startY + 5, startX + 3);
                Console.Write("^");
                Console.SetCursorPosition(startY + 6, startX + 3);
                Console.Write("\\");
                Console.SetCursorPosition(startY + 6, startX + 4);
                Console.Write("/");
                Console.SetCursorPosition(startY + 5, startX + 4);
                Console.Write("_");
                Console.SetCursorPosition(startY + 4, startX + 4);
                Console.Write("\\");
                Console.WriteLine();
            }
            if (Lives == 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(startY + 5, startX + 5 + i);
                    Console.Write("|");
                }
            }
            if (Lives == 3)
            {
                Console.SetCursorPosition(startY + 4, startX + 5);
                Console.Write("/");
                Console.SetCursorPosition(startY + 3, startX + 6);
                Console.Write("/");
            }

            if (Lives == 2)
            {
                Console.SetCursorPosition(startY + 6, startX + 5);
                Console.Write("\\");
                Console.SetCursorPosition(startY + 7, startX + 6);
                Console.Write("\\");
            }
            if (Lives == 1)
            {
                Console.SetCursorPosition(startY + 6, startX + 8);
                Console.Write("\\");
                Console.SetCursorPosition(startY + 7, startX + 9);
                Console.Write("\\");

            }
            if (Lives == 0)
            {
                Console.SetCursorPosition(startY + 4, startX + 8);
                Console.Write("/");
                Console.SetCursorPosition(startY + 3, startX + 9);
                Console.Write("/");
                CleanServiceMessage();
                PrintServiceMessage("ОБЕСЕН СТЕ!");
                PrintOnPosition(16, 15, "Натиснете произволен бутон за да видите финалния ви резултат . . .", Console.ForegroundColor = ConsoleColor.Red);
                Console.ReadKey();

            }
            changeInErrors = false;
            Console.SetCursorPosition(19, 7);
        }
        private static void CleanServiceMessage()
        {
            string EmptyPlaces = new string(' ', 63);
            PrintOnPosition(21, 11, EmptyPlaces);
        }
        private static void PrintOnPosition(int x, int y, string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text, color);
            Console.ResetColor();
        }
        private static void Timer()
        {
            if ((TimerTime - DateTime.Now).ToString("ss").CompareTo("00") != 1)
            {
                lives--;
                PrintGibbet(lives);
                PrintOnPosition(73, 2, lives.ToString() + " ");
                ResetTimer();
            }
            string TimerValue = (TimerTime - DateTime.Now).ToString("ss");
            PrintOnPosition(73, 4, TimerValue);
        }
        private static void ResetTimer()
        {
            TimerTime = DateTime.Now.AddSeconds(20);
        }
        private static void NoLives()
        {
            if (lives <= 0)
            {
                Console.Clear();
                int Number = 0;
                int columnSize = 40 * Number;
                for (int i = 0; i < level; i++)
                {
                    if (i == 39 || i == 79)
                    {
                        Number++;
                    }
                    Console.SetCursorPosition(columnSize, i);
                    Console.Write("{0} - {1} Points", FindedWordAndScore[i, 0], FindedWordAndScore[i, 1]);
                }
                string TotalScore = "TOTAL SCORE - " + score;

                PrintOnPosition((80 - (TotalScore.Length / 2)), 45, TotalScore, Console.ForegroundColor = ConsoleColor.Red);
                PrintOnPosition(75, 44, "GAME OVER ", Console.ForegroundColor = ConsoleColor.Red);
                Environment.Exit(0);
            }
        }
        private static void CalculateScore()
        {
            currentScore = (UsedLetters.Count - (7 - lives)) * 10;
            FindedWordAndScore[level - 1, 0] = secretWord;
            FindedWordAndScore[level - 1, 1] = currentScore.ToString();
            score += (UsedLetters.Count - (7 - lives)) * 10;
        }
        private static void CalculateScoreForFullWord()
        {
            currentScore = (secretWord.Length - (UsedLetters.Count - (7 - lives))) * 10;
            FindedWordAndScore[level - 1, 0] = secretWord;
            FindedWordAndScore[level - 1, 1] = currentScore.ToString();
            score += (secretWord.Length - (UsedLetters.Count - (7 - lives))) * 10;
        }
        private static bool CheckForCorrectWord()
        {
            if (secretWord.ToUpper().Equals(fullWord.ToString().ToUpper()))
            {
                CalculateScoreForFullWord();
                PrintServiceMessage("Браво! Познахте думата . Получавате " + currentScore + " точки");
                PrintOnPosition(16, 15, "Натиснете произволен бутон за следващата дума . . .", Console.ForegroundColor = ConsoleColor.Red);
                Console.ReadKey();
                level++;
                changeInLevels = true;
                return true;
            }
            else
            {
                fullWord.Clear();
                return false;
            }
        }
        private static void SetAvalibleChars()
        {
            for (int i = 'а'; i <= 'я'; i++)
            {
                AvailbleChars.Add((char)i);
            }
            for (int i = 'А'; i <= 'Я'; i++)
            {
                AvailbleChars.Add((char)i);
            }
        }
        private static bool WordIsFindByLetterInsert()
        {
            int count = 0;
            for (int i = 1; i < secretWord.Length - 1; i++)
            {
                for (int k = 0; k < UsedLetters.Count; k++)
                {
                    if (secretWord[i].Equals(UsedLetters[k]))
                    {
                        count++;
                        continue;
                    }
                }
            }
            if (count == secretWord.Length - 2)
            {
                CalculateScore();
                PrintServiceMessage("Браво! Познахте думата . Получавате " + currentScore + " точки");
                PrintOnPosition(16, 15, "Натиснете произволен бутон за следващата дума . . .", Console.ForegroundColor = ConsoleColor.Red);
                Console.ReadKey();
                level++;
                changeInLevels = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool LetterIsNotUsed(char p)
        {
            for (int i = 0; i < UsedLetters.Count; i++)
            {
                if (p.Equals(UsedLetters[i]))
                {
                    return false;
                }
            }
            return true;
        }
        private static void ColoringUsedLetter(char CurrentLetter)
        {
            int Possiton = CurrentLetter - 1040;
            if (Possiton == 31 || Possiton == 30)
            {
                Possiton -= 2;
            }
            else if (Possiton == 28)
            {
                Possiton -= 1;
            }
            int left = Possiton % 30;
            Console.SetCursorPosition(12 + left * 2, 19);
            Console.Write("{0}", (char)CurrentLetter, Console.ForegroundColor = ConsoleColor.Red);
            Console.ResetColor();
        }
        private static string[][] ReadwordsFromTextFileAndDistributeByLength()
        {
            int numberOfLevels = 10; //General data
            int[] counts = new int[numberOfLevels]; //To count words with each length between 6 and 6+levels-1
            StreamReader listOfWords = new StreamReader("../../WordsList.txt", Encoding.UTF8);
            //Count words listed in the text file
            using (listOfWords)
            {
                string line = listOfWords.ReadLine();
                while (line != null)
                {
                    if (line.Length >= 6 && line.Length <= (6 + numberOfLevels - 1))
                    {
                        counts[line.Length - 6]++;
                    }
                    line = listOfWords.ReadLine();
                }
            }
            //Create array of arrays
            string[][] matrixWithSecretWords = new string[numberOfLevels][];
            for (int i = 0; i < numberOfLevels; i++)
            {
                matrixWithSecretWords[i] = new string[counts[i]];
                counts[i] = 0;
            }
            //Save words from the text file into the array
            StreamReader listWithWords = new StreamReader("../../WordsList.txt", Encoding.GetEncoding("windows-1251"));
            using (listWithWords)
            {
                string line = listWithWords.ReadLine();
                while (line != null)
                {
                    if (line.Length >= 6 && line.Length <= (6 + numberOfLevels - 1))
                    {
                        matrixWithSecretWords[line.Length - 6][counts[line.Length - 6]] = line;
                        counts[line.Length - 6]++;
                    }
                    line = listWithWords.ReadLine();
                }
            }
            return matrixWithSecretWords;
        }
    }
}
