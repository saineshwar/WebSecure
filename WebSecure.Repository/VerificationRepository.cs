using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSecure.Models;

namespace WebSecure.Repository
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly DatabaseContext _databaseContext;
        public VerificationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <summary>
        /// Here we can Send VerificationToken to User In Email or OTP On Mobile
        /// </summary>
        public void SendRegistrationVerificationToken(long userid, string verficationToken)
        {
            RegisterVerification registerVerification = new RegisterVerification()
            {
                RegisterVerificationId = 0,
                GeneratedDate = DateTime.Now,
                GeneratedToken = verficationToken,
                UserId = userid,
                Status = true,
                VerificationStatus = false
            };

            _databaseContext.RegisterVerification.Add(registerVerification);
            _databaseContext.SaveChanges();
        }

        public RegisterVerification GetRegistrationGeneratedToken(string userid)
        {
            var tempuserid = Convert.ToInt64(userid);

            var registerVerification = (from rv in _databaseContext.RegisterVerification
                                        orderby rv.RegisterVerificationId descending
                                        where rv.UserId == tempuserid
                                        select rv).FirstOrDefault();

            return registerVerification;
        }


        public ResetPasswordVerification GetResetGeneratedToken(string userid)
        {
            var tempuserid = Convert.ToInt64(userid);

            var resetPasswordVerification = (from rv in _databaseContext.ResetPasswordVerification
                                             orderby rv.ResetTokenId descending
                                             where rv.UserId == tempuserid
                                             select rv).FirstOrDefault();

            return resetPasswordVerification;
        }

        public bool UpdateRegisterVerification(long userid)
        {
            var registerVerification = (from rv in _databaseContext.RegisterVerification
                                        where rv.UserId == userid
                                        select rv).FirstOrDefault();
            if (registerVerification != null)
            {
                registerVerification.VerificationStatus = true;
                registerVerification.VerificationDate = DateTime.Now;
                _databaseContext.RegisterVerification.Update(registerVerification);
            }

            return _databaseContext.SaveChanges() > 0;
        }

        public bool CheckIsAlreadyVerifiedRegistration(long userid)
        {
            var registerVerification = (from rv in _databaseContext.RegisterVerification
                                        where rv.UserId == userid && rv.VerificationStatus == true
                                        select rv).Any();

            return registerVerification;
        }

        public void SendResetVerificationToken(long userid, string verficationToken)
        {
            ResetPasswordVerification registerVerification = new ResetPasswordVerification()
            {
                ResetTokenId = 0,
                GeneratedDate = DateTime.Now,
                GeneratedToken = verficationToken,
                UserId = userid,
                Status = true,
                VerificationStatus = false
            };

            _databaseContext.ResetPasswordVerification.Add(registerVerification);
            _databaseContext.SaveChanges();
        }

        public bool UpdateResetVerification(long userid)
        {
            var resetVerification = (from rv in _databaseContext.ResetPasswordVerification
                                     where rv.UserId == userid
                                     select rv).FirstOrDefault();
            if (resetVerification != null)
            {
                resetVerification.VerificationStatus = true;
                resetVerification.VerificationDate = DateTime.Now;
                _databaseContext.ResetPasswordVerification.Update(resetVerification);
            }

            return _databaseContext.SaveChanges() > 0;
        }
    }
}
