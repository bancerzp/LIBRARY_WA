using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace Library.Services
{
    public class DictionaryService : ControllerBase
    {
        private readonly LibraryContext _context;

        public DictionaryService(LibraryContext context)
        {
            _context = context;
        }

        public List<string> GetAuthorsFullname()
        {
            return _context.Authors.Select(a => a.AuthorFullname).ToList();
        }

        public List<string> GetBookTypes()
        {
            return _context.Books.Select(a => a.Type).Distinct().ToList();
        }

        public List<string> GetLanguages()
        {
            return _context.Books.Select(a => a.Language).Distinct().ToList();
        }
    }
}