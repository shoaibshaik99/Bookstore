using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.FeedbackModels
{
    public class EditFeedbackModel
    {
        public int FeedbackId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Review { get; set; }
    }
}
