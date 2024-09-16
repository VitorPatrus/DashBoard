using BI.Sistemas.API.View;
using Microsoft.AspNetCore.Mvc;

namespace BI.Sistemas.API.Interfaces
{
    public interface IColaboradorService
    {
        ColaboradorDashboardView GetColaboradorDashboard(string id);
    }
}
