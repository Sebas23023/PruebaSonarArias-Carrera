using GestionCitasMedicas.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Linq;

namespace GestionCitasMedicas.Tests
{
    public class CitasControllerTest
    {
        private readonly CitasController _controller;
        private readonly AppDBContext _context;

        public CitasControllerTest()
        {
        }

    }
}
