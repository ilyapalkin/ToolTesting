using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;

namespace ToolTesting.TextArea
{
    public class Model
    {
        public string Text { get; set; }
    }
    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ => View["index"];
            Post["/"] = _ => 
            {
                var s = this.Bind<Model>();
                return string.Format("text '{0}' length is {1}", s.Text, s.Text.Length);
            };
        }
    }
}