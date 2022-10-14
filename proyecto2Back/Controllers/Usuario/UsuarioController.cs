using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using proyecto2Back.Models.Usuario;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;

namespace proyecto2Back.Controllers.Usuario;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly IConfiguration conf;

    public UsuarioController(IConfiguration config)
    {
        conf = config;
    }


    [HttpPost]
    public async Task<IActionResult> Login(AuthModels userlogin)
    {
        IEnumerable<UsuarioModels> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = @"
                    select * from usuario u 
                    where 
                    usuario  = '" + userlogin.usuario + @"'
                    and
                    contrasena  =  '" + userlogin.contrasena + @"'
                    and u.estado = 1;
                    ";


                lst = db.Query<UsuarioModels>(sql);
                Json = JsonConvert.SerializeObject(lst);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        if (lst.Count() > 0)
            return Ok(new
            {
                autenticado = true,
                userdata = lst,
                msg = "logueado exitosamente"
                // token
            });
        //return Ok(Json);
        return StatusCode(201);
    }



    [HttpPost("registro")]
    public async Task<IActionResult> Register(RegistroModels registro)
    {
        int correlativo = 0;
        IEnumerable<RegistroModels> lst = null;
        IEnumerable<Models.Empleado.Empleado> lst2 = null; 
        var Json = JsonConvert.SerializeObject(lst);
        int idEmpleado = 0;
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format("INSERT INTO empleado (nombres,apellidos,dpi) VALUES ('{0}','{1}','{2}');",registro.nombre,registro.apellido,registro.dpi);

                db.Execute(sql);

                var sql2 = string.Format("select id_empleado as empleado from empleado where nombres='{0}' and apellidos='{1}' and dpi='{2}';", registro.nombre, registro.apellido, registro.dpi);


                var query = db.QuerySingle(sql2);
                idEmpleado = query.empleado;




                var sql3 = string.Format("INSERT INTO usuario (usuario,contrasena,id_empleado,tipo_empleado,estado) VALUES ('{0}','{1}',{2},2,1);",registro.usuario, registro.contrasena, idEmpleado);
                    db.Execute(sql3);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

    
            return Ok(new
            {
                msg = "logueado exitosamente"
             
            });
     
    }

}