using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Create(Book entity)
        {
            await _context.Books.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Book entity)
        {
            _context.Books.Remove(entity);
            return await Save();
        }

        public async Task<IList<Book>> FindAll()
        {
            var books = await _context.Books
                .Include(q=>q.Author)
                .ToListAsync();
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            var book = await _context.Books
                .Include(q=>q.Author)
                .FirstOrDefaultAsync(q => q.Id == id);
            return book;
        }

        public async Task<bool> isExists(int id)
        {
            return await _context.Books.AnyAsync(q => q.Id == id);
        }

        public async Task<bool> Save()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Book entity)
        {
            _context.Books.Update(entity);
            return await Save();
        }
    }
}
