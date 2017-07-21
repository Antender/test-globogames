using System;

namespace Calculator
{

    public class Program
    {
        public static void Main(String[] args)
        {
            while (true)
            {
                Console.Write(">> ");
                Lexer lexer = new Lexer(Console.ReadLine());
                Parser parser = new CalculatorParser(lexer);
                Console.WriteLine(parser.ParseExpression());
            }
        }
    }
}
