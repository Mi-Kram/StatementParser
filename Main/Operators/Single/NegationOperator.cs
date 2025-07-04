namespace Main.Operators.Single
{
    /// <summary>
    /// Оператор Отрицания.
    /// </summary>
    public class NegationOperator : StatementSingleOperator
    {
        public override bool Operate(bool value) => !value;
    }
}
