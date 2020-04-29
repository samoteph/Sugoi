using System;

namespace Sugoi.Core.Scripts
{
    public class ScriptableMachine : Machine
    {
        public string ScriptString
        {
            get
            {
                return scriptString;
            }

            set
            {
                if(scriptString != value)
                {
                    scriptString = value;
                }
            }
        }

        private string scriptString;

        protected override void InternalStart()
        {
        }

        protected override void InternalStop()
        {
        }
    }
}
