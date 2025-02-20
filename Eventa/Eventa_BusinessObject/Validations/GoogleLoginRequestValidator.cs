using Eventa_BusinessObject.DTOs.Login;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Validations
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access Token is required!");
        }
    }
}
