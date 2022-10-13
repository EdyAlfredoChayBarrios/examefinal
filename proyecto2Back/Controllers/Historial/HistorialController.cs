using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace proyecto2Back.Controllers.Historial;

[Route("api/[controller]")]
[ApiController]
public class HistorialController : ControllerBase
{
    private readonly IConfiguration conf;

    public HistorialController(IConfiguration config)
    {
        conf = config;
    }


    #region select de empleado

    [HttpGet("ingreso/{id_usuario}/{tiempo}")]
    public async Task<IActionResult> PostMovimientos(int id_usuario, string tiempo)
    {
        IEnumerable<Models.Historial.HistorialModels> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"select
                           *
                            from
	                            historial_emp he
                            where
                            date_format ( he.tiempo, '%d-%m-%Y')=date_format ('{1}', '%d-%m-%Y') and he.id_usuario ={0} and he.tipo_entrada =1", id_usuario, tiempo);

                lst = db.Query<Models.Historial.HistorialModels>(sql);
                Json = JsonConvert.SerializeObject(lst);


                if (lst.Count() == 0)
                {
                    var sql2 = string.Format(@"INSERT INTO proyectodos.historial_emp (tiempo,id_usuario,tipo_entrada)
	                VALUES ('{1}',{0},1);", id_usuario,tiempo);
                    lst = db.Query<Models.Historial.HistorialModels>(sql2);
                    Json = JsonConvert.SerializeObject(lst);
                }
                else
                {
                    var sql3 = string.Format(@"select
                           *
                            from
	                            historial_emp he
                            where
                            date_format ( he.tiempo, '%d-%m-%Y')=date_format ('{1}', '%d-%m-%Y') and he.id_usuario ={0} and he.tipo_entrada =2", id_usuario, tiempo);
                    
                    lst = db.Query<Models.Historial.HistorialModels>(sql3);
                    Json = JsonConvert.SerializeObject(lst);

                    if (lst.Count() == 0)
                    {

                        var sql4 = string.Format(@"INSERT INTO proyectodos.historial_emp (tiempo,id_usuario,tipo_entrada)
	                VALUES ('{1}',{0},2);", id_usuario,tiempo);

                        lst = db.Query<Models.Historial.HistorialModels>(sql4);
                        Json = JsonConvert.SerializeObject(lst);
                    }
                    else
                    {


                        var sql5 = string.Format(@"UPDATE proyectodos.historial_emp
	                            SET tiempo='{1} '
	                        WHERE id_usuario={0} and tipo_entrada =2;", id_usuario, tiempo);
                        lst = db.Query<Models.Historial.HistorialModels>(sql5);
                        Json = JsonConvert.SerializeObject(lst);



                        
                    }

                }
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }

    #endregion select de empleado



    [HttpGet("historial/{id_usuario}")]
    public async Task<IActionResult> GetHistorial(int id_usuario)
    {
        IEnumerable<Models.Historial.HistorialRespuestaModels> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new MySqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"	select
                           	date_format ( he.tiempo,
	'%d-%m-%Y') as fecha, date_format ( he.tiempo,
	'%H:%i') as hora, if(1=he.tipo_entrada,'entrada','salida') as se
                            from
	                            historial_emp he
                            where he.id_usuario ={0} order by  he.tiempo  asc ", id_usuario);

                lst = db.Query<Models.Historial.HistorialRespuestaModels>(sql);
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