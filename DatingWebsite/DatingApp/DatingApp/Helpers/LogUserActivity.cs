using DatingApp.Extensions;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var username = resultContext.HttpContext.User.GetUsername();
            // var userId = resultContext.HttpContext.User.GetUserId();
            
            var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            //var user = await repo.GetUserByIdAsync(userId);
            var user = await uow.UserRepository.GetUserByNameAsync(username);
            user.LastActive = DateTime.UtcNow;
            await uow.Complete();
        }
    }
}
