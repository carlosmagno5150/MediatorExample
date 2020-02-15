using System.Collections.Generic;
using System.Linq;
using FluentValidation;

// ReSharper disable IdentifierTypo

namespace Project.Cqrs.Commands
{
    public abstract class Command : ICommand
    {
    }

    public abstract class ValidatableCommand: ICommand
    {
        public List<string> ValidationMessages { get; private set; }

        protected ValidatableCommand()
        {
            ValidationMessages = new List<string>();
        }

        protected abstract IValidator Validator { get; }

        public bool IsValid()
        {
            var val = Validator.Validate(this);
            ValidationMessages = val
                .Errors
                .Where(x => x != null)
                .Select(i => i.ErrorMessage)
                .ToList();
            return val.IsValid;
        }

       
    }
}