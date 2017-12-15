using QTFK.Attributes;
using System;

namespace SampleLibrary.Models
{
    public class Employee
    {
        [Auto]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birth { get; set; }
    }
}
