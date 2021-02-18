using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DayPhoto.API.Infrastructure.Data;
using DayPhoto.API.Repositories;
using DayPhotos.API.Entities;
using DayPhotos.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DayPhotos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDCRepositoryBase<Photo, Guid> _photoRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public PhotosController(IDCRepositoryBase<Photo, Guid> photoRepository, ILoggerFactory logger, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _logger = logger.CreateLogger<PhotosController>();
            _mapper = mapper;
        }

        //// GET: api/Photos/5
        //[HttpGet("{source}/{date}")]
        //public async Task<ActionResult<PhotoDto>> GetPhoto(PhotoSource source, DateTime date)
        //{
        //    var photo = await _photoAppService.GetPhotoAsync(source, date);
        //    return photo;
        //}

        [HttpGet("[action]")]
        public async Task<ActionResult<Page<PhotoDto>>> GetPhotos([FromQuery]QueryPhotoDto query)
        {
            var dbItmes = await _photoRepository.GetPageListAsync(query.PageIndex ?? 0, query.PageSize ?? 10,
                r => (r.Source == query.Source), r => r.OrderByDescending(x => x.Date), null);
            var model = _mapper.Map<Page<PhotoDto>>(dbItmes);
            return Ok(model);
        }
    }
}