using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ConsoleApp4
{
    internal class Program
    {
        private readonly List<Pet> _pets = new List<Pet>();

        private readonly Random _random = new Random();

        private int _maxCapacity;

        static void Main(string[] args)
        {
           
            Program application = new Program();
            application.Run();
        }

        private void Run()
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
                    "6. Продемонструвати static-методи",
                    "0. Вийти");

                string choice = Console.ReadLine();

                running = choice switch
                {
                    "1" => AddPetAction(),
                    "2" => ShowAllPetsAction(),
                    "3" => SearchPetsAction(),
                    "4" => DemonstrateBehaviorAction(),
                    "5" => DeletePetAction(),
                    "6" => DemonstrateStaticAction(),
                    "0" => false,
                    _ => InvalidOption()
                };
            }

            Console.WriteLine("\nПрограму завершено.");
        }

        private void RenderMenu(string title, params string[] items)
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

        private bool InvalidOption()
        {
            Console.WriteLine("Невірний пункт меню!");
            return true;
        }

        private bool AddPetAction()
        {
            if (_pets.Count >= _maxCapacity)
            {
                Console.WriteLine("Досягнуто максимальну кількість тварин!");
                return true;
            }

            RenderMenu("Додавання тварини",
                "1. Ввести дані вручну",
                "2. Створити автоматично (випадковий конструктор)",
                "3. Ввести рядком (перетворення через TryParse)");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => AddPetManually(),
                "2" => AddPetAutomatically(),
                "3" => AddPetFromString(),
                _ => InvalidOption()
            };
        }
 
        private bool AddPetManually()
        {
            Console.WriteLine();

            Pet pet = new Pet();

            bool hasError;

            do
            {
                hasError = false;
                Console.Write("Введіть кличку: ");

                try
                {
                    pet.Nickname = Console.ReadLine();
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);

            do
            {
                hasError = false;
                Console.Write("Введіть вік: ");

                try
                {
                    pet.Age = ParseInt(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);

            do
            {
                hasError = false;
                Console.Write("Введіть вагу (кг): ");

                try
                {
                    pet.Weight = ParseDouble(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);

            do
            {
                hasError = false;
                Console.Write("Тварина вакцинована? (так/ні): ");

                try
                {
                    pet.IsVaccinated = ParseBool(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);

            Console.Write("Введіть ім'я власника (Enter - пропустити): ");
            string owner = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(owner))
            {
                pet.Owner = owner.Trim();
            }

            do
            {
                hasError = false;
                PrintSpeciesMenu();
                Console.Write("Ваш вибір: ");

                try
                {
                    pet.Type = ParseSpecies(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                    hasError = true;
                }
            }
            while (hasError);

            _pets.Add(pet);

            Console.WriteLine("Тварину успішно додано!");
            Console.WriteLine($"Спрацював конструктор: {pet.UsedConstructor}");

            return true;
        }
 
        private bool AddPetAutomatically()
        {
            string[] names = { "Barsik", "Murzik", "Bobik", "Rex", "Luna", "Bella", "Rocky", "Max" };
            string[] owners = { "Іваненко І.", "Петренко П.", "Коваль О.", "Не вказано" };

            Species[] species = { Species.Cat, Species.Dog, Species.Snake, Species.Hamster, Species.Reptile };

            string nickname = names[_random.Next(names.Length)];
            Species type = species[_random.Next(species.Length)];
            int age = _random.Next(Pet.MinAge, Pet.MaxAge + 1);
            double weight = Math.Round(_random.NextDouble() * 30 + 0.5, 2);
            bool vaccinated = _random.Next(2) == 1;
            string owner = owners[_random.Next(owners.Length)];

            int constructorNumber = _random.Next(1, 5);

            Pet pet;

            try
            {
                pet = constructorNumber switch
                {
                   
                    1 => new Pet
                    {
                        Nickname = nickname,
                        Type = type,
                        Age = age,
                        Weight = weight,
                        IsVaccinated = vaccinated,
                        Owner = owner
                    },
                    2 => new Pet(nickname, type),
                    3 => new Pet(nickname, type, age, weight),
                    _ => new Pet(nickname, type, age, weight, vaccinated, owner)
                };
            }
            catch (Exception ex)
            {
                PrintError(ex);
                return true;
            }

            _pets.Add(pet);

            Console.WriteLine("\nТварину створено автоматично.");
            Console.WriteLine($"Спрацював конструктор: {pet.UsedConstructor}");

            if (constructorNumber == 1)
            {
                Console.WriteLine("(значення полів задані за допомогою ініціалізаторів об'єкта)");
            }

            Console.WriteLine();
            PrintPet(pet);

            return true;
        }
 
        private bool AddPetFromString()
        {
            Console.WriteLine();
            Console.WriteLine("Формат: кличка;вид;вік;вага;вакцинація;власник");
            Console.WriteLine("Вид: Cat, Dog, Snake, Hamster, Reptile або номер 1-5");
            Console.WriteLine("Приклад: Barsik;Cat;3;4.5;так;Іваненко І.");

            while (true)
            {
                Console.Write("\nВведіть рядок (Enter - скасувати): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Додавання скасовано.");
                    return true;
                }

                if (Pet.TryParse(input, out Pet pet, out string error))
                {
                    _pets.Add(pet);

                    Console.WriteLine("Рядок успішно перетворено, тварину додано!");
                    Console.WriteLine($"Спрацював: {pet.UsedConstructor}");
                    Console.WriteLine();
                    PrintPet(pet);

                    return true;
                }

                Console.WriteLine($"Помилка: {error}");
                Console.WriteLine("Спробуйте ще раз.");
            }
        }

        private bool ShowAllPetsAction()
        {
            Console.WriteLine();

            if (_pets.Count == 0)
            {
                Console.WriteLine("Список тварин порожній.");
                PrintCounters();
                return true;
            }

            PrintPetsTable(_pets);
            PrintCounters();
            return true;
        }
 
        private void PrintCounters()
        {
            Console.WriteLine($"Тварин у списку: {_pets.Count} з {_maxCapacity}");
            Console.WriteLine($"Коректно створено об'єктів Pet за весь час роботи програми: {Pet.CreatedCount}");
            Console.WriteLine($"Коефіцієнт добової норми корму: {Pet.FoodConversionRatio}");
        }

        private void PrintPetsTable(IEnumerable<Pet> pets)
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

        private void PrintPet(Pet pet)
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
            Console.WriteLine($"Створено: {pet.UsedConstructor}");
            Console.WriteLine($"Рядкове представлення (ToString): {pet}");
        }

        private bool SearchPetsAction()
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

        private List<Pet> SearchByNickname()
        {
            string nickname = ReadString("Введіть кличку: ");
            return _pets.Where(p => p.HasNickname(nickname)).ToList();
        }

        private List<Pet> SearchByAge()
        {
            int age = ReadInt("Введіть вік: ");
            return _pets.Where(p => p.HasAge(age)).ToList();
        }

        private List<Pet> SearchByWeight()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return _pets.Where(p => p.HasWeight(weight)).ToList();
        }

        private List<Pet> SearchByVaccination()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return _pets.Where(p => p.HasVaccination(vaccinated)).ToList();
        }

        private List<Pet> SearchByType()
        {
            Species type = ReadSpecies();
            return _pets.Where(p => p.HasType(type)).ToList();
        }

        private bool DemonstrateBehaviorAction()
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
                "1. Годувати - Feed()",
                "2. Годувати - Feed(кількість корму)",
                "3. Годувати - Feed(кількість корму, кількість порцій)",
                "4. Вигуляти - Walk()",
                "5. Вигуляти - Walk(хвилини)",
                "6. Подати голос - MakeSound()",
                "7. Подати голос - MakeSound(кількість повторів)",
                "8. Показати картку",
                "9. Продемонструвати всі перевантажені методи",
                "0. Назад");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => FeedDefaultAction(pet),
                "2" => FeedAmountAction(pet),
                "3" => FeedTimesAction(pet),
                "4" => WalkAction(pet),
                "5" => WalkMinutesAction(pet),
                "6" => SoundAction(pet),
                "7" => SoundTimesAction(pet),
                "8" => ShowCardAction(pet),
                "9" => DemonstrateOverloadsAction(pet),
                "0" => true,
                _ => InvalidOption()
            };
        }

         private bool FeedDefaultAction(Pet pet)
        {
            try
            {
                Console.WriteLine($"Годування добовою нормою ({pet.DailyFoodNorm:F2} кг):");
                Console.WriteLine(pet.Feed());
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool FeedAmountAction(Pet pet)
        {
            double foodWeight = ReadDouble("Введіть кількість корму (кг): ");

            try
            {
                Console.WriteLine(pet.Feed(foodWeight));
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool FeedTimesAction(Pet pet)
        {
            double foodWeight = ReadDouble("Введіть кількість корму на одну порцію (кг): ");
            int times = ReadInt("Введіть кількість порцій: ");

            try
            {
                Console.WriteLine(pet.Feed(foodWeight, times));
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool WalkAction(Pet pet)
        {
            Console.WriteLine(pet.Walk());
            return true;
        }

        private bool WalkMinutesAction(Pet pet)
        {
            int minutes = ReadInt("Введіть тривалість прогулянки (хв): ");

            try
            {
                Console.WriteLine(pet.Walk(minutes));
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool SoundAction(Pet pet)
        {
            Console.WriteLine(pet.MakeSound());
            return true;
        }

        private bool SoundTimesAction(Pet pet)
        {
            int times = ReadInt("Введіть кількість повторів: ");

            try
            {
                Console.WriteLine(pet.MakeSound(times));
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool ShowCardAction(Pet pet)
        {
            Console.WriteLine();
            PrintPet(pet);
            return true;
        }

 
        private bool DemonstrateOverloadsAction(Pet pet)
        {
            Console.WriteLine($"\n--- Перевантажені методи класу Pet для {pet.Nickname} ---");

            try
            {
                Console.WriteLine("\nWalk():");
                Console.WriteLine(pet.Walk());

                Console.WriteLine("\nWalk(30):");
                Console.WriteLine(pet.Walk(30));

                Console.WriteLine("\nMakeSound():");
                Console.WriteLine(pet.MakeSound());

                Console.WriteLine("\nMakeSound(3):");
                Console.WriteLine(pet.MakeSound(3));

                Console.WriteLine("\nFeed():");
                Console.WriteLine(pet.Feed());

                Console.WriteLine("\nFeed(0.5):");
                Console.WriteLine(pet.Feed(0.5));

                Console.WriteLine("\nFeed(0.2, 3):");
                Console.WriteLine(pet.Feed(0.2, 3));
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }
 
        private bool DemonstrateStaticAction()
        {
            RenderMenu("Static-методи класу Pet",
                "1. Лічильник створених об'єктів (Pet.CreatedCount)",
                "2. Змінити коефіцієнт добової норми (Pet.FoodConversionRatio)",
                "3. Вік у людських роках (Pet.ToHumanAge)",
                "4. Чи можна тримати два види разом (Pet.CanLiveTogether)",
                "5. Parse і TryParse на прикладах",
                "0. Назад");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => ShowCounterAction(),
                "2" => ChangeRatioAction(),
                "3" => HumanAgeAction(),
                "4" => LiveTogetherAction(),
                "5" => ParseDemoAction(),
                "0" => true,
                _ => InvalidOption()
            };
        }

        private bool ShowCounterAction()
        {
            Console.WriteLine();
            PrintCounters();
            Console.WriteLine("Лічильник не зменшується при видаленні: він рахує створені об'єкти, а не ті, що зараз у списку.");
            return true;
        }

        private bool ChangeRatioAction()
        {
            Console.WriteLine($"\nПоточний коефіцієнт: {Pet.FoodConversionRatio}");

            if (_pets.Count > 0)
            {
                Console.WriteLine("Добова норма ДО зміни:");
                PrintPetsTable(_pets);
            }

            double ratio = ReadDouble($"Новий коефіцієнт ({Pet.MinFoodConversionRatio} - {Pet.MaxFoodConversionRatio}): ");

            try
            {
                Pet.FoodConversionRatio = ratio;
                Console.WriteLine($"Коефіцієнт змінено на {Pet.FoodConversionRatio}.");
            }
            catch (Exception ex)
            {
                PrintError(ex);
                return true;
            }

            if (_pets.Count > 0)
            {
                Console.WriteLine("Добова норма ПІСЛЯ зміни (змінилася в усіх тварин одночасно):");
                PrintPetsTable(_pets);
            }

            return true;
        }

        private bool HumanAgeAction()
        {
            int age = ReadInt("Введіть вік тварини: ");
            Species type = ReadSpecies();

            try
            {
                int humanAge = Pet.ToHumanAge(age, type);
                Console.WriteLine($"{type} віком {age} р. орієнтовно відповідає людині віком {humanAge} р.");
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool LiveTogetherAction()
        {
            Console.WriteLine("\nПерший вид:");
            Species first = ReadSpecies();

            Console.WriteLine("\nДругий вид:");
            Species second = ReadSpecies();

            bool result = Pet.CanLiveTogether(first, second);

            Console.WriteLine(result
                ? $"{first} і {second} можна тримати разом."
                : $"{first} і {second} тримати разом НЕ можна: один з видів небезпечний для іншого.");

            return true;
        }

        private bool ParseDemoAction()
        {
            Console.WriteLine("\n--- Pet.Parse: коректний рядок ---");

            try
            {
                Pet parsed = Pet.Parse("Rex;Dog;4;12.5;так;Коваль О.");
                Console.WriteLine($"Отримано об'єкт: {parsed}");
                Console.WriteLine($"Кличка: {parsed.Nickname}, вид: {parsed.Type}, вік: {parsed.Age}, вага: {parsed.Weight:F2}");
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            Console.WriteLine("\n--- Pet.Parse: некоректний рядок (виняток ловить програма) ---");

            try
            {
                Pet.Parse("Rex;Dog;4");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}:");
                PrintError(ex);
            }

            Console.WriteLine("\n--- Pet.TryParse: різні рядки ---");

            string[] samples =
            {
                "Luna;Cat;2;3.8;ні;Не вказано",
                "",
                "Luna;Cat;2",
                "Luna;Dragon;2;3.8;ні;Іван",
                "Luna;Cat;два;3.8;ні;Іван",
                "Luna;Cat;2;3.8;можливо;Іван",
                "Luna;Cat;99;3.8;ні;Іван",
                "L;Cat;2;3.8;ні;Іван"
            };

            foreach (string sample in samples)
            {
                Console.WriteLine($"\nРядок: \"{sample}\"");

                bool success = Pet.TryParse(sample, out Pet result, out string error);

                Console.WriteLine(success
                    ? $"Результат: true, об'єкт = {result}"
                    : $"Результат: false, причина: {error}");
            }

            if (_pets.Count > 0)
            {
                Console.WriteLine("\n--- ToString -> TryParse для першої тварини зі списку ---");

                string text = _pets[0].ToString();
                Console.WriteLine($"ToString(): {text}");

                if (Pet.TryParse(text, out Pet copy))
                {
                    Console.WriteLine($"Відновлено з рядка: {copy}");
                }
            }

            Console.WriteLine($"\nЛічильник після демонстрації: {Pet.CreatedCount}");
            Console.WriteLine("(кожен успішний Parse створює новий коректний об'єкт і збільшує лічильник, невдалі - ні)");

            return true;
        }

        private bool DeletePetAction()
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

        private int RemoveByIndex()
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

        private int RemoveByNickname()
        {
            string nickname = ReadString("Введіть кличку: ");
            return _pets.RemoveAll(p => p.HasNickname(nickname));
        }

        private int RemoveByAge()
        {
            int age = ReadInt("Введіть вік: ");
            return _pets.RemoveAll(p => p.HasAge(age));
        }

        private int RemoveByWeight()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return _pets.RemoveAll(p => p.HasWeight(weight));
        }

        private int RemoveByVaccination()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return _pets.RemoveAll(p => p.HasVaccination(vaccinated));
        }

        private int RemoveByType()
        {
            Species type = ReadSpecies();
            return _pets.RemoveAll(p => p.HasType(type));
        }

        private void PrintError(Exception ex)
        {
            string message = ex.Message;

            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);

            if (index > 0)
            {
                message = message.Substring(0, index);
            }

            Console.WriteLine($"Помилка: {message}");
        }

        private void PrintSpeciesMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Оберіть вид тварини:");
            Console.WriteLine("1. Кіт");
            Console.WriteLine("2. Собака");
            Console.WriteLine("3. Змія");
            Console.WriteLine("4. Хом'як");
            Console.WriteLine("5. Рептилія");
        }

        private int ParseInt(string input)
        {
            if (!int.TryParse(input, out int value))
            {
                throw new FormatException("Введіть ціле число!");
            }

            return value;
        }

        private double ParseDouble(string input)
        {
            string prepared = (input ?? string.Empty).Replace(',', '.');

            if (!double.TryParse(prepared, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                throw new FormatException("Введіть правильне число!");
            }

            return value;
        }

        private bool ParseBool(string input)
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

        private Species ParseSpecies(string input)
        {
            int number = ParseInt(input);

            return (Species)(number - 1);
        }

        private string ReadString(string message)
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

        private int ReadInt(string message)
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

        private int ReadPositiveInt(string message)
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

        private double ReadDouble(string message)
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

        private bool ReadBool(string message)
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

        private Species ReadSpecies()
        {
            while (true)
            {
                PrintSpeciesMenu();
                Console.Write("Ваш вибір: ");

                try
                {
                    Species type = ParseSpecies(Console.ReadLine());

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