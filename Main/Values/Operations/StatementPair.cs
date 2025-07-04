using Main.Operators.Pair;

namespace Main.Values.Operations
{
    /// <summary>
    /// Значение части выражения, где в операции используется только два параметра.
    /// </summary>
    public class StatementPair : StatementOperation
    {
        /// <summary>
        /// Первое значение для обработки оператором.
        /// </summary>
        public StatementValue Value1 { get; set; }

        /// <summary>
        /// Второе значение для обработки оператором.
        /// </summary>
        public StatementValue Value2 { get; set; }

        /// <summary>
        /// Оператор, который будет применяться к двум параметрам.
        /// </summary>
        public StatementPairOperator Operator { get; set; }

        public StatementPair(StatementValue value1, StatementValue value2, StatementPairOperator op)
        {
            Value1 = value1;
            Value2 = value2;
            Operator = op;
        }

        public override bool GetValue(bool[] values) =>
            Operator.Operate(Value1.GetValue(values), Value2.GetValue(values));
    }
}
