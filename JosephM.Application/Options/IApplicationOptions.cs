﻿using System;
using System.Collections.Generic;

namespace JosephM.Application.Options
{
    public interface IApplicationOptions
    {
        void AddOption(string group, string optionLabel, Action action, string description = null);

        IEnumerable<IApplicationOption> GetAllOptions();
    }
}
