using System.Collections;
using System.Collections.Generic;
using System;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class C2SAnimateInput : IDataContract
    {
        [Key(0)]
        public string TriggeredName { get; set; }
    }
}