﻿namespace KPMGTask.Dtos
{
    public class SetNewPasswordForForgotPasswordDTO
    {

        public string Token { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    }
}
