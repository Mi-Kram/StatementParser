namespace Main.Operators.Single
{
    /// <summary>
    /// Оператор для одного параметра.
    /// </summary>
    public abstract class StatementSingleOperator
    {
        /// <summary>
        /// Обработать значение.
        /// </summary>
        /// <param name="value">Значение для обработки.</param>
        /// <returns>Результат обработки значения.</returns>
        public abstract bool Operate(bool value);
    }
}
