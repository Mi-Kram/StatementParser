namespace Main.Operators.Pair
{
    /// <summary>
    /// Оператор дизъюнкции.
    /// </summary>
    public class DisjunctionOperator : StatementPairOperator
    {
        public override bool Operate(bool value1, bool value2) => value1 || value2;
    }
}
