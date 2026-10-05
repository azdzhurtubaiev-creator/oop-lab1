using System;
using System.Globalization;

namespace ConsoleApp4
{
    public class Pet
    {
        public const int ShortGuidLength = 8;

        public const int MinAge = 0;
        public const int MaxAge = 60;

        public const int MinNameLength = 2;
        public const int MaxNameLength = 30;

        public const double MinWeight = 0.1;
        public const double MaxWeight = 500;

        public const double MinFoodWeight = 0.01;

        public const string DefaultNickname = "Безіменний";
        public const double DefaultWeight = 1.0;
        public const string DefaultOwner = "Не вказано";

  
        public const double DefaultFoodConversionRatio = 0.1;
        public const double MinFoodConversionRatio = 0.01;
        public const double MaxFoodConversionRatio = 0.5;

       
        public const char Separator = ';';
        private const int PartsCount = 6;

 
        private static int _createdCount;
        private static double _foodConversionRatio;

        private string _nickname;
        private int _age;
        private double _weight;
        private bool _isVaccinated;
        private Species _type;

     
        static Pet()
        {
            _createdCount = 0;
            _foodConversionRatio = DefaultFoodConversionRatio;
        }

  
        public Pet()
            : this(DefaultNickname, Species.Cat)
        {
            UsedConstructor = "Pet() - конструктор без параметрів";
        }

      
        public Pet(string nickname, Species type)
            : this(nickname, type, MinAge, DefaultWeight)
        {
            UsedConstructor = "Pet(nickname, type) - конструктор з двома параметрами";
        }

      
        public Pet(string nickname, Species type, int age, double weight)
            : this(nickname, type, age, weight, false, DefaultOwner)
        {
            UsedConstructor = "Pet(nickname, type, age, weight) - конструктор з чотирма параметрами";
        }

 
        public Pet(string nickname, Species type, int age, double weight, bool isVaccinated, string owner)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            FeededAt = DateTime.MinValue;

            Nickname = nickname;
            Type = type;
            Age = age;
            Weight = weight;
            IsVaccinated = isVaccinated;
            Owner = string.IsNullOrWhiteSpace(owner) ? DefaultOwner : owner.Trim();

            UsedConstructor = "Pet(nickname, type, age, weight, isVaccinated, owner) - повний конструктор";

            _createdCount++;
        }
 
        public static int CreatedCount
        {
            get
            {
                return _createdCount;
            }
        }

 
        public static double FoodConversionRatio
        {
            get
            {
                return _foodConversionRatio;
            }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentException(
                        "Коефіцієнт має бути дійсним числом!",
                        nameof(FoodConversionRatio));
                }

