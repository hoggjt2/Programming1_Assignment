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
        //For holding the prize pool

        static void Main(string[] args)
        {
            string input;
            string file = @"..\..\class.txt"; //File to read student data from
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
        //Change a students phone number. Returns true is student found, else false
        static bool ChangePhoneNumber(Student[] students)
        {
            string searchParameter;
            Console.Write("Please enter a students name or phone number: ");
            searchParameter = Console.ReadLine();
            for(int i=0; i<students.Length; i++)
            {
                if ((   searchParameter == students[i].firstName)//First name match
                    || (searchParameter == students[i].lastName)//Last name match
                    || (searchParameter == students[i].phoneNumber)//Phone number match
                    || (searchParameter == students[i].firstName+" "+ students[i].firstName))//Full name match
                {
                    Console.Write(students[i].firstName.PadLeft(15));
                    Console.Write(students[i].lastName.PadRight(15));
                    Console.WriteLine(students[i].phoneNumber.PadRight(15));
                    Console.Write("Is this the correct student?(y/n): ");
                    if(Console.ReadLine() == "y")
                    {
                        Console.Write("Please enter new phone number: ");
                        students[i].phoneNumber = Console.ReadLine();
                        return true;
                    }
                }
            }
            Console.WriteLine("Student not found.");
            return false;
        }
        //Sort students into ascending order
        static void Sort(Student[] students)
        {
            Student temp;
            for(int i=0; i<students.Length; i++)
            {
                for(int pos=0; pos<i; pos++)
                {
                    if (students[pos + 1].firstName.CompareTo(students[pos].firstName) == -1)
                    {
                        temp = students[pos];
                        students[pos] = students[pos + 1];
                        students[pos + 1] = temp;
                    }
                }
            }
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
            sr.Close();
        }
        //Save students array to file
        public static void SaveStudentData(Student[] students, string file)
        {
            StreamWriter sw = new StreamWriter(@file);
            foreach(Student s in students)
            {
                sw.WriteLine(s.firstName);
                sw.WriteLine(s.lastName);
                sw.WriteLine(s.phoneNumber);
            }
            sw.Close();
        }
    }
}
