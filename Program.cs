using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
 
namespace ConsoleApp4
{
    internal class Program
    {
        private static readonly List<Pet> _pets = new List<Pet>();
 
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
 
            Pet pet = new Pet();
 
            SetProperty("Введіть кличку: ",
                value => pet.Nickname = value);
 
            SetProperty("Введіть вік: ",
                value => pet.Age = ParseInt(value));
 
            SetProperty("Введіть вагу (кг): ",
                value => pet.Weight = ParseDouble(value));
 
            SetProperty("Тварина вакцинована? (так/ні): ",
                value => pet.IsVaccinated = ParseBool(value));
 
            SetProperty("Введіть ім'я власника (Enter - пропустити): ",
                value => pet.Owner = string.IsNullOrWhiteSpace(value) ? "Не вказано" : value.Trim());
 
            PrintSpeciesMenu();
            SetProperty("Ваш вибір: ",
                value => pet.Type = ParseSpecies(value));
 
            _pets.Add(pet);
 
            Console.WriteLine("Тварину успішно додано!");
            return true;
        }
 
        private static bool AddPetAutomatically()
        {
            Random random = new Random();
 
            string[] names = { "Barsik", "Murzik", "Bobik", "Rex", "Luna", "Bella", "Rocky", "Max" };
            string[] owners = { "Іваненко І.", "Петренко П.", "Коваль О.", "Не вказано" };
 
            Species[] species = { Species.Cat, Species.Dog, Species.Snake, Species.Hamster, Species.Reptile };
 
            Pet pet = new Pet();
 
            try
            {
                pet.Init(
                    names[random.Next(names.Length)],
                    random.Next(Pet.MinAge, Pet.MaxAge + 1),
                    Math.Round(random.NextDouble() * 30 + 0.5, 2),
                    random.Next(2) == 1,
                    species[random.Next(species.Length)]);
 
                pet.Owner = owners[random.Next(owners.Length)];
            }
            catch (Exception ex)
            {
                PrintError(ex);
                return true;
            }
 
            _pets.Add(pet);
 
            Console.WriteLine("\nТварину створено автоматично:");
            PrintPet(pet);
 
            return true;
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
            string line = new string('-', 110);
 
            Console.WriteLine(line);
            Console.WriteLine("{0,-4} {1,-12} {2,-5} {3,-10} {4,-12} {5,-10} {6,-10} {7,-10} {8,-14}",
                "№", "Кличка", "Вік", "Вага (кг)", "Вакцинація", "Вид", "Категорія", "Норма", "Власник");
            Console.WriteLine(line);
 
            int number = 1;
            foreach (Pet pet in pets)
            {
                Console.WriteLine(
                    "{0,-4} {1,-12} {2,-5} {3,-10:F2} {4,-12} {5,-10} {6,-10} {7,-10:F2} {8,-14}",
                    number,
                    pet.Nickname,
                    pet.Age,
                    pet.Weight,
                    pet.IsVaccinated ? "Так" : "Ні",
                    pet.Type,
                    pet.AgeCategory,
                    pet.DailyFoodNorm,
                    pet.Owner);
 
                number++;
            }
 
            Console.WriteLine(line);
        }
 
        private static void PrintPet(Pet pet)
        {
            Console.WriteLine($"ID: {pet.ShortId}");
            Console.WriteLine($"Кличка: {pet.Nickname}");
            Console.WriteLine($"Вік: {pet.Age} ({pet.AgeCategory})");
            Console.WriteLine($"Вага: {pet.Weight:F2} кг");
            Console.WriteLine($"Вакцинація: {(pet.IsVaccinated ? "Так" : "Ні")}");
            Console.WriteLine($"Вид: {pet.Type}");
            Console.WriteLine($"Добова норма корму: {pet.DailyFoodNorm:F2} кг");
            Console.WriteLine($"Власник: {pet.Owner}");
            Console.WriteLine($"Картку створено: {pet.CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Голодна: {(pet.IsHungry ? "Так" : "Ні")}");
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
            Species type = ReadSpecies();
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
                "4. Показати картку",
                "5. Виконати все",
                "0. Назад");
 
