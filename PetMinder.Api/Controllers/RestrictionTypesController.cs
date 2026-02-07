using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestrictionTypesController : ControllerBase
{
    private readonly IRestrictionService _restrictionService;

    public RestrictionTypesController(IRestrictionService restrictionService)
    {
        _restrictionService = restrictionService;
    }

    [HttpGet]
    [Authorize] 
    public async Task<IActionResult> GetAllRestrictionTypes()
    {
        var types = await _restrictionService.GetAllRestrictionTypesAsync();
        return Ok(types);
    }
}