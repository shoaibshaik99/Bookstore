using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entities
{
    public class FeedbackEntity
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Review { get; set; }
        public DateTime ReviewedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
