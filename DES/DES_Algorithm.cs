using System;
using System.Collections.Generic;
using System.Linq;

namespace DES
{
    internal static class DES_Algorithm
    {
        #region Tablice

        //4. Teraz musimy zająć się kluczem. Klucz standardowo ma 64 bity, ale jego rozmiar redukujemy do
        //56 bitów za pomocą następującej permutacji zwanej permutowanym wyborem(Permuted Choice – PC):
        private static readonly int[] PC_1 = {57, 49, 41, 33, 25, 17, 9,
                                                1, 58, 50, 42, 34, 26, 18,
                                                10, 2, 59, 51, 43, 35, 27,
                                                19, 11, 3, 60, 52, 44, 36,
                                                63, 55, 47, 39, 31, 23, 15,
                                                7, 62, 54, 46, 38, 30, 22,
                                                14, 6, 61, 53, 45, 37, 29,
                                                21, 13, 5, 28, 20, 12, 4};

        //7. Dla każdej iteracji powstaje klucz Kn, który tworzymy łącząc bloki Cn i Dn, a następnie ich
        //połączenie permutując ciągiem permutowanego wyboru 2 (Permuted Choice 2 – PC2):
        private static readonly int[] PC_2 = {14, 17, 11, 24, 1, 5,
                                                3, 28, 15, 6, 21, 10,
                                                23, 19, 12, 4, 26, 8,
                                                16, 7, 27, 20, 13, 2,
                                                41, 52, 31, 37, 47, 55,
                                                30, 40, 51, 45, 33, 48,
                                                44, 49, 39, 56, 34, 53,
                                                46, 42, 50, 36, 29, 32};

        //2. Initial Permutation.
        private static readonly int[] IP = {58, 50, 42, 34, 26, 18, 10, 2,
                                            60, 52, 44, 36, 28, 20, 12, 4,
                                            62, 54, 46, 38, 30, 22, 14, 6,
                                            64, 56, 48, 40, 32, 24, 16, 8,
                                            57, 49, 41, 33, 25, 17,  9, 1,
                                            59, 51, 43, 35, 27, 19, 11, 3,
                                            61, 53, 45, 37, 29, 21, 13, 5,
                                            63, 55, 47, 39, 31, 23, 15, 7},
                                    IP_1 = {40, 8, 48, 16, 56, 24, 64, 32,
                                            39, 7, 47, 15, 55, 23, 63, 31,
                                            38, 6, 46, 14, 54, 22, 62, 30,
                                            37, 5, 45, 13, 53, 21, 61, 29,
                                            36, 4, 44, 12, 52, 20, 60, 28,
                                            35, 3, 43, 11, 51, 19, 59, 27,
                                            34, 2, 42, 10, 50, 18, 58, 26,
                                            33, 1, 41, 9, 49, 17, 57, 25 };

        //6. tablica przesunięć
        private static readonly int[] leftShift = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        //8. Następnym krokiem jest przekształcenie 32-bitowego blok Rn-1 w blok 48 bitowy. Dokonujemy
        //tego za pomocą następującej permutacyjnej tablicy rozszerzenia(E):
        private static readonly int[] EBit_Selection = {32, 1, 2, 3, 4, 5,
                                                        4, 5, 6, 7, 8, 9,
                                                        8, 9, 10, 11, 12, 13,
                                                        12, 13, 14, 15, 16, 17,
                                                        16, 17, 18, 19, 20, 21,
                                                        20, 21, 22, 23, 24, 25,
                                                        24, 25, 26, 27, 28, 29,
                                                        28, 29, 30, 31, 32, 1};

        //12. Skoro już wiemy z jakich wierszy i kolumn mamy zczytywać dane, musimy teraz określić z
        //jakiej tablicy będziemy je brać.Każdy z 8 ciągów 6-bitowych ma przygotowaną osobną tablicę Sn.

