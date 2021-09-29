using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundJob
    {
        Task ActionToDoAsync();
        Task OnException(Exception exception);
    }
}