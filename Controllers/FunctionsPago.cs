using Microsoft.AspNetCore.Mvc;
using Data;
using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Classes
{
    [ApiController]
    [Route("[Controller]")]
    public class PagosController : ControllerBase
    {
        private readonly DataContext _context;

        public PagosController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        public ActionResult<List<Pago>> Get()
        {
            List<Pago> pagos = _context.Pagoss.OrderByDescending(x => x.id).ToList();
            return Ok(pagos);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Pago pago)
        {
            try
            {
                _context.Pagoss.Add(pago);
                _context.SaveChanges();

                EnviarCorreoAdministrador(pago);
                EnviarCorreoUsuario(pago);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void EnviarCorreoAdministrador(Pago pago)
        {
            string adminEmail = "eternalemotions@hotmail.com";
            string adminSubject = "Nuevo usuario registrado (Administrador)";
            string adminBody = $"Se ha registrado un nuevo usuario con los siguientes datos:\n\n" +
                               $"ID: {pago.id}\n" +
                               $"Nombre: {pago.name}\n" +
                               $"Correo Electrónico: {pago.email}\n" +
                               $"Total: {pago.total}\n";

            EnviarCorreo(adminEmail, adminSubject, adminBody);
        }

        private void EnviarCorreoUsuario(Pago pago)
        {
            string userEmail = pago.email;
            string userSubject = "Registro exitoso";
            string userBody = $"¡Gracias por registrarte, {pago.name}!\n\n" +
                              $"Tu registro ha sido exitoso. Agradecemos tu confianza.\n\n" +
                              $"Detalles de tu registro:\n\n" +
                              $"ID: {pago.id}\n" +
                              $"Nombre: {pago.name}\n" +
                              $"Correo Electrónico: {pago.email}\n" +
                              $"Total: {pago.total}\n";

            EnviarCorreo(userEmail, userSubject, userBody);
        }

        private void EnviarCorreo(string recipient, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.office365.com"))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("eternalemotions@hotmail.com", "eternal_Emotions23");
                smtpClient.EnableSsl = true;
                smtpClient.Port = 587;

                using (MailMessage message = new MailMessage("eternalemotions@hotmail.com", recipient, subject, body))
                {
                    smtpClient.Send(message);
                }
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            else
            {
                Pago pagoToDelete = _context.Pagoss.Find(id);
                if (pagoToDelete == null)
                {
                    return NotFound("Pago no encontrado");
                }

                _context.Pagoss.Remove(pagoToDelete);
                _context.SaveChanges();

                var orders = _context.OrderPro.ToList();
                orders.ForEach(o =>
                {
                    if (o.pagoId == id)
                    {
                        _context.OrderPro.Remove(o);
                    }
                });

                _context.SaveChanges();
                
                return Ok();
            }
        }

        [HttpPost("CalculateTotal")]
        public ActionResult<decimal> CalculateTotal([FromBody] List<int> presetIds)
        {
            try
            {
                if (presetIds == null || presetIds.Count == 0)
                {
                    return BadRequest("La lista de IDs de preset no puede estar vacía.");
                }

                var presets = _context.Presets.Where(p => presetIds.Contains(p.id)).ToList();

                if (presets.Count != presetIds.Count)
                {
                    return NotFound("No se encontraron todos los presets proporcionados.");
                }

                decimal total = presets.Sum(p => p.precio);

                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la solicitud: " + ex.Message);
            }
        }
    }
}
