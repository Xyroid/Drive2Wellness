using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive2Wellness.Model
{
    public class QuestionModel
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string question { get; set; }
        public string insert_date { get; set; }
        public string answer { get; set; }
        public string ans_date { get; set; }
        public string answer_review { get; set; }
    }
}
