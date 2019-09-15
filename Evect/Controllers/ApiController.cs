using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Evect.Models;
using Evect.Models.DB;

using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis;
namespace Evect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ApiController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Api
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InfoAboutUsers>>> GetInfoAboutUsers()
        {
            return await _context.InfoAboutUsers.ToListAsync();
        }
        [Route("getCommonInfoAboutUsers")]
        [HttpGet("id")]
        public async Task<JsonResult> CommonInformation(int idOfEvent)
        {
            EventDB eventDb = new EventDB();

            string amountOfUsers =await eventDb.GetInfrormationAboutUsers(idOfEvent, "Количество пользователей");
            string amountOfActivationsOfNetworking =await eventDb.GetInfrormationAboutUsers(idOfEvent, "Количество активаций режима общения");
            var obj = new
            {
                amountOfActivationsOfNetworking,
                amountOfUsers
            };
            return new JsonResult(obj);
        }
        [Route("getExcelFiles")]
        [HttpGet("{id}")]
         public ActionResult<InfoAboutUsers> GetExcelFiles()
        {
            return _context.InfoAboutUsers.First();
        }
        [Route("GetSurveyResults")]
        public JsonResult Kek()
        {
            string link = "https://vk.com";
            int count = 1488;
            return new JsonResult(new { excelLink =new {lizonka=link }, survCount = count });
        }
        // GET: api/Api/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InfoAboutUsers>> GetInfoAboutUsers(int id)
        {
            var infoAboutUsers = await _context.InfoAboutUsers.FindAsync(id);

            if (infoAboutUsers == null)
            {
                return NotFound();
            }

            return infoAboutUsers;
        }

        // PUT: api/Api/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInfoAboutUsers(int id, InfoAboutUsers infoAboutUsers)
        {
            if (id != infoAboutUsers.InfoAboutUsersId)
            {
                return BadRequest();
            }

            _context.Entry(infoAboutUsers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InfoAboutUsersExists(id))
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

        // POST: api/Api
        [HttpPost]
        public async Task<ActionResult<InfoAboutUsers>> PostInfoAboutUsers(InfoAboutUsers infoAboutUsers)
        {
            _context.InfoAboutUsers.Add(infoAboutUsers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInfoAboutUsers", new { id = infoAboutUsers.InfoAboutUsersId }, infoAboutUsers);
        }

        // DELETE: api/Api/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<InfoAboutUsers>> DeleteInfoAboutUsers(int id)
        {
            var infoAboutUsers = await _context.InfoAboutUsers.FindAsync(id);
            if (infoAboutUsers == null)
            {
                return NotFound();
            }

            _context.InfoAboutUsers.Remove(infoAboutUsers);
            await _context.SaveChangesAsync();

            return infoAboutUsers;
        }

        private bool InfoAboutUsersExists(int id)
        {
            return _context.InfoAboutUsers.Any(e => e.InfoAboutUsersId == id);
        }
    }
}
