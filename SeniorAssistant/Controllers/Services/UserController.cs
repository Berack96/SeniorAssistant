using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;

namespace SeniorAssistant.Controllers.Services
{
    [Route("api/[controller]")]
    public class UserController : CrudController<User>
    { }
}