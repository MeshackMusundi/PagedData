using Bogus;
using DynamicData;
using PagedData.WPF.Models;
using System;

namespace PagedData.WPF.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly ISourceCache<Employee, int> _employees;

        public EmployeesService() => _employees = new SourceCache<Employee, int>(e => e.ID);

        public IObservable<IChangeSet<Employee, int>> EmployeesConnection() => _employees.Connect();

        public void LoadData()
        {
            var employeesFaker = new Faker<Employee>()
                .RuleFor(e => e.ID, f => f.IndexFaker)
                .RuleFor(e => e.FirstName, f => f.Person.FirstName)
                .RuleFor(e => e.LastName, f => f.Person.LastName)
                .RuleFor(e => e.Age, f => f.Random.Int(20, 60))
                .RuleFor(e => e.Gender, f => f.Person.Gender.ToString());

            _employees.AddOrUpdate(employeesFaker.Generate(1500));
        }
    }
}
