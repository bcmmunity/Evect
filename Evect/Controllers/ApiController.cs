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
        [Route("getEventId")]
        [HttpPost]
        public async Task<JsonResult> getEventId(string orgCode,string orgEmail)
        {
            Event eventt = new Event();
            User user = new User();
            string Caution = "";
            if (_context.Events.FirstOrDefault(n => n.AdminCode == orgCode) != null)
                eventt = _context.Events.FirstOrDefault(n => n.AdminCode == orgCode);
            else Caution = "Вы неправильно ввел код. Введите код, пожалуйста, еще раз.";
            if (_context.Users.FirstOrDefault(n => n.Email == orgEmail) != null)
                user = _context.Users.FirstOrDefault(n => n.Email == orgEmail);
            else
            {
                if (Caution == "") Caution = "Вы ввели неправильно почту";
                else Caution = Caution + "Вы неправильно ввели код и почту";

            }
            if (Caution == "")
            {
                string ApiKey = Utils.GenerateNewCode(15);
                return new JsonResult(new { eventId = eventt.EventId, apiKey = ApiKey });
                    }
            else return new JsonResult(Caution);
            }
        [Route("getInfoAboutUsers")]
        [HttpPost]
        public async Task<JsonResult> getInfoAboutUsers(int eventId,string apiKey)
        {
            EventDB eventDb = new EventDB();
            string TotalCount =await eventDb.GetInfrormationAboutUsers(eventId, "Количество пользователей");
            string Networking=await eventDb.GetInfrormationAboutUsers(eventId,"Количество использования режим общения");//использовали режим общения
            string Meet=await eventDb.GetInfrormationAboutUsers(eventId,"Сколько встреч согласовано");//сколько встреч согласовано
            string Contacts=await eventDb.GetInfrormationAboutUsers(eventId,"Сколько запрошено контактов");//сколько запрошено контактов
            string AverageContacts = await eventDb.GetInfrormationAboutUsers(eventId, "Среднее число контактов"); ;//среднее число контактов
            return new JsonResult(new { totalCount = TotalCount, networking = Networking, meet = Meet, contacts = Contacts, averageContacts = AverageContacts });
        }
        public class Surveys
        {
            public int countOfRespondents { get; set; }
            public string type { get; set; }
           /* public Surveys(int CountOfRespondents,string Type)
            {
                countOfRespondents = CountOfRespondents;
                Type = type;
            }*/

        }
        [Route("getSurvey")]
        [HttpPost]
        public async Task<JsonResult> getSurvey(int eventId,string apiKey)
        {
            //как обработать apiKey?Просто проверить на то,есть он у чела или нет и если нет,то вернуть предупреждение или что,как?
            EventDB eventDb = new EventDB();
            List<int> idOfQuestions = eventDb.GetIdOfQuestions(_context,eventId);
            Dictionary<string,Surveys> necessaryInfo = new Dictionary<string,Surveys>();
            foreach (var id in idOfQuestions)
            {
                Question question = _context.Questions.FirstOrDefault(n => n.QuestionId == id);
                int countOfRespondents = eventDb.GetCountOfRespondents(_context,id);
                string type = eventDb.GetTypeOfQuestion(_context,id);
                Surveys survey = new Surveys();
                survey.countOfRespondents = countOfRespondents;
                survey.type = type;
                necessaryInfo.Add(question.Questions, survey);
            }
            var obj = new { necessaryInfo };
            return new JsonResult(obj);
            ///+сделать с возврат экселевских файлов
        }
        [Route("getTags")]
        [HttpPost]
        public async Task<JsonResult> getTags(int eventId,string apiKey)
        {
            Event eventt = _context.Events.FirstOrDefault(p => p.EventId == eventId);
            EventDB eventDb = new EventDB();
            List<string> parentTags = eventDb.GetTags(_context, eventId,"Parent");
            List<string> childTags = eventDb.GetTags(_context, eventId, "Child");
            var obj = new { nameOdEvent = eventt.Name, parentTags, childTags };
            return new JsonResult(obj);
        }
       /* [Route("getUserActivity")]
        [HttpPost]
        public async Task<JsonResult> getUserActivity(int eventId,string apiKey)
        {
            
        }*/
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
