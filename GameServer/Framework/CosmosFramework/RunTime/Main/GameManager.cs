﻿using Cosmos.Log;
using Cosmos.Network;
using Cosmos.Polling;
using Cosmos.Reference;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
   public class GameManager:ConcurrentSingleton<GameManager>, IControllable, IRefreshable
    {
        #region Properties
        public bool IsPause { get; private set; }
        /// <summary>
        /// 轮询更新委托
        /// </summary>
        Action refreshHandler;
        int moduleCount = 0;
        static Dictionary<ModuleEnum, IModule> moduleDict;
        internal static Dictionary<ModuleEnum, IModule> ModuleDict { get { return moduleDict; } }
        static ReferencePoolManager referencePoolManager;
        public static ReferencePoolManager ReferencePoolManager
        {
            get
            {
                if (referencePoolManager == null)
                {
                    referencePoolManager = new ReferencePoolManager();
                    Instance.ModuleInitialization(referencePoolManager);
                }
                return referencePoolManager;
            }
        }
        static NetworkManager networkManager;
        public static NetworkManager NetworkManager
        {
            get
            {
                if (networkManager == null)
                {
                    networkManager = new NetworkManager();
                    Instance.ModuleInitialization(networkManager);
                }
                return networkManager;
            }
        }
        static PollingManager  pollingManager;
        public static PollingManager PollingManager
        {
            get
            {
                if (pollingManager == null)
                {
                    pollingManager = new PollingManager();
                    Instance.ModuleInitialization(pollingManager);
                }
                return pollingManager;
            }
        }
        static LogManager  logManager;
        public static LogManager LogManager
        {
            get
            {
                if (logManager == null)
                {
                    logManager = new LogManager();
                    Instance.ModuleInitialization(logManager);
                }
                return logManager ;
            }
        }
        static ConcurrentDictionary<Type, IModule> extensionsModuleDict = new ConcurrentDictionary<Type, IModule>();
        #endregion
        #region Methods
        /// <summary>
        /// 构造函数，只有使用到时候才产生
        /// </summary>
        public GameManager()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<ModuleEnum, IModule>();
            }
        }
        public void OnPause()
        {
            IsPause = true;
        }
        public void OnUnPause()
        {
            IsPause = false;
        }
        public void OnRefresh()
        {
            if (IsPause)
                return;
            refreshHandler?.Invoke();
        }
        /// <summary>
        /// 获取外源模块；
        /// 此类模块不由CF框架生成，由用户自定义
        /// 需要从Module类派生;
        /// 线程安全；
        /// </summary>
        /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
        /// <returns>获取的模块</returns>
        public static TModule GetExtensionsModule<TModule>()
            where TModule : Module<TModule>, new()
        {
            Type type = typeof(TModule);
            IModule module = default;
            var result= extensionsModuleDict.TryGetValue(type, out module);
            if (!result)
            {
                module = new TModule();
                extensionsModuleDict.TryAdd(type, module);
                module.OnInitialization();
                Utility.Debug.LogInfo($"Create new module  , Module :{module.ToString()} ");
            }
            return module as TModule;

        }
        /// <summary>
        /// 清理外源模块；
        /// 此类模块不由CF框架生成，由用户自定义
        /// 需要从Module类派生;
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        public static void ClearExtensionsModule<TModule>()
    where TModule : Module<TModule>, new()
        {
            Type type = typeof(TModule);
            if (extensionsModuleDict.ContainsKey(type))
            {
                IModule removedModule;
                extensionsModuleDict.TryRemove(type,out removedModule);
                removedModule.OnTermination();
            }
        }
        internal void ModuleInitialization(IModule module)
        {
            module.OnInitialization();
            Instance.RegisterModule(module.ModuleEnum, module);
        }
        /// <summary>
        /// 注册模块
        /// </summary>
        internal void RegisterModule(ModuleEnum moduleEnum, IModule module)
        {
            if (!HasModule(moduleEnum))
            {
                moduleDict.Add(moduleEnum, module);
                moduleCount++;
                refreshHandler += module.OnRefresh;
                Utility.Debug.LogInfo("Module:\"" + moduleEnum.ToString() + "\" " + "  is OnInitialization" + " based on GameManager");
            }
            else
            {
               Utility.Debug.LogError( new ArgumentException("Module:\"" + moduleEnum.ToString() + "\" " + " is already exist!")); 
            }
        }
        internal void DeregisterModule(ModuleEnum module)
        {
            if (HasModule(module))
            {
                var m = moduleDict[module];
                refreshHandler -= m.OnRefresh;
                moduleDict.Remove(module);
                moduleCount--;
                Utility.Debug.LogInfo("Module:\"" + module.ToString() + "\" " + "  is OnTermination" + " based on GameManager", MessageColor.DARKBLUE);
            }
            else
                Utility.Debug.LogError( new ArgumentNullException("Module:\"" + module.ToString() + "\" " + " is  not exist!"));
        }
        internal bool HasModule(ModuleEnum module)
        {
            return moduleDict.ContainsKey(module);
        }
        #endregion
    }
}
