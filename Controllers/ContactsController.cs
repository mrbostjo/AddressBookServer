using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AddressBookServer.Data;
using AddressBookServer.Models;
using System.Linq.Expressions;

namespace AddressBookServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AddressBookServerContext _context;

        public ContactsController(AddressBookServerContext context)
        {
            _context = context;
            LoadTestData(context);
        }

        private static List<Contact> contactList = new List<Contact>();
        private string fileName = "Data/testContactList.json";

        private async void LoadTestData(AddressBookServerContext context)
        {
            if (contactList.Count == 0)
            {
                string jsonContactList = System.IO.File.ReadAllText(fileName);
                contactList = JsonSerializer.Deserialize<List<Contact>>(jsonContactList);
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE[Contact]");
                if (contactList.Count > 0)
                {
                    foreach (var contact in contactList)
                    {
                        contact.id = 0;
                        context.Contact.Add(contact);
                        context.SaveChanges();
                    }
                }
            }
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContact()
        {
            return await _context.Contact.ToListAsync();
        }

        // GET: api/contacts/search?firstName=&lastName=&address=&phone=
        [HttpGet()]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<Contact>>> Search([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] string address, [FromQuery] string phone)
        {
            var terms = new SearchComposer(firstName, lastName, address, phone);
            terms.Prepare();

            if (terms.Count == 0)//if no search params, return all
            {
                return _context.Contact.ToList();
            }

            IQueryable<Contact> result = default;
            foreach (ContactProperties prop in terms.Keys)
            {
                if (result == default || (!result.Any()))
                {
                    result = _context.Contact.Where(TermQuery(terms[prop], prop));//Do initial query
                }
                else
                {
                    result = result.Where(TermQuery(terms[prop], prop));//Query among returned results to get intersection
                }
            }
            if (result != default)
            {
                return result.Distinct().OrderBy(contact => contact.firstName).ToList<Contact>();
            }

            return NotFound();
        }


        private Expression<Func<Contact, bool>> TermQuery(string term, ContactProperties property)
        {
            Expression<Func<Contact, bool>> searchExp = default;
            switch (property)
            {
                case ContactProperties.FirstName:
                    {
                        searchExp = (contact => contact.firstName.Contains(term));
                        break;
                    }
                case ContactProperties.LastName:
                    {
                        searchExp = (contact => contact.lastName.Contains(term));
                        break;
                    }
                case ContactProperties.Address:
                    {
                        searchExp = (contact => contact.address.Contains(term));
                        break;
                    }
                case ContactProperties.Phone:
                    {
                        searchExp = (contact => contact.phone.Contains(term));
                        break;
                    }
            }
            return searchExp;
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(long id)
        {
            var contact = await _context.Contact.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(long id, Contact contact)
        {
            if (id != contact.id)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        // POST: api/Contacts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.id }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(long id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        private bool ContactExists(long id)
        {
            return _context.Contact.Any(e => e.id == id);
        }
    }
}
