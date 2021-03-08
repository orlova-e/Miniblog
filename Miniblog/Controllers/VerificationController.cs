using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Implementation;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Web.Filters;

namespace Web.Controllers
{
    [TypeFilter(typeof(AccessByRolesAttribute), Arguments = new object[] { new RoleType[] { RoleType.Editor, RoleType.Administrator } })]
    public class VerificationController : Controller
    {
        private CheckPreparerBuilder PreparerBuilder { get; }
        private IUserService UserService { get; }

        public VerificationController(CheckPreparerBuilder builder,
            IUserService userService)
        {
            PreparerBuilder = builder;
            UserService = userService;
        }

        [HttpGet]
        [Route("{controller}/{queueList}")]
        public IActionResult List([FromRoute] string queueList)
        {
            User user = UserService.FindByName(User.Identity.Name);
            ExtendedRole extendedRole = user.Role as ExtendedRole ?? new ExtendedRole();
            ICheckPreparer queuePreparer = PreparerBuilder.Build(user, queueList);

            if (!queuePreparer.HasAccess(queueList))
                return NotFound();

            IEnumerable<Entity> entities = new List<Entity>();
            int number = 30;
            try
            {
                static bool predicate(Entity e) => e.Accepted != true;
                entities = queuePreparer.EnumerationOf(predicate);
            }
            catch (NotImplementedException)
            {
                return NotFound(queueList);
            }

            if (entities?.Count() > number)
            {
                entities = entities.Take(number);
            }
            if (extendedRole.Type is RoleType.Administrator && queueList is "users")
            {
                ViewBag.UsersExtendedList = "user-search-results";
            }
            else
            {
                ViewBag.UsersExtendedList = "";
            }
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string list = textInfo.ToTitleCase(queueList);
            ViewBag.Header = list + "\' verification";
            if (queueList is "articles" or "pages" or "users" or "comments")
                return View("Table", entities);
            return View(entities);
        }
    }
}
