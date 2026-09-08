using System;

namespace ConsoleApp4
{
    public class Pet
    {
        public const int ShortGuidLength = 8;
        public const double FoodConversionRatio = 0.1;
        
        public Guid Id;
        public string Nickname;
        public int Age;
        public double Weight;
        public bool IsVaccinated;
        public Species Type;

        
        private DateTime CreatedAt;
        private DateTime FeededAt;

        public void Init(
            Guid id,
            string nickname,
            int age,
            double weight,
            bool isVaccinated,
            Species type)
        {
            Id = id;
            Nickname = nickname;
            Age = age;
            Weight = weight;
            IsVaccinated = isVaccinated;
            Type = type;

            CreatedAt = DateTime.Now;
            FeededAt = DateTime.MinValue;
        }

   

        public bool HasNickname(string nickname)
        {
            return Nickname.Equals(
                nickname,
                StringComparison.OrdinalIgnoreCase);
        }

        public bool HasAge(int age)
        {
            return Age == age;
        }

        public bool HasWeight(double weight)
        {
            return Math.Abs(Weight - weight) < 0.001;
        }

        public bool HasVaccination(bool vaccinated)
        {
            return IsVaccinated == vaccinated;
        }

        public bool HasType(Species type)
        {
            return Type == type;
        }

      

        public bool Feed(double foodWeight)
        {
            if (foodWeight <= 0)
            {
                return false;
            }

            Weight = Math.Round(
                Weight + foodWeight,
                2);

            FeededAt = DateTime.Now;

            return true;
        }

        public string Walk()
        {
            return $"{Nickname} гуляє на вулиці!";
        }

        public string MakeSound()
        {
            return Type switch
            {
                Species.Cat => "Мяу!",
                Species.Dog => "Гав!",
                Species.Snake => "Ш-ш-ш!",
                Species.Hamster => "Пи-пи!",
                Species.Reptile => "Хр-р!",
                _ => "Звук тварини!"
            };
        }
    }
}