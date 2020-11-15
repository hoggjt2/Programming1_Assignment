/*
 * Name:        Programming1_Assignment
 * Author:      James Hogg
 * Date:        9/11/2020
 * 
 * Description: Programming 1 Assignment.
 *              Create a program to manage a large scale raffle.
 * Due Date:    16/11/2020
 * 
 * Notes:   -Running the functionallity tests effects the prize pool. To prevent this, save changes before running
 *           tests then reset prize pool after.
 *          -For the sake of speed, buying tickets until a spacific prize is won is done by directly accessing the GetPrize
 *           method rather than ticket purchases. The number of tickets is then calculated by rounding up to the nearest 
 *           multiple of 4 then dividing by 4.
*/
using System;
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
            public Prize(string name, int quantity, int lowerBound = -1, int upperBound = -1)
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
        public static int prizesRemaining = 0; //Keep track of remining prize pool. If 0 print warning.
        static void Main(string[] args)
        {
            int input;
            string class_FileName = @"class.txt"; //File where student data is kept
            string prizes_FileName = @"prizes.txt"; //File holding prize data
            Student[] students = new Student[1]; //Array to hold student data read from file
            Prize[] prizePool = new Prize[13]; //An array of all available prizes
            ReadStudentData(ref students, @class_FileName); //Read student data from file
            prizesRemaining = LoadPrizePool(prizePool, prizes_FileName); //Read prize data from file
            Sort(students);
            double ticketPrice = 2;
            int ticketsSold = 0;

            do
            {
                input = Menu();
                Console.Clear();
                switch (input)//Display Menu. Get input.
                {
                    case 1:
                        ShowStudents(students);
                        break;
                    case 2:
                        Console.WriteLine("Prize Information");
                        ShowPrizePool(prizePool);
                        Console.WriteLine();
                        Console.WriteLine("Sale Information");
                        Console.WriteLine($"Tickets Sold:     {ticketsSold}");
                        Console.WriteLine($"Money Made:       {(double)ticketsSold * ticketPrice:C}");
                        Console.WriteLine($"Prizes Remaining: {prizesRemaining}");
                        break;
                    case 3:
                        students = AddStudent(ref students);
                        Sort(students);
                        break;
                    case 4:
                        RemoveStudent(ref students);
                        break;
                    case 5:
                        ChangePhoneNumber(students);
                        break;
                    case 6:
                        SellTicket(prizePool);
                        ticketsSold++;
                        break;
                    case 7:
                        ticketsSold += PurchaseAmountValue(prizePool, ticketPrice);
                        break;
                    case 8:
                        Console.Write("Please enter the prize you want to win: ");
                        string wanted = Console.ReadLine();
                        int attempts = WinPrize(prizePool, wanted);
                        Console.WriteLine($"You purchased {attempts} tickets before {wanted} was won.");
                        break;
                    case 9:
                        ReadStudentData(ref students, class_FileName);
                        break;
                    case 10:
                        prizesRemaining = LoadPrizePool(prizePool, prizes_FileName);
                        ticketsSold = 0;
                        Console.WriteLine("Prize Pool Reset");
                        break;
                    case 11:
                        WriteStudentData(students, class_FileName);
                        Console.WriteLine("Data Saved");
                        break;
                    case 12:
                        RunTests(students, prizePool, ref ticketsSold, ticketPrice);
                        break;
                    case -1:
                        Console.WriteLine("Invalid Input");
                        break;
                    case 99:
                        Console.WriteLine("Good Bye.");
                        break;
                }
                Console.ReadLine();
            }while(input != 99);
        }
        //Select 10 students an random and sell 1 ticket each
        public static int SellTen(Student[] students, Prize[] prizePool)
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
            return 10;
        }
        //Sell $ value worth of tickets
        public static int PurchaseAmountValue(Prize[] prizePool, double ticketPrice, double moneySpent=-1)
        {
            if (moneySpent < 0)//Ask user for input if parameter not provided
            {
                Console.Write("Please enter amount to spend: ");
                try
                {
                    moneySpent = Convert.ToDouble(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Invalid Input");
                    Console.ReadLine();
                    return 0;
                }
            }
            Prize[] prizesWon = new Prize[prizePool.Length];//Create array to hold users prizes
            string prizeName;
            int pos = 0;
            int losses = 0;
            int ticketsSold = Convert.ToInt32(Math.Floor(moneySpent/ticketPrice));//Number of tickets which can be purchased
            foreach(Prize p in prizePool)//Populate users prizes
            {
                prizesWon[pos] = new Prize(p.name, 0);
                pos++;
            }
            for(int i=0; i<ticketsSold; i++)//Purchase tickets
            {
                for(int j=0; j<4; j++)
                {
                    if((prizeName = GetPrize(prizePool))== "Better luck next time")
                    {
                        losses++;
                    }
                    else
                    {
                        for(int x=0; x<prizesWon.Length; x++)
                        {
                            if(prizesWon[x].name == prizeName)
                            {
                                prizesWon[x].quantity++;
                            }
                        }
                    }
                }
            }
            //Display users results
            Console.Write("Prize".PadRight(15));
            Console.WriteLine("Quantity".PadLeft(15));
            foreach(Prize p in prizesWon)
            {
                if (p.quantity > 0)
                {
                    Console.Write(p.name.PadRight(15));
                    Console.WriteLine(Convert.ToString(p.quantity).PadLeft(15));
                }
            }
            Console.Write("Losses".PadRight(15));
            Console.WriteLine(Convert.ToString(losses).PadLeft(15));
            return ticketsSold;//Return number of tickets purchased
        }
        //Continue selling tickets until specified prize is won. Return tickets sold.
        public static int WinPrize(Prize[] prizePool, string wanted)
        {
            bool prizeFound = false;
            foreach(Prize p in prizePool)
            {
                if(p.name == wanted && p.quantity>0)
                {
                    prizeFound = true;
                }
            }
            if (!prizeFound)
            {
                Console.WriteLine("Prize was not found in prize pool.");
                return 0;
            }
            int tickets = 0;
            do
            {
                tickets++;
            } while (GetPrize(prizePool)!=wanted);//SellTicket(prizePool, wanted));
            while(tickets%4 != 0)
            {
                tickets++;
            }
            return tickets/4;
        }
        //Sell 1 ticket. Returns true is optional parameter wanted is won.
        public static bool SellTicket(Prize[] prizePool, string wanted = "noItem", int tickets = 1)
        {
            bool prizeWon = false;
            string prize;
                for (int i = 1; i < 5; i++)
                {
                    Console.WriteLine($"Prize {i}: {prize = GetPrize(prizePool)}");
                    if (prize == wanted)
                    {
                        prizeWon = true;
                    }
                }
            return prizeWon;
        }
        public static  int Menu()
        {
            //Present title page
            Console.Clear();
            Console.WriteLine("Raffle Program".PadLeft(35));
            Console.Write("1. Display Students".PadRight(30));
            Console.WriteLine("7. Sell Tickets in Bulk");
            Console.Write("2. Display Prizepool".PadRight(30));
            Console.WriteLine("8. Sell Until Prize Won");
            Console.Write("3. Add Student".PadRight(30));
            Console.WriteLine("9. Reset Students");
            Console.Write("4. Remove Student".PadRight(30));
            Console.WriteLine("10. Reset Prizepool");
            Console.Write("5. Change Phone Number".PadRight(30));
            Console.WriteLine("11. Save Changes");
            Console.Write("6. Sell Ticket".PadRight(30));
            Console.WriteLine("12. Run Functionality Tests");
            Console.WriteLine();
            Console.Write("Please enter command: (99 to exit)");
            try
            {
                return Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                return -1;
            }
        }
        //Display help info
        public static void HelpMenu()
        {
            Console.WriteLine("Help Menu Goes Here.");
        }
        //Add a student to array
        public static Student[] AddStudent(ref Student[] students)
        {
            Student[] newArray = new Student[students.Length + 1];
            for(int i=0; i<students.Length; i++)
            {
                newArray[i] = students[i];
            }
            Console.Write("Please enter students first name: ");
            newArray[newArray.Length-1].firstName = Console.ReadLine();
            Console.Write("Please enter students last name: ");
            newArray[newArray.Length-1].lastName = Console.ReadLine();
            Console.Write("Please enter students phone number: ");
            newArray[newArray.Length-1].phoneNumber = Console.ReadLine();
            Console.WriteLine("Student added.");
            return newArray;
        }
        //Remove student from array
        public static void RemoveStudent(ref Student[] students)
        {
            string first, last;
            bool studentFound = false;
            int i = 0;
            ShowStudents(students);
            Console.Write("Please enter students first name: ");
            first = Console.ReadLine();
            Console.Write("Please enter students last name: ");
            last = Console.ReadLine();
            Student[] newStudents = new Student[students.Length];
            foreach(Student s in students)
            {
                if(first == s.firstName && last == s.lastName)
                {
                    studentFound = true;
                }
                else
                {
                    newStudents[i] = s;
                    i++;
                }
            }
            if (studentFound)
            {
                Array.Resize(ref newStudents, newStudents.Length - 1);
            }
            students = newStudents;
            Console.WriteLine("Student removed.");
        }
        //Change a students phone number. Returns true is student found, else false
        static bool ChangePhoneNumber(Student[] students)
        {
            string searchParameter;
            do
            {
                Console.Write("Please enter a students name or phone number: ");
                searchParameter = Console.ReadLine();
                for (int i = 0; i < students.Length; i++)
                {
                    if ((searchParameter == students[i].firstName)//First name match
                        || (searchParameter == students[i].lastName)//Last name match
                        || (searchParameter == students[i].phoneNumber)//Phone number match
                        || (searchParameter == students[i].firstName + " " + students[i].lastName))//Full name match
                    {
                        Console.Write(students[i].firstName.PadRight(15));
                        Console.Write(students[i].lastName.PadRight(15));
                        Console.WriteLine(students[i].phoneNumber.PadLeft(15));
                        Console.Write("Is this the correct student?(y/n): ");
                        if (Console.ReadLine() == "y")
                        {
                            Console.Write("Please enter new phone number: ");
                            students[i].phoneNumber = Console.ReadLine();
                            Console.WriteLine("Phone number changed.");
                            return true;
                        }
                    }
                }
                Console.WriteLine("Student not found.");
                Console.Write("Search again? (y/n)");
            } while (Console.ReadLine() == "y");
            return false;
        }
        //Sort students into ascending order
        public static void Sort(Student[] students)
        {
            Student temp;
            for(int i=0; i<students.Length-1; i++)
            {
                for(int pos=0; pos<students.Length-1; pos++)
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
        //Display current prize pool
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
            int x = rand.Next(1, 3616001);
            for(int i=0; i<prizePool.Length; i++)
            {
                if(x>=prizePool[i].lowerBound && x <= prizePool[i].upperBound)
                {
                    if (prizePool[i].quantity > 0)//Check remaining quantity
                    {
                        prizePool[i].quantity--;//Decrement remaining prizepool
                        prizesRemaining--;//Decrease total prizes remaining
                        return prizePool[i].name;//Return prize name
                    }
                    else
                    {
                        return "Better luck next time";//If no more of that prize remains
                    }
                }
            }
            return "Better luck next time";//If they lose
        }
        //Read student data from file, load into array
        public static void ReadStudentData(ref Student[] students, string file)
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
        public static int LoadPrizePool(Prize[] prizePool,string fileName)
        {
            StreamReader sr = new StreamReader(@fileName);
            int i = 0;
            int totalPrizes = 0;
            while (!sr.EndOfStream)
            {
                prizePool[i] = new Prize(sr.ReadLine(), Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()));
                totalPrizes += prizePool[i].quantity;
                i++;
            }
            return totalPrizes;
        }
        public static void WritePrizes(Prize[] prizePool, string file)
        {
            StreamWriter sw = new StreamWriter(@file);
            foreach(Prize p in prizePool)
            {
                sw.WriteLine(p.name);
                sw.WriteLine(p.quantity);
                sw.WriteLine(p.lowerBound);
                sw.WriteLine(p.upperBound);
            }
            sw.Close();
        }
        //Run Functionality Tests
        public static void RunTests(Student[] students, Prize[] prizePool, ref int ticketsSold, double ticketPrice)
        {
            Console.WriteLine("Sell ten tickets to ten random students:");
            ticketsSold += SellTen(students, prizePool);
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Sell tickets until Fiji is won:");
            int attempts = WinPrize(prizePool, "Fiji");
            Console.WriteLine($"You purchased {attempts} tickets before Fiji was won.");
            Console.WriteLine($"This cost {attempts * ticketPrice:C}");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Purchase $500 of tickets. Report results.");
            PurchaseAmountValue(prizePool, ticketPrice, 500);
        }
    }
}
