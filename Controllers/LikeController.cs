using Data;
using Microsoft.AspNetCore.Mvc;
namespace Classes;


[ApiController]
[Route("[Controller]")]

public class LikesController : ControllerBase
{

    private readonly DataContext _context;

    public LikesController(DataContext dataContext)
    {
        _context = dataContext;
    }
     [HttpGet ]
    public ActionResult<List<Likes>> Get()
    {
        List<Likes> like =_context.Likes.OrderByDescending(x => x.id).ToList();
        //Revisar orden
       
        return   Ok(like);
        
    }

         [HttpGet]
    [Route("{id:int}")]
    public ActionResult<Likes> Get(int id)
    {
    Likes Like = _context.Likes.Find(id);
        return Like == null? NotFound()
            : Ok(Like);
    }



 [HttpPost]
    public ActionResult<Likes> Post([FromBody] Likes likes)
    {
         Likes existingLikeItems= _context.Likes.Find(likes.id);
        if (existingLikeItems != null)
        {
            return Conflict("Ya existe un elemento ");
        }
        _context.Likes.Add(likes);
        _context.SaveChanges();

        string resourceUrl = Request.Path.ToString() + "/" + likes.id;
        return Created(resourceUrl, likes);
    }
    

}