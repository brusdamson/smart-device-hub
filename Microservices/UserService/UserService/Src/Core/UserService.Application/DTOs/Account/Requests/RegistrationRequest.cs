using FluentValidation;
using UserService.Application.Helpers;
using UserService.Application.Interfaces;

namespace UserService.Application.DTOs.Account.Requests;

public class RegistrationRequest
{
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
}

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator(ITranslator translator)
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .NotNull()
            .WithName(p => translator[nameof(p.UserName)]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .Matches(Regexs.Password)
            .WithName(p => translator[nameof(p.Password)]);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .Matches(Regexs.Email)
            .WithName(p => translator[nameof(p.Email)]);
    }
}