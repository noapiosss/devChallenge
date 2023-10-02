using System;
using System.Linq;
using System.Text;

namespace Domain.Helpers
{
    public static class ExpressionHeler
    {
        public static string PrepareExpression(this string value)
        {
            string expression = value.Replace(" ", "");

            if (expression.Contains("--") ||
                expression.Contains("-+") ||
                expression.Contains("+-") ||
                expression.Contains("++"))
            {
                char p = '1';
                StringBuilder sb = new();

                foreach (char c in expression)
                {
                    if ((p == '+' || p == '-') && (c == '+' || c == '-'))
                    {
                        sb.Remove(sb.Length-1, 1);

                        if (p == c)
                        {
                            sb.Append('+');
                        }
                        if (p != c)
                        {
                            sb.Append('-');
                        }
                        
                        p = sb[^1];
                    }
                    else
                    {
                        sb.Append(c);
                        p = c;
                    }
                }

                expression = sb.ToString();
            }

            if (expression.Contains("()") ||
                expression.Count(c => c == '(') != expression.Count(c => c == ')'))
            {
                throw new InvalidOperationException("Count of ( and ) are different or () present");
            }

            return expression.ToLower();
        }
    }
}