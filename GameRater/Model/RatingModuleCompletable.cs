﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    abstract class RatingModuleCompletable : RatingModule
    {
        protected IEnumerable<CompletionStatus> completionStatuses;

        public IEnumerable<CompletionStatus> CompletionStatuses
        {
            get { return completionStatuses; }
        }

        public override void Init()
        {
            base.Init();
            LoadCompletionStatuses();
        }

        protected abstract void LoadCompletionStatuses();
        public abstract void SaveCompletionStatuses();

        public CompletionStatus FindCompletionStatus(string name)
        {
            foreach (CompletionStatus cs in completionStatuses)
            {
                if (cs.Name == name)
                {
                    return cs;
                }
            }
            throw new NameNotFoundException("RatingModuleCompletable FindCompletionStatus: could not find name of " + name);
        }
    }
}
