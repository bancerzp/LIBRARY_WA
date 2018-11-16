﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;

namespace LIBRARY_WA.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly LibraryContext context;

        public UserController(LibraryContext context)
        {
            this._context = context.userContext;
            this.context = context;
        }



        // GET: api/User
        [HttpGet("{user}")]
        [EnableCors("CorsPolicy")]
        public String IsLogged([FromBody] User user)
        {
            //rom p in context.Professors
            // select p.Name).ToList()
           if( _context.User.Where(u => user.login == u.login && user.password == u.password).FirstOrDefault()!=null)
            {
                return _context.User.Where(u => user.login == u.login && user.password == u.password).FirstOrDefault().userType;
            }
            else
            {
                return "g";
            }
        }

        [HttpGet]
        public IEnumerable<User> GetUser([FromBody] User user)
        {
          //rom p in context.Professors
           // select p.Name).ToList()

            return _context.User;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public  IEnumerable<User> GetUser([FromRoute] string id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var user = await _context.User.FindAsync(id);

            //if (user == null)
            //{
            //    return NotFound();
            //}
            return _context.User;
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] string id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.userId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> addUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.userId }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.userId == id);
        }
    }
}