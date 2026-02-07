using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Data;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QualificationTypesController : ControllerBase
{
    private readonly IQualificationService _qualificationService;

    public QualificationTypesController(IQualificationService qualificationService)
    {
        _qualificationService = qualificationService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllQualificationTypes()
    {
        var types = await _qualificationService.GetAllQualificationTypesAsync();
        return Ok(types);
    }
}