using APISALQUOTE.Helpers;
using BE_360APP.Models;
using BE_360APP.Models.Repo;
using Microsoft.AspNetCore.Mvc;

namespace BE_360APP.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IRepository _repo;

        public HomeController(IConfiguration configuration, IRepository repo)
        {
            _configuration = configuration;
            _repo = repo;
        }

        [HttpGet]
        [Route("Dashboard/{Param1}")]
        public async Task<IActionResult> Dashboard(String? Param1)
        {
            try
            {
                whereString ws = new whereString();
                ws.Param1 = Param1;
                var dt = await _repo.Dashboards(ws);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");

            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }


        [HttpPost]
        [Route("Auth/Login")]
        public async Task<IActionResult> Login([FromBody] loginDto dto)
        {
            try
            {
                var dt = await _repo.Logins(dto);
                return Requests.Response(this, new ApiStatus(200), dt, dt.Msg);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }



        [HttpPost]
        [Route("master/value")]
        public async Task<IActionResult> masterValue([FromBody] string dto)
        {
            try
            {
                var dt = await _repo.masterValues();
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AssignToOther/RowSave")]
        public async Task<IActionResult> AssignToOtherRowSave([FromBody] AssignToOtherDto dto)
        {
            try
            {
                var dt = await _repo.AssignToOtherRowSaves(dto);
                return Requests.Response(this, new ApiStatus(200), dt, dt.Msg);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }


        [HttpPost]
        [Route("Auth/Register/LookupFullName")]
        public async Task<IActionResult> LookupFullName([FromBody] usernameDto dto)
        {
            try
            {
                var dt = await _repo.LookupFullNames(dto);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }


        [HttpPost]
        [Route("Auth/Submit/Register")]
        public async Task<IActionResult> Register([FromBody] registerDto dto)
        {
            try
            {
                var dt = await _repo.Registers(dto);
                return Requests.Response(this, new ApiStatus(200), dt, dt.Msg);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

    }
}
