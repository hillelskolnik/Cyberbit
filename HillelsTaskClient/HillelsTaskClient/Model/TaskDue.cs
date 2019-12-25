using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillelsTaskClient.Model
{
    public class TaskDue
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public bool Done { get; set; }
    }
}
