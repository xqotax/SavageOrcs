using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SavageOrcs.DbContext;
using SavageOrcs.Web.ViewModels.Mark;

namespace SavageOrcs.Web.Controllers
{
    public class MarkController : Controller
    {
        private readonly ILogger<MarkController> _logger;
        private UserManager<User> _userManager;

        public MarkController(ILogger<MarkController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            var addMarkViewModel = new AddMarkViewModel()
            {
                Lat = "48.6125528",
                Lng = "31.0275809",
                Zoom = "6",
                AreaId = null,
                Areas = new List<SelectListItem>()
            };
            addMarkViewModel.Areas.Add(new SelectListItem("Городенка", Guid.NewGuid().ToString()));
            addMarkViewModel.Areas.Add(new SelectListItem("Київ", Guid.NewGuid().ToString()));

            return View(addMarkViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult SaveMark([FromBody] AddMarkViewModel saveMarkViewModel)
        {
            try
            {
                Convert.ToDouble(saveMarkViewModel.Lat);
                Convert.ToDouble(saveMarkViewModel.Lng);
            }
            catch
            {
                //error
            }
            
            var a = _userManager.GetUserId(User);

            return Json(true);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Catalogue(string searchFilter = "")
        {
            return View("Catalogue");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }


    }
}
