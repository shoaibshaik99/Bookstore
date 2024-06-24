using ModelLayer.Models.BookModels;
using ModelLayer.Models.Miscelleaneous;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IMiscellaneousRepo
    {
        public BookEntity GetBookByTitleAndAuthor(string title, string author);

        public BookEntity UpsertBook(UpsertBookModel bookModel);

        public List<WishlistDetailModel> GetWishlistDetails();
    }
}