                if (value < MinFoodConversionRatio || value > MaxFoodConversionRatio)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(FoodConversionRatio),
                        $"Коефіцієнт має бути від {MinFoodConversionRatio} до {MaxFoodConversionRatio}!");
                }

                _foodConversionRatio = value;
            }
        }

        public string UsedConstructor { get; private set; }

        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime FeededAt { get; private set; }

        public string Owner { get; set; } = DefaultOwner;

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

                if (nickname.Contains(Separator))
                {
                    throw new ArgumentException(
                        $"Кличка не може містити символ '{Separator}'!",
                        nameof(Nickname));
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
 
        public override string ToString()
        {
            string weight = Weight.ToString("0.##", CultureInfo.InvariantCulture);
            string vaccinated = IsVaccinated ? "так" : "ні";

            return $"{Nickname}{Separator}{Type}{Separator}{Age}{Separator}{weight}{Separator}{vaccinated}{Separator}{Owner}";
        }

   
        public static Pet Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(
                    nameof(s),
                    "Рядок не може бути порожнім!");
            }

            string[] parts = s.Split(Separator);

            if (parts.Length != PartsCount)
            {
                throw new FormatException(
                    $"Рядок має містити {PartsCount} значень, розділених символом '{Separator}', " +
                    $"а містить {parts.Length}. Формат: кличка;вид;вік;вага;вакцинація;власник");
            }

            string nickname = parts[0].Trim();
            Species type = ParseSpecies(parts[1].Trim());
            int age = ParseAge(parts[2].Trim());
            double weight = ParseWeight(parts[3].Trim());
            bool isVaccinated = ParseVaccination(parts[4].Trim());
            string owner = parts[5].Trim();

     
            Pet pet = new Pet(nickname, type, age, weight, isVaccinated, owner);

          
            pet.UsedConstructor = "Pet.Parse(string) -> повний конструктор";

            return pet;
        }

 
        public static bool TryParse(string s, out Pet obj)
        {
            return TryParse(s, out obj, out _);
        }

        public static bool TryParse(string s, out Pet obj, out string error)
        {
            obj = null;
            error = null;

            try
            {
                obj = Parse(s);
                return true;
            }
            catch (Exception ex)
            {
                error = CleanMessage(ex);
                return false;
            }
        }

        
        public static int ToHumanAge(int age, Species type)
        {
            if (age < MinAge || age > MaxAge)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(age),
                    $"Вік має бути від {MinAge} до {MaxAge} років!");
            }

            switch (type)
            {
                case Species.Cat:
                case Species.Dog:
                    if (age == 0)
                    {
                        return 0;
                    }

                    if (age == 1)
                    {
                        return 15;
                    }

                    int yearStep = type == Species.Cat ? 4 : 5;
                    return 24 + (age - 2) * yearStep;

                case Species.Hamster:
                    return age * 25;

                case Species.Snake:
                case Species.Reptile:
                    return age * 2;

                default:
                    throw new ArgumentException(
                        "Обрано неіснуючий вид тварини!",
                        nameof(type));
            }
        }
 
        public static bool CanLiveTogether(Species first, Species second)
        {
            if (first == second)
            {
                return true;
            }

            return !IsDangerousPair(first, second) && !IsDangerousPair(second, first);
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

       

        private static Species ParseSpecies(string text)
        {
            if (int.TryParse(text, out int number))
            {
                if (number < 1 || number > 5)
                {
                    throw new FormatException(
                        $"Номер виду '{text}' має бути від 1 до 5!");
                }

                return (Species)(number - 1);
            }

            if (Enum.TryParse(text, true, out Species type) && Enum.IsDefined(typeof(Species), type))
            {
                return type;
            }

            throw new FormatException(
                $"Невідомий вид тварини '{text}'. Допустимі: Cat, Dog, Snake, Hamster, Reptile або номер 1-5");
        }

        private static int ParseAge(string text)
        {
            if (!int.TryParse(text, out int age))
            {
                throw new FormatException(
                    $"Вік '{text}' не є цілим числом!");
            }

            return age;
        }

        private static double ParseWeight(string text)
        {
            string prepared = text.Replace(',', '.');

            if (!double.TryParse(prepared, NumberStyles.Float, CultureInfo.InvariantCulture, out double weight))
            {
                throw new FormatException(
                    $"Вага '{text}' не є числом!");
            }

            return weight;
        }

        private static bool ParseVaccination(string text)
        {
            string prepared = text.ToLower();

            if (prepared == "так" || prepared == "true" || prepared == "yes" || prepared == "1")
            {
                return true;
            }

            if (prepared == "ні" || prepared == "false" || prepared == "no" || prepared == "0")
            {
                return false;
            }

            throw new FormatException(
                $"Значення вакцинації '{text}' некоректне. Допустимі: так / ні");
        }

        private static bool IsDangerousPair(Species predator, Species prey)
        {
            if (prey == Species.Hamster)
            {
                return predator == Species.Cat || predator == Species.Dog || predator == Species.Snake;
            }

            if (prey == Species.Snake)
            {
                return predator == Species.Cat || predator == Species.Dog;
            }

            return false;
        }

        private static string CleanMessage(Exception ex)
        {
            string message = ex.Message;

            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);

            if (index > 0)
            {
                message = message.Substring(0, index);
            }

            return message;
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
