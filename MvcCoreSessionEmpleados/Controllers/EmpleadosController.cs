using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
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
    }
}
