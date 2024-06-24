using ModelLayer.Models.FeedbackModels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class FeedbacksService : IFeedbacksService
    {
        private IFeedbacksRepo _feedbacksRepo;

        public FeedbacksService(IFeedbacksRepo feedbacksRepo)
        {
            _feedbacksRepo = feedbacksRepo;
        }

        public FeedbackEntity GiveFeedback(int userId, GiveFeedbackModel feedbackModel)
        {
            return _feedbacksRepo.GiveFeedback(userId, feedbackModel);
        }

        public List<FeedbackEntity> ViewAllFeedbacks()
        {
            return _feedbacksRepo.ViewAllFeedbacks();
        }

        public List<FeedbackEntity> ViewAllFeedbacksForBook(int bookId)
        {
            return _feedbacksRepo.ViewAllFeedbacksForBook(bookId);
        }

        public FeedbackEntity EditFeedback(int userId, EditFeedbackModel editFeedbackModel)
        {
            return _feedbacksRepo.EditFeedback(userId, editFeedbackModel);
        }

        public bool DeleteFeedback(int userId, int feedbackId)
        {
            return _feedbacksRepo.DeleteFeedback(userId,feedbackId);
        }
    }
}
