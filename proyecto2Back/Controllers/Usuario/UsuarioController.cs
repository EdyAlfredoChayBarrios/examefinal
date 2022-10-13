using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using proyecto2Back.Models.Usuario;
using Newtonsoft.Json;

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

}