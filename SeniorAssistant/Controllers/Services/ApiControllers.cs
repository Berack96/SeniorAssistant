using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Models.Users;

namespace SeniorAssistant.Controllers.Services
{
    [Route("api/[controller]")]
    public class HeartbeatController : CrudTimeController<Heartbeat>
    { }

    [Route("api/[controller]")]
    public class SleepController : CrudTimeController<Sleep>
    { }

    [Route("api/[controller]")]
    public class StepController : CrudTimeController<Step>
    { }

    [Route("api/[controller]")]
    public class UserController : CrudController<User>
    { }

    [Route("api/[controller]")]
    public class PatientController : CrudController<Patient>
    { }

    [Route("api/[controller]")]
    public class DoctorController : CrudController<Doctor>
    { }
}
