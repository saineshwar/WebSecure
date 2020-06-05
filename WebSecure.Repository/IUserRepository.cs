using WebSecure.Models;
using WebSecure.ViewModel;

namespace WebSecure.Repository
{
    public interface IUserRepository
    {
        long RegisterUser(User user, string salt);
        User GetUserbyUserName(string username);
        User GetUserbyUserId(long userid);
        bool CheckUserExists(string username);
        UserTokens GetUserSaltbyUserid(long userId);
        bool CheckEmailExists(string emailid);
        bool CheckPhonenoExists(string phoneno);
        int UpdatePasswordandHistory(long userId, string passwordHash, string passwordSalt, string processType);
    }
}