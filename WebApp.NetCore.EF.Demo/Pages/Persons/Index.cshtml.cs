using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.NetCore.EF.Demo.Data;

namespace WebApp.NetCore.EF.Demo.Pages.Persons
{
    public class IndexModel : PageModel
    {
        private readonly WebApp.NetCore.EF.Demo.Data.AppDbContext _context;

        public IndexModel(WebApp.NetCore.EF.Demo.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Person> Person { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Person = await _context.Persons.ToListAsync();
        }
    }
}
