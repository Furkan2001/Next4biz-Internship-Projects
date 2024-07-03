using System;

namespace lessonLetterGradeMiniProject
{
	public class Student
	{
		public string Name { get; private set; }
		public int Number { get; private set; }
        public double Midterm1 { get; private set; }
        public double Midterm2 { get; private set; }
        public double Final { get; private set; }
		public string LetterGrade { get; private set; }
		public double Avarage { get; private set; }

        public Student(string name, int number, double midterm1, double midterm2, double final)
		{
			this.Name = name;
			this.Number = number;
			this.Midterm1 = midterm1;
			this.Midterm2 = midterm2;
			this.Final = final;
			this.LetterGrade = SetLetterGrade(this.Midterm1, this.Midterm2, this.Final);

        }

		public void DisplayInfo()
		{
            Console.WriteLine($"Name: {this.Name}, Number: {this.Number}, Midterm1: {this.Midterm1}, Midterm2: {this.Midterm2}, Final: {this.Final}");
			if (this.LetterGrade == "F")
			{
				Console.WriteLine($"You are failed, avarage: {this.Avarage}\n");
			}
			else
			{
				Console.WriteLine($"You are succeeded, avarage: {this.Avarage}\n");
			}
        }

		public string SetLetterGrade(double mid1, double mid2, double final)
		{
            double avarage = (mid1 * 0.2) + (mid2 * 0.2) + (final * 0.6);

            this.Avarage = avarage;

            if (final < 50)
			{
				return "F";
			}

			if (avarage > 90)
			{
				return "A";
			}
			else if (avarage > 80)
			{
				return "B";
			}
			else if (avarage > 70)
			{
				return "C";
			}
			else if (avarage > 60)
			{
				return "D";
			}
			else
			{
				return "F";
			}
		}

        public override string ToString()
        {
            return $"{this.Name},{this.Number},{this.Midterm1},{this.Midterm2},{this.Final},{this.Avarage},{this.LetterGrade}";
        }
    }
}