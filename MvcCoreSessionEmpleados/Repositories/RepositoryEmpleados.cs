using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Models;

namespace MvcCoreSessionEmpleados.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.Empleados
                           select datos;
            List<Empleado> empleados = await consulta.ToListAsync();
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdEmpleado == idEmpleado
                           select datos;
            Empleado empleado = await consulta.FirstOrDefaultAsync();
            return empleado;
        }

        public async Task<List<Empleado>> GetEmpleadosSessionAsync(List<int> idsEmpleados)
        {
            var consulta = from datos in this.context.Empleados
                           where idsEmpleados.Contains(datos.IdEmpleado)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosNotSessionAsync(List<int> idsEmpleados)
        {
            var consulta = from datos in this.context.Empleados
                           where !idsEmpleados.Contains(datos.IdEmpleado)
                           select datos;
            return await consulta.ToListAsync();
        }

    }
}
