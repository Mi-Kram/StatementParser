using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Operators.Pair
{
    /// <summary>
    /// Оператор конъюнкции.
    /// </summary>
    public class СonjunctionOperator : StatementPairOperator
    {
        public override bool Operate(bool value1, bool value2) => value1 && value2;
    }
}
