using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Attributtes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class UiIconAttribute : Attribute
    {
        public string IconClass { get; }

        public UiIconAttribute(string iconClass)
        {
            IconClass = iconClass;
        }
    }
}
