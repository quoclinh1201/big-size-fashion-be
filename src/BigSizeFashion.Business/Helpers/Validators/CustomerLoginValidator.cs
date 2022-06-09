using BigSizeFashion.Business.Helpers.RequestObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Validators
{
    public class CustomerLoginValidator : AbstractValidator<CustomerLoginRequest>
    {
        public CustomerLoginValidator()
        {
            RuleFor(c => c.PhoneNumber)
                .NotEmpty()
                .NotNull()
                .Length(10)
                .Matches(new Regex(@"[0]{1}[0-9]{9}"));
        }
    }
}
