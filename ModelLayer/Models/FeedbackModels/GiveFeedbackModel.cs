using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.FeedbackModels
{
    public class GiveFeedbackModel
    {
        public int BookId { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
    }
}
