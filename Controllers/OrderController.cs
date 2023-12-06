using Microsoft.AspNetCore.Mvc;
using Data;
using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

[ApiController]
[Route("[Controller]")]
public class OrderController : ControllerBase
{
    private readonly DataContext _context;

    public OrderController(DataContext dataContext)
    {
        _context = dataContext;
    }

    [HttpGet]
    public ActionResult<List<OrderPro>> Get()
    {
        List<OrderPro> orders = _context.OrderPro.ToList();
        return orders == null ? NotFound() : Ok(orders);
    }

    [HttpGet]
    [Route("name")]
    public ActionResult<List<Usuario>> Get(string name)
    {
        List<Usuario> users = _context.Usuarios
            .Where(x => x.name.Contains(name))
            .OrderByDescending(x => x.name)
            .ToList();
        return users == null ? NotFound() : Ok(users);
    }

    [HttpGet]
    [Route("{id:int}")]
    public ActionResult GetByUserId(int id)
    {
        var orders = _context.OrderPro.Where(e => e.userId == id).ToList();
        if (orders.Count == 0)
        {
            return NotFound();
        }
        else
        {
            var paymentList = new List<Pago>();
            orders.ForEach(order =>
            {
                var payment = _context.Pagoss.FirstOrDefault(p => p.id == order.pagoId);
                paymentList.Add(payment);
            });
            return Ok(paymentList);
        }
    }

    [HttpPost]
    public ActionResult<OrderPro> Post([FromBody] OrderPro orderPro)
    {
        if (orderPro == null)
        {
            return BadRequest();
        }

        try
        {
            // Asignar los objetos completos asociados a las claves foráneas solo si los IDs están presentes
            if (orderPro.presetId != null && orderPro.presetId != 0)
            {
                var preset = _context.Presets.Find(orderPro.presetId);
                if (preset != null)
                {
                    orderPro.preset = preset;
                    orderPro.total += preset.precio;
                }
                else
                {
                    return NotFound($"Preset with ID {orderPro.presetId} not found.");
                }
            }

            _context.OrderPro.Add(orderPro);
            _context.SaveChanges();

            decimal totalPayment = orderPro.pago?.total ?? 0;

            string resourceUrl = Request.Path.ToString() + "/" + orderPro.id;

            return Created(resourceUrl, new { Order = orderPro, TotalPayment = totalPayment });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al procesar la solicitud: " + ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var orderToDelete = _context.OrderPro.Find(id);

        if (orderToDelete == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        try
        {
            _context.OrderPro.Remove(orderToDelete);
            _context.SaveChanges();
            return Ok($"Order with ID {id} has been deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting order with ID {id}: {ex.Message}");
        }
    }

[HttpGet]
[Route("sum")]
public ActionResult<decimal> GetTotalSum()
{
    try
    {
        decimal totalSum = _context.OrderPro.Sum(order => order.total);

        // Supongamos que tienes un único registro en Pagos (puedes adaptarlo según tus necesidades)
        var payment = _context.Pagoss.FirstOrDefault();

        if (payment != null)
        {
            payment.total = totalSum;
            _context.SaveChanges();
            return Ok(totalSum);
        }
        else
        {
            return NotFound("No payment record found to update.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error al procesar la solicitud: " + ex.Message);
    }
}

 
}
