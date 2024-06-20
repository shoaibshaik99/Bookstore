using ModelLayer.Models.BookModels;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IBooksRepo
    {
        public BookEntity AddBook(AddBookModel bookModel);

        public FetchBookModel GetBookById(int bookId);

        public List<FetchBookModel> GetAllBooks();

        public bool UpdateBook(int bookId, UpdateBookModel bookModel);

        public bool DeleteBook(int bookId);
    }
}   
