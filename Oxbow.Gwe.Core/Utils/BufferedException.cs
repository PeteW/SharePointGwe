using System;
using System.Collections.Generic;
using System.Text;

namespace Oxbow.Gwe.Core.Utils
{
    public class BufferedException
    {
        private List<string> _errors;
        public List<string> ErrorsList { get { return _errors; } }

        public BufferedException() { _errors = new List<string>(); }

        public string Errors { get { return string.Join(Environment.NewLine,_errors.ToArray()); } }

        public bool HasErrors { get { return Errors.Length > 0; } }

        public void AddToBuffer(Exception exception) { _errors.Add(exception.ToString());}

        public void AddToBuffer(string s) { _errors.Add(s); }

        public void ThrowIfNecessary()
        {
            if (HasErrors)
                throw new Exception(Errors);
        }
    }
}