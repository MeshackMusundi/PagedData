using DynamicData;
using PagedData.WPF.Models;
using System;

namespace PagedData.WPF.Services
{
    public interface IEmployeesService
    {
        IObservable<IChangeSet<Employee, int>> EmployeesConnection();
        void LoadData();
    }
}
