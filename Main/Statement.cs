using System.Text;
using System.Text.RegularExpressions;
using Main.Values;
using Main.Values.Operations;
using Main.Operators.Single;
using Main.Operators.Pair;

namespace Main
{
    /// <summary>
    /// Класс для вывода таблица истинности для какого-либо выражения.
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// Символы операторов.
        /// </summary>
        private readonly char[] OPERATORS = "¬∧∨→≡".ToArray();

        /// <summary>
        /// Выражение для парсинга.
        /// </summary>
        private string _statement { get; set; }

        /// <summary>
        /// Выражение в программном виде.
        /// </summary>
        private StatementValue _operation { get; set; }

        /// <summary>
        /// Список параметров в выражении.
        /// </summary>
        private string[] _params;

        /// <summary>
        /// Ширина параметров.
        /// </summary>
        private int[] _requiredSpace = { };

        /// <summary>
        /// Текущий номер строки таблицы.
        /// </summary>
        private int _number = 0;

        /// <summary>
        /// Максимальная длина номера строки таблицы.
        /// </summary>
        private int _numberLength = 0;

        private static readonly BracketOperator _bracketOperator;
        private static readonly NegationOperator _negationOperator;
        private static readonly СonjunctionOperator _conjunctionOperator;
        private static readonly DisjunctionOperator _disjunctionOperator;
        private static readonly ImplicationOperator _implicationOperator;
        private static readonly EquivalenceOperator _equivalenceOperator;

        static Statement()
        {
            _bracketOperator = new BracketOperator();
            _negationOperator = new NegationOperator();
            _conjunctionOperator = new СonjunctionOperator();
            _disjunctionOperator = new DisjunctionOperator();
            _implicationOperator = new ImplicationOperator();
            _equivalenceOperator = new EquivalenceOperator();
        }

        private Statement(string statement)
        {
            _statement = statement;
            _params = Array.Empty<string>();

            // Обработка входного выражения.
            Handle();
            if (!IsValid()) new InvalidDataException("Неверный ввод!");

            // Количество строк = (1 << Params.Count) = 2^n, где n - количество параметров.
            _numberLength = (1 << _params.Length).ToString().Length;

            _operation = Parse(_statement);
        }

        /// <summary>
        /// Удаление и замена символов для корректной работы программы.
        /// Первичная обработка.
        /// </summary>
        private void Handle()
        {
            _statement = _statement.Replace(" ", "");

            // Замена обычных символов на специальные.
            Regex regex = new Regex(@"[\^\&]");
            _statement = regex.Replace(_statement, "∧");

            regex = new Regex(@"[_\|]");
            _statement = regex.Replace(_statement, "∨");

            regex = new Regex(@"[\=\-]?>");
            _statement = regex.Replace(_statement, "→");

            regex = new Regex(@"[\!\:\-]");
            _statement = regex.Replace(_statement, "¬");

            regex = new Regex(@"\={1,2}");
            _statement = regex.Replace(_statement, "≡");

            // Поиск всех переменных.
            regex = new Regex("[a-zA-Z]+");
            MatchCollection matches = regex.Matches(_statement);
            _params = matches.Select(x => x.Value).Distinct().ToArray();
            Array.Sort(_params, (a, b) => string.Compare(a, b, true));
        }

        /// <summary>
        /// Проверка выражения на валидность.
        /// </summary>
        /// <returns>True, если выражение корректно, иначе - False.</returns>
        private bool IsValid()
        {
            if (string.IsNullOrEmpty(_statement)) return false;

            Regex regex = new Regex("[^01()a-zA-Z¬∧∨→≡]");
            if (regex.Match(_statement).Success) return false;

            // Просмотр валидности скобок.
            int openBracket = _statement.IndexOf('(');
            if (openBracket == -1) return _statement.IndexOf(')') == -1;

            int bracketsLevel = 1;
            for (int i = openBracket + 1; i < _statement.Length; i++)
            {
                if (_statement[i] == '(') bracketsLevel++;
                else if (_statement[i] == ')' && --bracketsLevel < 0) return false;
            }

            return bracketsLevel == 0;
        }

