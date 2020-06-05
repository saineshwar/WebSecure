using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using WebSecure.Models;
using WebSecure.ViewModel;

namespace WebSecure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ConnectionStrings _connectionStrings;
        public UserRepository(DatabaseContext databaseContext, IOptions<ConnectionStrings> connectionStrings)
        {
            _databaseContext = databaseContext;
            _connectionStrings = connectionStrings.Value;
        }

        public long RegisterUser(User user, string salt)
        {
            try
            {
                using (var dbContextTransaction = _databaseContext.Database.BeginTransaction())
                {
                    try
                    {
                        long result = 0;
                        _databaseContext.User.Add(user);
                        _databaseContext.SaveChanges();
                        result = user.UserId;

                        UserTokens userTokens = new UserTokens()
                        {
                            UserId = result,
                            HashId = 0,
                            PasswordSalt = salt,
                            CreatedDate = DateTime.Now
                        };

                        _databaseContext.UserTokens.Add(userTokens);
                        _databaseContext.SaveChanges();

                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        return 0;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public User GetUserbyUserName(string username)
        {
            var userdata = (from tempuser in _databaseContext.User
                            where tempuser.Username == username
                            select tempuser).FirstOrDefault();

            return userdata;
        }
        public User GetUserbyUserId(long userid)
        {
            var userdata = (from tempuser in _databaseContext.User
                            where tempuser.UserId == userid
                            select tempuser).FirstOrDefault();

            return userdata;
        }
        public bool CheckUserExists(string username)
        {
            var userdata = (from tempuser in _databaseContext.User
                            where tempuser.Username == username
                            select tempuser).Any();

            return userdata;
        }
        public bool CheckEmailExists(string emailid)
        {
            var userdata = (from tempuser in _databaseContext.User
                            where tempuser.Email == emailid
                            select tempuser).Any();

            return userdata;
        }
        public bool CheckPhonenoExists(string phoneno)
        {
            var userdata = (from tempuser in _databaseContext.User
                            where tempuser.Phoneno == phoneno
                            select tempuser).Any();

            return userdata;
        }
        public UserTokens GetUserSaltbyUserid(long userId)
        {
            var usertoken = (from tempuser in _databaseContext.UserTokens
                             where tempuser.UserId == userId
                             select tempuser).FirstOrDefault();

            return usertoken;
        }

        // C and R processType || C:- Change Password R :- Reset Password
        public int UpdatePasswordandHistory(long userId, string passwordHash , string passwordSalt, string processType)
        {
            try
            {
                using (var con = new SqlConnection(_connectionStrings.DatabaseConnection))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    var param = new DynamicParameters();
                    param.Add("@UserId", userId);
                    param.Add("@PasswordHash", passwordHash);
                    param.Add("@PasswordSalt", passwordSalt);
                    param.Add("@ProcessType", processType);
                    var result = con.Execute("Usp_UpdatePassword", param, transaction, 0, CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        transaction.Commit();
                        return result;
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
