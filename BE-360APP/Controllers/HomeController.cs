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

        /*[HttpGet]
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
        }*/


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


        [HttpGet]
        [Route("master/projectopty")]
        public async Task<IActionResult> masterProjectopty()
        {
            try
            {
                var dt = await _repo.masterProjectopties();
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }


        [HttpGet]
        [Route("master/all")]
        public async Task<IActionResult> allMaster()
        {
            try
            {
                var dt = await _repo.allMasters();
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
        [Route("responReview/getall")]
        public async Task<IActionResult> responReviewgetall([FromBody] formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.responReviewgetalls(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }



        [HttpPost]
        [Route("responReview/nextForm")]
        public async Task<IActionResult> responReviewnextForm([FromBody] formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.formFastRatesresponReviewnextForm(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "Karyawan junior sukses terpilih");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("personJunior/nextForm")]
        public async Task<IActionResult> personJuniornextForm([FromBody] formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.formFastRates(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "Karyawan junior sukses terpilih");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("personJunior")]
        public async Task<IActionResult> personJunior([FromBody] formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.personJuniors(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("personLayers/onSubmit")]
        public async Task<IActionResult> GivenFeedbackonSubmit([FromBody] personLayersubmitDto dto)
        {
            try
            {
                var dt = await _repo.GivenFeedbackonSubmits(dto);
                return Requests.Response(this, new ApiStatus(200), dt, dt.Msg);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("personLayers")]
        public async Task<IActionResult> personLayer([FromBody]formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.personLayers(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("formFastRate")]
        public async Task<IActionResult> formFastRate([FromBody]formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.formFastRates(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("/formFastRate/nextReview")]
        public async Task<IActionResult> formFastRatenextReview([FromBody] formFastRateDto dto)
        {
            try
            {
                var dt = await _repo.formFastRatenextReviews(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("formFastRate/submitForm")]
        public async Task<IActionResult> formFastRatesubmitForm([FromBody] ListFastRatingRowSaveDto dto)
        {
            try
            {
                var dt = await _repo.formFastRatesubmitForms(dto);
                return Requests.Response(this, new ApiStatus(200), dt, "360 Feedback form berhasil dibuat");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AssignToOther/RowUpdate")]
        public async Task<IActionResult> AssignToOtherRowUpdate([FromBody] AssignToOtherDto dto)
        {
            try
            {
                var dt = await _repo.AssignToOtherRowUpdates(dto);
                return Requests.Response(this, new ApiStatus(200), dt, dt.Msg);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost] //on progress develop
        [Route("AssignToMe/GetAll")]
        public async Task<IActionResult> AssignToMeGetAll([FromBody] AssignToMeGetAllDto dto)
        {
            try
            {
                var dt = await _repo.AssignToMeGetAlls(dto);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AssignToOther/GetAll")]
        public async Task<IActionResult> AssignToOtherGetAll([FromBody] AssignToOtherGetAllDto dto)
        {
            try
            {
                var dt = await _repo.AssignToOtherGetAlls(dto);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpGet]
        [Route("listFastRating/{username}")]
        public async Task<IActionResult> listFastRating(string username)
        {
            try
            {
                var dt = await _repo.listFastRatings(username);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [HttpGet]
        [Route("nameAssign/{id}/{fullname}")]
        public async Task<IActionResult> nameAssign(int id, String fullname)
        {
            try
            {
                var dt = await _repo.nameAssigns(id, fullname);
                return Requests.Response(this, new ApiStatus(200), dt, $"Read Data: {dt.Count()} Successfully");
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