            string choice = Console.ReadLine();
 
            return choice switch
            {
                "1" => FeedPetAction(pet),
                "2" => WalkPetAction(pet),
                "3" => SoundPetAction(pet),
                "4" => ShowCardAction(pet),
                "5" => AllPetActions(pet),
                "0" => true,
                _ => InvalidOption()
            };
        }
 
 
        private static bool FeedPetAction(Pet pet)
        {
            double foodWeight = ReadDouble("Введіть кількість корму (кг): ");
 
            try
            {
                pet.Feed(foodWeight);
 
                Console.WriteLine($"{pet.Nickname} поїла.");
                Console.WriteLine($"Нова вага: {pet.Weight:F2} кг");
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }
 
            return true;
        }
 
        private static bool WalkPetAction(Pet pet)
        {
            Console.WriteLine(pet.Walk());
            return true;
        }
 
        private static bool SoundPetAction(Pet pet)
        {
            Console.WriteLine(pet.MakeSound());
            return true;
        }
 
        private static bool ShowCardAction(Pet pet)
        {
            Console.WriteLine();
            PrintPet(pet);
            return true;
        }
 
        private static bool AllPetActions(Pet pet)
        {
            Console.WriteLine($"\n--- Поведінка {pet.Nickname} ---");
            Console.WriteLine(pet.Walk());
            Console.WriteLine(pet.MakeSound());
 
            FeedPetAction(pet);
            ShowCardAction(pet);
 
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
            Species type = ReadSpecies();
            return _pets.RemoveAll(p => p.HasType(type));
        }
 
   
        private static void SetProperty(string message, Action<string> setter)
        {
            bool hasError;
 
            do
            {
                hasError = false;
 
                Console.Write(message);
                string input = Console.ReadLine();
 
                try
                {
                    setter(input);
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);
        }
 
 
        private static void PrintError(Exception ex)
        {
            string message = ex.Message;
 
            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);
 
            if (index > 0)
            {
                message = message.Substring(0, index);
            }
 
            Console.WriteLine($"Помилка: {message}");
        }
 
        private static void PrintSpeciesMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Оберіть вид тварини:");
            Console.WriteLine("1. Кіт");
            Console.WriteLine("2. Собака");
            Console.WriteLine("3. Змія");
            Console.WriteLine("4. Хом'як");
            Console.WriteLine("5. Рептилія");
        }
 
        private static int ParseInt(string input)
        {
            if (!int.TryParse(input, out int value))
            {
                throw new FormatException("Введіть ціле число!");
            }
 
            return value;
        }
 
        private static double ParseDouble(string input)
        {
            string prepared = (input ?? string.Empty).Replace(',', '.');
 
            if (!double.TryParse(prepared, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                throw new FormatException("Введіть правильне число!");
            }
 
            return value;
        }
 
        private static bool ParseBool(string input)
        {
            string prepared = (input ?? string.Empty).Trim().ToLower();
 
            if (prepared == "так" || prepared == "yes" || prepared == "y" || prepared == "1")
            {
                return true;
            }
 
            if (prepared == "ні" || prepared == "нет" || prepared == "no" || prepared == "n" || prepared == "0")
            {
                return false;
            }
 
            throw new FormatException("Введіть 'так' або 'ні'!");
        }
 
        private static Species ParseSpecies(string input)
        {
            int number = ParseInt(input);
 
            
            return (Species)(number - 1);
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
 
                try
                {
                    return ParseInt(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
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
 
                try
                {
                    return ParseDouble(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
            }
        }
 
        private static bool ReadBool(string message)
        {
            while (true)
            {
                Console.Write(message);
 
                try
                {
                    return ParseBool(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
            }
        }
 
        private static Species ReadSpecies()
        {
            while (true)
            {
                PrintSpeciesMenu();
                Console.Write("Ваш вибір: ");
 
                string input = Console.ReadLine();
 
                try
                {
                    Species type = ParseSpecies(input);
 
                    if (!Enum.IsDefined(typeof(Species), type))
                    {
                        throw new ArgumentException("Невірний номер виду!");
                    }
 
                    return type;
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
            }
        }
    }
}
