﻿namespace Common
{
    public class VerifyRegisterUserRequestDto
    {
        public int UserID { get; set; }
        public string VerificationCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}