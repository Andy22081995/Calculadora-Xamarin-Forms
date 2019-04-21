namespace Calculator.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class MathExtension
    {
        #region Fields

        #region Markers
        private const string NumberMaker = "#";
        private const string OperatorMarker = "$";
        private const string FunctionMarker = "@";
        #endregion

        #region Internal tokens

        private const string Plus = OperatorMarker + "+";
        private const string UnPlus = OperatorMarker + "un+";
        private const string Minus = OperatorMarker + "-";
        private const string UnMinus = OperatorMarker + "un-";
        private const string Multiply = OperatorMarker + "*";
        private const string Divide = OperatorMarker + "/";
        private const string Degree = OperatorMarker + "^";
        private const string LeftParent = OperatorMarker + "(";
        private const string RightParent = OperatorMarker + ")";
        private const string Sqrt = FunctionMarker + "sqrt";
        private const string Sin = FunctionMarker + "sin";
        private const string Cos = FunctionMarker + "cos";
        private const string Tg = FunctionMarker + "tg";
        private const string Ctg = FunctionMarker + "ctg";
        private const string Sh = FunctionMarker + "sh";
        private const string Ch = FunctionMarker + "ch";
        private const string Th = FunctionMarker + "th";
        private const string Log = FunctionMarker + "log";
        private const string Ln = FunctionMarker + "ln";
        private const string Exp = FunctionMarker + "exp";
        private const string Abs = FunctionMarker + "abs";

        #endregion

        #region Dictionaries
        /// <summary>
        /// Contiene los operadores soportados
        /// </summary>
        private readonly Dictionary<string, string> supportedOperators =
            new Dictionary<string, string>
            {
            { "+", Plus },
            { "-", Minus },
            { "*", Multiply },
            { "/", Divide },
            { "^", Degree },
            { "(", LeftParent },
            { ")", RightParent }
            };

        /// <summary>
        /// Contains supported functions
        /// </summary>
        private readonly Dictionary<string, string> supportedFunctions =
            new Dictionary<string, string>
            {
            { "sqrt", Sqrt },
            { "√", Sqrt },
            { "sin", Sin },
            { "cos", Cos },
            { "tg", Tg },
            { "ctg", Ctg },
            { "sh", Sh },
            { "ch", Ch },
            { "th", Th },
            { "log", Log },
            { "exp", Exp },
            { "abs", Abs }
            };

        private readonly Dictionary<string, string> supportedConstants =
            new Dictionary<string, string>
            {
            {"pi", NumberMaker + Math.PI.ToString() },
            {"e", NumberMaker + Math.E.ToString() }
            };

        #endregion

        #endregion

        private readonly char decimalSeparator;
        private bool isRadians;

        #region Constructors

        /// <summary>
        /// Inicializa una nueva instancia de MathExtension
        /// </summary>
        public MathExtension()
        {
            try
            {
                decimalSeparator = Char.Parse(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Error: No se puede leer el separador de caracteres decimales del sistema, verifique la configuración regional.", ex);
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de MathExtension
        /// </summary>
        /// <param name="decimalSeparator">Añade el separador decimal</param>
        public MathExtension(char decimalSeparator)
        {
            this.decimalSeparator = decimalSeparator;
        }

        #endregion

        /// <summary>
        /// Produce el resultado de la expresión matemática dada
        /// </summary>
        /// <param name="expression">Expresión matemática(infix/standard notation)</param>
        /// <returns>Result</returns>
        public double Parse(string expression, bool isRadians = true)
        {
            this.isRadians = isRadians;

            try
            {
                return Calculate(ConvertToRPN(FormatString(expression)));
            }
            catch (DivideByZeroException e)
            {
                return 0;
                throw e;
            }
            catch (FormatException e)
            {
                return 0;
                throw e;
            }
            catch (InvalidOperationException e)
            {
                return 0;
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                return 0;
                throw e;
            }
            catch (ArgumentException e)
            {
                return 0;
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Produce un string formateado por el string dado
        /// Produce formatted string by the given string
        /// </summary>
        /// <param name="expression">expresión matemática sin formateo</param>
        /// <returns>Expresión matemática formateada</returns>
        private string FormatString(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("La expresión es nula o vacía");
            }

            StringBuilder formattedString = new StringBuilder();
            int balanceOfParenth = 0; // Revisa la cantidad de paréntesis

            // Formato de cadena en una iteración y verifica el número de paréntesis
            // Esta función hace 2 tareas por la prioridad de rendimiento
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];

                if (ch == '(')
                {
                    balanceOfParenth++;
                }
                else if (ch == ')')
                {
                    balanceOfParenth--;
                }

                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (Char.IsUpper(ch))
                {
                    formattedString.Append(Char.ToLower(ch));
                }
                else
                {
                    formattedString.Append(ch);
                }
            }

            if (balanceOfParenth != 0)
            {
                throw new FormatException("Número de paréntesis abiertos y cerrados no es igual");
            }

            return formattedString.ToString();
        }

        #region Convert to Reverse-Polish Notation

        /// <summary>
        /// Produce expresiones matemáticas en notación polaca inversa por la cadena dada
        /// </summary>
        /// <param name="expression">Expresión matemática en notación infix</param>
        /// <returns>Expresión matemática en notación postfix(RPN)</returns>
        private string ConvertToRPN(string expression)
        {
            int pos = 0; //Posición actual del analizador lexico
            StringBuilder outputString = new StringBuilder();
            Stack<string> stack = new Stack<string>();

            //Mientras existar un char sin manejar en la expresión
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisInfixNotation(expression, ref pos);

                outputString = SyntaxAnalysisInfixNotation(token, outputString, stack);
            }

            // Pop todos los elementos de la pila a la cadena de salida           
            while (stack.Count > 0)
            {
                // There should be only operators
                if (stack.Peek()[0] == OperatorMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
                else
                {
                    throw new FormatException("Format exception,"
                    + "Hay función sin paréntesis.");
                }
            }

            return outputString.ToString();
        }

        /// <summary>
        /// Produce un token por la expresión matemática dada
        /// </summary>
        /// <param name="expression">Expresión matemática en notación infix</param>
        /// <param name="pos">Posición actual del analizador sintáctico</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisInfixNotation(string expression, ref int pos)
        {
            // Recibe el primer char
            StringBuilder token = new StringBuilder();
            token.Append(expression[pos]);

            // Si es un operador
            if (supportedOperators.ContainsKey(token.ToString()))
            {
                // Determina si es un operador binario o único
                bool isUnary = pos == 0 || expression[pos - 1] == '(';
                pos++;

                switch (token.ToString())
                {
                    case "+":
                        return isUnary ? UnPlus : Plus;
                    case "-":
                        return isUnary ? UnMinus : Minus;
                    default:
                        return supportedOperators[token.ToString()];
                }
            }
            else if (Char.IsLetter(token[0])
                || supportedFunctions.ContainsKey(token.ToString())
                || supportedConstants.ContainsKey(token.ToString()))
            {
                // Lee el nombre de la función o constante

                while (++pos < expression.Length
                    && Char.IsLetter(expression[pos]))
                {
                    token.Append(expression[pos]);
                }

                if (supportedFunctions.ContainsKey(token.ToString()))
                {
                    return supportedFunctions[token.ToString()];
                }
                else if (supportedConstants.ContainsKey(token.ToString()))
                {
                    return supportedConstants[token.ToString()];
                }
                else
                {
                    throw new ArgumentException("Token desconocido");
                }

            }
            else if (Char.IsDigit(token[0]) || token[0] == decimalSeparator)
            {
                // Lee el número

                // Lee completamente la parte del número
                if (Char.IsDigit(token[0]))
                {
                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }
                else
                {
                    // Separador decimal será agregado luego
                    token.Clear();
                }

                // Lee el valor fraccional de un número
                if (pos < expression.Length
                    && expression[pos] == decimalSeparator)
                {
                    // Añade el decimal
                    token.Append(CultureInfo.CurrentCulture
                        .NumberFormat.NumberDecimalSeparator);

                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }

                // Lee la notación cientifica (suffix)
                if (pos + 1 < expression.Length && expression[pos] == 'e'
                    && (Char.IsDigit(expression[pos + 1])
                        || (pos + 2 < expression.Length
                            && (expression[pos + 1] == '+'
                                || expression[pos + 1] == '-')
                            && Char.IsDigit(expression[pos + 2]))))
                {
                    token.Append(expression[pos++]); // expresión

                    if (expression[pos] == '+' || expression[pos] == '-')
                        token.Append(expression[pos++]); // signo

                    while (pos < expression.Length
                        && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos++]); // poder
                    }

                    // Convierte el número de notación cientifica a notación decimal
                    return NumberMaker + Convert.ToDouble(token.ToString());
                }

                return NumberMaker + token.ToString();
            }
            else
            {
                throw new ArgumentException("Token desconocido en la expresión");
            }
        }

        /// <summary>
        /// Analiza la notación infix de la sintaxis
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="outputString">Output string (Expresión matemática en RPN)</param>
        /// <param name="stack">Pila que contiene los operadores o funciones</param>
        /// <returns>Expresión matemática en RPN</returns>
        private StringBuilder SyntaxAnalysisInfixNotation(string token, StringBuilder outputString, Stack<string> stack)
        {
            // Si es un número, solo lo coloca en un string       
            if (token[0] == NumberMaker[0])
            {
                outputString.Append(token);
            }
            else if (token[0] == FunctionMarker[0])
            {
                //si es una función, hace push a la pila
                stack.Push(token);
            }
            else if (token == LeftParent)
            {
                // Si es un parentesis abierto hace push a la pila
                stack.Push(token);
            }
            else if (token == RightParent)
            {
                //Hace pop en la pila cuando encuentre el cierre de parentesis

                string elem;
                while ((elem = stack.Pop()) != LeftParent)
                {
                    outputString.Append(elem);
                }

                // Si hay una función en el principio de la pila, la pone en el string
                if (stack.Count > 0 &&
                    stack.Peek()[0] == FunctionMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
            }
            else
            {
                while (stack.Count > 0 &&
                    Priority(token, stack.Peek()))
                {
                    outputString.Append(stack.Pop());
                }

                stack.Push(token);
            }

            return outputString;
        }

        /// <summary>
        /// Es la prioridad del token menor (o igual) que la prioridad de p
        /// </summary>
        private bool Priority(string token, string p)
        {
            return IsRightAssociated(token) ?
                GetPriority(token) < GetPriority(p) :
                GetPriority(token) <= GetPriority(p);
        }

        /// <summary>
        /// Tiene derecho el operador asociado
        /// </summary>
        private bool IsRightAssociated(string token)
        {
            return token == Degree;
        }

        /// <summary>
        /// Obtiene prioridad de operador
        /// </summary>
        private int GetPriority(string token)
        {
            switch (token)
            {
                case LeftParent:
                    return 0;
                case Plus:
                case Minus:
                    return 2;
                case UnPlus:
                case UnMinus:
                    return 6;
                case Multiply:
                case Divide:
                    return 4;
                case Degree:
                case Sqrt:
                    return 8;
                case Sin:
                case Cos:
                case Tg:
                case Ctg:
                case Sh:
                case Ch:
                case Th:
                case Log:
                case Ln:
                case Exp:
                case Abs:
                    return 10;
                default:
                    throw new ArgumentException("Operador desconocido");
            }
        }

        #endregion

        #region Calculate expression in RPN

        /// <summary>
        /// Calcula la expresión en RPN
        /// </summary>
        /// <param name="expression">Expresión matemática en RPN</param>
        /// <returns>Resultado</returns>
        private double Calculate(string expression)
        {
            int pos = 0; // Posición actual del analizador sintactico
            var stack = new Stack<double>(); // contiene operadores

            // Analiza completamente la expresión
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisRPN(expression, ref pos);

                stack = SyntaxAnalysisRPN(stack, token);
            }

            //Al finalizar debe haber solo un operador (resultado)
            if (stack.Count > 1)
            {
                throw new ArgumentException("Exceso de operadores");
            }

            return stack.Pop();
        }

        /// <summary>
        /// Produce un Token por la expresión matemática dada
        /// </summary>
        /// <param name="expression">expresión matemática en RPN</param>
        /// <param name="pos">Posición actual del analizador sintactico</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisRPN(string expression, ref int pos)
        {
            StringBuilder token = new StringBuilder();

            // Lee el token de marcador en marcador

            token.Append(expression[pos++]);

            while (pos < expression.Length && expression[pos] != NumberMaker[0]
                && expression[pos] != OperatorMarker[0]
                && expression[pos] != FunctionMarker[0])
            {
                token.Append(expression[pos++]);
            }

            return token.ToString();
        }

        /// <summary>
        /// Analiza el valor sintactico del RPN
        /// </summary>
        /// <param name="stack">Pila que contiene los operadores</param>
        /// <param name="token">Token</param>
        /// <returns>Pila que contiene los operadores</returns>
        private Stack<double> SyntaxAnalysisRPN(Stack<double> stack, string token)
        {
            // Si es operador
            if (token[0] == NumberMaker[0])
            {
                stack.Push(double.Parse(token.Remove(0, 1)));
            }
            // De lo contrario, aplicar el operador o la función a los elementos en la pila.
            else if (NumberOfArguments(token) == 1)
            {
                double arg = stack.Pop();
                double rst;

                switch (token)
                {
                    case UnPlus:
                        rst = arg;
                        break;
                    case UnMinus:
                        rst = -arg;
                        break;
                    case Sqrt:
                        rst = Math.Sqrt(arg);
                        break;
                    case Sin:
                        rst = ApplyTrigFunction(Math.Sin, arg);
                        break;
                    case Cos:
                        rst = ApplyTrigFunction(Math.Cos, arg);
                        break;
                    case Tg:
                        rst = ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case Ctg:
                        rst = 1 / ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case Sh:
                        rst = System.Math.Sinh(arg);
                        break;
                    case Ch:
                        rst = rst = Math.Cosh(arg);
                        break;
                    case Th:
                        rst = Math.Tanh(arg);
                        break;
                    case Ln:
                        rst = Math.Log(arg);
                        break;
                    case Exp:
                        rst = Math.Exp(arg);
                        break;
                    case Abs:
                        rst = Math.Abs(arg);
                        break;
                    default:
                        throw new ArgumentException("Operador desconocido");
                }

                stack.Push(rst);
            }
            else
            {
                // De lo contrario el número de argumentos del operador es igual a 2

                double arg2 = stack.Pop();
                double arg1 = stack.Pop();

                double rst;

                switch (token)
                {
                    case Plus:
                        rst = arg1 + arg2;
                        break;
                    case Minus:
                        rst = arg1 - arg2;
                        break;
                    case Multiply:
                        rst = arg1 * arg2;
                        break;
                    case Divide:
                        if (arg2 == 0)
                        {
                            throw new DivideByZeroException("El segundo argumento es 0");
                        }
                        rst = arg1 / arg2;
                        break;
                    case Degree:
                        rst = Math.Pow(arg1, arg2);
                        break;
                    case Log:
                        rst = Math.Log(arg2, arg1);
                        break;
                    default:
                        throw new ArgumentException("Operador desconocido");
                }

                stack.Push(rst);
            }

            return stack;
        }

        /// <summary>
        /// Aplica funciones trigonometricas
        /// </summary>
        /// <param name="func">Función trigonometrica</param>
        /// <param name="arg">Argumento</param>
        /// <returns>Resultado de la función</returns>
        private double ApplyTrigFunction(Func<double, double> func, double arg)
        {
            if (!isRadians)
            {
                arg = arg * System.Math.PI / 180; // Converte el valor a grados
            }

            return func(arg);
        }

        /// <summary>
        /// Produce el numero de argumentos dados por el operador dado
        /// </summary>
        private int NumberOfArguments(string token)
        {
            switch (token)
            {
                case UnPlus:
                case UnMinus:
                case Sqrt:
                case Tg:
                case Sh:
                case Ch:
                case Th:
                case Ln:
                case Ctg:
                case Sin:
                case Cos:
                case Exp:
                case Abs:
                    return 1;
                case Plus:
                case Minus:
                case Multiply:
                case Divide:
                case Degree:
                case Log:
                    return 2;
                default:
                    throw new ArgumentException("Operador desconocido");
            }
        }

        #endregion
    }
}
