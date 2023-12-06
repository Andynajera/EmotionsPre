using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Classes
{
    [ApiController]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuariosController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get()
        {
            List<Usuario> usuario = _context.Usuarios.OrderByDescending(x => x.id).ToList();
            return Ok(usuario);
        }

[HttpGet]
[Route("socios")]
public ActionResult<List<Usuario>> Get(bool esSocio)
{
    // Filtrar usuarios por si son socios
    List<Usuario> socios = _context.Usuarios
        .Where(x => x.esSocio == esSocio)
        .ToList();

    // Verificar si se encontraron socios
    return socios.Count == 0 ? NotFound() : Ok(socios);
}
  [HttpGet("GetByName")]
        public ActionResult<List<Usuario>> GetByName(string name)
        {
            // Filtrar usuarios por nombre
            List<Usuario> usuarios = _context.Usuarios
                .Where(u => u.name.Contains(name))
                .ToList();

            // Verificar si se encontraron usuarios
            if (usuarios.Count == 0)
            {
                return NotFound();
            }

            return Ok(usuarios);
        
    }

        [HttpPost]
        public ActionResult Post([FromBody] Usuario usuario)
        {
            try
            {
                // Asignar la fecha actual al campo de fecha de inscripción
                usuario.dateInscription = DateTime.Now;

                // Guardar el usuario en la base de datos
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Enviar correo electrónico al administrador
                EnviarCorreoAdministrador(usuario);

                // Enviar correo de confirmación al usuario
                EnviarCorreoUsuario(usuario);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

[HttpDelete]
[Route("{id}")]
public ActionResult Delete(int id)
{
    try
    {
        // Buscar el usuario por ID
        Usuario usuario = _context.Usuarios.Find(id);

        if (usuario == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }

        // Eliminar el usuario de la base de datos
        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();

        // Enviar correo de notificación al administrador
        EnviarCorreoEliminacion(usuario);

        return Ok();
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
private void EnviarCorreoEliminacion(Usuario usuarioEliminado)
{
    // Usar la dirección de correo electrónico del administrador
    string paraAdmin = "eternalemotions@hotmail.com";
    string asuntoAdmin = "Usuario eliminado (Administrador)";
    string cuerpoAdmin = $"Se ha eliminado un usuario con los siguientes datos:\n\n" +
                        $"ID: {usuarioEliminado.id}\n" +
                        $"Nombre: {usuarioEliminado.name}\n" +
                        $"Apellidos: {usuarioEliminado.apellidos}\n" +
                        $"Correo Electrónico: {usuarioEliminado.email}\n" +
                        $"Descripción: {usuarioEliminado.descripcion}\n" +
                        $"Fecha de Inscripción: {usuarioEliminado.dateInscription.ToString("dd/MM/yyyy HH:mm:ss")}\n";

    EnviarCorreo(paraAdmin, asuntoAdmin, cuerpoAdmin);
}

        private void EnviarCorreoAdministrador(Usuario usuario)
        {
            // Usar la dirección de correo electrónico del administrador
            string paraAdmin = "eternalemotions@hotmail.com";
            string asuntoAdmin = "Nuevo usuario registrado (Administrador)";
            string cuerpoAdmin = $"Se ha registrado un nuevo usuario con los siguientes datos:\n\n" +
                                $"ID: {usuario.id}\n" +
                                $"Nombre: {usuario.name}\n" +
                                $"Apellidos: {usuario.apellidos}\n" +
                                $"Correo Electrónico: {usuario.email}\n" +
                                $"Descripción: {usuario.descripcion}\n";

            EnviarCorreo(paraAdmin, asuntoAdmin, cuerpoAdmin);
        }

        private void EnviarCorreoUsuario(Usuario usuario)
        {
            // Usar la dirección de correo electrónico del usuario
            string paraUsuario = usuario.email;
            string asuntoUsuario = "Registro exitoso";
            string cuerpoUsuario = $"¡Gracias por registrarte, {usuario.name}!\n\n" +
                                   $"Tu registro ha sido exitoso. Agradecemos tu confianza.\n\n" +
                                   $"Detalles de tu registro:\n\n" +
                                   $"ID: {usuario.id}\n" +
                                   $"Nombre: {usuario.name}\n" +
                                   $"Apellidos: {usuario.apellidos}\n" +
                                   $"Correo Electrónico: {usuario.email}\n" +
                                   $"Descripción: {usuario.descripcion}\n" +
                                   $"Fecha de Inscripción: {usuario.dateInscription.ToString("dd/MM/yyyy HH:mm:ss")}\n";

            EnviarCorreo(paraUsuario, asuntoUsuario, cuerpoUsuario);
        }

        private void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            using (SmtpClient clienteSmtp = new SmtpClient("smtp.office365.com"))
            {
                clienteSmtp.UseDefaultCredentials = false;
                clienteSmtp.Credentials = new NetworkCredential("eternalemotions@hotmail.com", "eternal_Emotions23");
                clienteSmtp.EnableSsl = true;
                clienteSmtp.Port = 587;

                using (MailMessage mensaje = new MailMessage("eternalemotions@hotmail.com", destinatario, asunto, cuerpo))
                {
                    clienteSmtp.Send(mensaje);
                }
            }
        }
          [HttpPut("{id}")]
        public ActionResult UpdateUsuario(int id, [FromBody] Usuario usuarioUpdateDto)
        {
            try
            {
                // Buscar el usuario por ID
                Usuario usuario = _context.Usuarios.Find(id);

                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                // Actualizar solo si se proporcionan nuevos valores
                if (!string.IsNullOrEmpty(usuarioUpdateDto.name))
                {
                    usuario.name = usuarioUpdateDto.name;
                }

                if (!string.IsNullOrEmpty(usuarioUpdateDto.apellidos))
                {
                    usuario.apellidos = usuarioUpdateDto.apellidos;
                }

                // Actualizar si se proporciona el valor de EsSocio
                if (usuarioUpdateDto.esSocio)
                {
                    usuario.esSocio = usuarioUpdateDto.esSocio;
                }

                // Guardar los cambios en la base de datos
                _context.SaveChanges();

                // Enviar correo de notificación al administrador
                EnviarCorreoActualizacion(usuario);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void EnviarCorreoActualizacion(Usuario usuarioActualizado)
        {
            // Usar la dirección de correo electrónico del administrador
            string paraAdmin = "eternalemotions@hotmail.com";
            string asuntoAdmin = "Usuario actualizado (Administrador)";
            string cuerpoAdmin = $"Se ha actualizado la información de un usuario con los siguientes datos:\n\n" +
                                $"ID: {usuarioActualizado.id}\n" +
                                $"Nuevo Nombre: {usuarioActualizado.name}\n" +
                                $"Nuevos Apellidos: {usuarioActualizado.apellidos}\n" +
                                $"Es Socio: {usuarioActualizado.esSocio}\n" +
                                $"Correo Electrónico: {usuarioActualizado.email}\n" +
                                $"Descripción: {usuarioActualizado.descripcion}\n" +
                                $"Fecha de Actualización: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}\n";

            EnviarCorreo(paraAdmin, asuntoAdmin, cuerpoAdmin);
        }

    }
    }
