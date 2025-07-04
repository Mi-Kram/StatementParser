using Main.Operators.Single;

namespace Main.Values.Operations
{
    /// <summary>
    /// Значение выражения, где в операции используется только один параметр.
    /// </summary>
    public class StatementSingle : StatementOperation
    {
        /// <summary>
        /// Значение для обработки оператором.
        /// </summary>
        public StatementValue Value { get; set; }

        /// <summary>
        /// Оператор, который будет применяться к значению.
        /// </summary>
        public StatementSingleOperator Operator { get; set; }

        public StatementSingle(StatementValue value1, StatementSingleOperator op)
        {
            Value = value1;
            Operator = op;
        }

        public override bool GetValue(bool[] values) =>
            Operator.Operate(Value.GetValue(values));
    }
}
