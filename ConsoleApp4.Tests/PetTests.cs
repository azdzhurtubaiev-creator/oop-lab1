using System;
using ConsoleApp4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApp4.Tests
{
    // Клас змінює статичний стан Pet (коефіцієнт, лічильник),
    // тому тести в ньому не можна запускати паралельно
    [TestClass]
    [DoNotParallelize]
    public class PetTests
    {
        private Pet _pet;
        private double _savedRatio;

        // Виконується ПЕРЕД кожним тестом: свіжа тварина з відомими значеннями
        [TestInitialize]
        public void Setup()
        {
            _savedRatio = Pet.FoodConversionRatio;
            _pet = new Pet("Barsik", Species.Cat, 3, 4.5, true, "Іваненко І.");
        }

        // Виконується ПІСЛЯ кожного тесту: повертаємо статичний коефіцієнт,
        // щоб тести не впливали один на одного
        [TestCleanup]
        public void Cleanup()
        {
            Pet.FoodConversionRatio = _savedRatio;
            _pet = null;
        }

        // =====================================================================
        // Конструктори
        // =====================================================================

        [TestMethod]
        public void Constructor_NoParameters_SetsDefaultValues()
        {
            // Arrange + Act
            Pet pet = new Pet();

            // Assert
            Assert.IsNotNull(pet);
            Assert.AreEqual(Pet.DefaultNickname, pet.Nickname);
            Assert.AreEqual(Species.Cat, pet.Type);
            Assert.AreEqual(Pet.MinAge, pet.Age);
            Assert.AreEqual(Pet.DefaultWeight, pet.Weight);
            Assert.IsFalse(pet.IsVaccinated);
            Assert.AreEqual(Pet.DefaultOwner, pet.Owner);
        }

        [TestMethod]
        public void Constructor_TwoParameters_SetsNicknameAndTypeAndDefaultsForOthers()
        {
            // Arrange
            string nickname = "Rex";
            Species type = Species.Dog;

            // Act
            Pet pet = new Pet(nickname, type);

            // Assert
            Assert.AreEqual(nickname, pet.Nickname);
            Assert.AreEqual(type, pet.Type);
            Assert.AreEqual(Pet.MinAge, pet.Age);
            Assert.AreEqual(Pet.DefaultWeight, pet.Weight);
        }

        [TestMethod]
        public void Constructor_FourParameters_SetsAgeAndWeight()
        {
            // Arrange + Act
            Pet pet = new Pet("Luna", Species.Hamster, 2, 0.3);

            // Assert
            Assert.AreEqual(2, pet.Age);
            Assert.AreEqual(0.3, pet.Weight);
            Assert.IsFalse(pet.IsVaccinated);
            Assert.AreEqual(Pet.DefaultOwner, pet.Owner);
        }

        [TestMethod]
        public void Constructor_AllParameters_SetsAllValues()
        {
            // Assert (Arrange і Act виконано в Setup)
            Assert.AreEqual("Barsik", _pet.Nickname);
            Assert.AreEqual(Species.Cat, _pet.Type);
            Assert.AreEqual(3, _pet.Age);
            Assert.AreEqual(4.5, _pet.Weight);
            Assert.IsTrue(_pet.IsVaccinated);
            Assert.AreEqual("Іваненко І.", _pet.Owner);
        }

        [TestMethod]
        public void Constructor_EmptyOwner_SetsDefaultOwner()
        {
            // Arrange + Act
            Pet pet = new Pet("Rex", Species.Dog, 1, 5, false, "   ");

            // Assert
            Assert.AreEqual(Pet.DefaultOwner, pet.Owner);
        }

        [TestMethod]
        public void Constructor_TwoObjects_HaveDifferentIds()
        {
            // Arrange + Act
            Pet first = new Pet();
            Pet second = new Pet();

            // Assert
            Assert.AreNotEqual(first.Id, second.Id);
        }

        [TestMethod]
        public void Constructor_NoParameters_SetsUsedConstructorDescription()
        {
            // Arrange + Act
            Pet pet = new Pet();

            // Assert
            Assert.IsTrue(pet.UsedConstructor.StartsWith("Pet()"));
        }

        [TestMethod]
        public void Constructor_NewObject_IsHungry()
        {
            // Assert
            Assert.IsTrue(_pet.IsHungry);
        }

        [TestMethod]
        public void Constructor_InvalidAge_ThrowsArgumentOutOfRangeException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new Pet("Rex", Species.Dog, 99, 5));
        }

        // =====================================================================
        // Лічильник створених об'єктів (статична властивість)
        // =====================================================================

        [TestMethod]
        public void CreatedCount_ValidObject_IncreasesByOne()
        {
            // Arrange
            int before = Pet.CreatedCount;

            // Act
            new Pet("Rex", Species.Dog);
            int after = Pet.CreatedCount;

            // Assert
            Assert.AreEqual(before + 1, after);
        }

        [TestMethod]
        public void CreatedCount_InvalidObject_DoesNotChange()
        {
            // Arrange
            int before = Pet.CreatedCount;

            // Act
            try
            {
                new Pet("Rex", Species.Dog, -5, 5);
            }
            catch (ArgumentOutOfRangeException)
            {
                // очікуваний виняток: об'єкт не створено
            }

            int after = Pet.CreatedCount;

            // Assert
            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public void CreatedCount_EachConstructor_IncreasesExactlyOnce()
        {
            // Arrange
            int before = Pet.CreatedCount;

            // Act
            new Pet();
            new Pet("Rex", Species.Dog);
            new Pet("Rex", Species.Dog, 2, 10);
            new Pet("Rex", Species.Dog, 2, 10, true, "Коваль О.");
            int after = Pet.CreatedCount;

            // Assert: чотири об'єкти, а не більше, хоч конструктори викликають один одного
            Assert.AreEqual(before + 4, after);
        }

        // =====================================================================
        // Властивість Nickname (секція set з перевірками)
        // =====================================================================

        [TestMethod]
        [DataRow("Rex")]
        [DataRow("Ab")]
        [DataRow("Барсик")]
        public void Nickname_ValidValue_IsSet(string nickname)
        {
            // Act
            _pet.Nickname = nickname;

            // Assert
            Assert.AreEqual(nickname, _pet.Nickname);
        }

        [TestMethod]
        public void Nickname_ValueWithSpaces_IsTrimmed()
        {
            // Act
            _pet.Nickname = "   Rex   ";

            // Assert
            Assert.AreEqual("Rex", _pet.Nickname);
        }

        [TestMethod]
        public void Nickname_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _pet.Nickname = null);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Nickname_EmptyValue_ThrowsArgumentNullException(string nickname)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _pet.Nickname = nickname);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("ThisNicknameIsDefinitelyTooLong!")]
        public void Nickname_WrongLength_ThrowsArgumentOutOfRangeException(string nickname)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Nickname = nickname);
        }

        [TestMethod]
        public void Nickname_ContainsSeparator_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _pet.Nickname = "Bar;sik");
        }

        [TestMethod]
        public void Nickname_InvalidValue_KeepsOldValue()
        {
            // Arrange
            string oldNickname = _pet.Nickname;

            // Act
            try
            {
                _pet.Nickname = "A";
            }
            catch (ArgumentOutOfRangeException)
            {
                // очікуваний виняток
            }

            // Assert
            Assert.AreEqual(oldNickname, _pet.Nickname);
        }

        // =====================================================================
        // Властивість Age
        // =====================================================================

        [TestMethod]
        [DataRow(0)]
        [DataRow(30)]
        [DataRow(60)]
        public void Age_ValueInRange_IsSet(int age)
        {
            // Act
            _pet.Age = age;

            // Assert
            Assert.AreEqual(age, _pet.Age);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(61)]
        public void Age_ValueOutOfRange_ThrowsArgumentOutOfRangeException(int age)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Age = age);
        }

        // =====================================================================
        // Властивість Weight
        // =====================================================================

        [TestMethod]
        [DataRow(0.1)]
        [DataRow(250.0)]
        [DataRow(500.0)]
        public void Weight_ValueInRange_IsSet(double weight)
        {
            // Act
            _pet.Weight = weight;

            // Assert
            Assert.AreEqual(weight, _pet.Weight, 0.001);
        }

        [TestMethod]
        public void Weight_ManyDecimals_IsRoundedToTwo()
        {
            // Act
            _pet.Weight = 4.567;

            // Assert
            Assert.AreEqual(4.57, _pet.Weight);
        }

        [TestMethod]
        [DataRow(0.09)]
        [DataRow(500.01)]
        [DataRow(-3.0)]
        public void Weight_ValueOutOfRange_ThrowsArgumentOutOfRangeException(double weight)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Weight = weight);
        }

        [TestMethod]
        [DataRow(double.NaN)]
        [DataRow(double.PositiveInfinity)]
        public void Weight_NotANumber_ThrowsArgumentException(double weight)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _pet.Weight = weight);
        }

        // =====================================================================
        // Властивість Type
        // =====================================================================

        [TestMethod]
        public void Type_ValidValue_IsSet()
        {
            // Act
            _pet.Type = Species.Reptile;

            // Assert
            Assert.AreEqual(Species.Reptile, _pet.Type);
        }

        [TestMethod]
        public void Type_UndefinedValue_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _pet.Type = (Species)99);
        }

        // =====================================================================
        // Статична властивість FoodConversionRatio
        // =====================================================================

        [TestMethod]
        public void FoodConversionRatio_ValidValue_IsSet()
        {
            // Act
            Pet.FoodConversionRatio = 0.25;

            // Assert
            Assert.AreEqual(0.25, Pet.FoodConversionRatio);
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(0.6)]
        public void FoodConversionRatio_OutOfRange_ThrowsArgumentOutOfRangeException(double ratio)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pet.FoodConversionRatio = ratio);
        }

        [TestMethod]
        [DataRow(double.NaN)]
        [DataRow(double.PositiveInfinity)]
        public void FoodConversionRatio_NotANumber_ThrowsArgumentException(double ratio)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => Pet.FoodConversionRatio = ratio);
        }

        [TestMethod]
        public void FoodConversionRatio_Changed_AffectsAllPets()
        {
            // Arrange
            Pet second = new Pet("Rex", Species.Dog, 5, 20);

            // Act
            Pet.FoodConversionRatio = 0.2;

            // Assert: одна зміна статичного значення впливає на всі об'єкти
            Assert.AreEqual(0.9, _pet.DailyFoodNorm);
            Assert.AreEqual(4.0, second.DailyFoodNorm);
        }

        // =====================================================================
        // Обчислювальні властивості
        // =====================================================================

        [TestMethod]
        public void DailyFoodNorm_DefaultRatio_EqualsTenPercentOfWeight()
        {
            // Arrange
            Pet.FoodConversionRatio = 0.1;

            // Act
            double norm = _pet.DailyFoodNorm;

            // Assert
            Assert.AreEqual(0.45, norm);
        }

        [TestMethod]
        [DataRow(0, "Малюк")]
        [DataRow(1, "Малюк")]
        [DataRow(2, "Дорослий")]
        [DataRow(8, "Дорослий")]
        [DataRow(9, "Літній")]
        public void AgeCategory_DependsOnAge(int age, string expected)
        {
            // Arrange
            _pet.Age = age;

            // Act
            string actual = _pet.AgeCategory;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShortId_IsFirstEightSymbolsOfId()
        {
            // Act
            string shortId = _pet.ShortId;

            // Assert
            Assert.AreEqual(Pet.ShortGuidLength, shortId.Length);
            Assert.IsTrue(_pet.Id.ToString().StartsWith(shortId));
        }

        // =====================================================================
        // Методи Has...
        // =====================================================================

        [TestMethod]
        [DataRow("Barsik")]
        [DataRow("BARSIK")]
        [DataRow("  barsik  ")]
        public void HasNickname_SameNicknameAnyCase_ReturnsTrue(string nickname)
        {
            // Act
            bool actual = _pet.HasNickname(nickname);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void HasNickname_OtherNickname_ReturnsFalse()
        {
            // Act + Assert
            Assert.IsFalse(_pet.HasNickname("Murzik"));
        }

        [TestMethod]
        public void HasNickname_Null_ReturnsFalse()
        {
            // Act
            bool actual = _pet.HasNickname(null);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [DataRow(3, true)]
        [DataRow(4, false)]
        public void HasAge_ReturnsExpected(int age, bool expected)
        {
            // Act
            bool actual = _pet.HasAge(age);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(4.5, true)]
        [DataRow(4.5004, true)]
        [DataRow(4.6, false)]
        public void HasWeight_ComparesWithTolerance(double weight, bool expected)
        {
            // Act
            bool actual = _pet.HasWeight(weight);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void HasVaccination_ReturnsExpected(bool vaccinated, bool expected)
        {
            // Act
            bool actual = _pet.HasVaccination(vaccinated);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HasType_SameType_ReturnsTrue_OtherType_ReturnsFalse()
        {
            // Act
            bool same = _pet.HasType(Species.Cat);
            bool other = _pet.HasType(Species.Dog);

            // Assert
            Assert.IsTrue(same);
            Assert.IsFalse(other);
        }

        // =====================================================================
        // Перевантажений метод Feed
        // =====================================================================

        [TestMethod]
        public void Feed_NoParameters_AddsDailyNorm()
        {
            // Arrange
            Pet.FoodConversionRatio = 0.1;
            double expected = 4.5 + 0.45;

            // Act
            _pet.Feed();

            // Assert
            Assert.AreEqual(expected, _pet.Weight, 0.001);
        }

        [TestMethod]
        public void Feed_Amount_IncreasesWeightAndMakesNotHungry()
        {
            // Act
            string message = _pet.Feed(0.5);

            // Assert
            Assert.AreEqual(5.0, _pet.Weight);
            Assert.IsFalse(_pet.IsHungry);
            Assert.IsTrue(message.Contains("Barsik"));
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        public void Feed_AmountTooSmall_ThrowsArgumentOutOfRangeException(double food)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Feed(food));
        }

        [TestMethod]
        public void Feed_AmountNotANumber_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _pet.Feed(double.NaN));
        }

        [TestMethod]
        public void Feed_ResultOverMaxWeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            _pet.Weight = 499.9;

            // Act + Assert: перевищення ловить сетер властивості Weight
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Feed(1.0));
        }

        [TestMethod]
        public void Feed_AmountAndTimes_AddsAllPortions()
        {
            // Act
            _pet.Feed(0.2, 3);

            // Assert
            Assert.AreEqual(5.1, _pet.Weight, 0.001);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-2)]
        public void Feed_TimesNotPositive_ThrowsArgumentOutOfRangeException(int times)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Feed(0.2, times));
        }

        // =====================================================================
        // Перевантажений метод Walk
        // =====================================================================

        [TestMethod]
        public void Walk_NoParameters_ReturnsMessageWithNickname()
        {
            // Act
            string actual = _pet.Walk();

            // Assert
            Assert.AreEqual("Barsik гуляє на вулиці!", actual);
        }

        [TestMethod]
        public void Walk_Minutes_ReturnsMessageWithMinutes()
        {
            // Act
            string actual = _pet.Walk(30);

            // Assert
            Assert.AreEqual("Barsik гуляє на вулиці 30 хв!", actual);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-10)]
        public void Walk_MinutesNotPositive_ThrowsArgumentOutOfRangeException(int minutes)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.Walk(minutes));
        }

        // =====================================================================
        // Перевантажений метод MakeSound
        // =====================================================================

        [TestMethod]
        [DataRow(Species.Cat, "Мяу!")]
        [DataRow(Species.Dog, "Гав!")]
        [DataRow(Species.Snake, "Ш-ш-ш!")]
        [DataRow(Species.Hamster, "Пи-пи!")]
        [DataRow(Species.Reptile, "Хр-р!")]
        public void MakeSound_DependsOnSpecies(Species type, string voice)
        {
            // Arrange
            _pet.Type = type;

            // Act
            string actual = _pet.MakeSound();

            // Assert
            Assert.AreEqual($"Barsik: {voice}", actual);
        }

        [TestMethod]
        public void MakeSound_Times_RepeatsVoice()
        {
            // Act
            string actual = _pet.MakeSound(3);

            // Assert
            Assert.AreEqual("Barsik: Мяу! Мяу! Мяу!", actual);
        }

        [TestMethod]
        public void MakeSound_TimesZero_ThrowsArgumentOutOfRangeException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _pet.MakeSound(0));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [TestMethod]
        public void ToString_ReturnsParseFormat()
        {
            // Act
            string actual = _pet.ToString();

            // Assert
            Assert.AreEqual("Barsik;Cat;3;4.5;так;Іваненко І.", actual);
        }

        [TestMethod]
        public void ToString_NotVaccinated_WritesNi()
        {
            // Arrange
            Pet pet = new Pet("Rex", Species.Dog, 4, 12.5, false, "Коваль О.");

            // Act
            string actual = pet.ToString();

            // Assert
            Assert.AreEqual("Rex;Dog;4;12.5;ні;Коваль О.", actual);
        }

        [TestMethod]
        public void ToString_ThenParse_RestoresSameValues()
        {
            // Act
            Pet copy = Pet.Parse(_pet.ToString());

            // Assert
            Assert.AreEqual(_pet.Nickname, copy.Nickname);
            Assert.AreEqual(_pet.Type, copy.Type);
            Assert.AreEqual(_pet.Age, copy.Age);
            Assert.AreEqual(_pet.Weight, copy.Weight);
            Assert.AreEqual(_pet.IsVaccinated, copy.IsVaccinated);
            Assert.AreEqual(_pet.Owner, copy.Owner);
        }

        // =====================================================================
        // Статичний метод Parse
        // =====================================================================

        [TestMethod]
        public void Parse_ValidString_ReturnsPetWithValues()
        {
            // Arrange
            string s = "Rex;Dog;4;12.5;ні;Коваль О.";

            // Act
            Pet pet = Pet.Parse(s);

            // Assert
            Assert.IsInstanceOfType<Pet>(pet);
            Assert.AreEqual("Rex", pet.Nickname);
            Assert.AreEqual(Species.Dog, pet.Type);
            Assert.AreEqual(4, pet.Age);
            Assert.AreEqual(12.5, pet.Weight);
            Assert.IsFalse(pet.IsVaccinated);
            Assert.AreEqual("Коваль О.", pet.Owner);
        }

        [TestMethod]
        public void Parse_SpacesCommaAndSpeciesNumber_AreAccepted()
        {
            // Arrange
            string s = " Rex ; 2 ; 4 ; 12,5 ; ТАК ; Коваль О. ";

            // Act
            Pet pet = Pet.Parse(s);

            // Assert
            Assert.AreEqual("Rex", pet.Nickname);
            Assert.AreEqual(Species.Dog, pet.Type);
            Assert.AreEqual(12.5, pet.Weight);
            Assert.IsTrue(pet.IsVaccinated);
        }

        [TestMethod]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Pet.Parse(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Parse_EmptyString_ThrowsArgumentNullException(string s)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Pet.Parse(s));
        }

        [TestMethod]
        [DataRow("Rex;Dog;4")]
        [DataRow("Rex;Dog;4;12.5;ні;Коваль О.;зайве")]
        [DataRow("Rex;Dragon;4;12.5;ні;Коваль О.")]
        [DataRow("Rex;7;4;12.5;ні;Коваль О.")]
        [DataRow("Rex;0;4;12.5;ні;Коваль О.")]
        [DataRow("Rex;Dog;чотири;12.5;ні;Коваль О.")]
        [DataRow("Rex;Dog;4;багато;ні;Коваль О.")]
        [DataRow("Rex;Dog;4;12.5;можливо;Коваль О.")]
        public void Parse_WrongFormat_ThrowsFormatException(string s)
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => Pet.Parse(s));
        }

        [TestMethod]
        [DataRow("Rex;Dog;99;12.5;ні;Коваль О.")]
        [DataRow("Rex;Dog;4;999;ні;Коваль О.")]
        [DataRow("R;Dog;4;12.5;ні;Коваль О.")]
        public void Parse_ValueOutOfRange_ThrowsArgumentOutOfRangeException(string s)
        {
            // Act + Assert: діапазони перевіряють властивості всередині конструктора
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pet.Parse(s));
        }

        [TestMethod]
        public void Parse_AnyInvalidString_ThrowsExceptionDerivedFromException()
        {
            // Act + Assert: Throws (без Exactly) приймає і нащадків вказаного типу
            Assert.Throws<ArgumentException>(() => Pet.Parse(""));
        }

        // =====================================================================
        // Статичний метод TryParse
        // =====================================================================

        [TestMethod]
        public void TryParse_ValidString_ReturnsTrueAndObject()
        {
            // Act
            bool result = Pet.TryParse("Luna;Cat;2;3.8;ні;Не вказано", out Pet pet);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(pet);
            Assert.AreEqual("Luna", pet.Nickname);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("Luna;Cat;2")]
        [DataRow("Luna;Cat;99;3.8;ні;Іван")]
        public void TryParse_InvalidString_ReturnsFalseAndNull(string s)
        {
            // Act
            bool result = Pet.TryParse(s, out Pet pet);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(pet);
        }

        [TestMethod]
        public void TryParse_InvalidString_ReturnsErrorMessage()
        {
            // Act
            bool result = Pet.TryParse("Luna;Cat;два;3.8;ні;Іван", out Pet pet, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(pet);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("два"));
        }

        [TestMethod]
        public void TryParse_ValidString_ErrorIsNull()
        {
            // Act
            bool result = Pet.TryParse("Luna;Cat;2;3.8;ні;Іван", out Pet pet, out string error);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TryParse_InvalidString_DoesNotIncreaseCreatedCount()
        {
            // Arrange
            int before = Pet.CreatedCount;

            // Act
            Pet.TryParse("Luna;Cat;99;3.8;ні;Іван", out Pet pet);
            int after = Pet.CreatedCount;

            // Assert
            Assert.AreEqual(before, after);
        }

        // =====================================================================
        // Статичний метод ToHumanAge
        // =====================================================================

        [TestMethod]
        [DataRow(0, Species.Cat, 0)]
        [DataRow(1, Species.Cat, 15)]
        [DataRow(2, Species.Cat, 24)]
        [DataRow(5, Species.Cat, 36)]
        [DataRow(5, Species.Dog, 39)]
        [DataRow(2, Species.Hamster, 50)]
        [DataRow(10, Species.Snake, 20)]
        [DataRow(10, Species.Reptile, 20)]
        public void ToHumanAge_ReturnsExpected(int age, Species type, int expected)
        {
            // Act
            int actual = Pet.ToHumanAge(age, type);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(61)]
        public void ToHumanAge_AgeOutOfRange_ThrowsArgumentOutOfRangeException(int age)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pet.ToHumanAge(age, Species.Cat));
        }

        [TestMethod]
        public void ToHumanAge_UndefinedSpecies_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => Pet.ToHumanAge(3, (Species)99));
        }

        // =====================================================================
        // Статичний метод CanLiveTogether
        // =====================================================================

        [TestMethod]
        [DataRow(Species.Cat, Species.Cat, true)]
        [DataRow(Species.Cat, Species.Dog, true)]
        [DataRow(Species.Snake, Species.Reptile, true)]
        [DataRow(Species.Reptile, Species.Hamster, true)]
        [DataRow(Species.Cat, Species.Snake, false)]
        [DataRow(Species.Cat, Species.Hamster, false)]
        [DataRow(Species.Hamster, Species.Cat, false)]
        [DataRow(Species.Snake, Species.Hamster, false)]
        [DataRow(Species.Dog, Species.Snake, false)]
        public void CanLiveTogether_ReturnsExpected(Species first, Species second, bool expected)
        {
            // Act
            bool actual = Pet.CanLiveTogether(first, second);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}