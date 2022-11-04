using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Newtonsoft.Json;

namespace proyecto3Back.Controllers.Empleado;

[Route("api/[controller]")]
[ApiController]
public class DepartamentoController : ControllerBase
{
    private readonly IConfiguration conf;

    public DepartamentoController(IConfiguration config)
    {
        conf = config;
    }




    [HttpGet("todo")]
    public async Task<IActionResult> GetMovimientos()
    {
        IEnumerable<Models.Departamento.Departamento> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = @"select * from departamento d ";

                lst = db.Query<Models.Departamento.Departamento>(sql);
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