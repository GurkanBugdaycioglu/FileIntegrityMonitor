using System.Collections.Generic;
using FimCore.Models;

namespace FimCore.Interfaces
{
    public interface IBaselineRepository
    {
        List<BaselineEntry> LoadAll();
        void SaveAll(List<BaselineEntry> entries);
    }
}