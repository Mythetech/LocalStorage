using System;
using System.Diagnostics.CodeAnalysis;

namespace Mythetech.LocalStorage
{
    [ExcludeFromCodeCoverage]
    public class ChangingEventArgs : ChangedEventArgs
    {
        public bool Cancel { get; set; }
    }
}
