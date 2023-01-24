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

        protected override Status CreateStatusFromTypeName(string typeName, StatusExtensionModule module)
        {
            var obj = base.CreateStatusFromTypeName(typeName, module);
            switch (typeName.ToLower())
            {
                case "statusgame":
                    obj = new StatusGame(module);
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

        public Platform GetPlatform(string typeName, GameModule module)
        {
            Platform obj = CreatePlatformFromTypeName(typeName, module);
            if (obj == null)
                throw new ArgumentException("Unknown type: " + typeName);
            else
                return obj;
        }

        protected virtual Platform CreatePlatformFromTypeName(string typeName, GameModule module)
        {
            Platform obj = null;
            switch (typeName.ToLower())
            {
                case "platform":
                    obj = new Platform(module);
                    break;
            }
            return obj;
        }
    }
}
