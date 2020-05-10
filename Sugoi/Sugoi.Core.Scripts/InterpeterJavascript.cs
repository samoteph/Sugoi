using Esprima.Ast;
using Jint;
using Jint.Native;
using Jint.Runtime.Debugger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core.Scripts
{
    public class InterpeterJavascript
    {
        Engine jintEngine;
        
        JsValue functionUpdate;
        JsValue functionDraw;
        JsValue functionInit;

        public bool IsDebugging
        {
            get;
            private set;
        }

        public void Start(bool debug)
        {
            this.IsDebugging = debug;

            this.jintEngine = new Engine(options =>
            {
                options.DebugMode(debug);
            });

            if (debug)
            {
                this.jintEngine.Step += OnStepChange;
                this.jintEngine.Break += OnBreak;
            }
        }

        private StepMode OnBreak(object sender, DebugInformation e)
        {
            return StepMode.None;
        }

        private StepMode OnStepChange(object sender, DebugInformation e)
        {
            return StepMode.None;
        }

        public void Stop()
        {
            if (IsDebugging)
            {
                this.jintEngine.Step -= OnStepChange;
                this.jintEngine.Break -= OnBreak;
            }

            this.jintEngine = null;
        }

        /// <summary>
        /// Chargement de script et recupération des fonctions init, update et draw
        /// </summary>
        /// <param name="script"></param>

        public void LoadScript(string script)
        {
            this.jintEngine.Execute(script);

            this.functionInit = this.jintEngine.GetValue("init");
            this.functionDraw = this.jintEngine.GetValue("draw");
            this.functionUpdate = this.jintEngine.GetValue("update");        
        }

        public void ExecuteFunctionUpdate()
        {
            this.functionUpdate?.Invoke();
        }

        public void ExecuteFunctionDraw()
        {
            this.functionDraw?.Invoke();
        }

        public void ExecuteFunctionInit()
        {
            this.functionInit?.Invoke();
        }

        public void Declare(string function)
        {

        }
    }
}
