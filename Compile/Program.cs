using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Compile
{
    class Program
    {
        public static Tokens[] tokensArray = new Tokens[100];
        public static Tokens Token;
        public static int tokenCount = 0;
        public static Variables[] varTable = new Variables[100];
        public static int varCount = 0;
        public static string codigochilo = "";
        public static int codigoByteCount = 0;
        public static int LCount = 0;
        public static string JumpHelper = "";
        public static int directionHelper = 0;
        public static string lastDirection = "";
        public static int StringLength = 0;
        public static int sc = 0;
        public static string directionSize = "0000";
        public static int[] JumpList = new int[50];
        static void Main(string[] args)
        {
            string line = "{int a a=1 while(a<12){a=a+1 print(a) printl}}";
            Console.WriteLine("Evaluate: " + line);
            evaluate(line);
            Programa();
            CreateFile();
            RunVM();
            Console.WriteLine("LCOUNT" + LCount);
            Console.ReadLine();
        }

        private static void CreateFile()
        {
            File.WriteAllBytes("vmcode.Chop", StringToByteArray(codigochilo));
        }
        private static void RunVM()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"Funcionaplox.exe";
            startInfo.Arguments = @"vmcode.Chop";
            Process.Start(startInfo);
        }
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
        }
        public static void evaluate(string expression)
        {
            //variables
            int count = 0;
            string word = "";
            bool breakFlag = false;
            char[] charArray = expression.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                //Console.WriteLine("What im reading: " + charArray[i]);
                if (charArray[i] == ' ' || charArray[i] == ';')
                    continue;
                if (Char.IsLetter(charArray[i]))
                {
                    //Words
                    tokensArray[count] = new Tokens(10, "");
                    while (i < charArray.Length && breakFlag == false)
                    {
                        if (charArray[i] == ' ' || charArray[i] == ';' || (Char.IsNumber(charArray[i]) == false && Char.IsLetter(charArray[i]) == false))
                        {
                            breakFlag = true;
                            i--;
                        }
                        else
                        {
                            word += charArray[i];
                        }
                        i++;
                    }
                    i--;
                    breakFlag = false;
                    tokensArray[count].index = checkWord(word);
                    if (tokensArray[count].index == 50)
                        tokensArray[count].valor = word;
                    count++;
                    Console.WriteLine("Word:" + word);
                    word = "";
                }


                //Console.WriteLine("Count:" + count);

                //Numbers
                else if (Char.IsNumber(charArray[i]))
                {
                    tokensArray[count] = new Tokens(37, "");
                    while (i < charArray.Length && breakFlag == false)
                    {
                        //if (charArray[i] == ' ' || charArray[i] == ';')
                        if (charArray[i] == ' ' || charArray[i] == ';' || (Char.IsNumber(charArray[i]) == false && charArray[i] != '.'))
                        {
                            breakFlag = true;
                            i--;
                        }
                        else
                        {
                            word += charArray[i];
                        }
                        i++;
                    }
                    i--;
                    breakFlag = false;
                    //tokensArray[count].index = checkNumber(word);
                    //tokensArray[count].index = 37;
                    tokensArray[count].valor = word;
                    count++;
                    Console.WriteLine("Number:" + word);
                    word = "";
                }

                //Operadores
                else if (charArray[i] == '+')
                {
                    tokensArray[count] = new Tokens(13, "");
                    count++;
                }

                else if (charArray[i] == '-')
                {
                    tokensArray[count] = new Tokens(14, "");
                    count++;
                }

                else if (charArray[i] == '*')
                {
                    tokensArray[count] = new Tokens(15, "");
                    count++;
                }

                else if (charArray[i] == '/')
                {
                    tokensArray[count] = new Tokens(16, "");
                    count++;
                }

                //Simbolos
                else if (charArray[i] == '(')
                {
                    tokensArray[count] = new Tokens(11, "");
                    count++;
                }
                else if (charArray[i] == ')')
                {
                    tokensArray[count] = new Tokens(12, "");
                    count++;
                }
                else if (charArray[i] == '{')
                {
                    tokensArray[count] = new Tokens(23, "");
                    count++;
                }
                else if (charArray[i] == '}')
                {
                    tokensArray[count] = new Tokens(24, "");
                    count++;
                }
                else if (charArray[i] == '[')
                {
                    tokensArray[count] = new Tokens(25, "");
                    count++;
                }
                else if (charArray[i] == ']')
                {
                    tokensArray[count] = new Tokens(26, "");
                    count++;
                }
                else if (charArray[i] == ';')
                {
                    tokensArray[count] = new Tokens(28, "");
                    count++;
                }
                else if (charArray[i] == ',')
                {
                    tokensArray[count] = new Tokens(30, "");
                    count++;
                }
                else if (charArray[i] == '"') //guardar un string
                {
                    i++;
                    bool found = false;
                    tokensArray[count] = new Tokens(39, "");
                    while (found == false && i < charArray.Length)
                    {
                        if (charArray[i] == '"')
                        {
                            found = true;
                            tokensArray[count].valor = word;

                            count++;
                            //i++;
                        }
                        else
                        {
                            word += charArray[i] + "";
                            i++;
                        }
                    }
                    word = "";

                }
                else if (charArray[i] == '\'') //guardar un char
                {
                    if (i + 2 < charArray.Length)
                    {
                        if (charArray[i + 2] == '\'')
                        {
                            i++;
                            tokensArray[count] = new Tokens(40, "");
                            tokensArray[count].valor = charArray[i] + "";
                            i = i + 1;
                            count++;
                        }
                        else
                        {
                            Console.WriteLine("Error. Char not properly written.");
                            i++;
                        }
                    }
                    //tokensArray[count] = new Tokens(34, "");

                }
                else if (charArray[i] == '.')
                {
                    tokensArray[count] = new Tokens(36, "");
                    count++;
                }
                else if (charArray[i] == '&')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '&')
                        {
                            tokensArray[count] = new Tokens(31, "");
                            count++;
                            i++;
                        }
                        else
                        {
                            tokensArray[count] = new Tokens(51, "");
                            count++;
                        }

                    }
                    else
                    {
                        tokensArray[count] = new Tokens(51, "");
                        count++;
                    }
                }
                else if (charArray[i] == '|')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '|')
                        {
                            tokensArray[count] = new Tokens(32, "");
                            count++;
                            i++;
                        }
                        else
                        {
                            tokensArray[count] = new Tokens(51, "");
                            count++;
                        }
                    }
                    else
                    {
                        tokensArray[count] = new Tokens(51, "");
                        count++;
                    }
                }
                //Comparadores
                else if (charArray[i] == '<')
                {
                    if (charArray[i + 1] == '=')
                    {
                        tokensArray[count] = new Tokens(18, "");
                        count++;
                        i++;
                    }
                    else
                    {
                        tokensArray[count] = new Tokens(17, "");
                        count++;
                    }
                }

                else if (charArray[i] == '>')
                {
                    if (charArray[i + 1] == '=')
                    {
                        tokensArray[count] = new Tokens(20, "");
                        i++;
                    }
                    else
                        tokensArray[count] = new Tokens(19, "");
                    count++;
                }
                else if (charArray[i] == '!')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '=')
                        {
                            tokensArray[count] = new Tokens(22, "");
                            i++;
                        }
                        else
                            tokensArray[count] = new Tokens(51, "");
                    }
                    else
                        tokensArray[count] = new Tokens(51, "");
                    count++;
                }
                else if (charArray[i] == '=')
                {
                    //Console.WriteLine(" i + 1 <= charArray.Length " + (i + 1 <= charArray.Length));
                    if (i + 1 <= charArray.Length)
                    {
                        //Console.WriteLine("charArray[i + 1]" + (charArray[i + 1] == '='));
                        if (charArray[i + 1] == '=')
                        {
                            tokensArray[count] = new Tokens(21, "");
                            i++;
                        }
                        else
                            tokensArray[count] = new Tokens(27, "");
                    }
                    else
                        tokensArray[count] = new Tokens(27, "");
                    count++;
                }
                else
                {
                    tokensArray[count] = new Tokens(51, "");
                    count++;
                }
            }
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(tokensArray[i].index + ", " + tokensArray[i].valor);


            }

        }
        public static int checkWord(string word)
        {
            bool errorFlag = false;

            foreach (char c in word)
            {
                if (Char.IsLetter(c) == false || Char.IsNumber(c))
                    errorFlag = true;
            }
            if (errorFlag == true)
            {
                return 51;
            }
            else if (word == "char")
                return 0;
            else if (word == "string")
                return 1;
            else if (word == "int")
                return 2;
            else if (word == "float")
                return 3;
            else if (word == "double")
                return 4;
            else if (word == "if")
                return 5;
            else if (word == "else")
                return 6;
            else if (word == "for")
                return 7;
            else if (word == "while")
                return 8;
            else if (word == "print")
                return 9;
            else if (word == "read")
                return 10;
            else if (word == "printl")
                return 41;
            else
                return 50;
        }
        public static int checkNumber(string word)
        {
            if (IsDigitsOnly(word))
                return 37;
            else
                return 51;
        }
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        public static void Programa()
        {

            //codigochilo += "\n"; //testing purposes, must die eventually
            codigoByteCount = 15;
            Token = tokensArray[tokenCount];
            Bloque();
            Console.WriteLine("HALT");
            sc++;
            codigochilo += "00";

            Console.WriteLine("Last Direction: " + directionHelper + "hex: " + directionHelper.ToString("X4"));
            Console.WriteLine("SC: " + sc + "hex: " + sc.ToString("X4"));
            codigochilo = "2843294348554E4B554E" + directionSize + "" + sc.ToString("X4") + "" + codigochilo;

            for (int i = 0; i <= LCount; i++)
            {
                codigochilo = codigochilo.Replace("replace" + i + "replace", JumpList[i].ToString("X4"));
            }


            Console.WriteLine(codigochilo);
        }
        public static void Bloque()
        {
            Match("OB");
            while (isStatement())
            {
                Statement();
            }
            Match("CB");
        }
        public static void Statement()
        {
            if (isDeclaration())
            {
                DoDeclaration();
            }
            else if (Token.index == 50) // if token is nombre
            {
                DoAssignment();
            }
            else if (isInstruction()) // if token is print
            {
                DoInstruction();
            }
        }

        public static void DoDeclaration()
        {
            switch (Token.index)
            {
                case 0: //char
                    //char
                    nextToken();

                    if (Token.index == 50)
                    {
                        varTable[varCount] = new Variables();
                        Console.WriteLine("DEFC " + Token.valor);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "char";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                            Console.WriteLine("Error. Number expected in [ ].");
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                            Console.WriteLine("Error. Variable expected.");
                        Console.WriteLine("DEFAC " + Token.valor + ", " + number);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "charArray";
                        varTable[varCount].isArray = true;
                        varCount++;
                    }
                    break;
                case 1://string
                    nextToken();
                    if (Token.index == 50)
                    {
                        varTable[varCount] = new Variables();
                        Console.WriteLine("DEFS " + Token.valor + ", 50");
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "string";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                            Console.WriteLine("Error. Number expected in [ ].");
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                            Console.WriteLine("Error. Variable expected.");
                        Console.WriteLine("DEFAS " + Token.valor + ", 50, " + number);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "stringArray";
                        varTable[varCount].isArray = true;
                        varCount++;
                    }
                    break;
                case 2: //int
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFI " + Token.valor);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "int";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        nextToken();
                        if (Token.index != 37)
                            Console.WriteLine("Error. Number expected in [ ].");
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                            Console.WriteLine("Error. Variable expected.");
                        Console.WriteLine("DEFAI " + Token.valor + ", " + number);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "intArray";
                        varTable[varCount].isArray = true;
                        varCount++;
                    }
                    break;
                case 3: //float
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFF " + Token.valor);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "float";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                            Console.WriteLine("Error. Number expected in [ ].");
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                            Console.WriteLine("Error. Variable expected.");
                        Console.WriteLine("DEFAF " + Token.valor + ", " + number);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "floatArray";
                        varTable[varCount].isArray = true;
                        varCount++;
                    }
                    break;
                case 4: //double
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFD " + Token.valor);
                        sc++;
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "double";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                            Console.WriteLine("Error. Number expected in [ ].");
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                            Console.WriteLine("Error. Variable expected.");
                        Console.WriteLine("DEFAD " + Token.valor + ", " + number);
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "doubleArray";
                        varTable[varCount].isArray = true;
                        varCount++;
                    }
                    break;

                default:
                    //wat
                    Console.WriteLine("Error. Tipo no reconocido.");
                    break;
            }
            nextToken();
        }
        public static void DoAssignment()
        {
            if (Token.index != 50)
                Console.WriteLine("Error. No es una variable.");
            String nombre = Token.valor;
            nextToken();
            Match("Eq");
            if (Token.index == 37 || Token.index == 50 || Token.index == 11)
            {
                BoolExpretion();
                Console.WriteLine("POPI " + nombre);
                sc = sc + 3;
                lastDirection = GetDirection(nombre);
                if (lastDirection == "-1")
                {
                    assignDirection(nombre);
                }
                codigochilo += "1C" + lastDirection;
                //codigochilo += "\n"; //testing purposes, must die eventually
            }
            else if (Token.index == 40)
            {
                Console.WriteLine("PUSHKC " + Token.valor);
                sc = sc + 2;
                codigochilo += "16" + ConvertStringtoHexa(Token.valor);
                Console.WriteLine("POPC " + nombre);
                sc = sc + 3;
                lastDirection = GetDirection(nombre);
                if (lastDirection == "-1")
                {
                    assignDirection(nombre);
                }
                codigochilo += "1B" + lastDirection;
                nextToken();
            }
            else if (Token.index == 39)
            {
                Console.WriteLine("PUSHKS " + Token.valor);
                sc++; // pending
                codigochilo += "1A50" + ConvertStringtoHexa(Token.valor).PadLeft(Token.valor.Length * 2, '0');
                sc++;
                StringLength = Token.valor.Length;
                Console.WriteLine("POPS" + nombre);
                sc = sc + 3;
                lastDirection = GetDirection(nombre);
                if (lastDirection == "-1")
                {
                    assignDirection(nombre);
                }
                codigochilo += "1F" + lastDirection;
                nextToken();
            }
        }
        private static void DoInstruction()
        {
            if (Token.index == 5) //ifp
            {
                nextToken();
                Match("OP");
                BoolExpretion();
                Match("CP");
                Console.WriteLine(JumpHelper + " L" + LCount); //JUMPNE L0
                JumpWriteCode(JumpHelper);
                codigochilo += "replace" + LCount + "replace";
                sc = sc + 3;
                Bloque();
                Console.WriteLine("L" + LCount + ":");
                JumpList[LCount] = sc;
                LCount++;

            }
            else if (Token.index == 8) //while
            {
                nextToken();
                Console.WriteLine("L" + LCount + ":");
                JumpList[LCount] = sc;
                Match("OP");
                BoolExpretion();
                Match("CP");
                Console.WriteLine(JumpHelper + " L" + (LCount + 1));
                JumpWriteCode(JumpHelper);
                codigochilo += "replace" + (LCount + 1) + "replace";
                sc = sc + 3;
                Bloque();
                Console.WriteLine("JMP L" + LCount);
                codigochilo += "30" + "replace" + LCount + "replace";
                sc = sc + 3;
                Console.WriteLine("L" + (LCount + 1) + ":");
                JumpList[LCount + 1] = sc;
                LCount++;

            }
            else if (Token.index == 9) //print
            {
                nextToken();
                Match("OP");
                PreparePrint();
                Match("CP");
                //Console.WriteLine("prtcr");
            }
            else if (Token.index == 41) //printl
            {
                Console.WriteLine("PRTCR");
                sc++;
                codigochilo += "01";
                //codigochilo += "\n"; //testing purposes, must die eventually
                nextToken();
            }
        }
        private static void PreparePrint() // aqui buscamos que tipo de print se hace, falta la var table
        {
            switch (CheckVarTable())
            {
                case "char": // char
                    Console.WriteLine("PRTC " + Token.valor);
                    sc = sc + 3;
                    codigochilo += "02" + GetDirection(Token.valor);
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                    break;
                case "charArray":
                    Console.WriteLine("PRTAC " + Token.valor);
                    sc = sc + 3;
                    nextToken();
                    break;
                case "string": //String
                    Console.WriteLine("PRTS " + Token.valor);
                    sc = sc + 3;
                    codigochilo += "06" + GetDirection(Token.valor);
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                    break;
                case "stringArray":
                    Console.WriteLine("PRTAS " + Token.valor);
                    sc = sc + 3;
                    nextToken();
                    break;
                case "int": //int
                    Console.WriteLine("PRTI " + Token.valor);
                    sc = sc + 3;
                    codigochilo += "03" + GetDirection(Token.valor);
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                    break;
                case "intArray":
                    Console.WriteLine("PRTAI " + Token.valor);
                    sc = sc + 3;
                    nextToken();
                    break;
                case "float": //float
                    Console.WriteLine("PRTF " + Token.valor);
                    sc = sc + 3;
                    codigochilo += "04" + GetDirection(Token.valor);
                    nextToken();
                    break;
                case "floatArray":
                    Console.WriteLine("PRTAF " + Token.valor);
                    sc = sc + 3;
                    nextToken();
                    break;
                case "double": //double
                    Console.WriteLine("PRTD " + Token.valor);
                    sc = sc + 3;
                    codigochilo += "05" + GetDirection(Token.valor);
                    nextToken();
                    break;
                case "doubleArray":
                    Console.WriteLine("PRTAD " + Token.valor);
                    sc = sc + 3;
                    nextToken();
                    break;
                default:
                    Console.WriteLine("Error. Variable no reconocida.");
                    nextToken();
                    break;
            }
        }
        public static void BoolExpretion()
        {
            Comparison();
            while (Token.index == 31 || Token.index == 32)
            {
                if (Token.index == 31)
                {
                    //pending, no idea what happens here
                }
                else if (Token.index == 32)
                {
                    //pending, no idea what happens here
                }
                else
                {
                    Console.WriteLine("Error. This shouldn't happen.");
                }
            }
        }
        public static void Comparison()
        {
            Expretion();
            while (Token.index == 17 || //<
                Token.index == 18 || //<=
                Token.index == 19 || //>
                Token.index == 20 || //>=
                Token.index == 21 || //==
                Token.index == 22) // !=
            {

                if (Token.index == 17)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    //Console.WriteLine("JMPLT " ); 
                    JumpHelper = "JMPGE"; //opuesto a la comparacion realizada
                }
                if (Token.index == 18)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPLE ");
                    JumpHelper = "JMPGT";
                }
                if (Token.index == 19)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPGT ");
                    JumpHelper = "JMPLE";

                }
                if (Token.index == 20)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPGE ");
                    JumpHelper = "JMMPLT";
                }
                if (Token.index == 21)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPEQ ");
                    JumpHelper = "JMPNE";
                }
                if (Token.index == 22)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPNE "); 
                    JumpHelper = "JMPEQ";
                }

            }
        }
        public static void Expretion()
        {
            Factor();
            while (Token.index == 13 || Token.index == 14)
            {
                if (Token.index == 13)
                {
                    nextToken();
                    Factor();
                    Console.WriteLine("ADD");
                    sc++;
                    codigochilo += "3B";
                    //codigochilo += "\n"; //testing purposes, must die eventually
                }
                else if (Token.index == 14)
                {
                    nextToken();
                    Factor();
                    Console.WriteLine("SUB");
                    sc++;
                    codigochilo += "3C";
                }
            }
        }
        public static void Factor()
        {
            Terminal();
            while (Token.index == 15 || Token.index == 16)
            {
                if (Token.index == 15)
                {
                    nextToken();
                    Terminal();
                    Console.WriteLine("MULT");
                    sc++;
                    codigochilo += "3D";
                }
                else if (Token.index == 16)
                {
                    nextToken();
                    Terminal();
                    Console.WriteLine("DIV");
                    sc++;
                    codigochilo += "3E";
                }
            }
        }
        public static void Terminal()
        {
            if (Token.index == 37) //numero
            {
                Console.WriteLine("PUSHKI " + Token.valor);
                sc = sc + 5;
                //codigochilo += "17" + ConvertStringtoHexa(Token.valor).PadLeft(8, '0');
                //Console.WriteLine(" Este es el token valor:" + Token.valor);
                //Console.WriteLine(" Y su hexa: " + ConvertStringtoHexa(Token.valor));
                codigochilo += "17" + Int32.Parse(Token.valor).ToString("X4").PadLeft(8, '0');
                //codigochilo += "\n"; //testing purposes, must die eventually
                nextToken();
            }
            else if (Token.index == 50) // variable
            {
                Console.WriteLine("PUSHI " + Token.valor);
                sc = sc + 3;
                codigochilo += "0D" + GetDirection(Token.valor);
                //codigochilo += "\n"; //testing purposes, must die eventually
                nextToken();
            }
            else if (Token.index == 11) // OP  (
            {
                nextToken();
                Expretion();
                Match("CP");
            }
        }
        public static void Match(string comp)
        {
            switch (comp)
            {
                case "OB":
                    if (Token.index != 23)
                    {
                        Console.WriteLine("Error. { esperado. ");
                    }
                    break;
                case "CB":
                    if (Token.index != 24)
                    {
                        Console.WriteLine("Error. } esperado. ");
                    }
                    break;
                case "Eq":
                    if (Token.index != 27)
                    {
                        Console.WriteLine("Error. = esperado.");
                    }
                    break;
                case "OP":
                    if (Token.index != 11)
                    {
                        Console.WriteLine("Error. ( esperado.");
                    }
                    break;
                case "CP":
                    if (Token.index != 12)
                    {
                        Console.WriteLine("Error. ) esperado.");
                    }
                    break;
                case "]":
                    if (Token.index != 26)
                    {
                        Console.WriteLine("Error. ] esperado.");
                    }
                    break;
            }
            nextToken();

        }
        public static bool isStatement()
        {
            return
                Token.index == 0 || //char
                Token.index == 1 || //string
                Token.index == 2 || //int
                Token.index == 3 || //float
                Token.index == 4 || //double
                Token.index == 5 || //if
                Token.index == 7 || //for
                Token.index == 8 || //while
                Token.index == 9 || // print
                Token.index == 41 || //printl
                Token.index == 50;
        }
        public static bool isDeclaration()
        {
            return Token.index == 0 || //char
                Token.index == 1 || //string
                Token.index == 2 || //int
                Token.index == 3 || //float
                Token.index == 4; //double
        }
        private static bool isInstruction()
        {
            return Token.index == 41|| //printl
                Token.index == 9 || // print
                Token.index == 8 || // while
                Token.index == 7 || // for
                Token.index == 6 || // else
                Token.index == 5; // if


        }
        public static void nextToken()
        {
            tokenCount++;
            Token = tokensArray[tokenCount];
        }
        public static string CheckVarTable()
        {
            bool found = false;
            //Console.WriteLine("varCount " + varCount);
            for (int i = 0; i < varCount; i++)
            {
               // Console.WriteLine("Se compararon estos: " + Token.valor + "    y    "  + varTable[i].name);
                if(Token.valor == varTable[i].name)
                {
                    found = true;
                    //Console.WriteLine("///Found");
                    return varTable[i].type;
                    
                }
            }
            if(found== false)
            {
                Console.WriteLine("Error. Variable no existe");
            }
            return "null";
        }
        public static void assignDirection(string name)
        {
            bool found = false;
            for (int i = 0; i < varCount && found==false; i++)
            {
                if (name == varTable[i].name)
                {
                    varTable[i].direction = "0000";

                    if(varTable[i].type == "char")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        Console.WriteLine("direccion:" + varTable[i].direction);
                        directionSize = varTable[i].direction;
                        directionHelper++;
                    }
                    if (varTable[i].type == "string")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + StringLength;

                    }
                    if (varTable[i].type == "int")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 4;
                    }
                    if (varTable[i].type == "float")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 4;
                    }
                    if (varTable[i].type == "double")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 4;
                    }
                    lastDirection = varTable[i].direction;
                    //Console.WriteLine("pup" + varTable[i].direction);
                }
            }

            if (found == false)
            {
                //Console.WriteLine("Error. Variable no existe");
            }
        }
        public static string GetDirection(string n)
        {
            for(int i=0; i< varTable.Length;i++)
            {
                if(n == varTable[i].name)
                {
                    return varTable[i].direction;
                }
            }
            Console.WriteLine("Error. Variable sin direccion.");
            return "";
        }
        public static string ConvertStringtoHexa(string input)
        {
            string result = "";
            char[] values = input.ToCharArray();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                result += ""+hexOutput;
                Console.WriteLine("Hexadecimal value of {0} is {1}", letter, hexOutput);
            }
            Console.WriteLine("Result: " + result);
            return result;
        }
        public static void JumpWriteCode(string JumpHelper)
        {
            if (JumpHelper == "JMPEQ")
                codigochilo += "31";
            else if (JumpHelper == "JMPNE")
                codigochilo += "32";
            else if (JumpHelper == "JMPGT")
                codigochilo += "33";
            else if (JumpHelper == "JMPGE")
                codigochilo += "34";
            else if (JumpHelper == "JMPLT")
                codigochilo += "35";
            else if (JumpHelper == "JMPLE")
                codigochilo += "36";
            
        }
        public static void Debug()
        {
            Console.WriteLine("///Debug///");
            Console.WriteLine("Token index: " + Token.index);
            Console.WriteLine("Token valor: " + Token.valor);
            Console.WriteLine("Token Count: " + tokenCount);
            Console.WriteLine("///Fin ///");
        }
    }
    class Tokens
    {
        public int index { get; set; }
        public String valor { get; set; }
        public Tokens()
        {
            index = 0;
            valor = "";
        }
        public Tokens(int ind, String val)
        {
            index = ind;
            valor = val;
        }
    }
    class Variables
    {
        public string name { get; set; }
        public string type { get; set; }
        public string direction { get; set; }
        public bool isArray  { get; set; }
        public bool initialized { get; set; }
        public Variables()
        {
            name = "noname";
            type = "null";
            isArray = false;
            initialized = false;
            direction = "-1";
        }

    }

}


//0 char
//1 string
//2 int
//3 float
//4 double
//5 if
//6 else
//7 for
//8 while
//9 print
//10 read
//11 (
//12 )
//13 +
//14 -
//15 *
//16 /
//17 <
//18 <=
//19 >
//20 >=
//21 ==
//22 !=
//23 {
//24 }
//25 [
//26 ]
//27 =
//28 ;
//29 \n dead
//30 ,
//31 &&
//32 ||
//33 ! dead
//34 "
//35 '
//36 .
//37 number
//38 decimal
//39 un string
//40 un char
//41 printl
//50 "variable"
//51 unrecognized