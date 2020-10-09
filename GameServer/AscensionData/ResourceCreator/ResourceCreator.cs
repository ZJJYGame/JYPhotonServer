using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.Random;
namespace AscensionData
{
    /// <summary>
    /// 资源创建者；
    /// 资源的位置、旋转在服务器中是需要扩大千倍
    /// </summary>
    public class ResourceCreator : IBehaviour
    {
        Random random = new Random();
        public void OnInitialization(){}
        public void OnTermination(){}
        /// <summary>
        /// 创建随机资源
        /// </summary>
        /// <param name="resVariable">资源参数类</param>
        /// <param name="border">边界信息</param>
        /// <returns>被生成后的对象</returns>
        public ResourceUnitSetDTO CreateRandomResourceSet(ResVariable resVariable, Vector2 border)
        {

            var resSetDTO = GameManager.ReferencePoolManager.Spawn<ResourceUnitSetDTO>();
            resSetDTO.GlobalID = resVariable.GlobalID;
            int count = GetRandomNumber(resVariable.Count, (int)resVariable.CountOffset);
            for (int i = 0; i < count; i++)
            {
                var randonVector = GetRandomVector2(Vector2.zero, border);
               var randonRotate =  GetRandomVector3(Vector3.zero, new Vector3(0, 360000, 0));
                int FlowValue = GetRandomNumber(resVariable.FlowValue, (int)resVariable.FlowValueOffset);
                ResourceUnitDTO resUnit = new ResourceUnitDTO()
                { ID = i, FlowValue = FlowValue, Position = new TransformDTO() { PositionX= randonVector.x, PositionY = 0, PositionZ =  randonVector.y, RotationX = 0, RotationY = randonRotate.y, RotationZ = 0 } };
                resSetDTO.AddResUnit(resUnit);
            }
            return resSetDTO;
        }
        /// <summary>
        /// 获得扩大千倍后的随机向量
        /// </summary>
        /// <param name="min">最小范围</param>
        /// <param name="max">最大范围</param>
        /// <returns></returns>
        public Vector3 GetRandomVector3(Vector3 min,Vector3 max)
        {
            var x = random.Next((int)Math.Ceiling( min.x),(int)Math.Ceiling( max.x))*GetRandomSymbol();
            var y = random.Next((int)Math.Ceiling( min.y),(int)Math.Ceiling( max.y))*GetRandomSymbol();
            var z = random.Next((int)Math.Ceiling( min.z),(int)Math.Ceiling( max.z))*GetRandomSymbol();
            return new Vector3(x, y, z);
        }
       public  Vector2 GetRandomVector2(Vector2 min, Vector2 max)
        {
            var x = random.Next((int)Math.Ceiling(min.x), (int)Math.Ceiling(max.x)) * GetRandomSymbol();
            var y = random.Next((int)Math.Ceiling(min.y), (int)Math.Ceiling(max.y)) * GetRandomSymbol();
            return new Vector2(x, y);
        }

        /// <summary>
        /// 随机获得正数或者负数符号
        /// </summary>
        /// <returns></returns>
        int GetRandomSymbol()
        {
            var r = random.Next(1, 99);
            int symbol = 0;
            if (r % 2 == 0)
                symbol = 1;
            else
                symbol = -1;
            return symbol;
        }
        int GetRandomNumber(int number,int offset)
        {
            if (offset <= 0)
                return number;
            var result= random.Next(number- offset,number+offset);
            return result;
        }
    }
}
