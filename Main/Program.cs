using System;

namespace Main
{
    internal class Program
    {
        static void Main()
        {
            /*
            ^ &         ∧
            _ |         ∨
            => -> >     →
            ! : -       ¬
            = ==        ≡
            

            (a => (b | c))-> !a
            ((((a->b)->a)->a)->a)->a
            (a->(b->c)) => ((a->b)->(a->c))
            (a-> (c & d))-> (((a->b) & (e-> !d)) -> ((c->b) | (d & b & e)))
            HELLO->BRIGHT->WORLDDDDD
            A->B = !A | B
            A->B = !B-> !A
            A-> 1
            */

            while (true)
            {
                Console.Write("Введите выражение: ");
                string statement = Console.ReadLine()?.Trim() ?? "";
                if (statement.Length == 0) break;

                try
                {
                    Console.Clear();
                    Statement.DrawTable(statement);
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка!\n\n");
                    Console.WriteLine(ex);
                }

                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}