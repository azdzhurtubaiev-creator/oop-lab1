using FluentValidation;

namespace ConsoleApp4
{
    public class PetValidator : AbstractValidator<Pet>
    {
        public PetValidator()
        {
            RuleFor(p => p.Nickname)
                .NotEmpty().WithMessage("Кличка не може бути порожньою!")
                .Length(Program.MinNameLength, Program.MaxNameLength)
                .WithMessage($"Довжина клички має бути від {Program.MinNameLength} до {Program.MaxNameLength} символів!");

            RuleFor(p => p.Age)
                .InclusiveBetween(Program.MinAge, Program.MaxAge)
                .WithMessage($"Вік має бути від {Program.MinAge} до {Program.MaxAge} років!");

            RuleFor(p => p.Weight)
                .InclusiveBetween(Program.MinWeight, Program.MaxWeight)
                .WithMessage($"Вага має бути від {Program.MinWeight} до {Program.MaxWeight} кг!");

            RuleFor(p => p.Type)
                .IsInEnum()
                .WithMessage("Обрано неіснуючий вид тварини!");
        }
    }
}