using System;
using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorTests
{
    class ResultException : Exception
    {

    }

    [TestClass]
    public class CalculatorTests
    {

        [TestMethod]
        public void Test1()
        {
            Test("123", "123");
        }

        [TestMethod]
        public void Test2()
        {
            Test("123.1 + 456", "579.1");
        }

        [TestMethod]
        public void Test3()
        {
            Test("-+123.0", "-123");
        }

        [TestMethod]
        public void Test4()
        {
            Test("-12 * 4.0", "-48");
        }

        [TestMethod]
        public void Test5()
        {
            Test("3 ^ 2", "9");
        }

        [TestMethod]
        public void Test6()
        {
            Test("1 + 2 * 3 ^ 4 - 5 / 5", "162");
        }

        [TestMethod]
        public void Test7()
        {
            Test("1 + 2 - 3", "0");
        }

        [TestMethod]
        public void Test8()
        {
            Test("1 * 3 / 3", "1");
        }

        [TestMethod]
        public void Test9()
        {
            Test("4 ^ 3 ^ 2", "262144");
        }

        [TestMethod]
        public void Test10()
        {
            Test("2 ^ (1 + 2)", "8");
        }

        public void Test(string source, string expected)
        {
            Lexer lexer = new Lexer(source);
            Parser parser = new CalculatorParser(lexer);
            double result = parser.ParseExpression();
            string actual = result.ToString();

            if (expected != actual)
            {
                throw new ResultException();
            }
        }
    }
}
