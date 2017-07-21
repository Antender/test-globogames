using System;
using System.Collections.Generic;

namespace Calculator
{
    public enum ParsletPrecedence : int
    {
        ASSIGNMENT = 1,
        CONDITIONAL = 2,
        SUM = 3,
        PRODUCT = 4,
        EXPONENT = 5,
        PREFIX = 6,
        POSTFIX = 7,
        CALL = 8
    }

    public interface IInfixParselet
    {
        double Parse(Parser parser, double left, Token token);
        ParsletPrecedence Precedence { get; }
    }

    public interface IPrefixParselet
    {
        double Parse(Parser parser, Token token);
    }

    public class BinaryOperatorParselet : IInfixParselet
    {
        public ParsletPrecedence Precedence { get; }
        private bool isRight;

        public BinaryOperatorParselet(ParsletPrecedence precedence, bool isRight)
        {
            Precedence = precedence;
            this.isRight = isRight;
        }

        public double Parse(Parser parser, double left, Token token)
        {
            // To handle right-associative operators like "^", we allow a slightly
            // lower precedence when parsing the right-hand side. This will let a
            // parselet with the same precedence appear on the right, which will then
            // take *this* parselet's result as its left-hand argument.
            double right = parser.ParseExpression(Precedence - (isRight ? 1 : 0));
            switch (token.Type)
            {
                case TokenType.ASTERISK:
                    return left * right;
                case TokenType.CARET:
                    return Math.Pow(left, right);
                case TokenType.MINUS:
                    return left - right;
                case TokenType.PLUS:
                    return left + right;
                case TokenType.SLASH:
                    return left / right;
            }
            throw new ParseException("Unknown operation");
        }
    }

    public class GroupParselet : IPrefixParselet
    {
        public double Parse(Parser parser, Token token)
        {
            double expression = parser.ParseExpression();
            parser.Consume(TokenType.RIGHT_PAREN);
            return expression;
        }
    }

    public class NumberParselet : IPrefixParselet
    {
        public double Parse(Parser parser, Token token)
        {
            return double.Parse(token.Text);
        }
    }

    public class PrefixOperatorParselet : IPrefixParselet
    {
        public PrefixOperatorParselet(ParsletPrecedence precedence)
        {
            Precedence = precedence;
        }

        public ParsletPrecedence Precedence { get; }
        public double Parse(Parser parser, Token token)
        {
            double right = parser.ParseExpression(Precedence);
            switch (token.Type)
            {
                case TokenType.MINUS:
                    return -right;
                case TokenType.PLUS:
                    return right;
            }
            throw new ParseException("Unknown operation");
        }
    }
}
