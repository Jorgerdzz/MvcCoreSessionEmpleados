using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if(salario != null)
            {
                //QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
                //QUE TENGAMOS EN SESSION
                int sumaTotal = 0;
                if(HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //SI YA TENEMOS DATOS ALMACENADOS, LOS RECUPERAMOS
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL
                sumaTotal += salario.Value;
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarial()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SessionEmpleados(int? idEmpleado)
        {
            if(idEmpleado != null)
            {
                Empleado emp = await this.repo.FindEmpleadoAsync(idEmpleado.Value);
                //VAMOS A GUARDAR EN SESSION UN CONJUNTO DE EMPLEAODOS
                List<Empleado> empleadosList;
                //PREGUNTAMOS SI HAY EMPLEADOS EN SESSION
                if(HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA DE SESSION
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS
                    empleadosList = new List<Empleado>();
                }
                empleadosList.Add(emp);
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + emp.Apellido + " almacenado correctamente.";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        public async Task<IActionResult> SessionEmpleadosOk(int? idEmpleado)
        {
            if(idEmpleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                List<int> idsEmpleados;
                if(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosOk()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if(idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        public async Task<IActionResult> SessionEmpleadosV4(int? idEmpleado)
        {
            List<int> idsEmpleados;
            if (idEmpleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                List<Empleado> empleadosSinSession = await this.repo.GetEmpleadosNotSessionAsync(idsEmpleados);
                return View(empleadosSinSession);
            }
            else
            {
                idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                List<Empleado> empleados = await this.repo.GetEmpleadosNotSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        [ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> SessionEmpleadosV5(int? idEmpleado, int? idfavorito)
        {
            if (idfavorito != null)
            {
                //COMO ESTOY ALMACENANDO EN CACHE, VAMOS A GUARDAR
                //DIRECTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS
                List<Empleado> empleadosFavoritos;
                if(this.memoryCache.Get("FAVORITOS") == null)
                {
                    //NO EXISTE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //RECUPERAMOS EL CACHE
                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>> ("FAVORITOS");
                }
                //BUSCAMOS AL EMPLEADO PARA GUARDARLO
                Empleado empleadoFavorito = await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleadoFavorito);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }
            List<int> idsEmpleados;
            if (idEmpleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosFavoritos()
        {
            if(this.memoryCache.Get("FAVORITOS") == null)
            {
                ViewData["MENSAJE"] = "No tenemos empleados favoritos";
                return View();
            }
            else
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                return View(favoritos);
            }
        }

        [ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                //PREGUNTAMOS SI HEMOS RECIBIDO ALGUN DATO PARA ELIMINAR
                if(ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    //SI NO TENEMOS EMPLEADOS EN SESSION, NUESTRA COLECCION EXISTE Y SE QUEDA A 0
                    //ELIMINAMOS SESSION
                    if(idsEmpleados.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                        return View();
                    }
                    else
                    {
                        //ACTUALIZAMOS SESSION
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                }
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

    }
}
