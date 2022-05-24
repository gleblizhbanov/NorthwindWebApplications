using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NorthwindMvcClient.Models;
using NorthwindMvcClient.ViewModels;

namespace NorthwindMvcClient.Controllers
{
    [Route("[controller]/")]
    public class EmployeesController : Controller
    {
        private readonly HttpClient client;

        public EmployeesController()
        {
            this.client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:44344/api/Employees/"),
            };

            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> Index(int page = 1, int limit = PageViewModel.BasePageSize)
        {
            if (page <= 0)
            {
                return limit != PageViewModel.BasePageSize ? RedirectToAction(nameof(Index), new { limit }) : RedirectToAction(nameof(Index));
            }

            var uriBuilder = new UriBuilder(this.client.BaseAddress!);
            var query = new Dictionary<string, int>
            {
                ["offset"] = (page - 1) * limit,
                ["limit"] = limit,
            };

            uriBuilder.Query = string.Join('&', query.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            
            var json = await this.client.GetStringAsync(uriBuilder.Uri).ConfigureAwait(false);
            var employees = JsonConvert.DeserializeObject<IList<Employee>>(json)
                                                        .Select(employee => new EmployeeViewModel(employee))
                                                        .ToList();

            if (employees.Count == 0 && page > 1)
            {
                return limit != PageViewModel.BasePageSize ? RedirectToAction(nameof(Index), new { limit }) : RedirectToAction(nameof(Index));
            }

            int totalCount = JsonConvert.DeserializeObject<IList<Employee>>(await this.client.GetStringAsync("").ConfigureAwait(false)).Count;

            ViewData["Title"] = "Employees";
            return View(new EmployeesPageViewModel()
            {
                Employees = employees,
                PageModel = new PageViewModel(totalCount, page, limit),
            });
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var json = await this.client.GetStringAsync($"{id}");
            var employee = JsonConvert.DeserializeObject<EmployeeDetailsViewModel>(json);

            return View(employee);
        }
        
        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            return Ok();
        }
    }
}
