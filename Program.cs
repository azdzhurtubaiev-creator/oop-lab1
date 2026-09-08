using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ConsoleApp4
{
    internal class Program
    {
        public const int MinAge = 0;
        public const int MaxAge = 60;

        public const int MinNameLength = 2;
        public const int MaxNameLength = 30;

        public const double MinWeight = 0.1;
        public const double MaxWeight = 500;

        private static readonly List<Pet> _pets = new List<Pet>();
        private static readonly PetValidator _validator = new PetValidator();

        private static int _maxCapacity;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Програма обліку домашніх тварин ===");

            _maxCapacity = ReadPositiveInt("Введіть максимальну кількість тварин: ");

            bool running = true;

            while (running)
            {
                RenderMenu("МЕНЮ",
                    "1. Додати тварину",
                    "2. Показати всіх тварин",
                    "3. Пошук тварини",
                    "4. Поведінка тварини",
                    "5. Видалити тварину",
                    "0. Вийти");

                string choice = Console.ReadLine();

                running = choice switch
                {
                    "1" => AddPetAction(),
                    "2" => ShowAllPetsAction(),
                    "3" => SearchPetsAction(),
                    "4" => DemonstrateBehaviorAction(),
                    "5" => DeletePetAction(),
                    "0" => false,
                    _ => InvalidOption()
                };
            }

            Console.WriteLine("\nПрограму завершено.");
        }
        
        private static void RenderMenu(string title, params string[] items)
        {
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine($"            {title.ToUpper()}");
            Console.WriteLine("================================");

            foreach (var item in items)
            {
                Console.WriteLine(item);
            }

            Console.Write("Ваш вибір: ");
        }

        private static bool InvalidOption()
        {
            Console.WriteLine("Невірний пункт меню!");
            return true;
        }

        private static bool AddPetAction()
        {
            if (_pets.Count >= _maxCapacity)
            {
                Console.WriteLine("Досягнуто максимальну кількість тварин!");
                return true;
            }

            RenderMenu("Додавання тварини",
                "1. Ввести дані вручну",
                "2. Створити автоматично");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => AddPetManually(),
                "2" => AddPetAutomatically(),
                _ => InvalidOption()
            };
        }

        private static bool AddPetManually()
        {
            Console.WriteLine();

            string nickname = ReadString("Введіть кличку: ");
            int age = ReadInt("Введіть вік: ");
            double weight = ReadDouble("Введіть вагу (кг): ");
            bool vaccinated = ReadBool("Тварина вакцинована? (так/ні): ");
            Species type = ReadPetType();

            Pet pet = new Pet();
            pet.Init(Guid.NewGuid(), nickname, age, weight, vaccinated, type);

            if (!ValidatePet(pet))
            {
                return true;
            }

            _pets.Add(pet);
            Console.WriteLine("Тварину успішно додано!");

            return true;
        }

        private static bool AddPetAutomatically()
        {
            Random random = new Random();

            string[] names = { "Barsik", "Murzik", "Bobik", "Rex", "Luna", "Bella", "Rocky", "Max" };

            string nickname = names[random.Next(names.Length)];
            int age = random.Next(MinAge, MaxAge + 1);
            double weight = Math.Round(random.NextDouble() * 30 + 0.5, 2);
            bool vaccinated = random.Next(2) == 1;

            Species[] species = { Species.Cat, Species.Dog, Species.Snake, Species.Hamster, Species.Reptile };
            Species type = species[random.Next(species.Length)];

            Pet pet = new Pet();
            pet.Init(Guid.NewGuid(), nickname, age, weight, vaccinated, type);

            if (!ValidatePet(pet))
            {
                return true;
            }

            _pets.Add(pet);

            Console.WriteLine("\nТварину створено автоматично:");
            PrintPet(pet);

            return true;
        }

        private static bool ValidatePet(Pet pet)
        {
            var result = _validator.Validate(pet);

            if (result.IsValid)
            {
                return true;
            }

            Console.WriteLine("\nПомилки в даних:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.ErrorMessage}");
            }

            return false;
        }
        private static bool ShowAllPetsAction()
        {
            Console.WriteLine();

            if (_pets.Count == 0)
            {
                Console.WriteLine("Список тварин порожній.");
                return true;
            }

            PrintPetsTable(_pets);
            return true;
        }

        private static void PrintPetsTable(IEnumerable<Pet> pets)
        {
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} {1,-12} {2,-5} {3,-10} {4,-15} {5,-10}", "№", "Кличка", "Вік", "Вага (кг)", "Вакцинація", "Вид");
            Console.WriteLine("--------------------------------------------------------------------------------");

            int number = 1;
            foreach (Pet pet in pets)
            {
                Console.WriteLine(
                    "{0,-5} {1,-12} {2,-5} {3,-10:F2} {4,-15} {5,-10}",
                    number,
                    pet.Nickname,
                    pet.Age,
                    pet.Weight,
                    pet.IsVaccinated ? "Так" : "Ні",
                    pet.Type);

                number++;
            }

            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        private static void PrintPet(Pet pet)
        {
            Console.WriteLine($"ID: {pet.Id.ToString().Substring(0, Pet.ShortGuidLength)}");
            Console.WriteLine($"Кличка: {pet.Nickname}");
            Console.WriteLine($"Вік: {pet.Age}");
            Console.WriteLine($"Вага: {pet.Weight:F2} кг");
            Console.WriteLine($"Вакцинація: {(pet.IsVaccinated ? "Так" : "Ні")}");
            Console.WriteLine($"Вид: {pet.Type}");
        }

        private static bool SearchPetsAction()
        {
            if (_pets.Count == 0)
            {
                Console.WriteLine("Список тварин порожній.");
                return true;
            }

            RenderMenu("Пошук",
                "1. За кличкою",
                "2. За віком",
                "3. За вагою",
                "4. За вакцинацією",
                "5. За видом");

            string choice = Console.ReadLine();

            List<Pet>? results = choice switch
            {
                "1" => SearchByNickname(),
                "2" => SearchByAge(),
                "3" => SearchByWeight(),
                "4" => SearchByVaccination(),
                "5" => SearchByType(),
                _ => null
            };

            if (results == null)
            {
                Console.WriteLine("Невірний пункт меню!");
                return true;
            }

            if (results.Count == 0)
            {
                Console.WriteLine("Тварин за заданою характеристикою не знайдено.");
                return true;
            }

            Console.WriteLine("\n=== Результат пошуку ===");
            PrintPetsTable(results);

            return true;
        }

        private static List<Pet> SearchByNickname()
        {
            string nickname = ReadString("Введіть кличку: ");
            return _pets.Where(p => p.HasNickname(nickname)).ToList();
        }

        private static List<Pet> SearchByAge()
        {
            int age = ReadInt("Введіть вік: ");
            return _pets.Where(p => p.HasAge(age)).ToList();
        }

        private static List<Pet> SearchByWeight()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return _pets.Where(p => p.HasWeight(weight)).ToList();
        }

        private static List<Pet> SearchByVaccination()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return _pets.Where(p => p.HasVaccination(vaccinated)).ToList();
        }

        private static List<Pet> SearchByType()
        {
            Species type = ReadPetType();
            return _pets.Where(p => p.HasType(type)).ToList();
        }

    

        private static bool DemonstrateBehaviorAction()
        {
            if (_pets.Count == 0)
            {
                Console.WriteLine("Список тварин порожній.");
                return true;
            }

            Console.WriteLine("\n=== Поведінка тварини ===");
            PrintPetsTable(_pets);

            int number = ReadInt("Виберіть номер тварини: ");

            if (number < 1 || number > _pets.Count)
            {
                Console.WriteLine("Невірний номер тварини!");
                return true;
            }

            Pet pet = _pets[number - 1];

            RenderMenu($"Обрана тварина: {pet.Nickname}",
                "1. Годувати",
                "2. Вигуляти",
                "3. Подати голос",
                "4. Виконати все",
                "0. Назад");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => FeedPetAction(pet),
                "2" => WalkPetAction(pet),
                "3" => SoundPetAction(pet),
                "4" => AllPetActions(pet),
                "0" => true,
                _ => InvalidOption()
            };
        }

        private static bool FeedPetAction(Pet pet)
        {
            double foodWeight = ReadDouble("Введіть кількість корму (кг): ");

            if (!pet.Feed(foodWeight))
            {
                Console.WriteLine("Кількість корму повинна бути більше 0.");
                return true;
            }

            Console.WriteLine($"{pet.Nickname} поїла.");
            Console.WriteLine($"Нова вага: {pet.Weight:F2} кг");

            return true;
        }

        private static bool WalkPetAction(Pet pet)
        {
            Console.WriteLine(pet.Walk());
            return true;
        }

        private static bool SoundPetAction(Pet pet)
        {
            Console.WriteLine($"{pet.Nickname}: {pet.MakeSound()}");
            return true;
        }

        private static bool AllPetActions(Pet pet)
        {
            Console.WriteLine($"\n--- Поведінка {pet.Nickname} ---");
            Console.WriteLine(pet.Walk());
            Console.WriteLine($"{pet.Nickname}: {pet.MakeSound()}");

            double foodWeight = ReadDouble("Введіть кількість корму (кг): ");
            if (pet.Feed(foodWeight))
            {
                Console.WriteLine($"{pet.Nickname} поїла.");
                Console.WriteLine($"Нова вага: {pet.Weight:F2} кг");
            }
            else
            {
                Console.WriteLine("Кількість корму повинна бути більше 0.");
            }

            return true;
        }

 

        private static bool DeletePetAction()
        {
            if (_pets.Count == 0)
            {
                Console.WriteLine("Список тварин порожній.");
                return true;
            }

            RenderMenu("Видалення",
                "1. За порядковим номером у таблиці",
                "2. За кличкою",
                "3. За віком",
                "4. За вагою",
                "5. За вакцинацією",
                "6. За видом");

            string choice = Console.ReadLine();

            int removed = choice switch
            {
                "1" => RemoveByIndex(),
                "2" => RemoveByNickname(),
                "3" => RemoveByAge(),
                "4" => RemoveByWeight(),
                "5" => RemoveByVaccination(),
                "6" => RemoveByType(),
                _ => -1
            };

            if (removed == -1)
            {
                Console.WriteLine("Невірний пункт меню!");
                return true;
            }

            if (removed == 0)
            {
                Console.WriteLine("Тварин за заданим критерієм не знайдено або операцію скасовано.");
                return true;
            }

            Console.WriteLine($"Видалено тварин: {removed}");
            return true;
        }

        private static int RemoveByIndex()
        {
            PrintPetsTable(_pets);
            int index = ReadInt("Введіть порядковий номер тварини для видалення: ");

            if (index < 1 || index > _pets.Count)
            {
                Console.WriteLine("Невірний номер!");
                return 0;
            }

            _pets.RemoveAt(index - 1);
            return 1;
        }

        private static int RemoveByNickname()
        {
            string nickname = ReadString("Введіть кличку: ");
            return _pets.RemoveAll(p => p.HasNickname(nickname));
        }

        private static int RemoveByAge()
        {
            int age = ReadInt("Введіть вік: ");
            return _pets.RemoveAll(p => p.HasAge(age));
        }

        private static int RemoveByWeight()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return _pets.RemoveAll(p => p.HasWeight(weight));
        }

        private static int RemoveByVaccination()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return _pets.RemoveAll(p => p.HasVaccination(vaccinated));
        }

        private static int RemoveByType()
        {
            Species type = ReadPetType();
            return _pets.RemoveAll(p => p.HasType(type));
        }

     

        private static string ReadString(string message)
        {
            while (true)
            {
                Console.Write(message);
                string value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }

                Console.WriteLine("Значення не може бути порожнім!");
            }
        }

        private static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                {
                    return value;
                }

                Console.WriteLine("Введіть ціле число!");
            }
        }

        private static int ReadPositiveInt(string message)
        {
            while (true)
            {
                int value = ReadInt(message);

                if (value > 0)
                {
                    return value;
                }

                Console.WriteLine("Число повинно бути більше 0!");
            }
        }

        private static double ReadDouble(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine()?.Replace(',', '.') ?? "";

                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    return value;
                }

                Console.WriteLine("Введіть правильне число!");
            }
        }

        private static bool ReadBool(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "так" || input == "yes" || input == "y" || input == "1")
                {
                    return true;
                }

                if (input == "ні" || input == "нет" || input == "no" || input == "n" || input == "0")
                {
                    return false;
                }

                Console.WriteLine("Введіть 'так' або 'ні'!");
            }
        }

        private static Species ReadPetType()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Оберіть вид тварини:");
                Console.WriteLine("1. Кіт");
                Console.WriteLine("2. Собака");
                Console.WriteLine("3. Змія");
                Console.WriteLine("4. Хом'як");
                Console.WriteLine("5. Рептилія");
                Console.Write("Ваш вибір: ");

                string input = Console.ReadLine();

                if (int.TryParse(input, out int number) && number >= 1 && number <= 5)
                {
                    return (Species)(number - 1);
                }

                Console.WriteLine("Невірний номер виду!");
            }
        }
    }
}