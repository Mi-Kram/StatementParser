namespace Main.Values
{
    /// <summary>
    /// Статический параметр.
    /// </summary>
    public class StaticValue : StatementValue
    {
        /// <summary>
        /// Сохранённое значение.
        /// </summary>
        public bool Value { get; set; }

        public StaticValue(bool value)
        {
            Value = value;
        }

        public override bool GetValue(bool[] values) => Value;
    }
}
