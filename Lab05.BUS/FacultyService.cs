using Lab05.DAL.Models;
using Lab05.DAL.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab05.BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Faculties.ToList();
        }
    }
}
