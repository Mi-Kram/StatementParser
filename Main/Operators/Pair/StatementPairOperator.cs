namespace Main.Operators.Pair
{
    /// <summary>
    /// Оператор для двух параметров.
    /// </summary>
    public abstract class StatementPairOperator
    {
        /// <summary>
        /// Обработать два значения.
        /// </summary>
        /// <param name="value1">Первое значение для обработки.</param>
        /// <param name="value2">Второе значение для обработки.</param>
        /// <returns>Результат обработки значений.</returns>
        public abstract bool Operate(bool value1, bool value2);
    }
}
