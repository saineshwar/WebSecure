using WebSecure.Models;

namespace WebSecure.Repository
{
    public interface IVerificationRepository
    {
        void SendRegistrationVerificationToken(long userid, string verficationToken);
        RegisterVerification GetRegistrationGeneratedToken(string userid);
        ResetPasswordVerification GetResetGeneratedToken(string userid);
        bool UpdateRegisterVerification(long userid);
        bool CheckIsAlreadyVerifiedRegistration(long userid);
        void SendResetVerificationToken(long userid, string verficationToken);
        bool UpdateResetVerification(long userid);
    }
}