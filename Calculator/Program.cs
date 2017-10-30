using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


//Калькулятор с возможностью введения новых операций (не динамически) и установки их приоритетов
namespace Calculator
{
    static class MathExpression
    {
        public static string Clean(string expr)
        {
            expr = expr.Replace(" ", string.Empty);

            //Валидация на корректность ввода
            // Regex regex = new Regex(@"(\d+(\,\d+)?\+)+(\d+(\,\d+)?)");
            // MatchCollection matches = regex.Matches(expr);
            //
            // if (matches.Count == 0) { expr = null; }

            return expr;
        }

    }

    delegate double OperationDelegate(double x, double y);

    abstract class Operation
    {
        public string Calculate(string expression, OperationDelegate operationMethod, string symbol)
        {
            Regex regex = new Regex(@"\d+(\,\d+)?\" + symbol + @"\d+(\,\d+)?");
            MatchCollection matches = regex.Matches(expression);


            if (matches.Count > 0)
            {
                string itemExpr = matches[0].Value;
                double operand1 = Convert.ToDouble(itemExpr.Substring(0, itemExpr.IndexOf(symbol)));

                int indexOfAction = itemExpr.IndexOf(symbol) + 1;
                double operand2 = Convert.ToDouble(itemExpr.Substring(indexOfAction, itemExpr.Length - indexOfAction));
                double result = operationMethod(operand1, operand2);

                expression = expression.Replace(matches[0].Value, result.ToString());
                expression = Operate(expression);
            }

            return expression;

        }

        public abstract string Operate(string expression);

    }


    class Plus : Operation
    {
        private double PlusOperation(double x, double y)
        {
            return x + y;
        }

        public override string Operate(string expression)
        {
            return Calculate(expression, PlusOperation, "+");
        }
    }

    class Minus : Operation
    {
        private double MinusOpeartion(double x, double y)
        {
            return x - y;
        }

        public override string Operate(string expression)
        {
            return Calculate(expression, MinusOpeartion, "-");
        }
    }

    class Multiply : Operation
    {
        private double MultiplyOperation(double x, double y)
        {
            return x * y;
        }

        public override string Operate(string expression)
        {
            return Calculate(expression, MultiplyOperation, "*");
        }
    }

    class Divide : Operation
    {
        private double DivideOperation(double x, double y)
        {
            return x / y;
        }

        public override string Operate(string expression)
        {
            return Calculate(expression, DivideOperation, "/");
        }
    }

    class Expression
    {
        string expression;

        public List<Operation> OrderOperationsList { get; set; }

        public Expression(string expression)
        {
            this.expression = expression;
        }

        public double? GetResult()
        {
            // Очищаем от пробелов и валидируем введенное выражение.В текущем варианте удаляются только лишние пробелы
            expression = MathExpression.Clean(expression);

            if (expression == null)
            {
                return null;
            }

            foreach (Operation operation in OrderOperationsList)
            {
                expression = operation.Operate(expression);
            }

            return Convert.ToDouble(expression);
        }

    }


    class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                
                //Пользовательсикй ввод, "5+7-11*7"
                Console.WriteLine("Введите выражение (e - exit)");
                string userInput = Console.ReadLine();
                if (userInput == "e") { break; }

                Expression expression = new Expression(userInput);
                
                //Добавление возможных операций. Порядок операции в этом списке будет соответствовать приоритету операции
                expression.OrderOperationsList = new List<Operation>() { new Multiply(), new Divide(), new Plus(), new Minus() };

                double? result = expression.GetResult();


                Console.WriteLine("Результат: {0}", result == null ? "Некорректное выражение" : result.ToString());
                Console.WriteLine();
            }
        }


    }

}