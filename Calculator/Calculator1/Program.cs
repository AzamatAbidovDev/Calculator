using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator1
{
    class Program
    {
        static char[] znaki = new char[] { '+', '-', '*', '/'};
        static void Main(string[] args)
        {
            while(true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                var line = getExample();
                line = line.Replace("-", "+-");
                
                string result1 = Skobki(line);
                string result = Counting(result1);

                Console.WriteLine($"Result: {result}");
                Console.ReadLine();
            }
        }
        /// <summary>
        /// Метод вычисления
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static string Counting(string line)
        {
            var znakIndex = actionIndex(line);
            if (znakIndex.Item2 == -1)
                return line;
            
            string textLeft = line.Substring(0, znakIndex.Item2);
            string textRight = line.Substring(znakIndex.Item2 + 1, line.Length - znakIndex.Item2 - 1);
            var numberLeft = getNumberLast(textLeft);
            var numberRight = getNumberFirst(textRight); 

            if (znakIndex.Item1 == "/" && numberRight.Item2 == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return "Invalid example(problem with dividing to 0)";               
            }
            
            double newNumber = calc(numberLeft.Item2, numberRight.Item2, znakIndex.Item1);
            string newLine = numberLeft.Item1 + newNumber + numberRight.Item1;
            return Counting(newLine);
        }
        /// <summary>
        /// Вычисляет выражение внутри скобки
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static string Skobki(string line)
        {
            int indexskobka1 = line.IndexOf(')');
            int indexskobka = 0;
            if (indexskobka1 == -1)
                return line;
            for (int i = 0; i < indexskobka1; i++)
            {
                if (line[i] == '(')
                {
                    indexskobka = i;
                }
            }
            string inskobki = line.Substring(indexskobka, indexskobka1 - indexskobka + 1);
            inskobki = inskobki.Replace("(", "").Replace(")", "");
            string newline = Counting(inskobki);
            
            line = line.Substring(0, indexskobka) + newline + line.Substring(indexskobka1 + 1, line.Length - indexskobka1 - 1);

            return Skobki(line);
        }
        
        static (string, double) getNumberLast(string text)
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if(!char.IsDigit(text[i]) && text[i] != '-' && text[i] != ',')
                {
                    return (text.Substring(0, i + 1), double.Parse(text.Substring(i + 1, text.Length - i - 1)));
                }
            }
            return ("", double.Parse(text));
        }
        static (string, double) getNumberFirst(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsDigit(text[i]) && text[i] != '-' && text[i] != ',')
                {
                    return (text.Substring(i, text.Length - i), double.Parse(text.Substring(0, i)));
                }
            }
            return ("", double.Parse(text));
        }

        static double calc(double number1, double number2, string action)
        { 
            switch(action)
            {
                case "*": return number1 * number2;
                case "/": return number1 / number2;
                case "-": return number1 - number2;
                case "+": return number1 + number2;
                default: return 0;
            }
        }
        
        static (string, int) actionIndex(string line)
        {
            int indexMultiply = line.IndexOf('*');   
            int indexDivision = line.IndexOf('/');
            int indexPlus = line.IndexOf('+');

            if (indexMultiply != -1)
            {
                if (indexDivision != -1)
                    return (indexMultiply < indexDivision ? "*" : "/", indexMultiply < indexDivision ? indexMultiply : indexDivision);
                else
                    return ("*", indexMultiply);
            }
            else if (indexDivision != -1)
                return ("/", indexDivision);
            else if (indexPlus != -1)
                return ("+", indexPlus);
            else
                return ("", -1);
        }
        /// <summary>
        /// Ввод в консоль
        /// </summary>
        /// <returns></returns>
        static string getExample()
        {
            Console.WriteLine("Enter example");
            var line = Console.ReadLine();
            if (isCorrectLine(line))
                return line;
            else 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid example");
                Console.ForegroundColor = ConsoleColor.White;
                return getExample(); 
            }
        }
        /// <summary>
        /// Проверка выражения
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static bool isCorrectLine(string line)
        {
            if (string.IsNullOrEmpty(line) || line.Length < 3) return false;

            int countSkobki = 0;

            if(line[0] == ')') return false;

            int indexZnak = -2;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '/' && line[i + 1] == '0') return false;

                char item = line[i];
                
                if (!char.IsDigit(item) && item != ',')
                {
                    if (item == '(')
                    {
                        countSkobki++;

                        if (i + 1 == line.Length) return false;
                        
                        if (!(line[i + 1] == '(' || char.IsDigit(line[i + 1])))
                            return false;
                        if (i > 0)
                        {
                            char prevNumber = line[i - 1];
                            if (!(znaki.Contains(prevNumber) || prevNumber == '('))
                                return false;
                        }
                    }
                    else if (item == ')')
                    {
                        countSkobki--;
                        if (countSkobki < 0)
                            return false;
                        if (line[i - 1] != ')' && !char.IsDigit(line[i - 1]))
                            return false;
                        if (i + 1 < line.Length)
                        {
                            char nextNumber = line[i + 1];
                            if (!znaki.Contains(nextNumber) && nextNumber != ')')
                                return false;
                        }
                    }
                    else if (!znaki.Contains(item))
                        return false;
                    else
                    {
                        if (indexZnak + 1 == i)
                            return false;
                        else if (i + 1 == line.Length)
                            return false;
                        else
                            indexZnak = i;
                    }
                }
            }
            if (countSkobki != 0)
                return false;
            return true;
        }
}
}

