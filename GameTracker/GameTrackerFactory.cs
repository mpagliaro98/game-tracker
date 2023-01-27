using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class GameTrackerFactory : RatableTrackerFactory
    {
        protected override RankedObject CreateModelObjectFromTypeName(string typeName, Settings settings, TrackerModule module)
        {
            var obj = base.CreateModelObjectFromTypeName(typeName, settings, module);
            switch (typeName.ToLower())
            {
                case "gameobject":
                    obj = new GameObject((SettingsGame)settings, (GameModule)module);
                    break;
                case "gamecompilation":
                    obj = new GameCompilation((SettingsGame)settings, (GameModule)module);
                    break;
            }
            return obj;
        }

        protected override Status CreateStatusFromTypeName(string typeName, IModuleStatus module, Settings settings)
        {
            var obj = base.CreateStatusFromTypeName(typeName, module, settings);
            switch (typeName.ToLower())
            {
                case "statusgame":
                    obj = new StatusGame(module, (SettingsGame)settings);
                    break;
            }
            return obj;
        }

        protected override Settings CreateSettingsFromTypeName(string typeName)
        {
            var obj = base.CreateSettingsFromTypeName(typeName);
            switch (typeName.ToLower())
            {
                case "settingsgame":
                    obj = new SettingsGame();
                    break;
            }
            return obj;
        }

        public Platform GetPlatform(string typeName, GameModule module, SettingsGame settings)
        {
            Platform obj = CreatePlatformFromTypeName(typeName, module, settings);
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual Platform CreatePlatformFromTypeName(string typeName, GameModule module, SettingsGame settings)
        {
            Platform obj = null;
            switch (typeName.ToLower())
            {
                case "platform":
                    obj = new Platform(module, settings);
                    break;
            }
            return obj;
        }
    }
}
