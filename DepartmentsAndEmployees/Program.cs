using System;
using System.Collections.Generic;
using System.Linq;
using DapperDepartments.Data;
using DapperDepartments.Models;

namespace DapperDepartments
{
    class Program
    {
       
        static void Main(string[] args)
        {
            Repository repository = new Repository();

            List<Department> departments = repository.GetAllDepartments();

            // PrintDepartmentReport should print a department report to the console, but does it?
            //  Take a look at how it's defined below...
            PrintDepartmentReport("All Departments", departments);

            Pause();

            // Create an new instance of a Department, so we can save our new department to the database.
            Department accounting = new Department { DeptName = "Accounting" };
            // Pass the accounting object as an argument to the repository's AddDepartment() method.
            repository.AddDepartment(accounting);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after adding Accounting department", departments);

            Pause();

            // Pull the object that represents the Accounting department from the list of departments that we got from the database.
            // First() is a handy LINQ method that gives us the first element in a list that matches the condition.
            Department accountingDepartmentFromDB = departments.First(d => d.DeptName == "Accounting");

            // How are the "accounting" and "accountingDepartmentFromDB" objects different?
            //  Why are they different?
            Console.WriteLine($"                accounting --> {accounting.Id}: {accounting.DeptName}");
            Console.WriteLine($"accountingDepartmentFromDB --> {accountingDepartmentFromDB.Id}: {accountingDepartmentFromDB.DeptName}");

            Pause();

            // Change the name of the Accounting department and save the change to the database.
            accountingDepartmentFromDB.DeptName = "Creative Accounting";
            repository.UpdateDepartment(accountingDepartmentFromDB.Id, accountingDepartmentFromDB);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after updating Accounting department", departments);

            Pause();


            // Maybe we don't need an Accounting department after all...
            repository.DeleteDepartment(accountingDepartmentFromDB.Id);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after deleting Accounting department", departments);

            Pause();

            // Create a new variable to contain a list of Employees and get that list from the database
            List<Employee> employees = repository.GetAllEmployees();

            // Does this method do what it claims to do, or does it need some work?
            PrintEmployeeReport("All Employees", employees, false);

            Pause();


            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees with departments", employees, true);

            Pause();


            // Here we get the first department by index.
            //  We could use First() here without passing it a condition, but using the index is easy enough.
            Department firstDepartment = departments[0];
            employees = repository.GetAllEmployeesWithDepartmentByDepartmentId(firstDepartment.Id);
            PrintEmployeeReport($"Employees in {firstDepartment.DeptName}", employees, true);

            Pause();


            // Instantiate a new employee object.
            //  Note we are making the employee's DepartmentId refer to an existing department.
            //  This is important because if we use an invalid department id, we won't be able to save
            //  the new employee record to the database because of a foreign key constraint violation.
            Employee jane = new Employee
            {
                FirstName = "Jane",
                LastName = "Lucas",
                DepartmentId = firstDepartment.Id
            };
            repository.AddEmployee(jane);

            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees after adding Jane", employees, true);

            Pause();


            // Once again, we see First() in action.
            Employee dbJane = employees.First(e => e.FirstName == "Jane");

            // Get the second department by index.
            Department secondDepartment = departments[1];

            dbJane.DepartmentId = secondDepartment.Id;
            repository.UpdateEmployee(dbJane.Id, dbJane);

            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees after updating Jane", employees, true);

            Pause();


            repository.DeleteEmployee(dbJane.Id);
            employees = repository.GetAllEmployeesWithDepartment();

            PrintEmployeeReport("All Employees after updating Jane", employees, true);

            Pause();

        }

        public static void PrintDepartmentReport(string title, List<Department> departments) {
            Console.WriteLine(title);
            int index = 0;
            foreach (Department dept in departments) {
                index++;
                Console.WriteLine($"{index}. {dept.DeptName}");
            }
        }

        public static void PrintEmployeeReport(string title, List<Employee> employees, bool withDept) {
            Console.WriteLine(title);
            int index = 0;

            if (!withDept) {
                foreach (Employee emp in employees) {
                    index++;
                    Console.WriteLine($"{index}. {emp.FirstName} {emp.LastName}");
                }
            } else {
                foreach (Employee emp in employees) {
                    index++;
                    Console.WriteLine($"{index}. {emp.FirstName} {emp.LastName}. Dept: {emp.Department.DeptName}");
                }
            }
        }

        /// <summary>
        ///  Custom function that pauses execution of the console app until the user presses a key
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
