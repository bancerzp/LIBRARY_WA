using LIBRARY_WA.Controllers.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LIBRARY_WA.Controllers.Function
{
    public class DictionaryController
    {
        private DictionaryService _dictionaryService;

        public DictionaryController(DictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        // get data to combobox
        [HttpGet]
        public List<string> GetAuthor()
        {
            return _dictionaryService.GetAuthorsFullname();
        }

        [HttpGet]
        public List<string> GetBookType()
        {
            return _dictionaryService.GetBookTypes();
        }

        [HttpGet]
        public List<string> GetLanguage()
        {
            return _dictionaryService.GetLanguages();
        }

    }
}
