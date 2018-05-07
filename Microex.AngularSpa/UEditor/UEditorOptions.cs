using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Microex.AngularSpa.UEditor
{
    public class UEditorOptions
    {
        internal UEditorJsonConfig JsonConfig { get; set; }
        public string EndPoint { get; set; }
        public string ConfigUrl { get; set; }
    }

}
