using BestStoreApi.Net_Core_7.Models;
using BestStoreApi.Net_Core_7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreApi.Net_Core_7.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public IActionResult GetUsers(int? page)
        {

            if(page == null || page < 1)
            {
                page = 1;
            }


            int pageSize = 1;
            int totalPages = 0;

            decimal count = context.Users.Count();
            totalPages = (int) Math.Ceiling(count/ pageSize);





            var users = context.Users
                .OrderByDescending(u =>u.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            List<UserProfileDto> userProfiles = new List<UserProfileDto> ();
            foreach (var user in users)
            {
                var userProfileDto = new UserProfileDto()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.Created
                };
                userProfiles.Add(userProfileDto);
            }

            var response = new
            {
                Users = userProfiles,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);

        }



        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = context.Users.Find(id);

            if(user == null)
            {
                return NotFound();
            }

            var userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Phone = user.Phone,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.Created
            };
            return Ok(userProfileDto);

        }

    }
}
