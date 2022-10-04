using System;

namespace ItsPeetah.DebugConsole
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DebugConsoleCommandAttribute : Attribute
    {
        private string command;
        public string Command => command;

        //private bool exitConsoleOnCall;
        //public bool ExitConsoleOnCall => exitConsoleOnCall;

        public DebugConsoleCommandAttribute(string command/*, bool exitConsoleOnCall = false*/)
        {
            this.command = command;
            //this.exitConsoleOnCall = exitConsoleOnCall;
        }
    }
}