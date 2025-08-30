using Microsoft.AspNetCore.Mvc;
using PlatformSchool.Application.Contracts;
using PlatformSchool.Domain.Models.Department;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformSchool.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentApiController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentApiController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        // GET: api/<DepartmentApiController>
        [HttpGet("ObtenerDepartamentos")]
        public async Task<IActionResult> Get()
        {
            var result = await _departmentService.GetAllDepartmentsAsync();

            if (!result.IsSuccess)
                return BadRequest(result);
            

            return Ok(result);

        }

        // GET api/<DepartmentApiController>/5
        [HttpGet("ObtenerDeptoPorId")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _departmentService.GetDepartmentByIdAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);


            return Ok(result);
        }

        // POST api/<DepartmentApiController>
        [HttpPost("Guardar")]
        public async Task<IActionResult> Post([FromBody] DepartmentCreateModel createModel)
        {
            var result = await _departmentService.CreateDepartmentAsync(createModel);

            if (!result.IsSuccess)
                return BadRequest(result);


            return Ok(result);
        }

        // PUT api/<DepartmentApiController>/5
        [HttpPost("Actualizar")]
        public async Task<IActionResult> Put([FromBody] DepartmentUpdateModel updateModel)
        {
            var result = await _departmentService.UpdateDepartmentAsync(updateModel);

            if (!result.IsSuccess)
                return BadRequest(result);


            return Ok(result);
        }

        
    }
}
