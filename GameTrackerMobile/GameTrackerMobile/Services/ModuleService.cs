using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.IO;
using RatableTracker.Framework.LoadSave;

namespace GameTrackerMobile.Services
{
    public static class ModuleService
    {
        private static RatingModuleGame rm = null;

        public static RatingModuleGame GetActiveModule()
        {
            if (rm == null)
            {
                rm = GenerateNewModule();
                rm.Init();
                return rm;
            }
            else
            {
                return rm;
            }
        }

        public static async Task<RatingModuleGame> GetActiveModuleAsync()
        {
            if (rm == null)
            {
                rm = GenerateNewModule();
                await rm.InitAsync();
                return rm;
            }
            else
            {
                return rm;
            }
        }

        public static void ResetActiveModule()
        {
            rm = null;
        }

        private static RatingModuleGame GenerateNewModule()
        {
            PathController.PathControllerInstance = new PathControllerMobile();
            GlobalSettings.Autosave = true;
            IContentLoadSave<string, string> cls;
            if (ContentLoadSaveAWSS3.KeyFileExists())
                cls = new ContentLoadSaveAWSS3();
            else
                cls = new ContentLoadSaveLocal();
            LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
            {
                ContentLoadSaveInstance = cls
            };
            return new RatingModuleGame(engine);
        }
    }
}
