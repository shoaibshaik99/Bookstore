using ModelLayer.Models.BookModels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IBooksService
    {
        public BookEntity AddBook(Add_or_Update_BookModel bookModel);

        public FetchBookModel GetBookById(int bookId);

        public List<FetchBookModel> GetAllBooks();

        public bool UpdateBook(int bookId, Add_or_Update_BookModel bookModel);

        public bool DeleteBook(int bookId);
    }
}
