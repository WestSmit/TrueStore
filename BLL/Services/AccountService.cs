﻿using AutoMapper;
using BLL.Services.Interfaces;
using BLL.Models;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using BLL.Helpers;

namespace BLL.Services
{
    public class AccountService :IAccountService
    {
        private readonly IUnitOfWork _database;
        private readonly IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _database = unitOfWork;
            _mapper = mapper;
        }

        public LoginResultModel UpdateUserInfo(UserModel userModel)
        {
            User user = _mapper.Map<User>(userModel);

            LoginResultModel result = new LoginResultModel();
            result.User = _mapper.Map<UserModel>(_database.UserRepostitory.Update(user));

            return result;
        }

        public void TryEmailChanging(string userId, string newEmail)
        {
            var user = _database.UserRepostitory.GetUser(userId);
            var token = _database.UserRepostitory.GenereteEmailChangingToken(userId, newEmail);
            EmailSender emailSender = new EmailSender();
            emailSender.SendEmail(user.Email, 
                "Confirm your email changing...",
                $"<h3>To confirm the change  your email click " +
                $"<a href='http://192.168.0.102:50395/api/Account/ConfirmEmailChanging?userId={userId}&newEmail={newEmail}&token={token}'>here</a></h3>");
        }

        public LoginResultModel ChangeEmail(string userId, string newEmail, string token)
        {
            LoginResultModel result = new LoginResultModel();
            result.User = _mapper.Map<UserModel>(_database.UserRepostitory.ChangeEmail(userId, newEmail, token));
            return result;
        }

        public void ChangePassword(string userId, string currentPassword, string newPassword)
        {
            _database.UserRepostitory.ChangePassword(userId, currentPassword, newPassword);
        }

    }
}