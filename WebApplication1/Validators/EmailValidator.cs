//using ConsoleApp1.Data.Entities;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Identity;
//using System.Text.RegularExpressions;

//namespace WebApplication1.Validators
//{
//    public class CustomUserValidator : IIdentityValidator<User>
//    {
//        public async Task<IdentityResult> ValidateAsync(User item)
//        {
//            List<string> errors = new List<string>();

//            if (String.IsNullOrEmpty(item.fullName.Trim()))
//                errors.Add("Вы указали пустое имя.");

//            string userNamePattern = @"^[a-zA-Z0-9а-яА-Я]+$";

//            if (!Regex.IsMatch(item.fullName, userNamePattern))
//                errors.Add("В имени разрешается указывать буквы английского или русского языков, и цифры");

//            if (errors.Count > 0)
//                return IdentityResult.Failed(errors.ToArray());

//            return IdentityResult.Success;
//        }
//    }
//}
