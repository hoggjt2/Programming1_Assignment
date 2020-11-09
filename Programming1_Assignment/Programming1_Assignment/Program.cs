/*
 * Name:        Programming1_Assignment
 * Author:      James Hogg
 * Date:        9/11/2020
 * 
 * Description: A draft of the assignment for programming 1.
 * Due Date:    17/11/2020
*/
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Programming1_Assignment
{
    class Program
    {
        //For holding data about each student
        public struct Student
        {
            public string firstName, lastName, phoneNumber;
            public Student(string firstName, string lastName, string phoneNumber)
            {
                this.firstName = firstName;
                this.lastName = lastName;
                this.phoneNumber = phoneNumber;
            }
        }
        static void Main(string[] args)
        {
            string input;
            string file = "..\..\class.txt"; //File to read student data from
            Student[] students = new Student[10]; //Array to hold student data read from file
            ReadStudentData(students, file); //Load data from file into students array

            //Main input loop.
            do
            {
                input = Menu();
                Console.Clear();
            } while (input != "Exit");
        }
        static string Menu()
        {
            Console.Write("Please enter command: ");
            return Console.ReadLine();
        }
        //Display all student data in array
        static void ShowStudents(Student[] students)
        {
            Console.Write("First Name".PadLeft(15));
            Console.Write("Last Name".PadRight(15));
            Console.WriteLine("Phone Number".PadRight(15));
            foreach (Student s in students)
            {
                Console.Write(s.firstName.PadLeft(15));
                Console.Write(s.lastName.PadRight(15));
                Console.WriteLine(s.phoneNumber.PadRight(15));
            }
        }
        //Read student data from file, load into array
        public static void ReadStudentData(Student[] students, string file)
        {
            int i = 0;
            StreamReader sr = new StreamReader(@file);
            while (!sr.EndOfStream)
            {
                if (i == students.Length)//If array is too small, add extra space
                {
                    Array.Resize(ref students, students.Length + 1);
                }
                students[i] = new Student(sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                i++;
            }
        }
    }
}
