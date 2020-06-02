/*
*Author : Don
*Since 	:2020-05-06
*Description  : 二级操作码  增删查改
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol
{
  public  enum SubOperationCode:byte
    {
        None=0,
        Default=1,
        Add = 2,
        Remove = 3,
        Get = 4,
        Update = 5,
        Verify=6
    }
}
