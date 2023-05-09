using System;
using System.Linq;
using System.Text;

namespace DES
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                int choice = 0;
                int[] choiceOptions = { 1, 2, 3 };

                Console.WriteLine("Wybierz opcję: \n1. Szyfrowanie DES\n2. Deszyfrowanie DES\n3. Wyczyść konsolę\n");

                do
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                } while (!choiceOptions.Contains(choice));

                //szyfrowanie
                if (choice == 1)
                {
                    string message, key;
                    Console.WriteLine("Podaj tekst jawny: ");
                    message = Console.ReadLine();

                    Console.WriteLine("\nPodaj klucz: ");
                    key = Console.ReadLine();

                    int rows = (int)Math.Ceiling(message.Length / 16.0);
                    char[,] messageTab = new char[rows, 16];

                    int index = 0;

                    //podział treści na 64-bitowe bloki, każdy będzie osobno poddany szyfrowaniu
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            if (index < message.Length)
                            {
                                messageTab[i, j] = message[index++];
                            }
                            else
                            {
                                messageTab[i, j] = '0';
                            }
                        }
                    }

                    Console.Write("\nZakodowano: " + message + " kluczem: " + key + "\nWynik: ");

                    index = 0;

                    //oddzielne kodowanie każdego bloku według podanego klucza
                    for (int i = 0; i < rows; i++)
                    {
                        StringBuilder rowString = new StringBuilder("");

                        for (int j = 0; j < 16; j++)
                        {
                            if (index++ < message.Length)
                            {
                                rowString.Append(messageTab[i, j]);
                            }
                            else
                            {
                                rowString.Append('0');
                            }
                        }

                        Console.Write(DES_Algorithm.Encoding(rowString.ToString(), key));
                    }

                    Console.Write("\n\n");
                    Console.ReadLine();
                }//deszyfrowanie
                else if (choice == 2)
                {
                    string message, key;

                    Console.WriteLine("Podaj ciąg zakodowany: ");
                    message = Console.ReadLine();

                    Console.WriteLine("\nPodaj klucz: ");
                    key = Console.ReadLine();

                    int rows = (int)Math.Ceiling(message.Length / 16.0);
                    char[,] messageTab = new char[rows, 16];

                    int index = 0;

                    //podział treści na 64-bitowe bloki, każdy będzie osobno poddany szyfrowaniu
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            if (index < message.Length)
                            {
                                messageTab[i, j] = message[index++];
                            }
                            else
                            {
                                messageTab[i, j] = ' ';
                            }
                        }
                    }

                    Console.Write("\nOdkodowano: " + message + " kluczem: " + key + "\nWynik: ");

                    index = 0;

                    //oddzielne kodowanie każdego bloku według podanego klucza
                    for (int i = 0; i < rows; i++)
                    {
                        StringBuilder row_str = new StringBuilder("");

                        for (int j = 0; j < 16; j++)
                        {
                            if (index++ < message.Length)
                            {
                                row_str.Append(messageTab[i, j]);
                            }
                            else
                            {
                                row_str.Append('0');
                            }
                        }

                        Console.Write(DES_Algorithm.Decoding(row_str.ToString(), key));
                    }

                    Console.Write("\n\n");
                    Console.ReadLine();
                }
                else if (choice == 3)
                {
                    Console.Clear();
                }
            }
        }
    }
}