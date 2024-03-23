using System.Diagnostics;
using System.Linq.Expressions;

namespace Wix.QueryLanguage
{
    public class ExpressionParser
    {
        public Expression<Func<T, bool>> ParseExpression<T>(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Debug.WriteLine($"Parsing expression: {expression}");
            ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");


            Expression body = ParseAnyExpression(expression, parameter);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private Expression ParseAnyExpression(string expression, ParameterExpression parameter)
        {
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

            throw new ArgumentException($"Unknown expression type: {expression}");
        }

        private Expression ParseNotExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[4..^1].Trim(); // Strip "NOT(" and ")"

            try
            {
                Expression operand = ParseAnyExpression(expression, parameter);
                return Expression.Not(operand);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse NOT expression: {expression}. Error: {ex.Message}", ex);
            }
        }

        private Expression ParseOrExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[3..^1].Trim(); // Strip "OR(" and ")"

            try
            {
                int commaIndex = FindCommaIndexNotInsideParenthesis(expression);
                string leftExpression = expression.Substring(0, commaIndex).Trim();
                string rightExpression = expression.Substring(commaIndex + 1).Trim();

                Expression left = ParseAnyExpression(leftExpression, parameter);
                Expression right = ParseAnyExpression(rightExpression, parameter);

                return Expression.OrElse(left, right);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse OR expression: {expression}. Error: {ex.Message}", ex);
            }
        }

        private Expression ParseAndExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[4..^1].Trim(); // Strip "AND(" and ")"

            try
            {
                int commaIndex = FindCommaIndexNotInsideParenthesis(expression);
                string leftExpression = expression.Substring(0, commaIndex).Trim();
                string rightExpression = expression.Substring(commaIndex + 1).Trim();

                Expression left = ParseAnyExpression(leftExpression, parameter);
                Expression right = ParseAnyExpression(rightExpression, parameter);

                return Expression.AndAlso(left, right);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse AND expression: {expression}. Error: {ex.Message}", ex);
            }
        }

        private Expression ParseEqualExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[6..^1].Trim(); // Strip "EQUAL(" and ")"

            try
            {
                string[] parts = expression.Split(',');
                string propertyName = parts[0].Trim().Trim('"');
                string value = parts[1].Trim().Trim('"');

                MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
                object convertedValue = Convert.ChangeType(value, property.Type);
                ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

                return Expression.Equal(property, constant);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse EQUAL expression: {expression}. Error: {ex.Message}", ex);
            }
        }

        private Expression ParseGreaterThanExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[13..^1].Trim(); // Strip "GREATER_THAN(" and ")"

            try
            {
                string[] parts = expression.Split(',');
                string propertyName = parts[0].Trim().Trim('"');
                string value = parts[1].Trim().Trim('"');

                MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
                object convertedValue = Convert.ChangeType(value, property.Type);
                ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

                return Expression.GreaterThan(property, constant);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse GREATER_THAN expression: {expression}. Error: {ex.Message}", ex);
            }
        }

        private Expression ParseLessThanExpression(string expression, ParameterExpression parameter)
        {
            expression = expression[10..^1].Trim(); // Strip "LESS_THAN(" and ")"

            try
            {
                string[] parts = expression.Split(',');
                string propertyName = parts[0].Trim().Trim('"');
                string valueString = parts[1].Trim().Trim('"');

                MemberExpression property = Expression.PropertyOrField(parameter, propertyName);

                object convertedValue = Convert.ChangeType(valueString, property.Type);
                ConstantExpression constant = Expression.Constant(convertedValue, property.Type);

                return Expression.LessThan(property, constant);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse LESS_THAN expression: {expression}. Error: {ex.Message}", ex);
            }
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
