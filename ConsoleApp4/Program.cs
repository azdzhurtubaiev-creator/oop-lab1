using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ConsoleApp4
{
    public class Program
    {
        public const string CsvHeader = "Nickname;Type;Age;Weight;IsVaccinated;Owner";

        public const string DefaultFileName = "test";

        private readonly List<Pet> _pets = new List<Pet>();

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        private readonly Random _random = new Random();

        private int _maxCapacity;

        public Program()
        {
        }

        public Program(int maxCapacity)
        {
            SetMaxCapacity(maxCapacity);
        }

        public int MaxCapacity
        {
            get
            {
                return _maxCapacity;
            }
        }

        public int Count
        {
            get
            {
                return _pets.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _pets.Count == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return _pets.Count >= _maxCapacity;
            }
        }

        static void Main(string[] args)
        {
            Program application = new Program();
            application.Run();
        }

        public void SetMaxCapacity(int maxCapacity)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxCapacity),
                    "Максимальна кількість тварин має бути більше 0!");
            }

            if (maxCapacity < _pets.Count)
            {
                throw new InvalidOperationException(
                    "У списку вже більше тварин, ніж нова максимальна кількість!");
            }

            _maxCapacity = maxCapacity;
        }

        public IReadOnlyList<Pet> GetAll()
        {
            return _pets.AsReadOnly();
        }

        public void AddPet(Pet pet)
        {
            if (pet == null)
            {
                throw new ArgumentNullException(
                    nameof(pet),
                    "Тварина не може бути відсутньою!");
            }

            if (IsFull)
            {
                throw new InvalidOperationException(
                    "Досягнуто максимальну кількість тварин!");
            }

            if (_pets.Contains(pet))
            {
                throw new InvalidOperationException(
                    "Ця тварина вже є у списку!");
            }

            _pets.Add(pet);
        }

        public Pet GetByNumber(int number)
        {
            if (number < 1 || number > _pets.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    "Невірний номер тварини!");
            }

            return _pets[number - 1];
        }

        public bool ContainsNickname(string nickname)
        {
            return _pets.Exists(p => p.HasNickname(nickname));
        }

        public List<Pet> FindByNickname(string nickname)
        {
            return _pets.FindAll(p => p.HasNickname(nickname));
        }

        public List<Pet> FindByAge(int age)
        {
            return _pets.FindAll(p => p.HasAge(age));
        }

        public List<Pet> FindByWeight(double weight)
        {
            return _pets.FindAll(p => p.HasWeight(weight));
        }

        public List<Pet> FindByVaccination(bool vaccinated)
        {
            return _pets.FindAll(p => p.HasVaccination(vaccinated));
        }

        public List<Pet> FindByType(Species type)
        {
            return _pets.FindAll(p => p.HasType(type));
        }

        public bool RemoveByNumber(int number)
        {
            if (number < 1 || number > _pets.Count)
            {
                return false;
            }

            _pets.RemoveAt(number - 1);
            return true;
        }

        public int RemoveByNickname(string nickname)
        {
            return _pets.RemoveAll(p => p.HasNickname(nickname));
        }

        public int RemoveByAge(int age)
        {
            return _pets.RemoveAll(p => p.HasAge(age));
        }

        public int RemoveByWeight(double weight)
        {
            return _pets.RemoveAll(p => p.HasWeight(weight));
        }

        public int RemoveByVaccination(bool vaccinated)
        {
            return _pets.RemoveAll(p => p.HasVaccination(vaccinated));
        }

        public int RemoveByType(Species type)
        {
            return _pets.RemoveAll(p => p.HasType(type));
        }

        public int AddPets(List<Pet> pets)
        {
            if (pets == null)
            {
                throw new ArgumentNullException(
                    nameof(pets),
                    "Список тварин не може бути відсутнім!");
            }

            int added = 0;

            foreach (Pet pet in pets)
            {
                if (IsFull)
                {
                    break;
                }

                if (pet == null || _pets.Contains(pet))
                {
                    continue;
                }

                _pets.Add(pet);
                added++;
            }

            return added;
        }

        public int ClearPets()
        {
            int removed = _pets.Count;

            _pets.Clear();
            Pet.ResetCreatedCount();

            return removed;
        }

        public string GetFileName(string? input, string extension)
        {
            string name = string.IsNullOrWhiteSpace(input) ? DefaultFileName : input.Trim();

            if (Path.GetExtension(name) == string.Empty)
            {
                name = $"{name}.{extension}";
            }

            return name;
        }

        public void SaveToFileCsv(string path)
        {
            string fileName = ParseNonEmpty(path);

            List<string> lines = new List<string>();
            lines.Add(CsvHeader);

            foreach (Pet pet in _pets)
            {
                lines.Add(pet.ToString());
            }

            File.WriteAllLines(fileName, lines);
        }

        public List<Pet> ReadFromFileCsv(string path)
        {
            string fileName = ParseNonEmpty(path);

            string[] lines = File.ReadAllLines(fileName);

            List<Pet> pets = new List<Pet>();

            foreach (string line in lines)
            {
                if (line.Trim() == CsvHeader)
                {
                    continue;
                }

                if (Pet.TryParse(line, out Pet pet))
                {
                    pets.Add(pet);
                }
            }

            return pets;
        }

        public void SaveToFileJson(string path)
        {
            string fileName = ParseNonEmpty(path);

            string json = JsonSerializer.Serialize(_pets, _jsonOptions);

            File.WriteAllText(fileName, json);
        }

        public List<Pet> ReadFromFileJson(string path)
        {
            string fileName = ParseNonEmpty(path);

            string json = File.ReadAllText(fileName);

            List<Pet>? pets = JsonSerializer.Deserialize<List<Pet>>(json, _jsonOptions);

            if (pets == null)
            {
                return new List<Pet>();
            }

            pets.RemoveAll(p => p == null);

            return pets;
        }

        public string ParseNonEmpty(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Значення не може бути порожнім!");
            }

            return input.Trim();
        }

        public int ParseInt(string? input)
        {
            if (!int.TryParse(input, out int value))
            {
                throw new FormatException("Введіть ціле число!");
            }

            return value;
        }

        public int ParsePositiveInt(string? input)
        {
            int value = ParseInt(input);

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input),
                    "Число повинно бути більше 0!");
            }

            return value;
        }

        public double ParseDouble(string? input)
        {
            string prepared = (input ?? string.Empty).Replace(',', '.');

            if (!double.TryParse(prepared, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                throw new FormatException("Введіть правильне число!");
            }

            return value;
        }

        public bool ParseBool(string? input)
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

        public Species ParseSpecies(string? input)
        {
            int number = ParseInt(input);

            return (Species)(number - 1);
        }

        public Species ParseDefinedSpecies(string? input)
        {
            Species type = ParseSpecies(input);

            if (!Enum.IsDefined(typeof(Species), type))
            {
                throw new ArgumentException("Невірний номер виду!");
            }

            return type;
        }

        public string GetUserMessage(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            string message = ex.Message;

            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);

            if (index > 0)
            {
                message = message.Substring(0, index);
            }

            return message;
        }

        private void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Програма обліку домашніх тварин ===");

            SetMaxCapacity(ReadPositiveInt("Введіть максимальну кількість тварин: "));

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
                    "7. Зберегти колекцію у файлі",
                    "8. Зчитати колекцію з файлу",
                    "9. Очистити колекцію",
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
                    "7" => SaveAction(),
                    "8" => LoadAction(),
                    "9" => ClearAction(),
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
            if (IsFull)
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

            if (ContainsNickname(pet.Nickname))
            {
                Console.WriteLine($"Увага: у списку вже є тварина з кличкою {pet.Nickname}.");
            }

            AddPet(pet);

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

            AddPet(pet);

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
                    AddPet(pet);

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

            if (IsEmpty)
            {
                Console.WriteLine("Список тварин порожній.");
                PrintCounters();
                return true;
            }

            PrintPetsTable(GetAll());
            PrintCounters();
            return true;
        }

        private void PrintCounters()
        {
            Console.WriteLine($"Тварин у списку: {Count} з {MaxCapacity}");
            Console.WriteLine($"Лічильник створених об'єктів Pet (Pet.CreatedCount): {Pet.CreatedCount}");
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
            if (IsEmpty)
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
                "1" => SearchByNicknameAction(),
                "2" => SearchByAgeAction(),
                "3" => SearchByWeightAction(),
                "4" => SearchByVaccinationAction(),
                "5" => SearchByTypeAction(),
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

        private List<Pet> SearchByNicknameAction()
        {
            string nickname = ReadString("Введіть кличку: ");
            return FindByNickname(nickname);
        }

        private List<Pet> SearchByAgeAction()
        {
            int age = ReadInt("Введіть вік: ");
            return FindByAge(age);
        }

        private List<Pet> SearchByWeightAction()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return FindByWeight(weight);
        }

        private List<Pet> SearchByVaccinationAction()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return FindByVaccination(vaccinated);
        }

        private List<Pet> SearchByTypeAction()
        {
            Species type = ReadSpecies();
            return FindByType(type);
        }

        private bool DemonstrateBehaviorAction()
        {
            if (IsEmpty)
            {
                Console.WriteLine("Список тварин порожній.");
                return true;
            }

            Console.WriteLine("\n=== Поведінка тварини ===");
            PrintPetsTable(GetAll());

            int number = ReadInt("Виберіть номер тварини: ");

            Pet pet;

            try
            {
                pet = GetByNumber(number);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                PrintError(ex);
                return true;
            }

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
            Console.WriteLine("Лічильник не зменшується при видаленні окремих тварин, але обнуляється при очищенні колекції (пункт 9).");
            return true;
        }

        private bool ChangeRatioAction()
        {
            Console.WriteLine($"\nПоточний коефіцієнт: {Pet.FoodConversionRatio}");

            if (!IsEmpty)
            {
                Console.WriteLine("Добова норма ДО зміни:");
                PrintPetsTable(GetAll());
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

            if (!IsEmpty)
            {
                Console.WriteLine("Добова норма ПІСЛЯ зміни (змінилася в усіх тварин одночасно):");
                PrintPetsTable(GetAll());
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

            if (!IsEmpty)
            {
                Console.WriteLine("\n--- ToString -> TryParse для першої тварини зі списку ---");

                string text = GetByNumber(1).ToString();
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
            if (IsEmpty)
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
                "1" => RemoveByIndexAction(),
                "2" => RemoveByNicknameAction(),
                "3" => RemoveByAgeAction(),
                "4" => RemoveByWeightAction(),
                "5" => RemoveByVaccinationAction(),
                "6" => RemoveByTypeAction(),
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

        private int RemoveByIndexAction()
        {
            PrintPetsTable(GetAll());
            int index = ReadInt("Введіть порядковий номер тварини для видалення: ");

            if (!RemoveByNumber(index))
            {
                Console.WriteLine("Невірний номер!");
                return 0;
            }

            return 1;
        }

        private int RemoveByNicknameAction()
        {
            string nickname = ReadString("Введіть кличку: ");
            return RemoveByNickname(nickname);
        }

        private int RemoveByAgeAction()
        {
            int age = ReadInt("Введіть вік: ");
            return RemoveByAge(age);
        }

        private int RemoveByWeightAction()
        {
            double weight = ReadDouble("Введіть вагу (кг): ");
            return RemoveByWeight(weight);
        }

        private int RemoveByVaccinationAction()
        {
            bool vaccinated = ReadBool("Вакцинована? (так/ні): ");
            return RemoveByVaccination(vaccinated);
        }

        private int RemoveByTypeAction()
        {
            Species type = ReadSpecies();
            return RemoveByType(type);
        }

        private bool SaveAction()
        {
            if (IsEmpty)
            {
                Console.WriteLine("Список тварин порожній - нічого зберігати.");
                return true;
            }

            RenderMenu("Збереження у файл",
                "1. Зберегти у файл *.csv",
                "2. Зберегти у файл *.json");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => SaveCsvAction(),
                "2" => SaveJsonAction(),
                _ => InvalidOption()
            };
        }

        private bool SaveCsvAction()
        {
            string path = ReadFileName("csv");

            try
            {
                SaveToFileCsv(path);
                Console.WriteLine($"Збережено об'єктів: {Count}");
                Console.WriteLine($"Файл: {Path.GetFullPath(path)}");
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool SaveJsonAction()
        {
            string path = ReadFileName("json");

            try
            {
                SaveToFileJson(path);
                Console.WriteLine($"Збережено об'єктів: {Count}");
                Console.WriteLine($"Файл: {Path.GetFullPath(path)}");
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool LoadAction()
        {
            RenderMenu("Зчитування з файлу",
                "1. Зчитати з файлу *.csv",
                "2. Зчитати з файлу *.json");

            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => LoadCsvAction(),
                "2" => LoadJsonAction(),
                _ => InvalidOption()
            };
        }

        private bool LoadCsvAction()
        {
            string path = ReadFileName("csv");

            try
            {
                List<Pet> loaded = ReadFromFileCsv(path);
                PrintLoadResult(loaded);
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private bool LoadJsonAction()
        {
            string path = ReadFileName("json");

            try
            {
                List<Pet> loaded = ReadFromFileJson(path);
                PrintLoadResult(loaded);
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }

            return true;
        }

        private void PrintLoadResult(List<Pet> loaded)
        {
            int added = AddPets(loaded);

            Console.WriteLine($"Десеріалізовано об'єктів з файлу: {loaded.Count}");
            Console.WriteLine($"Додано до колекції: {added}");

            if (added < loaded.Count)
            {
                Console.WriteLine($"Не додано через ліміт ({MaxCapacity}): {loaded.Count - added}");
            }

            Console.WriteLine($"Тварин у списку: {Count} з {MaxCapacity}");
        }

        private bool ClearAction()
        {
            if (IsEmpty)
            {
                Console.WriteLine("Список тварин вже порожній.");
                return true;
            }

            bool confirm = ReadBool($"Видалити всіх тварин ({Count})? (так/ні): ");

            if (!confirm)
            {
                Console.WriteLine("Очищення скасовано.");
                return true;
            }

            int removed = ClearPets();

            Console.WriteLine($"Колекцію очищено. Видалено тварин: {removed}");
            PrintCounters();

            return true;
        }

        private string ReadFileName(string extension)
        {
            Console.Write($"Введіть назву файлу *.{extension} (Enter - {DefaultFileName}.{extension}): ");
            return GetFileName(Console.ReadLine(), extension);
        }

        private void PrintError(Exception ex)
        {
            Console.WriteLine($"Помилка: {GetUserMessage(ex)}");
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

        private string ReadString(string message)
        {
            while (true)
            {
                Console.Write(message);

                try
                {
                    return ParseNonEmpty(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
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
                Console.Write(message);

                try
                {
                    return ParsePositiveInt(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
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
                    return ParseDefinedSpecies(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
            }
        }
    }
}