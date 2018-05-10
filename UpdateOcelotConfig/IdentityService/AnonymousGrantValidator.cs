using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityService
{
    public class AnonymousGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public AnonymousGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public string GrantType => "anonymous";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //var userToken = context.Request.Raw.Get("token");

            //if (string.IsNullOrEmpty(userToken))
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //    return;
            //}

            //var result = await _validator.ValidateAccessTokenAsync(userToken);
            //if (result.IsError)
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //    return;
            //}

            // get user's identity
            //var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var claims = new List<Claim>() { new Claim("role", GrantType) }; // Claim 用于配置服务站点 [Authorize("anonymous")]
            context.Result = new GrantValidationResult(GrantType, GrantType, claims);
        }
    }
}
