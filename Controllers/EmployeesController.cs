using ASPNETMVC.Data;
using ASPNETMVC.Models;
using ASPNETMVC.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETMVC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MVCDemoDbContext mvcDemoDbContext;

        public EmployeesController(MVCDemoDbContext mvcDemoDbContext) //ctor kısayolu tab tab ile oluşturudk Program.cs de injekte edilen bu bağlamı controller da kullanabiliriz.
        //MvcDemoContext ve bir ad veriyoruz küçükharfle sonra mvcDemoContext Ctrl . ile create and assign field oluşturuyoruz. readonly yi ve this.mvc bölümünü otomatik  oluşturuyor

        {
            this.mvcDemoDbContext = mvcDemoDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await mvcDemoDbContext.Employees.ToListAsync();
            return View(employees);
        }
        [HttpGet] 
        public IActionResult Add() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)// AdEmployeeVieeModel Modelimiz  Employee ise domain deki model entity.  
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DateOfBirth = addEmployeeRequest.DateOfBirth,
            };
           await mvcDemoDbContext.Employees.AddAsync(employee);  // async yapmak için Async leri await ile koyduk bir de Task < > ile sarmalladık .
           await mvcDemoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
           var employee = await mvcDemoDbContext.Employees.FirstOrDefaultAsync(x=>x.Id ==id);
            
            if(employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DateOfBirth = employee.DateOfBirth,
                };
                return await Task.Run(()=>View("View",viewModel));

            };
         
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel model)
        {
            var employee = await mvcDemoDbContext.Employees.FindAsync(model.Id);
            if(employee != null)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Salary = model.Salary;
                employee.Department = model.Department;

               await mvcDemoDbContext.SaveChangesAsync();
                return RedirectToAction("Index");     
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel model)
        {
            var employee =await mvcDemoDbContext.Employees.FindAsync(model.Id);   
            if(employee != null)
            {
             mvcDemoDbContext.Employees.Remove(employee);
                await mvcDemoDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


    }
}
