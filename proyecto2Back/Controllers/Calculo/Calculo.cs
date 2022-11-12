using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using proyecto3Back.Models.Usuario;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;

namespace proyecto3Back.Controllers.Calculo;

[Route("api/[controller]")]
[ApiController]
public class CalculoController : ControllerBase
{
    private readonly IConfiguration conf;

    public CalculoController(IConfiguration config)
    {
        conf = config;
    }




    [HttpGet("todo")]
    public async Task<IActionResult> GetTodos()
    {
        IEnumerable<Models.Calculo.Calculo> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = @"select * from calculo ";

                lst = db.Query<Models.Calculo.Calculo>(sql);
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