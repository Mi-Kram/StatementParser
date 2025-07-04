using Main.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    /// <summary>
    /// Информация о токене.
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// Программное выражение.
        /// </summary>
        public StatementValue Value { get; set; } = null;

        /// <summary>
        /// Выражение в виде строки.
        /// </summary>
        public string Str { get; set; } = string.Empty;

        /// <summary>
        /// Тип токена.
        /// </summary>
        public TokenType Type { get; set; } = TokenType.Undefined;

        public TokenInfo() { }

        public TokenInfo(string data, TokenType type)
        {
            Str = data;
            Type = type;
        }
    }
}
