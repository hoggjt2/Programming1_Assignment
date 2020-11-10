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
        //For holding prize data
        public struct Prize
        {
            public string name;
            public int quantity;
            public int lowerBound, upperBound;
            public Prize(string name, int quantity, int lowerBound, int upperBound)
            {
                this.name = name;
                this.quantity = quantity;
                this.lowerBound = lowerBound;
                this.upperBound = upperBound;
            }
        }
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
            string class_FileName = @"..\..\..\..\class.txt"; //File where student data is kept
            string prizes_FileName = @"..\..\..\..\prizes.txt";
            Student[] students = new Student[21]; //Array to hold student data read from file
            Prize[] prizePool = new Prize[13]; //An array of all available prizes
            ReadStudentData(students, @class_FileName); //Load data from file into students array
            LoadPrizePool(prizePool, prizes_FileName);
            ////Main input loop.
            //do
            //{
            //    input = Menu();
            //    Console.Clear();
            //} while (input != "Exit");
            //ShowPrizePool(prizePool);
            //SellTen(students, prizePool);
            //ShowStudents(students);
            Console.WriteLine(WinPrize(prizePool, "Fiji"));
            //SellTicket(prizePool);
            ShowPrizePool(prizePool);
            Console.ReadLine();
        }
        //Select 10 students an random and sell 1 ticket each
        public static void SellTen(Student[] students, Prize[] prizePool)
        {
            Random rand = new Random();
            Student[] winners = new Student[10];
            for(int i=0; i<winners.Length; i++)//Select 10 students at random, load into winners
            {
                winners[i] = students[rand.Next(21)];
                for(int j=0; j<i; j++)
                {
                    if((winners[j].firstName==winners[i].firstName) && (winners[j].lastName == winners[i].lastName))
                    {
                        i--;
                    }
                }
            }
            foreach(Student s in winners)//Choose 4 prize(1 ticket) for the 10 students
            {
                Console.WriteLine(s.firstName+" "+s.lastName);
                SellTicket(prizePool);
            }
        }
        public static int WinPrize(Prize[] prizePool, string wanted)
        {
            int tickets = 0;
            do
            {
                tickets++;
            } while (!SellTicket(prizePool, wanted));
            return tickets;
        }
        public static bool SellTicket(Prize[] prizePool, string wanted = "noItem")
        {
            bool prizeWon = false;
            string prize;
            for (int i = 1; i < 5; i++)
            {
                Console.WriteLine($"Prize {i}: {prize=GetPrize(prizePool)}");
                if (prize == wanted)
                {
                    prizeWon = true;
                }
            }
            return prizeWon;
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
                    if (students[pos + 1].lastName.CompareTo(students[pos].lastName) == -1)
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
            Console.Write("First Name".PadRight(15));
            Console.Write("Last Name".PadLeft(20));
            Console.WriteLine("Phone Number".PadLeft(15));
            foreach (Student s in students)
            {
                Console.Write(s.firstName.PadRight(15));
                Console.Write(s.lastName.PadLeft(20));
                Console.WriteLine(s.phoneNumber.PadLeft(15));
            }
        }
        public static void ShowPrizePool(Prize[] prizePool)
        {
            Console.Write("Prize".PadRight(15));
            Console.WriteLine("Quantity".PadLeft(15));
            foreach(Prize p in prizePool)
            {
                Console.Write(p.name.PadRight(15));
                Console.WriteLine(Convert.ToString(p.quantity).PadLeft(15));
            }
        }
        //Choose a prize at random
        public static string GetPrize(Prize[] prizePool)
        {
            Random rand = new Random();
            int x = rand.Next(3616000);
            for(int i=0; i<prizePool.Length; i++)
            {
                if(x>=prizePool[i].lowerBound && x < prizePool[i].upperBound)
                {
                    prizePool[i].quantity--;//Decrement remaining prizepool
                    return prizePool[i].name;//Return prize name
                }
            }
            return "Better luck next time";//If they lose
        }
        //Read student data from file, load into array
        public static void ReadStudentData(Student[] students, string file)
        {
            int i = 0;
            StreamReader sr = new StreamReader(@"..\..\..\..\class.txt");
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
        public static void WriteStudentData(Student[] students, string file)
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
        public static void LoadPrizePool(Prize[] prizePool,string fileName)
        {
            StreamReader sr = new StreamReader(@fileName);
            int i = 0;
            while (!sr.EndOfStream)
            {
                prizePool[i] = new Prize(sr.ReadLine(), Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()));
                i++;
            }
     
        }
    }
}
