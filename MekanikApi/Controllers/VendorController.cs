﻿using MekanikApi.Api.Extensions;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.DTOs.Vendor;
using MekanikApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorController> _logger;
        public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("CreateProfile")]
        [SwaggerOperation(Summary = "Create A Vendor Profile", Description = "Requires Authorization Role")]

        public async Task<IActionResult> CreateVendorProfile([FromForm] CreateVendorDTO details)
        {
            try
            {
                var accessToken = HttpContext.GetAuthorizationHeader();
                var result = await _vendorService.CreateVendorProfile(details, accessToken);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating vendor profile: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMechanics()
        {
            try
            {
                var result = await _vendorService.GetAllVendors();
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }

       

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
