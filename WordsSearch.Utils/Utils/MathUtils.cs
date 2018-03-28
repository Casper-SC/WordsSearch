using System;

namespace WordsSearch.Utils.Utils
{
    public class MathUtils
    {
        /// <summary>
        /// Получить количество цифр в числе
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetDigitsCount(int number)
        {
            int counter = 1;
            if (number == int.MinValue)
            {
                number--;
            }
            int temp = Math.Abs(number);
            while ((temp = (temp / 10)) > 0)
            {
                counter++;
            }

            return counter;
        }
    }
}
