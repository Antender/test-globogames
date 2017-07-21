using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class Parser
    {
        private List<Token> Read = new List<Token>();
        private Dictionary<TokenType, IPrefixParselet> PrefixParselets = new Dictionary<TokenType, IPrefixParselet>();
        private Dictionary<TokenType, IInfixParselet> InfixParselets = new Dictionary<TokenType, IInfixParselet>();

        private IEnumerable<Token> tokens;

        public Parser(IEnumerable<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IEnumerable<Token> Tokens { get => tokens; }

        public void Register(TokenType token, IPrefixParselet parselet)
        {
            PrefixParselets.Add(token, parselet);
        }

        public void Register(TokenType token, IInfixParselet parselet)
        {
            InfixParselets.Add(token, parselet);
        }

        public double ParseExpression(ParsletPrecedence precedence)
        {
            Token token = Consume();
            IPrefixParselet prefix = PrefixParselets[token.Type];

            if (prefix == null) throw new ParseException("Could not parse \"" + token.Text + "\".");

            double left = prefix.Parse(this, token);

            while (precedence < GetPrecedence())
            {
                token = Consume();

                IInfixParselet infix = InfixParselets[token.Type];
                left = infix.Parse(this, left, token);
            }

            return left;
        }

        public double ParseExpression()
        {
            return ParseExpression(0);
        }

        public bool Match(TokenType expected)
        {
            Token token = LookAhead(0);
            if (token.Type != expected)
            {
                return false;
            }

            Consume();
            return true;
        }

        public Token Consume(TokenType expected)
        {
            Token token = LookAhead(0);
            if (token.Type != expected)
            {
                throw new Exception("Expected token " + expected + " and found " + token.Type);
            }
            return Consume();
        }

        public Token Consume()
        {
            // Make sure we've read the token.
            LookAhead(0);
            var first = Read[0];
            Read.RemoveAt(0);
            return first;
        }

        private Token LookAhead(int distance)
        {
            // Read in as many as needed.
            Read.AddRange(Tokens.Take(distance - Read.Count + 1));
            tokens = Tokens.Skip(distance - Read.Count + 1);

            // Get the queued token.
            return Read[distance];
        }

        private ParsletPrecedence GetPrecedence()
        {
            var type = LookAhead(0).Type;
            if (InfixParselets.ContainsKey(type))
            {
                return InfixParselets[type].Precedence;
            }
            return 0;
        }
    }

    public class ParseException : Exception
    {
        public ParseException(String message) : base(message)
        {
        }
    }

    public class CalculatorParser : Parser
    {
        public CalculatorParser(Lexer lexer) : base((IEnumerable<Token>)lexer)
        {

            // Register all of the parselets for the grammar.

            // Register the ones that need special parselets.
            Register(TokenType.NUMBER, new NumberParselet());
            Register(TokenType.LEFT_PAREN, new GroupParselet());

            // Register the simple operator parselets.
            Prefix(TokenType.PLUS, ParsletPrecedence.PREFIX);
            Prefix(TokenType.MINUS, ParsletPrecedence.PREFIX);

            InfixLeft(TokenType.PLUS, ParsletPrecedence.SUM);
            InfixLeft(TokenType.MINUS, ParsletPrecedence.SUM);
            InfixLeft(TokenType.ASTERISK, ParsletPrecedence.PRODUCT);
            InfixLeft(TokenType.SLASH, ParsletPrecedence.PRODUCT);
            InfixRight(TokenType.CARET, ParsletPrecedence.EXPONENT);
        }

        /**
         * Registers a prefix unary operator parselet for the given token and
         * precedence.
         */
        public void Prefix(TokenType token, ParsletPrecedence precedence)
        {
            Register(token, new PrefixOperatorParselet(precedence));
        }

        /**
         * Registers a left-associative binary operator parselet for the given token
         * and precedence.
         */
        public void InfixLeft(TokenType token, ParsletPrecedence precedence)
        {
            Register(token, new BinaryOperatorParselet(precedence, false));
        }

        /**
         * Registers a right-associative binary operator parselet for the given token
         * and precedence.
         */
        public void InfixRight(TokenType token, ParsletPrecedence precedence)
        {
            Register(token, new BinaryOperatorParselet(precedence, true));
        }
    }
}
