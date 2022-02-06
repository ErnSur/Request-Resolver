using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace QuickEye.ReferenceValidator
{
    /*
    
    looks for a prop with name that ends with "Controller"
    .*Controller$

    looks for prop that has type of Image
    t: Image

    same but value
    v: 10
    v: true
    v: 10f
    v: "some String"
    v: #F6E7E7
    
    Looks for a object reference with a object id "-11001"
    v: *-11001
    
     */
    public class QueryInterpreter
    {
        private List<TokenDefinition> _tokenDefinitions;

        public QueryInterpreter()
        {
            _tokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition(TokenType.TypeStatement, "t:", 1),
                new TokenDefinition(TokenType.ValueStatement, "v:", 1),
                new TokenDefinition(TokenType.ColorLiteral, "#[0-9a-fA-F]{6}", 1),
                new TokenDefinition(TokenType.TextLiteral, "\".*?\"", 1),
                new TokenDefinition(TokenType.NameLiteral, "[a-zA-Z\\.]+", 2),
                new TokenDefinition(TokenType.NumericLiteral, "-?\\d+", 2),
                new TokenDefinition(TokenType.Comma, ",", 1)
            };
        }

        public static IEnumerable<SerializedProperty> GetProps(GameObject go, string query)
        {
            var qi = new QueryInterpreter();

            var currentStatement = TokenType.NotDefined;

            var props = GameObjectAnalyzer.GetProperties(go);
            var selectors = new List<Func<SerializedProperty, bool>>();

            foreach (var token in qi.Tokenize(query))
            {
                switch (token.TokenType)
                {
                    case TokenType.NotDefined:
                        break;
                    case TokenType.TypeStatement:
                        currentStatement = token.TokenType;
                        break;
                    case TokenType.ValueStatement:
                        currentStatement = token.TokenType;
                        break;
                    case TokenType.ColorLiteral when currentStatement == TokenType.ValueStatement:
                        selectors.Add(p => p.propertyType == SerializedPropertyType.Color
                                           && ColorUtility.ToHtmlStringRGB(p.colorValue) == token.Value);
                        break;
                    case TokenType.TextLiteral when currentStatement == TokenType.ValueStatement:
                        selectors.Add(p => p.propertyType == SerializedPropertyType.String
                                           && p.stringValue == token.Value);
                        break;
                    case TokenType.RegexTextLiteral:
                        break;
                    case TokenType.NameLiteral when currentStatement == TokenType.NotDefined:
                        selectors.Add(p => p.name.Contains(token.Value));
                        break;
                    // TODO: Add support for namespaces
                    case TokenType.NameLiteral when currentStatement == TokenType.TypeStatement:
                        selectors.Add(p => p.GetPropertyType().Name.ToLower(CultureInfo.InvariantCulture) == token.Value.ToLower(CultureInfo.InvariantCulture));

                        break;
                    case TokenType.NumericLiteral:
                        break;
                    case TokenType.BooleanLiteral:
                        break;
                    case TokenType.ObjectReferenceLiteral:
                        break;
                    case TokenType.Comma:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var msg = $"{token.TokenType}: {token.Value}";
                Debug.Log($"MES: {msg}");
            }

            return props.Where(p => selectors.Any(s => s(p)));
        }

        public IEnumerable<Token> Tokenize(string errorMessage)
        {
            var tokenMatches = FindTokenMatches(errorMessage);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new Token(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }
        }


        private List<TokenMatch> FindTokenMatches(string errorMessage)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(errorMessage).ToList());

            return tokenMatches;
        }
    }

    public enum TokenType
    {
        NotDefined,
        TypeStatement,
        ValueStatement,
        ColorLiteral,
        TextLiteral,
        RegexTextLiteral,
        NameLiteral,
        NumericLiteral,
        BooleanLiteral,
        ObjectReferenceLiteral,
        Comma
    }

    public class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly int _precedence;

        public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = _returnsToken,
                    Value = matches[i].Value,
                    Precedence = _precedence
                };
            }
        }
    }

    public class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
    }

    public class Token
    {
        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
            if (tokenType == TokenType.TextLiteral)
                Value = Value.Substring(1, Value.Length - 2);
            if (tokenType == TokenType.ColorLiteral)
                Value = Value.Substring(1);
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }

        public Token Clone()
        {
            return new Token(TokenType, Value);
        }
    }
}