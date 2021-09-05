using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiTemplate.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IRepository _repository;
        public StudentController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("get-students")]
        public async Task<IActionResult> GetStudents()
        {
            return Ok(await _repository.GetStudents());
        }
    }
}
