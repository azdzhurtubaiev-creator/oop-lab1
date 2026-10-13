using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ConsoleApp4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApp4.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class ProgramTests
    {
        private Program _program;
        private Pet _barsik;
        private Pet _rex;
        private Pet _luna;

        [TestInitialize]
        public void Setup()
        {
            _program = new Program(3);
            _barsik = new Pet("Barsik", Species.Cat, 3, 4.5, true, "Іваненко І.");
            _rex = new Pet("Rex", Species.Dog, 5, 20, false, "Коваль О.");
            _luna = new Pet("Luna", Species.Cat, 1, 3, true, "Не вказано");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _program = null;
            _barsik = null;
            _rex = null;
            _luna = null;
        }

        private void FillProgram()
        {
            _program.AddPet(_barsik);
            _program.AddPet(_rex);
            _program.AddPet(_luna);
        }

        [TestMethod]
        public void Constructor_ValidCapacity_CreatesEmptyList()
        {
            // Arrange + Act
            Program program = new Program(5);

            // Assert
            Assert.AreEqual(5, program.MaxCapacity);
            Assert.AreEqual(0, program.Count);
            Assert.IsTrue(program.IsEmpty);
            Assert.IsFalse(program.IsFull);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public void Constructor_CapacityNotPositive_ThrowsArgumentOutOfRangeException(int capacity)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Program(capacity));
        }

        [TestMethod]
        public void AddPet_ValidPet_IncreasesCount()
        {
            // Act
            _program.AddPet(_barsik);

            // Assert
            Assert.AreEqual(1, _program.Count);
            Assert.IsFalse(_program.IsEmpty);
        }

        [TestMethod]
        public void AddPet_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _program.AddPet(null));
        }

        [TestMethod]
        public void AddPet_WhenFull_ThrowsInvalidOperationException()
        {
            // Arrange
            FillProgram();
            Pet extra = new Pet("Max", Species.Snake);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _program.AddPet(extra));
            Assert.AreEqual(3, _program.Count);
        }

        [TestMethod]
        public void AddPet_SamePetTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            _program.AddPet(_barsik);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _program.AddPet(_barsik));
            Assert.AreEqual(1, _program.Count);
        }

        [TestMethod]
        public void IsFull_CountEqualsMaxCapacity_ReturnsTrue()
        {
            // Act
            FillProgram();

            // Assert
            Assert.IsTrue(_program.IsFull);
        }

        [TestMethod]
        public void GetAll_ReturnsPetsInAddingOrder()
        {
            // Arrange
            FillProgram();

            // Act
            IReadOnlyList<Pet> all = _program.GetAll();

            // Assert
            Assert.AreEqual(3, all.Count);
            Assert.AreSame(_barsik, all[0]);
            Assert.AreSame(_rex, all[1]);
            Assert.AreSame(_luna, all[2]);
        }

        [TestMethod]
        public void GetAll_ReturnsReadOnlyCollection()
        {
            // Arrange
            FillProgram();

            // Act
            IReadOnlyList<Pet> all = _program.GetAll();

            // Assert
            Assert.IsInstanceOfType<ReadOnlyCollection<Pet>>(all);
        }

        [TestMethod]
        [DataRow(1, "Barsik")]
        [DataRow(2, "Rex")]
        [DataRow(3, "Luna")]
        public void GetByNumber_ValidNumber_ReturnsPet(int number, string expectedNickname)
        {
            // Arrange
            FillProgram();

            // Act
            Pet pet = _program.GetByNumber(number);

            // Assert
            Assert.AreEqual(expectedNickname, pet.Nickname);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(-1)]
        public void GetByNumber_InvalidNumber_ThrowsArgumentOutOfRangeException(int number)
        {
            // Arrange
            FillProgram();

            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _program.GetByNumber(number));
        }

        [TestMethod]
        [DataRow("Barsik")]
        [DataRow("REX")]
        [DataRow("  luna  ")]
        public void ContainsNickname_ExistingNickname_ReturnsTrue(string nickname)
        {
            // Arrange
            FillProgram();

            // Act
            bool actual = _program.ContainsNickname(nickname);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void ContainsNickname_MissingNickname_ReturnsFalse()
        {
            // Arrange
            FillProgram();

            // Act
            bool actual = _program.ContainsNickname("Sharik");

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ContainsNickname_EmptyList_ReturnsFalse()
        {
            // Act
            bool actual = _program.ContainsNickname("Barsik");

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void FindByNickname_SeveralMatches_ReturnsAll()
        {
            // Arrange
            _program.AddPet(_barsik);
            _program.AddPet(new Pet("Barsik", Species.Dog, 7, 15));
            _program.AddPet(_rex);

            // Act
            List<Pet> found = _program.FindByNickname("barsik");

            // Assert
            Assert.AreEqual(2, found.Count);
            Assert.IsTrue(found.TrueForAll(p => p.Nickname == "Barsik"));
        }

        [TestMethod]
        public void FindByNickname_MissingNickname_ReturnsEmptyList()
        {
            // Arrange
            FillProgram();

            // Act
            List<Pet> found = _program.FindByNickname("Sharik");

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual(0, found.Count);
        }

        [TestMethod]
        [DataRow(3, 1)]
        [DataRow(5, 1)]
        [DataRow(10, 0)]
        public void FindByAge_ReturnsExpectedCount(int age, int expectedCount)
        {
            // Arrange
            FillProgram();

            // Act
            List<Pet> found = _program.FindByAge(age);

            // Assert
            Assert.AreEqual(expectedCount, found.Count);
        }

        [TestMethod]
        [DataRow(4.5, 1)]
        [DataRow(20.0, 1)]
        [DataRow(7.0, 0)]
        public void FindByWeight_ReturnsExpectedCount(double weight, int expectedCount)
        {
            // Arrange
            FillProgram();

            // Act
            List<Pet> found = _program.FindByWeight(weight);

            // Assert
            Assert.AreEqual(expectedCount, found.Count);
        }

        [TestMethod]
        [DataRow(true, 2)]
        [DataRow(false, 1)]
        public void FindByVaccination_ReturnsExpectedCount(bool vaccinated, int expectedCount)
        {
            // Arrange
            FillProgram();

            // Act
            List<Pet> found = _program.FindByVaccination(vaccinated);

            // Assert
            Assert.AreEqual(expectedCount, found.Count);
        }

        [TestMethod]
        [DataRow(Species.Cat, 2)]
        [DataRow(Species.Dog, 1)]
        [DataRow(Species.Snake, 0)]
        public void FindByType_ReturnsExpectedCount(Species type, int expectedCount)
        {
            // Arrange
            FillProgram();

            // Act
            List<Pet> found = _program.FindByType(type);

            // Assert
            Assert.AreEqual(expectedCount, found.Count);
        }

        [TestMethod]
        public void FindByType_DoesNotChangeList()
        {
            // Arrange
            FillProgram();

            // Act
            _program.FindByType(Species.Cat);

            // Assert
            Assert.AreEqual(3, _program.Count);
        }

        [TestMethod]
        public void RemoveByNumber_ValidNumber_RemovesPetAndShiftsOthers()
        {
            // Arrange
            FillProgram();

            // Act
            bool removed = _program.RemoveByNumber(1);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual(2, _program.Count);
            Assert.AreSame(_rex, _program.GetByNumber(1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        public void RemoveByNumber_InvalidNumber_ReturnsFalse(int number)
        {
            // Arrange
            FillProgram();

            // Act
            bool removed = _program.RemoveByNumber(number);

            // Assert
            Assert.IsFalse(removed);
            Assert.AreEqual(3, _program.Count);
        }

        [TestMethod]
        public void RemoveByNickname_SeveralMatches_RemovesAll()
        {
            // Arrange
            _program.AddPet(_barsik);
            _program.AddPet(new Pet("Barsik", Species.Dog, 7, 15));
            _program.AddPet(_rex);

            // Act
            int removed = _program.RemoveByNickname("Barsik");

            // Assert
            Assert.AreEqual(2, removed);
            Assert.AreEqual(1, _program.Count);
            Assert.IsFalse(_program.ContainsNickname("Barsik"));
        }

        [TestMethod]
        public void RemoveByNickname_MissingNickname_ReturnsZero()
        {
            // Arrange
            FillProgram();

            // Act
            int removed = _program.RemoveByNickname("Sharik");

            // Assert
            Assert.AreEqual(0, removed);
            Assert.AreEqual(3, _program.Count);
        }

        [TestMethod]
        [DataRow(3, 1)]
        [DataRow(10, 0)]
        public void RemoveByAge_ReturnsRemovedCount(int age, int expected)
        {
            // Arrange
            FillProgram();

            // Act
            int removed = _program.RemoveByAge(age);

            // Assert
            Assert.AreEqual(expected, removed);
            Assert.AreEqual(3 - expected, _program.Count);
        }

        [TestMethod]
        [DataRow(20.0, 1)]
        [DataRow(7.0, 0)]
        public void RemoveByWeight_ReturnsRemovedCount(double weight, int expected)
        {
            // Arrange
            FillProgram();

            // Act
            int removed = _program.RemoveByWeight(weight);

            // Assert
            Assert.AreEqual(expected, removed);
            Assert.AreEqual(3 - expected, _program.Count);
        }

        [TestMethod]
        [DataRow(true, 2)]
        [DataRow(false, 1)]
        public void RemoveByVaccination_ReturnsRemovedCount(bool vaccinated, int expected)
        {
            // Arrange
            FillProgram();

            // Act
            int removed = _program.RemoveByVaccination(vaccinated);

            // Assert
            Assert.AreEqual(expected, removed);
            Assert.AreEqual(3 - expected, _program.Count);
        }

        [TestMethod]
        [DataRow(Species.Cat, 2)]
        [DataRow(Species.Hamster, 0)]
        public void RemoveByType_ReturnsRemovedCount(Species type, int expected)
        {
            // Arrange
            FillProgram();

            // Act
            int removed = _program.RemoveByType(type);

            // Assert
            Assert.AreEqual(expected, removed);
            Assert.AreEqual(3 - expected, _program.Count);
        }

        [TestMethod]
        public void AddPet_AfterRemovingFromFullList_IsAllowed()
        {
            // Arrange
            FillProgram();
            _program.RemoveByNumber(2);
            Pet max = new Pet("Max", Species.Snake);

            // Act
            _program.AddPet(max);

            // Assert
            Assert.AreEqual(3, _program.Count);
            Assert.AreSame(max, _program.GetByNumber(3));
        }

        [TestMethod]
        public void SetMaxCapacity_LessThanCount_ThrowsInvalidOperationException()
        {
            // Arrange
            FillProgram();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _program.SetMaxCapacity(2));
            Assert.AreEqual(3, _program.MaxCapacity);
        }

        [TestMethod]
        public void SetMaxCapacity_ValidValue_AllowsMorePets()
        {
            // Arrange
            FillProgram();

            // Act
            _program.SetMaxCapacity(5);

            // Assert
            Assert.AreEqual(5, _program.MaxCapacity);
            Assert.IsFalse(_program.IsFull);
        }

        [TestMethod]
        [DataRow("Barsik", "Barsik")]
        [DataRow("  Rex  ", "Rex")]
        public void ParseNonEmpty_ValidValue_ReturnsTrimmed(string input, string expected)
        {
            // Act
            string actual = _program.ParseNonEmpty(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void ParseNonEmpty_EmptyValue_ThrowsArgumentException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _program.ParseNonEmpty(input));
        }

        [TestMethod]
        public void ParseNonEmpty_Null_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _program.ParseNonEmpty(null));
        }

        [TestMethod]
        [DataRow("5", 5)]
        [DataRow("-3", -3)]
        [DataRow("0", 0)]
        public void ParseInt_ValidNumber_ReturnsValue(string input, int expected)
        {
            // Act
            int actual = _program.ParseInt(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("4.5")]
        [DataRow("")]
        public void ParseInt_NotInteger_ThrowsFormatException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParseInt(input));
        }

        [TestMethod]
        public void ParseInt_Null_ThrowsFormatException()
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParseInt(null));
        }

        [TestMethod]
        [DataRow("1", 1)]
        [DataRow("10", 10)]
        public void ParsePositiveInt_PositiveNumber_ReturnsValue(string input, int expected)
        {
            // Act
            int actual = _program.ParsePositiveInt(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("-3")]
        public void ParsePositiveInt_NotPositive_ThrowsArgumentOutOfRangeException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _program.ParsePositiveInt(input));
        }

        [TestMethod]
        public void ParsePositiveInt_NotNumber_ThrowsFormatException()
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParsePositiveInt("abc"));
        }

        [TestMethod]
        [DataRow("4.5", 4.5)]
        [DataRow("4,5", 4.5)]
        [DataRow("10", 10.0)]
        [DataRow("-0.1", -0.1)]
        public void ParseDouble_ValidNumber_ReturnsValue(string input, double expected)
        {
            // Act
            double actual = _program.ParseDouble(input);

            // Assert
            Assert.AreEqual(expected, actual, 0.0001);
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("")]
        public void ParseDouble_NotNumber_ThrowsFormatException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParseDouble(input));
        }

        [TestMethod]
        [DataRow("так")]
        [DataRow("YES")]
        [DataRow(" y ")]
        [DataRow("1")]
        public void ParseBool_PositiveAnswer_ReturnsTrue(string input)
        {
            // Act
            bool actual = _program.ParseBool(input);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [DataRow("ні")]
        [DataRow("нет")]
        [DataRow("No")]
        [DataRow("0")]
        public void ParseBool_NegativeAnswer_ReturnsFalse(string input)
        {
            // Act
            bool actual = _program.ParseBool(input);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [DataRow("можливо")]
        [DataRow("")]
        public void ParseBool_UnknownAnswer_ThrowsFormatException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParseBool(input));
        }

        [TestMethod]
        [DataRow("1", Species.Cat)]
        [DataRow("2", Species.Dog)]
        [DataRow("3", Species.Snake)]
        [DataRow("4", Species.Hamster)]
        [DataRow("5", Species.Reptile)]
        public void ParseSpecies_MenuNumber_ReturnsSpecies(string input, Species expected)
        {
            // Act
            Species actual = _program.ParseSpecies(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseSpecies_UnknownNumber_DoesNotValidate()
        {
            // Act
            Species actual = _program.ParseSpecies("7");

            // Assert
            Assert.IsFalse(Enum.IsDefined(typeof(Species), actual));
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("6")]
        public void ParseDefinedSpecies_UnknownNumber_ThrowsArgumentException(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _program.ParseDefinedSpecies(input));
        }

        [TestMethod]
        public void ParseDefinedSpecies_ValidNumber_ReturnsSpecies()
        {
            // Act
            Species actual = _program.ParseDefinedSpecies("2");

            // Assert
            Assert.AreEqual(Species.Dog, actual);
        }

        [TestMethod]
        public void ParseDefinedSpecies_NotNumber_ThrowsFormatException()
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => _program.ParseDefinedSpecies("кіт"));
        }

        [TestMethod]
        public void GetUserMessage_MessageWithParameterName_RemovesServicePart()
        {
            // Arrange
            Exception ex = new ArgumentOutOfRangeException("Age", "Вік має бути від 0 до 60 років!");

            // Act
            string actual = _program.GetUserMessage(ex);

            // Assert
            Assert.AreEqual("Вік має бути від 0 до 60 років!", actual);
        }

        [TestMethod]
        public void GetUserMessage_PlainMessage_ReturnsUnchanged()
        {
            // Arrange
            Exception ex = new FormatException("Введіть ціле число!");

            // Act
            string actual = _program.GetUserMessage(ex);

            // Assert
            Assert.AreEqual("Введіть ціле число!", actual);
        }

        [TestMethod]
        public void GetUserMessage_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _program.GetUserMessage(null));
        }
    }
}