        /// <summary>
        /// Разложение выражения на отдельные независимые компоненты для дальнейшей работы.
        /// </summary>
        /// <param name="str">Выражение для парсинга.</param>
        /// <returns>Выражение в программном виде.</returns>
        /// <exception cref="Exception"></exception>
        private StatementValue Parse(string str)
        {
            if (string.IsNullOrEmpty(str)) new InvalidDataException("Неверный ввод!");

            // Парсинг выражения, получение элементов (токенов).
            List<TokenInfo> tokens = TokenizeStatement(str);

            // Обработка выражения с самого приоритетного оператора - отрицания.
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type != TokenType.Operator) continue;
                if (tokens[i].Str != "¬") continue;
                if (i + 1 == tokens.Count) throw new InvalidDataException("Неверный ввод!");

                // Получить элемент, к которому применяется оператор.
                StatementValue sv = ConvertValue(tokens[i + 1]);

                // Заменить текущий элемент на новый.
                tokens.RemoveAt(i);
                tokens[i].Value = new StatementSingle(sv, _negationOperator);
                tokens[i].Type = TokenType.Converted;
                tokens[i].Str = string.Empty;
            }

            // Массив следующих операторов в порядке их приоритетов.
            (string ch, StatementPairOperator op)[] ops =
            {
                ("∧", _conjunctionOperator),
                ("∨", _disjunctionOperator),
                ("→", _implicationOperator),
                ("≡", _equivalenceOperator)
            };

            foreach ((string ch, StatementPairOperator op) item in ops)
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i].Type != TokenType.Operator) continue;
                    if (tokens[i].Str != item.ch) continue;
                    if (i == 0 || i + 1 == tokens.Count) throw new InvalidDataException("Неверный ввод!");

                    // Получить элементы, к которым применяются операторы.
                    StatementValue sv1 = ConvertValue(tokens[i - 1]);
                    StatementValue sv2 = ConvertValue(tokens[i + 1]);

                    // Заменить текущие элементы на новый.
                    tokens[i - 1].Value = new StatementPair(sv1, sv2, item.op);
                    tokens[i - 1].Type = TokenType.Converted;
                    tokens[i - 1].Str = string.Empty;
                    tokens.RemoveRange(i--, 2);
                }
            }

            // В конце должен остаться толькко один элемент (токен).
            if (tokens.Count != 1) throw new InvalidDataException("Неверный ввод!");
            return tokens[0].Type == TokenType.Converted ? tokens[0].Value : ConvertValue(tokens[0]);
        }

        /// <summary>
        /// Обработка токена. Преобразование в программное значение.
        /// </summary>
        /// <param name="token">Токен.</param>
        /// <returns>Программное значение токена.</returns>
        /// <exception cref="InvalidDataException"></exception>
        private StatementValue ConvertValue(TokenInfo token)
        {
            // Если токен уже был преобразован.
            if (token.Type == TokenType.Converted) return token.Value;
            if (string.IsNullOrWhiteSpace(token.Str)) throw new InvalidDataException("Неверный ввод!");

            switch (token.Type)
            {
                case TokenType.Bracket:
                    token.Value = new StatementSingle(Parse(token.Str[1..^1]), _bracketOperator);
                    return token.Value;

                case TokenType.StaticNum:
                    token.Value = new StaticValue(token.Str == "1");
                    return token.Value;

                case TokenType.DynamicNum:
                    // Поиск индекса переменной в списке.
                    int paramIndex = Array.IndexOf(_params, token.Str);
                    if (paramIndex == -1) throw new InvalidDataException("Неверный ввод!");
                    token.Value = new DynamicValue(paramIndex);
                    return token.Value;

                default: throw new InvalidDataException("Неверный ввод!");
            }
        }

        /// <summary>
        /// Парсинг выражения на элементы.
        /// </summary>
        /// <param name="str">Выражение для парсинга.</param>
        /// <returns>
        /// Список токенов, где data - значение токена, а info - тип токена
        /// </returns>
        /// <exception cref="InvalidDataException"></exception>
        private List<TokenInfo> TokenizeStatement(string str)
        {
            List<TokenInfo> lst = new List<TokenInfo>();

            for (int i = 0; i < str.Length; i++)
            {
                // Если встречается скобка, то обработать всё выражение этой скобки как один элемент.
                if (str[i] == '(')
                {
                    // Поиск закрывающей скобки на том же уровне вложенности.
                    int closeIndex = -1;
                    int bracketLevel = 1;
                    for (int j = i + 1; j < str.Length; j++)
                    {
                        if (str[j] == '(') bracketLevel++;
                        else if (str[j] == ')' && --bracketLevel == 0)
                        {
                            closeIndex = j;
                            break;
                        }
                    }

                    // Если индекс закрывающей скобки не найден.
                    if (closeIndex == -1) throw new InvalidDataException("Неверный ввод!");
                    lst.Add(new(str[i..(closeIndex + 1)], TokenType.Bracket));

                    i = closeIndex;
                    continue;
                }

                // Обработка операторов.
                if (OPERATORS.Contains(str[i]))
                {
                    lst.Add(new(str[i].ToString(), TokenType.Operator));
                    continue;
                }

                // Обработка статисечких значений.
                if (str[i] == '0' || str[i] == '1')
                {
                    lst.Add(new(str[i].ToString(), TokenType.StaticNum));
                    continue;
                }

                // Обработка динамических переменных.
                if (char.IsLetter(str[i]))
                {
                    // Поиск конца переменной.
                    int j = i + 1;
                    while (j < str.Length && char.IsLetter(str[j])) j++;
                    string param = str[i..j];

                    if (!_params.Contains(param)) throw new InvalidDataException("Неверный ввод!");
                    lst.Add(new(param, TokenType.DynamicNum));

                    i = j - 1;
                    continue;
                }
            }

            return lst;
        }

        /// <summary>
        /// Вывесли таблицу истинности.
        /// </summary>
        /// <param name="statement">Выражение для формирования таблицы истинности.</param>
        public static void DrawTable(string statement)
        {
            if (string.IsNullOrWhiteSpace(statement)) return;

            // Обработка входных данных.
            Statement st = new Statement(statement);

            // Вывод обработанного выражения.
            st.PrintStatement("Выражение: ");
            Console.WriteLine();

            // Вывод таблицы.
            st.ProcessAllValues();
        }

        /// <summary>
        /// Создание таблицы. Формирование истинности 
        /// выражения при всех значениях параметров.
        /// </summary>
        private void ProcessAllValues()
        {
            // Номер строки.
            _number = 0;

            // Сгенирировать строки-разделители для таблицы.
            var line = GetTableLine();

            // Вывод заголовка.
            Console.WriteLine(line.first);
            Console.Write($"│ {{0,-{_numberLength}}} ", "#");
            foreach (string item in _params) Console.Write($"│ {item} ");

            // Ячейка результата.
            Console.WriteLine("│   │");

            // Значение переменных выражения.
            bool[] values = new bool[_params.Length];
            int cnt = 0;

            while (true)
            {
                // Вычисление результата выражения при параметрах values.
                bool result = _operation.GetValue(values);

                // Вывод строки результата.
                Console.WriteLine(line.value);
                PrintRow(result, values);

                // Изменить значения параметров, сохраняя количество истинных параметров.
                // Это более человеческий вариант подстановки значений (для лучшего восприятия).
                bool step = false;
                for (int i = 0; i < values.Length - 1; i++)
                {
                    if (!values[i] || values[i + 1]) continue;
                    // Ждем такой последовательности:
                    // values[i] = True, values[i+1] = False.

                    step = true;

                    // Переносим истинность на следующий элемент.
                    (values[i], values[i + 1]) = (false, true);

                    // Теперь все предшествующие истинные значения переносим в самое начало.
                    // Количество истинных значений.
                    int tmp = 0;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (values[j])
                        {
                            tmp++;
                            values[j] = false;
                        }
                    }

                    // Устанавливаем начальное количество истинных значений.
                    for (int j = 0; j < tmp; j++)
                    {
                        values[j] = true;
                    }

                    break;
                }

                // Если не получилось изменить параметры,
                // добавить новое истинное значение.
                if (!step)
                {
                    // Если больше нет параметров - вывод окончен.
                    if (++cnt > values.Length) break;

                    // Установить все истинные параметры в начало.
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = i < cnt;
                    }
                }
            }

            Console.WriteLine(line.last);
        }

        // Символы для таблицы.
        // ┌────┬────┐
        // │    │    │
        // ├────┼────┤
        // │    │    │
        // └────┴────┘
        // ─ │ ┌ ┐ └ ┘ ├ ┤ ┬ ┴ ┼

        /// <summary>
        /// Генерация строк для таблицы.
        /// </summary>
        /// <returns>
        /// Строки-делители таблицы. <br/>
        /// first - верхняя линия таблицы. <br/>
        /// value - линия таблицы между строк. <br/>
        /// last - нижняя линия таблицы. <br/>
        /// </returns>
        private (string first, string value, string last) GetTableLine()
        {
            // Подсчёт мест (количества символов), которые нужно выделять для каждого столбца.
            _requiredSpace = _params.Select(x => x.Length).ToArray();

            // Добавление элементов-линий для строк-разделителей таблицы.
            StringBuilder sb = new StringBuilder("├");
            sb.Append(new string('─', _requiredSpace.Sum() + _requiredSpace.Length * 2 + 5 + _numberLength));

            int index = _numberLength + 3;
            sb.Insert(index, '┼');

            for (int i = 0; i < _params.Length; i++)
            {
                index += _requiredSpace[i] + 2;
                sb.Insert(++index, '┼');
            }
            sb.Append('┤');

            // Формирования строк-разделителей для "шапки", середины и "подвала" таблицы.
            string line = sb.ToString();
            return (
                line.Replace('├', '┌').Replace('┤', '┐').Replace('┼', '┬'),
                line,
                line.Replace('├', '└').Replace('┤', '┘').Replace('┼', '┴'));
        }

        /// <summary>
        /// Вывод одной строки таблицы.
        /// </summary>
        /// <param name="result">Результат выражения.</param>
        /// <param name="values">Значение параметров выражения.</param>
        private void PrintRow(bool result, bool[] values)
        {
            // Вывод номера строки.
            Console.Write($"│ {{0,-{_numberLength}}} ", ++_number);

            // Вывод параметров.
            for (int i = 0; i < values.Length; i++)
            {
                PrintCell(values[i], _requiredSpace[i]);
            }

            // Вывод результата выражения.
            PrintCell(result);
            Console.WriteLine("│");
        }

        /// <summary>
        /// Вывод ячейки таблицы.
        /// </summary>
        /// <param name="value">Значение ячейки: True = 1, False = 0.</param>
        /// <param name="space"></param>
        private void PrintCell(bool value, int space = 1)
        {
            Console.Write("│ ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = value ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(Convert.ToByte(value));
            Console.ResetColor();
            Console.Write(new string(' ', space));
        }

        /// <summary>
        /// Вывод отфоматированного выражения.
        /// </summary>
        /// <param name="beforeMsg">Сообщение перед выводом выражения.</param>
        private void PrintStatement(string beforeMsg = "")
        {
            string statement = _statement.Replace("∧", @" /\ ")
                .Replace("∨", @" \/ ").Replace("→", " -> ").Replace("≡", " = ");
            Console.WriteLine($"{beforeMsg}{statement}");
        }
    }
}

