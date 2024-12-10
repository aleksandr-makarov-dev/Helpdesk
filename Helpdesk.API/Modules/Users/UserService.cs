using FluentResults;
using Helpdesk.API.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Helpdesk.API.Errors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder.Extensions;

namespace Helpdesk.API.Modules.Users
{
    public class UserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JsonWebTokenService _jsonWebTokenService;

        public UserService(SignInManager<User> signInManager, UserManager<User> userManager, JsonWebTokenService jsonWebTokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jsonWebTokenService = jsonWebTokenService;
        }

        public async Task<Result> RegisterUserAsync(RegisterRequest request)
        {
            var foundUser = await _userManager.FindByEmailAsync(request.Email);

            if (foundUser is not null)
            {
                return Result.Fail(new Error("Email address is already registered"));
            }

            var userToCreate = request.ToUser();
            
            // TODO: remove when email sending is implemented
            userToCreate.EmailConfirmed = true;

            var createUserResult = await _userManager.CreateAsync(userToCreate, request.Password);

            if (!createUserResult.Succeeded)
            {
                return Result.Fail(new Error(createUserResult.Errors.First().Description));
            }

            return Result.Ok();
        }

        public async Task<Result<SessionResponse>> LoginUserAsync(LoginRequest request, HttpContext httpContext)
        {
            var foundUser = await _userManager.FindByEmailAsync(request.Email);

            if (foundUser is null || !(await _userManager.IsEmailConfirmedAsync(foundUser)))
            {
                return Result.Fail(new Error("Email address not found or it must be confirmed"));
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(foundUser,request.Password,true);

            if (signInResult.IsLockedOut)
            {
                return Result.Fail(new Error("Account is locked out. Try again later"));
            }

            // TODO: handle cases like two-factor authentication and not allowed
            if (!signInResult.Succeeded)
            {
                return Result.Fail(new Error("Invalid email or password"));
            }

            var identity = (await _signInManager.CreateUserPrincipalAsync(foundUser)).Identities.First();

            var refreshToken = await _userManager.GenerateUserTokenAsync(foundUser, "Default","Token");

            await _userManager.SetAuthenticationTokenAsync(foundUser, "local", "refresh_token", refreshToken);

            // somehow set cookie... (fix later)
            await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
            var accessToken = _jsonWebTokenService.GenerateTokenAsync(identity);

            return Result.Ok(new SessionResponse(accessToken));
        }

        public async Task<Result<SessionResponse>> RefreshSessionAsync(ClaimsPrincipal user, HttpContext httpContext)
        {
            var foundUser = await _userManager.FindByIdAsync(user.FindFirstValue(ClaimTypes.NameIdentifier));

            if (foundUser is null)
            {
                return Result.Fail(new NotFoundError("Session expired"));
            }

            ClaimsIdentity? identity = (await _signInManager.CreateUserPrincipalAsync(foundUser)).Identities.FirstOrDefault();

            var accessToken = _jsonWebTokenService.GenerateTokenAsync(identity);

            return Result.Ok(new SessionResponse(accessToken));
        }

        public async Task LogoutUserAsync()
        { 
            await _signInManager.SignOutAsync();
        }
    }
}
