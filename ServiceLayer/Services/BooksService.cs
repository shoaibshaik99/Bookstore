using ModelLayer.Models.BookModels;
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
    public class BooksService: IBooksService
    {
        private readonly IBooksRepo _booksRepo;

        public BooksService(IBooksRepo booksRepo)
        {
            _booksRepo = booksRepo;
        }

        public BookEntity AddBook(Add_or_Update_BookModel bookModel)
        {
            return _booksRepo.AddBook(bookModel);
        }

        public FetchBookModel GetBookById(int bookId)
        {
            return _booksRepo.GetBookById(bookId);
        }

        public List<FetchBookModel> GetAllBooks()
        {
            return _booksRepo.GetAllBooks();
        }

        public bool UpdateBook(int bookId, Add_or_Update_BookModel bookModel)
        {
            return (_booksRepo.UpdateBook(bookId, bookModel));
        }

        public bool DeleteBook(int bookId)
        {
            return (_booksRepo.DeleteBook(bookId));
        }
    }
}
