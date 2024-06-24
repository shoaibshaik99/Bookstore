using ModelLayer.Models.FeedbackModels;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IFeedbacksRepo
    {
        public FeedbackEntity GiveFeedback(int userId, GiveFeedbackModel feedbackModel);
        
        public List<FeedbackEntity> ViewAllFeedbacks();

        public List<FeedbackEntity> ViewAllFeedbacksForBook(int bookId);

        public FeedbackEntity EditFeedback(int userId, EditFeedbackModel editFeedbackModel);

        public bool DeleteFeedback(int userId, int feedbackId);
    }
}
