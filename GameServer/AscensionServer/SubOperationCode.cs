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
        Add = 1,
        Remove = 2,
        Get = 3,
        Update = 4,
        Verify=5
    }
}
