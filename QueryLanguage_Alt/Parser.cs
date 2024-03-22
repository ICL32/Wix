using System.Diagnostics;
using System.Linq.Expressions;
using Wix_Technical_Test.Models;
using Wix_Technical_Test.QueryLanguage.Parser;

namespace Wix_Technical_Test.QueryLanguage_Alt
{
    public class ExpressionParser
    {
        // Entry point for parsing
        public Expression<Func<T, bool>> ParseExpression<T>(string expression)
        {
            Debug.WriteLine($"Parsing expression: {expression}");
            ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");

            Expression body = ParseAnyExpression(expression, parameter);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private Expression ParseAnyExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing any expression: {expression}");

            // Trim any outer whitespace and remove outermost parentheses if they are at the start and end
            expression = expression.Trim();
            if (expression.StartsWith("(") && expression.EndsWith(")"))
            {
                expression = expression[1..^1].Trim();
            }

            if (expression.StartsWith("AND"))
            {
                return ParseAndExpression(expression, parameter);
            }
            else if (expression.StartsWith("EQUAL"))
            {
                return ParseEqualExpression(expression, parameter);
            }
            else if (expression.StartsWith("GREATER_THAN"))
            {
                return ParseGreaterThanExpression(expression, parameter);
            }
            else if (expression.StartsWith("LESS_THAN"))
            {
                return ParseLessThanExpression(expression, parameter);
            }
            else if (expression.StartsWith("OR"))
            {
                return ParseOrExpression(expression, parameter);
            }
            else if (expression.StartsWith("NOT"))
            {
                return ParseNotExpression(expression, parameter);
            }

            throw new NotImplementedException($"Unsupported expression: {expression}");
        }

        private Expression ParseNotExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing NOT expression: {expression}");
            // Remove the "NOT(" at the start and the ")" at the end
            expression = expression[4..^1]; // Strip "NOT(" and ")"

            // There is only one expression to negate
            string innerExpression = expression.Trim();

            Expression operand = ParseAnyExpression(innerExpression, parameter);

            return Expression.Not(operand);
        }

        private Expression ParseOrExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing OR expression: {expression}");
            // Remove the "OR(" at the start and the ")" at the end
            expression = expression[3..^1]; // Strip "OR(" and ")"

            int commaIndex = FindCommaIndexNotInsideParenthesis(expression);
            string leftExpression = expression.Substring(0, commaIndex).Trim();
            string rightExpression = expression.Substring(commaIndex + 1).Trim();

            Expression left = ParseAnyExpression(leftExpression, parameter);
            Expression right = ParseAnyExpression(rightExpression, parameter);

            return Expression.OrElse(left, right);
        }

        private Expression ParseAndExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing AND expression: {expression}");
            expression = expression[4..^1]; // Strip "AND(" and ")"

            int commaIndex = FindCommaIndexNotInsideParenthesis(expression);
            string leftExpression = expression.Substring(0, commaIndex).Trim();
            string rightExpression = expression.Substring(commaIndex + 1).Trim();

            Expression left = ParseAnyExpression(leftExpression, parameter);
            Expression right = ParseAnyExpression(rightExpression, parameter);

            return Expression.AndAlso(left, right);
        }

        private Expression ParseEqualExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing EQUAL expression: {expression}");
            expression = expression[6..^1]; // Strip "EQUAL(" and ")"

            string[] parts = expression.Split(',');
            string propertyName = parts[0].Trim().Trim('"');
            string value = parts[1].Trim().Trim('"');

            MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
            object convertedValue = Convert.ChangeType(value, property.Type);
            ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

            return Expression.Equal(property, constant);
        }

        private Expression ParseGreaterThanExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing GREATER_THAN expression: {expression}");
            expression = expression[13..^1]; // Strip "GREATER_THAN(" and ")"

            string[] parts = expression.Split(',');
            string propertyName = parts[0].Trim().Trim('"');
            string value = parts[1].Trim().Trim('"');

            MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
            object convertedValue = Convert.ChangeType(value, property.Type);
            ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

            return Expression.GreaterThan(property, constant);
        }

        private Expression ParseLessThanExpression(string expression, ParameterExpression parameter)
        {
            Debug.WriteLine($"Parsing LESS_THAN expression: {expression}");
            expression = expression[10..^1]; // Strip "LESS_THAN(" and ")"

            string[] parts = expression.Split(',');
            Debug.WriteLine($"Property part: {parts[0]}");
            Debug.WriteLine($"Value part: {parts[1]}");

            string propertyName = parts[0].Trim().Trim('"');
            string valueString = parts[1].Trim().Trim('"');
            Debug.WriteLine($"Property Name: {propertyName}");
            Debug.WriteLine($"Value as string: {valueString}");

            MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
            Debug.WriteLine($"Property Type: {property.Type}");

            object convertedValue;
            try
            {
                convertedValue = Convert.ChangeType(valueString, property.Type);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Conversion failed: {ex.Message}");
                throw;
            }

            Debug.WriteLine($"Converted Value: {convertedValue}");
            ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

            return Expression.LessThan(property, constant);
        }

        private int FindCommaIndexNotInsideParenthesis(string expression)
        {
            int parenthesisCount = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(') parenthesisCount++;
                if (expression[i] == ')') parenthesisCount--;
                if (expression[i] == ',' && parenthesisCount == 0) return i;
            }
            throw new ArgumentException("Could not find the comma separating the expressions.");
        }
    }
}
