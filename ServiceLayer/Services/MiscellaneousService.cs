using ModelLayer.Models.BookModels;
using ModelLayer.Models.Miscelleaneous;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class MiscellaneousService : IMiscellaneousService
    {
        private readonly IMiscellaneousRepo _miscellaneousRepo;

        public MiscellaneousService(IMiscellaneousRepo miscellaneousRepo)
        {
            _miscellaneousRepo = miscellaneousRepo;
        }

        public BookEntity GetBookByTitleAndAuthor(string title, string author)
        {
            return _miscellaneousRepo.GetBookByTitleAndAuthor(title, author);
        }

        public BookEntity UpsertBook(UpsertBookModel bookModel)
        {
            return _miscellaneousRepo.UpsertBook(bookModel);
        }

        public List<WishlistDetailModel> GetWishlistDetails()
        {
            return _miscellaneousRepo.GetWishlistDetails();
        }

    }
}
