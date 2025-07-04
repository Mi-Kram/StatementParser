namespace Main.Operators.Pair
{
    /// <summary>
    /// Оператор эквивалентности.
    /// </summary>
    public class EquivalenceOperator : StatementPairOperator
    {
        public override bool Operate(bool value1, bool value2) => value1 == value2;
    }
}
