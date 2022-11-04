using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Newtonsoft.Json;

namespace proyecto3Back.Controllers.Historial;

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
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"select
                           *
                            from
	                            historial_emp 
                            where
                            to_char (  tiempo, 'YYYY-MON-DD')=to_char (date('{1}'), 'YYYY-MON-DD') and  id_usuario ={0} and  tipo_entrada =1", id_usuario, tiempo);

                lst = db.Query<Models.Historial.HistorialModels>(sql);
                Json = JsonConvert.SerializeObject(lst);


                if (lst.Count() == 0)
                {
                    var sql2 = string.Format(@"INSERT INTO historial_emp (tiempo,id_usuario,tipo_entrada)
	                VALUES ('{1}',{0},1);", id_usuario,tiempo);
                    lst = db.Query<Models.Historial.HistorialModels>(sql2);
                    Json = JsonConvert.SerializeObject(lst);
                }
                else
                {
                    var sql3 = string.Format(@"select
                           *
                            from
	                            historial_emp
                            where
                            to_char (tiempo, 'YYYY-MON-DD')=to_char (date('{1}'), 'YYYY-MON-DD') and  id_usuario ={0} and  tipo_entrada =2", id_usuario, tiempo);
                    
                    lst = db.Query<Models.Historial.HistorialModels>(sql3);
                    Json = JsonConvert.SerializeObject(lst);

                    if (lst.Count() == 0)
                    {

                        var sql4 = string.Format(@"INSERT INTO historial_emp (tiempo,id_usuario,tipo_entrada)
	                VALUES ('{1}',{0},2);", id_usuario,tiempo);

                        lst = db.Query<Models.Historial.HistorialModels>(sql4);
                        Json = JsonConvert.SerializeObject(lst);
                    }
                    else
                    {


                        var sql5 = string.Format(@"UPDATE historial_emp
	                            SET tiempo='{1}'
	                        WHERE id_usuario={0} and tipo_entrada =2 and to_char (tiempo, 'YYYY-MON-DD')=to_char (date('{1}'), 'YYYY-MON-DD');", id_usuario, tiempo);
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
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"	select
                           	to_char (  tiempo,
	'YYYY-MON-DD') as fecha, to_char (  tiempo,
	'HH:MI AM') as hora,  (case when 1= tipo_entrada then 'entrada' else 'salida' end) as se
                            from
	                            historial_emp
                            where  id_usuario ={0} order by   tiempo  asc ", id_usuario);

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





    [HttpGet("general")]
    public async Task<IActionResult> GetHistorialGeneral()
    {
        IEnumerable<Models.Historial.HistorialEmpleadoGeneral> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"
                                                select
                                                 e.nombres, 
                                                 e.apellidos,  
                                                 (case when 1= he.tipo_entrada then 'entrada' else 'salida' end) as se,
                                                 to_char ( he.tiempo,	'HH:MI AM') as hora,
                                                 to_char ( he.tiempo,	'DD-MON-YYYY') as fecha
                                                from
	                                                empleado e
                                                inner join usuario u on
	                                                u.id_empleado = e.id_empleado
                                                inner join historial_emp he on
	                                                he.id_usuario = u.id_usuario order by e.nombres,he.tiempo

                                                ");

                lst = db.Query<Models.Historial.HistorialEmpleadoGeneral>(sql);
                Json = JsonConvert.SerializeObject(lst);
                    

            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }




    [HttpGet("generaltarde")]
    public async Task<IActionResult> GetHistorialGeneralTarde()
    {
        IEnumerable<Models.Empleado.EmpleadoTarde> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"
                                        		select
	                                                e.nombres,
	                                                e.apellidos ,
	                                                sum( (case when (to_char ( he.tiempo, 'HH24')> '07' and to_char ( he.tiempo, 'MI')> '00' and he.tipo_entrada = 1 ) then 1 else 0 end) ) entrada_tarde,
	                                                sum( (case when (to_char ( he.tiempo, 'HH24')> '16' and to_char ( he.tiempo, 'MI')> '00' and he.tipo_entrada = 2 ) then 1 else 0 end) ) salida_tarde
                                                from 
	                                                empleado e
                                                inner join usuario u on
	                                                u.id_empleado = e.id_empleado
                                                left join historial_emp he on
	                                                he.id_usuario = u.id_usuario
                                                group by
	                                                e.nombres,
	                                                e.apellidos
                                                having
	                                                sum( (case when (to_char ( he.tiempo, 'HH24')> '07' and to_char ( he.tiempo, 'MI')> '00' and he.tipo_entrada = 1 ) then 1 else 0 end) ) >0 or
                                                sum( (case when (to_char ( he.tiempo, 'HH24')> '16' and to_char ( he.tiempo, 'MI')> '00' and he.tipo_entrada = 2 ) then 1 else 0 end) ) >0    

                                             ");

                lst = db.Query<Models.Empleado.EmpleadoTarde>(sql);
                Json = JsonConvert.SerializeObject(lst);


            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }




    [HttpGet("generalsalidaanticipada")]
    public async Task<IActionResult> GetHistorialGeneralSalidaAnticipada()
    {
        IEnumerable<Models.Empleado.EmpleadoTarde> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"
                                  	select
	                                    e.nombres,
	                                    e.apellidos ,
	                                    sum( (case when (to_char ( he.tiempo, 'HH24')<'17'  and he.tipo_entrada = 2 ) then 1 else 0 end) ) salida_tarde
                                    from 
	                                    empleado e
                                    inner join usuario u on
	                                    u.id_empleado = e.id_empleado
                                    left join historial_emp he on
	                                    he.id_usuario = u.id_usuario
                                    group by
	                                    e.nombres,
	                                    e.apellidos
                                    having
                                    sum( (case when (to_char ( he.tiempo, 'HH24')< '17'  and he.tipo_entrada = 2 ) then 1 else 0 end) ) >0   

                                             ");

                lst = db.Query<Models.Empleado.EmpleadoTarde>(sql);
                Json = JsonConvert.SerializeObject(lst);


            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Json);
    }




    [HttpGet("historialdepatamento/{id_departamento}")]
    public async Task<IActionResult> GetHistorialDepartamento(int id_departamento)
    {
        IEnumerable<Models.Historial.HistorialEmpleadoDepartamento> lst = null;
        var Json = JsonConvert.SerializeObject(lst);
        try
        {
            using (var db = new NpgsqlConnection(conf.GetConnectionString("dbconnection")))
            {
                var sql = string.Format(@"
                                                select
                                                 e.nombres, 
                                                 e.apellidos,  
                                                 (case when 1= he.tipo_entrada then 'entrada' else 'salida' end) as se,
                                                 to_char ( he.tiempo,	'hh:MI AM') as hora,
                                                 to_char ( he.tiempo,	'DD-MON-YYYY') as fecha,
                                                 d.nombre_departamento as depertamento
                                                from
	                                                empleado e
                                                inner join usuario u on
	                                                u.id_empleado = e.id_empleado
                                                inner join historial_emp he on
	                                                he.id_usuario = u.id_usuario 
	                                                inner join departamento d ON 
	                                                e.id_departamento = d.id_departamento 
	                                                where e.id_departamento ={0}        
                                                order by e.nombres,he.tiempo ", id_departamento);

                lst = db.Query<Models.Historial.HistorialEmpleadoDepartamento>(sql);
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