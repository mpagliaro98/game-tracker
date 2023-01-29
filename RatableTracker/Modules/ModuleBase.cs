using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public abstract class ModuleBase : ModulePatternBase
    {
        public ModuleBase(ILoadSaveHandler<ILoadSaveMethod> loadSave) : base(loadSave) { }

        public ModuleBase(ILoadSaveHandler<ILoadSaveMethod> loadSave, Logger logger) : base(loadSave, logger) { }

        public void LoadData(Settings settings)
        {
            using var conn = LoadSave.NewConnection();
            LoadDataConsecutively(settings, conn);
        }

        protected abstract void LoadDataConsecutively(Settings settings, ILoadSaveMethod conn);

        public Task LoadDataAsync(Settings settings)
        {
            var conn = LoadSave.NewConnection();
            var list = LoadDataCreateTaskList(settings, conn);
            var task = Task.WhenAll(list);
            task = task.ContinueWith(t => conn.Dispose());
            return task;
        }

        protected abstract IList<Task> LoadDataCreateTaskList(Settings settings, ILoadSaveMethod conn);

        internal ILoadSaveMethod GetNewConnection()
        {
            return LoadSave.NewConnection();
        }
    }
}
