using AutoMapper;
using BookingWorkplace.Business.ServiceImplementations;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Controllers
{
    public class WorkplaceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWorkplaceService _workplaceService;

        public WorkplaceController(IMapper mapper, 
            IWorkplaceService workplaceService)
        {
            _mapper = mapper;
            _workplaceService = workplaceService;
        }

        [HttpGet]
        public IActionResult Index(QueryStringParameters parameters)
        {
            try
            {
                var workplaces = _workplaceService
                    .GetWorkplacesByQueryStringParameters(parameters);

                var model = new ListOfWorkplacesModel()
                {
                    Workplaces = workplaces,
                    SearchString = parameters,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Shows the create page
        /// </summary>
        /// <returns>ViewResult for response</returns>
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Endpoint that create the workplace record in the data source.
        /// </summary>
        /// <param name="model">workplace model</param>
        /// <returns>redirect to /Workplace/Index if model is valid</returns>
        [HttpPost]
        public async Task<IActionResult> Create(WorkplaceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();

                    var dto = _mapper.Map<WorkplaceDto>(model);

                    var result = await _workplaceService.CreateWorkplaceAsync(dto);
                    return RedirectToAction("Index", "Workplace");
                }

                return View(model);
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Shows the edit page
        /// </summary>
        /// <param name="id">unique identifier of the object to be changed</param>
        /// <returns>ViewResult for response</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                if (id == Guid.Empty) throw new ArgumentException(nameof(id));

                var dto = await _workplaceService.GetWorkplaceByIdAsync(id);

                var editModel = _mapper.Map<WorkplaceModel>(dto);

                return View(editModel);
            }
            catch (ArgumentException ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Endpoint that changes the workplace record in the data source.
        /// </summary>
        /// <param name="model">modified model</param>
        /// <returns>redirect to /Workplace/Index if model is valid</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(WorkplaceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dto = _mapper.Map<WorkplaceDto>(model);
                    await _workplaceService.UpdateAsync(model.Id, dto);

                    return RedirectToAction("Index", "Workplace");

                }
                return View(model);
            }
            catch (ArgumentException ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Checks the workplace for existing workplace with the same room number
        /// and floor number and desk number in the data source.
        /// </summary>
        /// <param name="deskNumber">desk number of the current workplace as a string</param>
        /// <param name="floor">floor number of the current workplace as a string</param>
        /// <param name="room">room number of the current workplace as a string</param>
        /// <returns>OkObjectResult with true if the equipment is does not exist or false if the equipment exist</returns>
        [HttpPost]
        public async Task<IActionResult> CheckWorkplaceForExistence(string deskNumber, string floor, string room)
        {
            try
            {
                var isValid = await _workplaceService.IsWorkplaceExistAsync(room, floor, deskNumber);
                return Ok(!isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
