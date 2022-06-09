using BigSizeFashion.Business.Helpers.RequestObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Validators
{
    public class StaffLoginValidator : AbstractValidator<StaffLoginRequest>
    {
        public StaffLoginValidator()
        {
            RuleFor(s => s.Username)
                .NotEmpty()
                .NotNull()
                .MaximumLength(20);

            RuleFor(s => s.Password)
                .NotEmpty()
                .NotNull()
                .MaximumLength(20);
        }
    }
}
