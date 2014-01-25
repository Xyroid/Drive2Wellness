using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive2Wellness.Repository
{
    [Table("CategoriesTable")] 
    public class CategoriesTable
    {
        [PrimaryKey, Unique, AutoIncrement]
        public int _id { get; set; }
        public int weight { get; set; }
        public string username { get; set; }
        public string blood_pressure { get; set; }
        public int oxygen_level { get; set; }
        public int glucose_level { get; set; }
        public int pedometer { get; set; }
        public double bmi { get; set; }
        public DateTime date { get; set; }
    }
}
