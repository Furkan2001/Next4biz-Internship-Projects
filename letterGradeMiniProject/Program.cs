using System;
using System.IO;
using System.Collections.Generic;

namespace lessonLetterGradeMiniProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Student student = CreateStudentFromUserInput();

            student.DisplayInfo();

            AppendStudentToFile(student, "data.txt");

            List<Student> students = ReadStudentsFromFile("data.txt");

            Console.WriteLine("\nAll students grades:");
            foreach (var s in students)
            {
                s.DisplayInfo();
            }

            Console.ReadLine();
        }

        static Student CreateStudentFromUserInput()
        {
            Console.Write("Enter student name: ");
            string? name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Name cannot be empty. Enter student name: ");
                name = Console.ReadLine();
            }

            Console.Write("Enter student number: ");
            string? numberInput = Console.ReadLine();
            int number;
            while (!int.TryParse(numberInput, out number))
            {
                Console.Write("Invalid input. Enter a valid student number: ");
                numberInput = Console.ReadLine();
            }

            Console.Write("Enter midterm 1 grade: ");
            string? midterm1Input = Console.ReadLine();
            double midterm1;
            while (!double.TryParse(midterm1Input, out midterm1))
            {
                Console.Write("Invalid input. Enter a valid midterm 1 grade: ");
                midterm1Input = Console.ReadLine();
            }

            Console.Write("Enter midterm 2 grade: ");
            string? midterm2Input = Console.ReadLine();
            double midterm2;
            while (!double.TryParse(midterm2Input, out midterm2))
            {
                Console.Write("Invalid input. Enter a valid midterm 2 grade: ");
                midterm2Input = Console.ReadLine();
            }

            Console.Write("Enter final grade: ");
            string? finalInput = Console.ReadLine();
            double final;
            while (!double.TryParse(finalInput, out final))
            {
                Console.Write("Invalid input. Enter a valid final grade: ");
                finalInput = Console.ReadLine();
            }

            // Verileri kullanarak bir Student nesnesi oluştur ve geri dön
            return new Student(name, number, midterm1, midterm2, final);
        }

        static void AppendStudentToFile(Student student, string filePath)
        {
            try
            {
                using var sw = new StreamWriter(filePath, true);
                sw.WriteLine(student.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing to the file:");
                Console.WriteLine(ex.Message);
            }
        }

        static List<Student> ReadStudentsFromFile(string filePath)
        {
            List<Student> students = new List<Student>();

            try
            {
                using var sr = new StreamReader(filePath);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    // Satırı virgülle ayrıştırarak verileri al
                    string[] parts = line.Split(',');
                    if (parts.Length == 7)
                    {
                        string name = parts[0];
                        if (!int.TryParse(parts[1], out int number))
                        {
                            Console.WriteLine($"Invalid number format: {parts[1]}");
                            continue; // Geçersiz veri durumunda bu satırı atla
                        }

                        if (!double.TryParse(parts[2], out double midterm1))
                        {
                            Console.WriteLine($"Invalid midterm1 format: {parts[2]}");
                            continue; // Geçersiz veri durumunda bu satırı atla
                        }

                        if (!double.TryParse(parts[3], out double midterm2))
                        {
                            Console.WriteLine($"Invalid midterm2 format: {parts[3]}");
                            continue; // Geçersiz veri durumunda bu satırı atla
                        }

                        if (!double.TryParse(parts[4], out double final))
                        {
                            Console.WriteLine($"Invalid final format: {parts[4]}");
                            continue; // Geçersiz veri durumunda bu satırı atla
                        }

                        // Yeni bir Student nesnesi oluştur ve listeye ekle
                        Student student = new Student(name, number, midterm1, midterm2, final);
                        students.Add(student);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid data format: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while reading from the file:");
                Console.WriteLine(ex.Message);
            }

            return students;
        }
    }
}