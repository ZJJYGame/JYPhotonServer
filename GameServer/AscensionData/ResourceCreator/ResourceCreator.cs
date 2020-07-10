﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionRegion;
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
        public ResourceSetDTO CreateRandomResourceSet(ResVariable resVariable, Vector2 border)
        {

            var resSetDTO =Singleton<ReferencePoolManager>.Instance.Spawn<ResourceSetDTO>();
            resSetDTO.GlobalID = resVariable.GlobalID;
            int count = GetRandomNumber(resVariable.Count, (int)resVariable.CountOffset);
            for (int i = 0; i < count; i++)
            {
                var randonVector = GetRandomVector(Vector2.Zero, border);
                int level = GetRandomNumber(resVariable.Level, (int)resVariable.LevelOffset);
                ResourceUnitDTO resUnit = new ResourceUnitDTO()
                { ID = i, Level = level, Position = new Vector3DTO(randonVector.X, 0, randonVector.Y) };
                resSetDTO.ResUnitSet.Add(resUnit);
            }
            return resSetDTO;
        }
        /// <summary>
        /// 获得扩大千倍后的随机向量
        /// </summary>
        /// <param name="min">最小范围</param>
        /// <param name="max">最大范围</param>
        /// <returns></returns>
        Vector3 GetRandomVector(Vector3 min,Vector3 max)
        {
            var x = random.Next((int)Math.Ceiling( min.X),(int)Math.Ceiling( max.X))*GetRandomSymbol();
            var y = random.Next((int)Math.Ceiling( min.Y),(int)Math.Ceiling( max.Y))*GetRandomSymbol();
            var z = random.Next((int)Math.Ceiling( min.Z),(int)Math.Ceiling( max.Z))*GetRandomSymbol();
            return new Vector3(x, y, z);
        }
        Vector2 GetRandomVector(Vector2 min, Vector2 max)
        {
            var x = random.Next((int)Math.Ceiling(min.X), (int)Math.Ceiling(max.X)) * GetRandomSymbol();
            var y = random.Next((int)Math.Ceiling(min.Y), (int)Math.Ceiling(max.Y)) * GetRandomSymbol();
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