namespace Main.Values
{
    /// <summary>
    /// Значение части выражения, будь то параметр или операция.
    /// </summary>
    public abstract class StatementValue
    {
        /// <summary>
        /// Получить значение.
        /// </summary>
        /// <param name="values">Параметры выражения.</param>
        /// <returns>Конкретное значение.</returns>
        public abstract bool GetValue(bool[] values);
    }
}
