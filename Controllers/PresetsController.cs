using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Data;

namespace Classes;

[ApiController]
[Route("[Controller]")]

    public class PresetsController : ControllerBase
    {

    private readonly DataContext _context;

    public PresetsController(DataContext dataContext)
    {
        _context = dataContext;
    }
    [HttpGet ]
    public ActionResult<List<Preset>> Get()
    {
        List<Preset> presets =_context.Presets.OrderByDescending(x => x.id).ToList();
        //Revisar orden
       
        return   Ok(presets);
        
    }
        // POST: api/presets
        [HttpPost]
   public ActionResult<Preset> Post([FromBody] Preset presets)
    {
         Preset existingPresetsItems= _context.Presets.Find(presets.id);
        if (existingPresetsItems != null)
        {
            return Conflict("Ya existe un elemento ");
        }
        _context.Presets.Add(presets);
        _context.SaveChanges();

        string resourceUrl = Request.Path.ToString() + "/" + presets.id;
        return Created(resourceUrl, presets);
    }
    
     [HttpGet]
    [Route("{id:int}")]
    public ActionResult<Preset> Get(int id)
    {
    Preset preset = _context.Presets.Find(id);
        return preset == null? NotFound()
            : Ok(preset);
    }
     [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        Preset presetToDelete = _context.Presets.Find(id);
        if (presetToDelete == null)
        {
            return NotFound("menu no encontrado");
        }
        _context.Presets.Remove(presetToDelete);
        _context.SaveChanges();
        return Ok(presetToDelete);
    }
    
[HttpGet]
[Route("precio")]
public ActionResult<IEnumerable<Preset>> GetPrecios([FromQuery] string sortOrder = "asc")
{
    IQueryable<Preset> query = _context.Presets;

    if (sortOrder.ToLower() == "asc")
    {
        query = query.OrderBy(p => p.precio);
    }
    else if (sortOrder.ToLower() == "desc")
    {
        query = query.OrderByDescending(p => p.precio);
    }
    else
    {
        return BadRequest("Invalid sortOrder. Use 'asc' or 'desc'.");
    }

    List<Preset> presets = query.ToList();

    return presets == null ? NotFound() : Ok(presets);
}


        [HttpGet]
    [Route("descuento")]
    public ActionResult<Preset> Get(bool descuento)
    {

        List<Preset> presets = _context.Presets.Where(x => x.descuento == descuento).ToList();
        //buscar por bool   
        return presets == null ? NotFound()
            : Ok(presets);
    
}
    
    }

