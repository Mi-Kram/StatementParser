namespace Main.Operators.Pair
{
    /// <summary>
    /// Оператор импликации.
    /// </summary>
    public class ImplicationOperator : StatementPairOperator
    {
        public override bool Operate(bool value1, bool value2) => !value1 || value2;
    }
}
