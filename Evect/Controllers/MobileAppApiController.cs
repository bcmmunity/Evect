using System.Threading.Tasks;
using Evect.Models;
using Evect.Models.DB;
using Microsoft.AspNetCore.Mvc;

namespace Evect.Controllers
{
    [Route("mobile/api")]
    public class MobileAppApiController : ControllerBase
    {
        
        private readonly ApplicationContext _context;

        public MobileAppApiController(ApplicationContext context)
        {
            _context = context;
        }
        
        [Route("sendVerify")]
        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail([FromBody]EmailCode emailCode)
        {
            
            if (Utils.IsEmailValid(emailCode.Email))
            {
                
                string code = Utils.GenerateRandomCode();


                if (await UserDB.IsMobileUserExists(_context, emailCode.Email))
                {
                    MobileUser us = await UserDB.GetMobileUserByEmail(_context, emailCode.Email);
                    us.EmailCode = code;


                    User user = await UserDB.GetUserByEmail(_context, emailCode.Email);

                    if (user != null)
                    {
                        us.TelegramId = user.TelegramId;
                    }
                        
                    _context.MobileUsers.Update(us);
                    _context.SaveChanges();
                    
                }
                else
                {
                    UserDB.AddMobileUser(_context, emailCode.Email, code);
                }

                
                await Utils.SendEmailAsync(emailCode.Email, "Потверждение почты", $"Ваш кода для потверждения почты: {code}");
                return Ok();
            }

            return BadRequest();
        }

        [Route("verify")]
        [HttpPost]
        public async Task<IActionResult> Verify([FromBody]EmailCode emailCode)
        {
            string email = emailCode.Email;
            string code = emailCode.Code;
            MobileUser user = await UserDB.GetMobileUserByEmail(_context, email);
            if (user != null)
            {
                if (user.EmailCode == code)
                {
                    return Ok(user.ApiKey);
                }
            }

            return BadRequest();
        }
        
        

        [Route("send")]
        [HttpGet]
        public async Task<IActionResult> Send()
        {
            return Ok();
        }
    }
}