        private static readonly int[,] S1 = {{14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                                            { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                                            { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                                            { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }},
                                        S2 = {{15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                                            { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                                            { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                                            { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 } },
                                        S3 = {{10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                                            { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                                            { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                                            { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12} },
                                        S4 = {{7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                                            { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                                            { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                                            { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14} },
                                        S5 = { {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                                            { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                                            { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                                            { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3} },
                                        S6 = {{12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                                            { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                                            { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                                            { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 } },
                                        S7 = {{4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                                            { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                                            { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                                            { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0 ,15, 14, 2, 3, 12 } },
                                        S8 = {{13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                                            { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                                            { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                                            { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 } };

        //14. Otrzymany ciąg 32-bitowy modyfikujemy za pomocą funkcji permutacji (P)
        private static readonly int[] P = {16, 7, 20, 21,
                                            29, 12, 28, 17,
                                            1, 15, 23, 26,
                                            5, 18, 31, 10,
                                            2, 8, 24, 14,
                                            32, 27, 3, 9,
                                            19, 13, 30, 6,
                                            22, 11, 4, 25};

        #endregion Tablice

        #region Funkcje pomocnicze

        // Konwertuje string na tablicę intów
        private static int[] ToArray(string s)
        {
            int[] ints = s.ToCharArray()
                .Where(x => int.TryParse(x.ToString(), out int myInt))
                .Select(x => int.Parse(x.ToString()))
                .ToArray();

            return ints;
        }

        // Konwertuje tablicę intów na string
        private static string FromArrayToString(int[] array)
        {
            return string.Join("", array);
        }

        // Przesuwa bity w lewo raz lub dwa w zależności od tablicy przesunięć
        private static int[] ShiftToLeft(int[] toShift, int shiftCount)
        {
            int[] shifted = new int[toShift.Length];

            for (int j = 0; j < shiftCount; j++)
            {
                if (j == 1)
                {
                    shifted.CopyTo(toShift, 0);
                }

                for (int i = 0; i < toShift.Length; i++)
                {
                    if (i == toShift.Length - 1)
                    {
                        shifted[i] = toShift.First();
                    }
                    else
                    {
                        shifted[i] = toShift[i + 1];
                    }
                }
            }
            return shifted;
        }

        // Konwertuje tekst szesnastkowy na binarny i dopełnia go zerami do 4 bitów
        private static string HexToBin4Bit(string hexadecimal)
        {
            string binary = "";

            foreach (char c in hexadecimal)
            {
                binary += Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0');
            }

            return binary;
        }

        private static string BinToHex(string binary)
        {
            return string.Join("", Enumerable.Range(0, binary.Length / 4)
                .Select(i => Convert.ToByte(binary.Substring(i * 4, 4), 2)
                .ToString("X")));
        }

        // Zwraca permutowaną tablicę o konkretnym rozmiarze w zależności od wybranej funkcji permutacyjnej
        private static int[] Permute(int[] toPermute, int[] permutationRule, int outputSize)
        {
            int[] output = new int[outputSize];

            for (int i = 0; i < outputSize; i++)
            {
                output[i] = toPermute[permutationRule[i] - 1];
            }

            return output;
        }

        // Konwertuje binarny string na decimal integer
        private static int BinToDec(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }

        // Konwertuje wartość dziesiętną na binarny tekst zapisany na 4 bitach (dopełniany)
        private static string DecToBin4Bit(int decimalValue)
        {
            return Convert.ToString(decimalValue, 2).PadLeft(4, '0');
        }

        // Zwraca decimala z wybranego rzędu i kolumny z tablicy S1-S8 w zależności od packageIndex
        private static int GetValueFromSNArray(int packageIndex, int rowIndex, int columnIndex)
        {
            int valueDecimal = 0;

            switch (packageIndex)
            {
                case 0:
                    valueDecimal = S1[rowIndex, columnIndex];
                    break;

                case 1:
                    valueDecimal = S2[rowIndex, columnIndex];
                    break;

                case 2:
                    valueDecimal = S3[rowIndex, columnIndex];
                    break;

                case 3:
                    valueDecimal = S4[rowIndex, columnIndex];
                    break;

                case 4:
                    valueDecimal = S5[rowIndex, columnIndex];
                    break;

                case 5:
                    valueDecimal = S6[rowIndex, columnIndex];
                    break;

                case 6:
                    valueDecimal = S7[rowIndex, columnIndex];
                    break;

                case 7:
                    valueDecimal = S8[rowIndex, columnIndex];
                    break;
            }

            return valueDecimal;
        }

        // Dzieli tablicę źródłową na równe podtablice
        private static void SplitArray(int[] sourceArray, out int[] firstArray, out int[] secondArray)
        {
            int halfOfSize = sourceArray.Length / 2;
            firstArray = new int[halfOfSize];
            secondArray = new int[halfOfSize];

            Array.Copy(sourceArray, 0, firstArray, 0, halfOfSize);
            Array.Copy(sourceArray, halfOfSize, secondArray, 0, halfOfSize);
        }

        #endregion Funkcje pomocnicze

        public static string Encoding(string message, string key)
        {
            //konwertowanie stringa z wartościami szesnastkowymi na tablicę intów
            int[] binaryMessage = new int[64],
                binaryKey = new int[64];

            binaryMessage = ToArray(HexToBin4Bit(message)); // M
            binaryKey = ToArray(HexToBin4Bit(key)); // K

            // wypełnienie tablicy bitami
            if (binaryMessage.Length < 64)
            {
                binaryMessage = binaryMessage.Append(1).ToArray();

                while (binaryMessage.Length != 64)
                {
                    binaryMessage = binaryMessage.Append(0).ToArray();
                }
            }

            if (binaryKey.Length < 64)
            {
                return null;
            }

            //2. przekształcenie 64-bitowego bloku zgodnie z permutacją początkową IP
            int[] messageIP = Permute(binaryMessage, IP, 64);

            //3. podzielenie bloku 64-bitowego na dwa bloku 32-bitowe
            int[] Lprev = new int[32],  // Ln-1 (początkowy L0)
                Rprev = new int[32];    // Rn-1 (początkowy R0)

            SplitArray(messageIP, out Lprev, out Rprev);

            //4. redukcja rozmiaru klucza do 56 bitów za pomocą permutacji zwanej permutowanym wyborem (PC)
            int[] key56B = Permute(binaryKey, PC_1, 56);

            //5. podział przetworzonego klucza na 2 części - pierwsza (C) i druga (D), mają po 28 bitów.
            int[] C = new int[28],
                D = new int[28],
                CD = new int[56];

            SplitArray(key56B, out C, out D);

            List<int[]> joinedPermutatedKeys = new List<int[]>(); //(Kn)

            //6. w każdej części klucza przesuwamy w lewo o X bitów (zależne od tablicy leftShift)
            for (int i = 0; i < 16; i++)
            {
                C = ShiftToLeft(C, leftShift[i]);
                D = ShiftToLeft(D, leftShift[i]);

                CD = C.Concat(D).ToArray();
                //7. dla każdej iteracji powstaje klucz Kn, który tworzymy łącząc bloku Cn i Dn, a następnie
                //ich połączenie permutując ciągiem permutowanego wyboru 2 (PC2). Przerabiamy ciąg 56-bit na 48-bit
                int[] CD48B = Permute(CD, PC_2, 48);    // 48-bit

                joinedPermutatedKeys.Add(CD48B);
            }

            int[] E = new int[48]; // E(Rn-1)

            for (int i = 0; i < 16; i++)
            {
                int[] L = new int[32], R = new int[32], xored = new int[48]; // Kn+E(Rn-1)
                L = Rprev;

                //8. Przekształcenie 32-bitowego bloku Rn-1 w blok 48-bitowy za pomocą permutacyjnej tablicy rozszerzenia (EBit_Selection)
                E = Permute(L, EBit_Selection, 48);

                //9. Gdy posiadamy 48-bitowy klucz Kn i 48-bitową wersję bloku Rn-1, XOR'ujemy te tablice,
                //uzyskując ciąg 48 bitów
                for (int j = 0; j < E.Length; j++)
                {
                    xored[j] = E[j] ^ joinedPermutatedKeys.ElementAt(i)[j];
                }

                int packageIndex = 0, valueDecimal;
                string valueBinary, sString = "";
                int[] S = new int[32];

                //10. Uzyskany ciąg 48 bitów dzielimy na 8 ciągów 6-bitowych
                for (int j = 0; j < xored.Length; j += 6)
                {
                    //11.Każdy ciąg 6 - bitowy określa pozycję, z której powinno się zczytywać dane.
                    // Pierwszy i ostatni bit oznaczają indeks wiersza
                    string rowIndexBin = xored[j].ToString() + xored[j + 5].ToString(),
                        columnIndexBin = FromArrayToString(xored.Skip(j + 1)
                                        .Take(4)
                                        .ToArray());

                    // konwersja indeksów na decimal
                    int rowIndex = BinToDec(rowIndexBin),
                        columnIndex = BinToDec(columnIndexBin);

                    // 12. Skoro już wiemy z jakich wierszy i kolumn mamy zczytywać dane, musimy teraz określić z
                    //jakiej tablicy będziemy je brać.Każdy z 8 ciągów 6 - bitowych ma przygotowaną osobną tablicę Sn.
                    valueDecimal = GetValueFromSNArray(packageIndex, rowIndex, columnIndex);

                    // Zamiana wartości na binarną
                    valueBinary = DecToBin4Bit(valueDecimal);
                    sString += valueBinary;
                    packageIndex++;
                    valueBinary = "";
                }

                S = ToArray(sString);

                //14. Otrzymany ciąg 32-bitowy modyfikujemy za pomocą funkcji permutacji (P):
                int[] f = Permute(S, P, 32);

                //15. Blok 32-bitowy który otrzymaliśmy wykonując liczne przekształcenia na bloku Rn-1 XOR'ujemy
                //z blokiem Ln - 1, uzyskując blok Rn.
                for (int j = 0; j < R.Length; j++)
                {
                    R[j] = Lprev[j] ^ f[j];
                }

                //16. Blok Ln uzyskujemy kopiując blok Rn-1.
                Lprev = L;
                Rprev = R;
            }

            //17. Łączymy ze sobą 32-bitowe bloki R16 i L16. Warto tu zwrócić uwagę, że kolejność jest
            //odwrócona – blok prawy(R) jest teraz pierwszym blokiem, a blok lewy(L) jest drugim blokiem.
            //18. Otrzymany blok 64-bitowy poddajemy odwróconej tablicy permutacji (Initial Permutation -1 –
            //IP - 1).
            int[] finalRL = Permute(Rprev.Concat(Lprev).ToArray(), IP_1, 64);

            string hexEncriptedM = BinToHex(FromArrayToString(finalRL));

            return hexEncriptedM;
            //19. Wszystko powtarzamy dla pozostałych bloków 64-bitowych.
        }

        public static string Decoding(string message, string key)
        {
            //konwertowanie stringa z wartościami szesnastkowymi na tablicę intów
            int[] binaryMessage = new int[64],
                binaryKey = new int[64];

            binaryMessage = ToArray(HexToBin4Bit(message)); // M
            binaryKey = ToArray(HexToBin4Bit(key)); // K

            // wypełnienie tablicy bitami
            if (binaryMessage.Length < 64)
            {
                binaryMessage = binaryMessage.Append(1).ToArray();

                while (binaryMessage.Length != 64)
                {
                    binaryMessage = binaryMessage.Append(0).ToArray();
                }
            }

            if (binaryKey.Length < 64)
            {
                return null;
            }

            //2. przekształcenie 64-bitowego bloku zgodnie z permutacją początkową IP
            int[] messageIP = Permute(binaryMessage, IP, 64);

            //3. podzielenie bloku 64-bitowego na dwa bloku 32-bitowe
            int[] Lprev = new int[32],  // Ln-1 (początkowy L0)
                Rprev = new int[32];    // Rn-1 (początkowy R0)

            SplitArray(messageIP, out Lprev, out Rprev);

            //4. redukcja rozmiaru klucza do 56 bitów za pomocą permutacji zwanej permutowanym wyborem (PC)
            int[] key56B = Permute(binaryKey, PC_1, 56);

            //5. podział przetworzonego klucza na 2 części - pierwsza (C) i druga (D), mają po 28 bitów.
            int[] C = new int[28],
                D = new int[28],
                CD = new int[56];

            SplitArray(key56B, out C, out D);

            List<int[]> joinedPermutatedKeys = new List<int[]>(); //(Kn)

            //6. w każdej części klucza przesuwamy w lewo o X bitów (zależne od tablicy leftShift)
            for (int i = 0; i < 16; i++)
            {
                C = ShiftToLeft(C, leftShift[i]);
                D = ShiftToLeft(D, leftShift[i]);

                CD = C.Concat(D).ToArray();
                //7. dla każdej iteracji powstaje klucz Kn, który tworzymy łącząc bloku Cn i Dn, a następnie
                //ich połączenie permutując ciągiem permutowanego wyboru 2 (PC2). Przerabiamy ciąg 56-bit na 48-bit
                int[] CD48B = Permute(CD, PC_2, 48);

                joinedPermutatedKeys.Add(CD48B);
            }

            int[] E = new int[48]; // E(Rn-1)

            for (int i = 15; i >= 0; i--)
            {
                int[] L = new int[32], R = new int[32], xored = new int[48]; // Kn+E(Rn-1)
                L = Rprev;

                //8. Przekształcenie 32-bitowego bloku Rn-1 w blok 48-bitowy za pomocą permutacyjnej tablicy rozszerzenia (EBit_Selection)
                E = Permute(L, EBit_Selection, 48);

                //9. Gdy posiadamy 48-bitowy klucz Kn i 48-bitową wersję bloku Rn-1, XOR'ujemy te tablice,
                //uzyskując ciąg 48 bitów
                for (int j = 0; j < E.Length; j++)
                {
                    xored[j] = E[j] ^ joinedPermutatedKeys.ElementAt(i)[j];
                }

                int packageIndex = 0, valueDecimal;
                string valueBinary, sString = "";
                int[] S = new int[32];

                //10. Uzyskany ciąg 48 bitów dzielimy na 8 ciągów 6-bitowych
                for (int j = 0; j < xored.Length; j += 6)
                {
                    //11.Każdy ciąg 6 - bitowy określa pozycję, z której powinno się zczytywać dane.
                    // Pierwszy i ostatni bit oznaczają indeks wiersza
                    string rowIndexBin = xored[j].ToString() + xored[j + 5].ToString(),
                        columnIndexBin = FromArrayToString(xored.Skip(j + 1).Take(4).ToArray());

                    // konwersja indeksów na decimal
                    int rowIndex = BinToDec(rowIndexBin),
                        columnIndex = BinToDec(columnIndexBin);

                    // 12. Skoro już wiemy z jakich wierszy i kolumn mamy zczytywać dane, musimy teraz określić z
                    //jakiej tablicy będziemy je brać.Każdy z 8 ciągów 6 - bitowych ma przygotowaną osobną tablicę Sn.
                    valueDecimal = GetValueFromSNArray(packageIndex, rowIndex, columnIndex);

                    // Zamiana wartości na binarną
                    valueBinary = DecToBin4Bit(valueDecimal);
                    sString += valueBinary;
                    packageIndex++;
                    valueBinary = "";
                }

                S = ToArray(sString);

                //14. Otrzymany ciąg 32-bitowy modyfikujemy za pomocą funkcji permutacji (P):
                int[] f = Permute(S, P, 32);

                //15. Blok 32-bitowy który otrzymaliśmy wykonując liczne przekształcenia na bloku Rn-1 XOR'ujemy
                //z blokiem Ln - 1, uzyskując blok Rn.
                for (int j = 0; j < R.Length; j++)
                {
                    R[j] = Lprev[j] ^ f[j];
                }

                //16. Blok Ln uzyskujemy kopiując blok Rn-1.
                Lprev = L;
                Rprev = R;
            }

            //17. Łączymy ze sobą 32-bitowe bloki R16 i L16. Warto tu zwrócić uwagę, że kolejność jest
            //odwrócona – blok prawy(R) jest teraz pierwszym blokiem, a blok lewy(L) jest drugim blokiem.
            //18. Otrzymany blok 64-bitowy poddajemy odwróconej tablicy permutacji (Initial Permutation -1 –
            //IP - 1).
            int[] finalRL = Permute(Rprev.Concat(Lprev).ToArray(), IP_1, 64);
            string hexDecriptedM = BinToHex(FromArrayToString(finalRL));

            return hexDecriptedM;
        }
    }
}