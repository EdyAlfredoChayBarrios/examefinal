using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace proyecto2Back.Controllers.Empleado;

[Route("api/[controller]")]
[ApiController]
public class EmpleadoController : ControllerBase
{
    private readonly IConfiguration conf;

    public EmpleadoController(IConfiguration config)
    {
        conf = config;
    }




    [HttpGet("todo")]
    public async Task<IActionResult> GetMovimientos()
    {
        IEnumerable<Models.Empleado.Empleado> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = @"select * from empleado e inner join usuario u on u.id_empleado=e.id_empleado where u.estado=1 ";

                lst = db.Query<Models.Empleado.Empleado>(sql);
                Json = JsonConvert.SerializeObject(lst);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }



    [HttpPost("eliminar/{idUsuario}")]
    public async Task<IActionResult> PostEliminar(int idUsuario)
    {
     
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"update usuario set estado=0 where id_usuario={0} ",idUsuario);

                db.Execute(sql);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok("Datos ingresados con exito!");
    }





    [HttpGet("horarios/{idUsuario}")]
    public async Task<IActionResult> GetHorariosEmpleado(int idUsuario)
    {
        IEnumerable<Models.Empleado.Empleado> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"select * from historial_emp where id_usuario={0} ",idUsuario);

                lst = db.Query<Models.Empleado.Empleado>(sql);
                Json = JsonConvert.SerializeObject(lst);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }
}