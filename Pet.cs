using System;

namespace ConsoleApp4
{
    public class Pet
    {
        public const int ShortGuidLength = 8;
        public const double FoodConversionRatio = 0.1;

        public const int MinAge = 0;
        public const int MaxAge = 60;

        public const int MinNameLength = 2;
        public const int MaxNameLength = 30;

        public const double MinWeight = 0.1;
        public const double MaxWeight = 500;

        public const double MinFoodWeight = 0.01;

    
        public const string DefaultNickname = "Безіменний";
        public const double DefaultWeight = 1.0;

        private string _nickname;
        private int _age;
        private double _weight;
        private bool _isVaccinated;
        private Species _type;

      
        public Pet()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            FeededAt = DateTime.MinValue;

            Nickname = DefaultNickname;
            Age = MinAge;
            Weight = DefaultWeight;
            IsVaccinated = false;
            Type = Species.Cat;

            UsedConstructor = "Pet() - конструктор без параметрів";
        }

     
        public Pet(string nickname, Species type)
            : this()
        {
            Nickname = nickname;
            Type = type;

            UsedConstructor = "Pet(nickname, type) - конструктор з двома параметрами";
        }

    
        public Pet(string nickname, Species type, int age, double weight)
            : this(nickname, type)
        {
            Age = age;
            Weight = weight;

            UsedConstructor = "Pet(nickname, type, age, weight) - конструктор з чотирма параметрами";
        }

         
        public Pet(string nickname, Species type, int age, double weight, bool isVaccinated, string owner)
            : this(nickname, type, age, weight)
        {
            IsVaccinated = isVaccinated;
            Owner = owner;

            UsedConstructor = "Pet(nickname, type, age, weight, isVaccinated, owner) - повний конструктор";
        }

   
        public string UsedConstructor { get; private set; }

        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime FeededAt { get; private set; }

        public string Owner { get; set; } = "Не вказано";

        public string Nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(
                        nameof(Nickname),
                        "Кличка не може бути порожньою!");
                }

                string nickname = value.Trim();

                if (nickname.Length < MinNameLength || nickname.Length > MaxNameLength)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Nickname),
                        $"Довжина клички має бути від {MinNameLength} до {MaxNameLength} символів!");
                }

                _nickname = nickname;
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (value < MinAge || value > MaxAge)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Age),
                        $"Вік має бути від {MinAge} до {MaxAge} років!");
                }

                _age = value;
            }
        }

        public double Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentException(
                        "Вага має бути дійсним числом!",
                        nameof(Weight));
                }

                if (value < MinWeight || value > MaxWeight)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Weight),
                        $"Вага має бути від {MinWeight} до {MaxWeight} кг!");
                }

                _weight = Math.Round(value, 2);
            }
        }

        public bool IsVaccinated
        {
            get
            {
                return _isVaccinated;
            }
            set
            {
                _isVaccinated = value;
            }
        }

        public Species Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (!Enum.IsDefined(typeof(Species), value))
                {
                    throw new ArgumentException(
                        "Обрано неіснуючий вид тварини!",
                        nameof(Type));
                }

                _type = value;
            }
        }

        public string ShortId
        {
            get
            {
                return Id.ToString().Substring(0, ShortGuidLength);
            }
        }

        public double DailyFoodNorm
        {
            get
            {
                return Math.Round(Weight * FoodConversionRatio, 2);
            }
        }

        public string AgeCategory
        {
            get
            {
                if (Age <= 1)
                {
                    return "Малюк";
                }

                if (Age <= 8)
                {
                    return "Дорослий";
                }

                return "Літній";
            }
        }

        public bool IsHungry
        {
            get
            {
                return FeededAt == DateTime.MinValue;
            }
        }

        public bool HasNickname(string nickname)
        {
            return Normalize(Nickname).Equals(
                Normalize(nickname),
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
 
        public string Feed()
        {
            return Feed(DailyFoodNorm);
        }

      
        public string Feed(double foodWeight)
        {
            EnsureFoodWeightIsValid(foodWeight);

            Weight = Weight + foodWeight;
            FeededAt = DateTime.Now;

            return $"{Nickname} отримала {foodWeight:F2} кг корму. Нова вага: {Weight:F2} кг";
        }

  
        public string Feed(double foodWeight, int times)
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(times),
                    "Кількість годувань має бути більше 0!");
            }

            EnsureFoodWeightIsValid(foodWeight);

            for (int i = 0; i < times; i++)
            {
                Weight = Weight + foodWeight;
            }

            FeededAt = DateTime.Now;

            return $"{Nickname} отримала {times} порції по {foodWeight:F2} кг. Нова вага: {Weight:F2} кг";
        }

      
        public string Walk()
        {
            return $"{Nickname} гуляє на вулиці!";
        }

      
        public string Walk(int minutes)
        {
            if (minutes <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minutes),
                    "Тривалість прогулянки має бути більше 0 хвилин!");
            }

            return $"{Nickname} гуляє на вулиці {minutes} хв!";
        }

   
        public string MakeSound()
        {
            return $"{Nickname}: {GetVoice()}";
        }

        
        public string MakeSound(int times)
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(times),
                    "Кількість повторів має бути більше 0!");
            }

            string voice = string.Empty;

            for (int i = 0; i < times; i++)
            {
                voice = voice + GetVoice() + " ";
            }

            return $"{Nickname}: {voice.Trim()}";
        }

        private string GetVoice()
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

        private void EnsureFoodWeightIsValid(double foodWeight)
        {
            if (double.IsNaN(foodWeight) || double.IsInfinity(foodWeight))
            {
                throw new ArgumentException(
                    "Кількість корму має бути дійсним числом!",
                    nameof(foodWeight));
            }

            if (foodWeight < MinFoodWeight)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(foodWeight),
                    $"Кількість корму має бути не менше {MinFoodWeight} кг!");
            }
        }

        private string Normalize(string text)
        {
            return text == null ? string.Empty : text.Trim();
        }
    }
}