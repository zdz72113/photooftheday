using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DayPhoto.API.Entities;
using Microsoft.Extensions.Logging;
using DayPhoto.API.Repositories;
using AutoMapper;
using DayPhoto.API.Models;

namespace DayPhoto.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemParametersController : ControllerBase
    {
        private readonly IDCRepositoryBase<SystemParameter, Guid> _systemParameterRepository;
        private readonly AppUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public SystemParametersController(IDCRepositoryBase<SystemParameter, Guid> systemParameterRepository, AppUnitOfWork unitOfWork,
            ILoggerFactory logger, IMapper mapper)
        {
            _systemParameterRepository = systemParameterRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger<SystemParametersController>();
            _mapper = mapper;
        }

        // GET: api/SystemParameters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemParameterDto>>> GetSystemParameters()
        {
            var systemParameters = await _systemParameterRepository.GetListAsync();
            var model = _mapper.Map<IEnumerable<SystemParameterDto>>(systemParameters);
            return Ok(model);
        }

        // GET: api/SystemParameters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemParameterDto>> GetSystemParameter(Guid id)
        {
            var systemParameter = await _systemParameterRepository.GetByKeyAsync(id);

            if (systemParameter == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<SystemParameterDto>(systemParameter);
            return model;
        }

        // PUT: api/SystemParameters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSystemParameter(Guid id, SystemParameterUpdateDto systemParameter)
        {
            try
            {
                var dbItem = await _systemParameterRepository.GetByKeyAsync(id);
                dbItem.Value = systemParameter.Value;
                dbItem.Remark = systemParameter.Remark;
                _systemParameterRepository.Update(dbItem);
                await _unitOfWork.SaveChangesAsync(); ;
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await SystemParameterExists(id);
                if (!exists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SystemParameters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<SystemParameter>> PostSystemParameter(SystemParameter systemParameter)
        //{
        //    _context.SystemParameters.Add(systemParameter);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetSystemParameter", new { id = systemParameter.Id }, systemParameter);
        //}

        // DELETE: api/SystemParameters/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<SystemParameter>> DeleteSystemParameter(Guid id)
        //{
        //    var systemParameter = await _context.SystemParameters.FindAsync(id);
        //    if (systemParameter == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.SystemParameters.Remove(systemParameter);
        //    await _context.SaveChangesAsync();

        //    return systemParameter;
        //}

        private async Task<bool> SystemParameterExists(Guid id)
        {
            return await _systemParameterRepository.IsExistAsync(e => e.Id == id);
        }
    }
}
