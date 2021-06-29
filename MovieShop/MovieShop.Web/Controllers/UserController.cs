using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Domain.DomainModels;
using MovieShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<MovieShopApplicationUser> userManager;

        public UserController(UserManager<MovieShopApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ImportUsers(IFormFile file)
        {

            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            List<UserWithRoleDto> users = getAllUsersFromFile(file.FileName);

            bool status = true;

            foreach (var item in users)
            {
                var userCheck = userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {
                    var user = new MovieShopApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };

                    var result = userManager.CreateAsync(user, item.Password).Result;

                    await userManager.AddToRoleAsync(user, item.Role);

                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            if (status)
            {
                return RedirectToAction("Index", "UserRoles");
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
            
        }

        private List<UserWithRoleDto> getAllUsersFromFile(string fileName)
        {

            List<UserWithRoleDto> users = new List<UserWithRoleDto>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using(var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using(var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {

                        users.Add(new UserWithRoleDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(1).ToString(),
                            Role = reader.GetValue(2).ToString()
                        });

                    }
                }
            }

            return users;
        }
    }
}
