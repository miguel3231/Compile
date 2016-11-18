﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compile
{
    class Program
    {
        public static Tokens[] tokensArray = new Tokens[100];
        public static Tokens Token;
        public static int tokenCount = 0;
        public static String[,] varTable = new String[100,2];
        public static int varCount = 0;
        public static String codigochilo = "";
        public static int codigoByteCount = 0;
        public static int LCount = 1;
        public static string JumpHelper = "";
        static void Main(string[] args)
        {
            string line = " { if(a==5){b=5}}";
            evaluate(line);
            Programa();
            Console.ReadLine();
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
                else if (charArray[i] == '"')
                {
                    tokensArray[count] = new Tokens(34, "");
                    count++;
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
            codigochilo += "2843294348554e4b554e00000000";
            codigoByteCount = 15;
            Token = tokensArray[tokenCount];
            Bloque();
            Console.WriteLine("HALT");
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
                        Console.WriteLine("DEFC " + Token.valor);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "char";
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
                        Console.WriteLine("DEFAC " + Token.valor + ", " + number);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "charArray";
                        varCount++;
                    }
                    break;
                case 1://string
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFS " + Token.valor + ", 50");
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "string";
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
                        Console.WriteLine("DEFAS " + Token.valor + ", 50, " + number);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "stringArray";
                        varCount++;
                    }
                    break;
                case 2: //int
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFI " + Token.valor);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "int";
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
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "intArray";
                        varCount++;
                    }
                    break;
                case 3: //float
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFF " + Token.valor);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "float";
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
                        Console.WriteLine("DEFAF " + Token.valor + ", " + number);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "floatArray";
                        varCount++;
                    }
                    break;
                case 4: //double
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine("DEFD " + Token.valor);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "double";
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
                        Console.WriteLine("DEFAD " + Token.valor + ", " + number);
                        varTable[varCount, 0] = Token.valor;
                        varTable[varCount, 1] = "doubleArray";
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
            }
            else if (Token.index == 0)
            {
                Console.WriteLine("PUSHC " + Token.valor);
                Console.WriteLine("POPC" + nombre);
                nextToken();
            }
            else if (Token.index == 1)
            {
                Console.WriteLine("PUSHS " + Token.valor);
                Console.WriteLine("POPS" + nombre);
                nextToken();
            }

        }
        private static void DoInstruction()
        {
            if (Token.index == 5) //if
            {
                nextToken();
                Match("OP");
                BoolExpretion();
                Match("CP");
                Console.WriteLine(JumpHelper + " L" +LCount);
                Bloque();
                Console.WriteLine("L" + LCount+":");
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
        }
        private static void PreparePrint() // aqui buscamos que tipo de print se hace, falta la var table
        {
            switch (CheckVarTable())
            {
                case "char": // char
                    Console.WriteLine("PRTC " + Token.valor);
                    nextToken();
                    break;
                case "charArray":
                    Console.WriteLine("PRTAC " + Token.valor);
                    nextToken();
                    break;
                case "string": //String
                    Console.WriteLine("PRTS " + Token.valor);
                    nextToken();
                    break;
                case "stringArray":
                    Console.WriteLine("PRTAS " + Token.valor);
                    nextToken();
                    break;
                case "int": //int
                    Console.WriteLine("PRTI " + Token.valor);
                    nextToken();
                    break;
                case "intArray":
                    Console.WriteLine("PRTAI " + Token.valor);
                    nextToken();
                    break;
                case "float": //float
                    Console.WriteLine("PRTF " + Token.valor);
                    nextToken();
                    break;
                case "floatArray":
                    Console.WriteLine("PRTAF " + Token.valor);
                    nextToken();
                    break;
                case "double": //double
                    Console.WriteLine("PRTD " + Token.valor);
                    nextToken();
                    break;
                case "doubleArray":
                    Console.WriteLine("PRTAD " + Token.valor);
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
            while(Token.index == 31 || Token.index == 32)
            {
                if(Token.index == 31)
                {
                    //pending, no idea what happens here
                }
                else if(Token.index == 32)
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
            while(Token.index == 17 || //<
                Token.index == 18 || //<=
                Token.index == 19 || //>
                Token.index == 20 || //>=
                Token.index == 21 || //==
                Token.index == 22) // !=
            {
                if(Token.index == 17)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    //Console.WriteLine("JMPLT " ); 
                    JumpHelper = "JMPGE"; //opuesto a la comparacion realizada
                }
                if (Token.index == 18)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    //Console.WriteLine("JMPLE ");
                    JumpHelper = "JMPGT";
                }
                if (Token.index == 19)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    //Console.WriteLine("JMPGT ");
                    JumpHelper = "JMPLE";

                }
                if (Token.index == 20)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    //Console.WriteLine("JMPGE ");
                    JumpHelper = "JMMPLT";
                }
                if (Token.index == 21)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
                    //Console.WriteLine("JMPEQ ");
                    JumpHelper = "JMPNE";
                }
                if (Token.index == 22)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine("CMP");
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
                }
                else if (Token.index == 14)
                {
                    nextToken();
                    Factor();
                    Console.WriteLine("SUB");
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
                }
                else if (Token.index == 16)
                {
                    nextToken();
                    Terminal();
                    Console.WriteLine("DIV");
                }
            }
        }
        public static void Terminal()
        {
            if (Token.index == 37) //numero
            {
                Console.WriteLine("PUSHKI " + Token.valor);
                nextToken();
            }
            else if (Token.index == 50) // variable
            {
                Console.WriteLine("PUSHI " + Token.valor);
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
            return Token.index == 9 || // print
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
        public static String CheckVarTable()
        {
            bool found = false;
            for (int i = 0; i < varCount || found; i++)
            {
                if(Token.valor == varTable[i,0])
                {
                    return varTable[i, 1];
                }
            }
            if(found== false)
            {
                Console.WriteLine("Error. Variable no existe");
            }
            return "";
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
//50 "variable"
//51 unrecognized