using Data;
using Microsoft.AspNetCore.Mvc;
namespace Classes;


[ApiController]
[Route("[Controller]")]

public class TestimoniosController : ControllerBase
{

    private readonly DataContext _context;

    public TestimoniosController(DataContext dataContext)
    {
        _context = dataContext;
    }
     [HttpGet ]
    public ActionResult<List<Testimonio>> Get()
    {
        List<Testimonio> testimonio =_context.Testimonios.OrderBy(x => x.id).ToList();
        //Revisar orden
       
        return   Ok(testimonio);
        
    }
 [HttpPost]
    public ActionResult<Testimonio> Post([FromBody] Testimonio testimonio)
    {
         Testimonio existingTestimonioItems= _context.Testimonios.Find(testimonio.id);
        if (existingTestimonioItems != null)
        {
            return Conflict("Ya existe un elemento ");
        }
        _context.Testimonios.Add(testimonio);
        _context.SaveChanges();

        string resourceUrl = Request.Path.ToString() + "/" + testimonio.id;
        return Created(resourceUrl, testimonio);
    }
    [HttpDelete ]
         [Route("{id}")]

 public  ActionResult Delete (int id)
       {
        Testimonio testimonioToDelete = _context.Testimonios.Find(id);
        if (testimonioToDelete == null)
        {
            return NotFound("menu no encontrado");
        }
        _context.Testimonios.Remove(testimonioToDelete);
        _context.SaveChanges();
         if (testimonioToDelete == null)
        {
            return NotFound();
        }
        return Ok(testimonioToDelete);
    }


[HttpGet]
[Route("fecha")]
public ActionResult<IEnumerable<Testimonio>> Get(DateTime datePost)
{
    // Filtrar testimonios por fecha
    List<Testimonio> testimonios = _context.Testimonios
        .Where(x => x.datePost.Date == datePost.Date)
        .ToList();

    // Verificar si se encontraron testimonios
    return testimonios.Count == 0 ? NotFound() : Ok(testimonios);
}
}