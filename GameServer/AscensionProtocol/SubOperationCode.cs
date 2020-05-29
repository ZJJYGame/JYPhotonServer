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
        Add = 0,
        Remove = 1,
        Get = 2,
        Update = 3,
        Verify=4
    }
}
