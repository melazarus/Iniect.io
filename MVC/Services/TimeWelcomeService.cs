using System;

namespace MVC.Services
{
    internal class TimeWelcomeService : IWelcomeService
    {
        public string WelcomeMessage => $"Hello there! It is now {DateTime.Now}";
    }
}