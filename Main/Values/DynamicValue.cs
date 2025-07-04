namespace Main.Values
{
    /// <summary>
    /// Динамический параметр.
    /// </summary>
    public class DynamicValue : StatementValue
    {
        /// <summary>
        /// Сохранение индекса в массиве.
        /// </summary>
        public int ValueIndex { get; set; }

        public DynamicValue(int valueIndex)
        {
            ValueIndex = valueIndex;
        }

        public override bool GetValue(bool[] values) => values[ValueIndex];
    }
}
